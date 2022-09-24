using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using OwlCore.ComponentModel;
using OwlCore.Diagnostics;
using OwlCore.Extensions;
using OwlCore.Storage;
using StrixMusic.Cores.Storage.FileMetadata.Models;
using StrixMusic.Cores.Storage.FileMetadata.Repositories;
using StrixMusic.Cores.Storage.FileMetadata.Scanners;

namespace StrixMusic.Cores.Storage.FileMetadata;

/// <summary>
/// Given an OwlCore.Storage implementation, this manages scanning and caching all the music metadata from files in folder, including child folders.
/// </summary>
internal sealed class FileMetadataManager : IAsyncInit, IAsyncDisposable
{
    private readonly IFolderScanner _folderScanner;
    private readonly IModifiableFolder _metadataStorage;
    private readonly IProgress<FileScanState>? _scanProgress;
    private CancellationTokenSource? _inProgressScanCancellationTokenSource;
    private TaskCompletionSource<object?>? _inProgressScanTaskCompletionSource;

    private FileScanState _scanState;

    /// <summary>
    /// Creates a new instance of <see cref="FileMetadataManager"/>.
    /// </summary>
    /// <param name="rootFolderToScan">The folder where data is scanned.</param>
    /// <param name="folderScanner">The scanner that should be used for discovering files.</param>
    /// <param name="metadataStorage">The folder the metadata manager can persist metadata.</param>
    /// <param name="scanProgress">An <see cref="IProgress{T}"/> implementation that reports scan progress.</param>
    public FileMetadataManager(IFolderScanner folderScanner, IModifiableFolder metadataStorage, IProgress<FileScanState>? scanProgress = null)
    {
        _folderScanner = folderScanner;
        Images = new ImageRepository();
        Tracks = new TrackRepository();
        Albums = new AlbumRepository();
        AlbumArtists = new ArtistRepository(id: "AlbumArtists");
        TrackArtists = new ArtistRepository(id: "TrackArtists");
        Playlists = new PlaylistRepository();

        _metadataStorage = metadataStorage;
        _scanProgress = scanProgress;
    }

    /// <inheritdoc />
    public Task InitAsync(CancellationToken cancellationToken = default)
    {
        Guard.IsFalse(IsInitialized, nameof(IsInitialized));

        IsInitialized = true;

        Logger.LogInformation($"{nameof(InitAsync)} completed.");
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public bool IsInitialized { get; private set; }

    /// <summary>
    /// The current scan state.
    /// </summary>
    public FileScanState ScanState
    {
        get => _scanState;
        set
        {
            _scanState = value;
            _scanProgress?.Report(value);
        }
    }

    /// <summary>
    /// Stores album metadata.
    /// </summary>
    public IAlbumRepository Albums { get; }

    /// <summary>
    /// Stores metadata about artists who contributed towards an album.
    /// </summary>
    public IArtistRepository AlbumArtists { get; }

    /// <summary>
    /// Stores artist metadata that contributed towards one or more tracks.
    /// </summary>
    public IArtistRepository TrackArtists { get; }

    /// <summary>
    /// Stores track metadata.
    /// </summary>
    public ITrackRepository Tracks { get; }

    /// <summary>
    /// Stores playlist metadata.
    /// </summary>
    public IPlaylistRepository Playlists { get; }

    /// <summary>
    /// Stores image metadata.
    /// </summary>
    public IImageRepository Images { get; }

    /// <summary>
    /// Gets the stream for the provided image Id.
    /// </summary>
    /// <param name="imageId">The unique identifier for this image, created as part of a scan.</param>
    /// <returns>A Task containing the image stream, if found.</returns>
    /// <exception cref="ArgumentException">Couldn't extract scanned image type from image ID.</exception>
    public async Task<Stream?> GetImageStreamById(string imageId)
    {
        var fileId = AudioMetadataScanner.GetFileIdFromImageId(imageId);

        // Check if we've seen the file already. KnownFiles is expected to be always up to date.
        var targetFile = _folderScanner.KnownFiles.FirstOrDefault(x => x.Id == fileId);
        if (targetFile is null)
        {
            // This method can't be used before a scan is kicked off in the core.
            // If we haven't seen the file yet, check if there's an active scan.
            if (_inProgressScanTaskCompletionSource is null)
            {
                // If there's no active scan and we haven't seen the file, the file may not exist. Return nothing.
                return null;
            }

            var taskCompletionSource = new TaskCompletionSource<IAddressableFile>();

            _folderScanner.KnownFiles.CollectionChanged += KnownFilesOnCollectionChanged;
            await Task.WhenAny(_inProgressScanTaskCompletionSource.Task, taskCompletionSource.Task);

            // If file was not added to KnownFiles during file discovery / scan.
            if (targetFile is null)
            {
                // Scan ran to completion. If the file exists at all, we should have it now.
                targetFile = _folderScanner.KnownFiles.FirstOrDefault(x => x.Id == fileId);
            }

            // The file does not exist.
            if (targetFile is null)
                return null;

            void KnownFilesOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
            {
                if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems.Cast<IStorable>().FirstOrDefault(x => x is IFile file && file.Id == fileId) is IAddressableFile addedTargetFile)
                {
                    targetFile = addedTargetFile;
                    taskCompletionSource.SetResult(addedTargetFile);
                }
            }
        }

        return await AudioMetadataScanner.GetImageStream(targetFile, imageId);
    }

    /// <summary>
    /// Starts scanning the given folder.
    /// </summary>
    /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
    public async Task ScanAsync(CancellationToken cancellationToken = default)
    {
        var seenFiles = new List<IFile>();
        var digestedMetadata = new List<Models.FileMetadata>();

        Logger.LogInformation($"Scan started. Setting up.");
        {
            _inProgressScanTaskCompletionSource?.TrySetCanceled();
            _inProgressScanCancellationTokenSource?.Dispose();

            _inProgressScanTaskCompletionSource = new TaskCompletionSource<object?>();
            if (_inProgressScanCancellationTokenSource is not null)
            {
                // Cancel previous scan.
                _inProgressScanCancellationTokenSource.Cancel();
                _inProgressScanCancellationTokenSource.Dispose();
            }

            _inProgressScanCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cancellationToken = _inProgressScanCancellationTokenSource.Token;

            using var cancellationCallback = cancellationToken.Register(() => _inProgressScanTaskCompletionSource.SetCanceled());
            cancellationToken.ThrowIfCancellationRequested();

            if (!IsInitialized)
                await InitAsync(cancellationToken);
        }

        {
            Logger.LogInformation($"Loading metadata cache");
            var fileMetadataCache = await TryGetFileMetadataCacheAsync(cancellationToken) ?? new();

            // Files must exist before relevant metadata is made available.
            // Metadata for files that can't be found, shouldn't be available.
            Logger.LogInformation($"Discovering files");
            ScanState = new FileScanState(FileScanStage.ScanningFiles, ScanState.FilesProcessed, ScanState.FilesFound);

            await foreach (var file in _folderScanner.ScanFolderAsync(cancellationToken))
            {
                ScanState = new FileScanState(ScanState.Stage, ScanState.FilesProcessed, ScanState.FilesFound + 1);
                seenFiles.Add(file);

                // If cached metadata is found, we have everything we need to emit Album, Artist, Track info safely.
                if (fileMetadataCache.TryGetValue(file.Id, out var fileMetadata))
                {
                    digestedMetadata.Add(fileMetadata);
                    fileMetadata.Id = file.Id;

                    GarbageCollectMetadataReferences(digestedMetadata);

                    Logger.LogInformation($"Digesting file metadata {fileMetadata.Id}");
                    await DigestFileMetadataAsync(fileMetadata);

                    // Get updated metadata as a low priority fire-and-forget task.
                    _ = Task.Run(() =>
                    {
                        Thread.CurrentThread.Priority = ThreadPriority.Lowest;
                        return UpdateMetadataAsync();
                    }, cancellationToken);
                }

                // If no cache, scan file.
                if (fileMetadata is null)
                {
                    await UpdateMetadataAsync();
                }

                async Task UpdateMetadataAsync()
                {
                    // File properties are MUCH faster than taglib. If music properties are present, use them. Otherwise, use taglib.
                    fileMetadata = await AudioMetadataScanner.ScanMusicFileAsync(file, MetadataScanTypes.FileProperties, cancellationToken) ??
                                   await AudioMetadataScanner.ScanMusicFileAsync(file, MetadataScanTypes.TagLib, cancellationToken);

                    // File has no audio metadata
                    if (fileMetadata is not null)
                    {
                        fileMetadata.Id = file.Id;

                        GarbageCollectMetadataReferences(digestedMetadata);

                        Logger.LogInformation($"Digesting file metadata {fileMetadata.Id}");
                        await DigestFileMetadataAsync(fileMetadata);
                        await SaveFileMetadataCache(fileMetadataCache, cancellationToken);
                    }
                }

                ScanState = new FileScanState(ScanState.Stage, ScanState.FilesProcessed + 1, ScanState.FilesFound);
            }
        }

        // Playlists must be scanned after all files have been seen, so removed files can have metadata omitted.
        Logger.LogInformation($"Starting metadata scan of playlist files");
        {
            ScanState = new FileScanState(FileScanStage.Playlists, ScanState.FilesProcessed, ScanState.FilesFound);

            foreach (var file in seenFiles)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var playlistData = await PlaylistMetadataScanner.ScanPlaylistFileAsync(file, _folderScanner.KnownFiles.ToList<IFile>(), cancellationToken);
                if (playlistData is not null)
                {
                    await Playlists.AddOrUpdateAsync(new[] { playlistData });
                }
            }

            cancellationToken.ThrowIfCancellationRequested();

            ScanState = new FileScanState(FileScanStage.Complete, ScanState.FilesProcessed, ScanState.FilesFound);
        }

        _inProgressScanTaskCompletionSource.SetResult(null);
        _inProgressScanCancellationTokenSource = null;
        _inProgressScanTaskCompletionSource = null;
    }

    private void GarbageCollectMetadataReferences(IList<Models.FileMetadata> allKnownItems)
    {
        var allKnownTracks = allKnownItems.Select(x => x.TrackMetadata).PruneNull().ToList();
        var allKnownAlbumArtists = allKnownItems.SelectMany(x => x.AlbumArtistMetadata).ToList();
        var allKnownTrackArtists = allKnownItems.SelectMany(x => x.TrackArtistMetadata).ToList();
        var allKnownImages = allKnownItems.SelectMany(x => x.ImageMetadata).ToList();
        var allKnownPlaylists = allKnownItems.Select(x => x.PlaylistMetadata).PruneNull().ToList();
        var allKnownAlbums = allKnownItems.Select(x => x.AlbumMetadata).PruneNull().ToList();

        foreach (var track in allKnownTracks)
            PruneTrackData(track);

        foreach (var artist in allKnownAlbumArtists)
            PruneArtistData(artist);

        foreach (var artist in allKnownTrackArtists)
            PruneArtistData(artist);

        foreach (var album in allKnownAlbums)
            PruneAlbumData(album);

        foreach (var playlist in allKnownPlaylists)
            PrunePlaylistData(playlist);

        void PruneTrackData(TrackMetadata track)
        {
            Guard.IsNotNullOrWhiteSpace(track.Id);

            // Remove IDs for images that don't exist.
            if (track.ImageIds is not null)
                track.ImageIds = new HashSet<string>(track.ImageIds.Where(x => allKnownImages.Any(y => y.Id == x)));

            // Remove IDs for track artists that don't exist.
            if (track.ArtistIds is not null)
                track.ArtistIds = new HashSet<string>(track.ArtistIds.Where(x => allKnownTrackArtists.Any(y => y.Id == x)));

            // Each scanned file must contain
            // 1. A track
            // 2. An album (even if empty name)
            Guard.IsNotNullOrWhiteSpace(track.Id);
            Guard.IsNotNullOrWhiteSpace(track.AlbumId);
        }

        void PruneAlbumData(AlbumMetadata album)
        {
            Guard.IsNotNullOrWhiteSpace(album.Id);

            // Remove IDs for images that don't exist.
            if (album.ImageIds is not null)
                album.ImageIds = new HashSet<string>(album.ImageIds.Where(x => allKnownImages.Any(y => y.Id == x)));

            // Remove IDs for album artists that don't exist.
            if (album.ArtistIds is not null)
                album.ArtistIds = new HashSet<string>(album.ArtistIds.Where(x => allKnownAlbumArtists.Any(y => y.Id == x)));

            // Remove IDs for tracks that don't exist.
            if (album.TrackIds is not null)
                album.TrackIds = new HashSet<string>(album.TrackIds.Where(x => allKnownTracks.Any(y => y.Id == x)));
        }

        void PruneArtistData(ArtistMetadata artist)
        {
            Guard.IsNotNullOrWhiteSpace(artist.Id);

            // Remove IDs for images that don't exist.
            if (artist.ImageIds is not null)
                artist.ImageIds = new HashSet<string>(artist.ImageIds.Where(x => allKnownImages.Any(y => y.Id == x)));

            // Remove IDs for albums that don't exist.
            if (artist.AlbumIds is not null)
                artist.AlbumIds = new HashSet<string>(artist.AlbumIds.Where(x => allKnownAlbums.Any(y => y.Id == x)));
        }

        void PrunePlaylistData(PlaylistMetadata playlist)
        {
            Guard.IsNotNullOrWhiteSpace(playlist.Id);

            // Remove IDs for tracks that don't exist.
            if (playlist.TrackIds is not null)
                playlist.TrackIds = new HashSet<string>(playlist.TrackIds.Where(x => allKnownTracks.Any(y => y.Id == x)));
        }
    }

    private async Task DigestFileMetadataAsync(Models.FileMetadata fileMetadata)
    {
        Guard.IsNotNull(fileMetadata.ImageMetadata);
        await DigestImagesAsync(fileMetadata.ImageMetadata.ToArray());

        Guard.IsNotNull(fileMetadata.TrackMetadata);
        await DigestTrackAsync(fileMetadata.TrackMetadata);

        Guard.IsNotNull(fileMetadata.AlbumArtistMetadata);
        await DigestArtistsAsync(fileMetadata.AlbumArtistMetadata.ToArray(), AlbumArtists);

        Guard.IsNotNull(fileMetadata.TrackArtistMetadata);
        await DigestArtistsAsync(fileMetadata.TrackArtistMetadata.ToArray(), TrackArtists);

        if (fileMetadata.AlbumMetadata is not null)
            await DigestAlbumAsync(fileMetadata.AlbumMetadata);

        Task DigestTrackAsync(TrackMetadata track) => Tracks.AddOrUpdateAsync(new[] { track });
        Task DigestImagesAsync(ImageMetadata[] images) => Images.AddOrUpdateAsync(images);
        Task DigestAlbumAsync(AlbumMetadata album) => DigestAlbumsAsync(new[] { album });
        Task DigestAlbumsAsync(AlbumMetadata[] albums) => Albums.AddOrUpdateAsync(albums);
        async Task DigestArtistsAsync(ArtistMetadata[] artists, IArtistRepository artistRepository) => await artistRepository.AddOrUpdateAsync(artists.ToArray());
    }

    private async Task SaveFileMetadataCache(Dictionary<string, Models.FileMetadata> fileMetadataCache, CancellationToken cancellationToken)
    {
        var cacheFile = await GetFileCacheFile(cancellationToken);

        using var storageStream = await cacheFile.OpenStreamAsync(FileAccess.ReadWrite, cancellationToken);
        using var serializedStream = await FileMetadataRepoSerializer.Singleton.SerializeAsync(fileMetadataCache, cancellationToken);

        if (storageStream.CanSeek)
            storageStream.Seek(0, SeekOrigin.Begin);

        if (storageStream.CanSeek)
            serializedStream.Seek(0, SeekOrigin.Begin);

        await serializedStream.CopyToAsync(storageStream);
    }

    internal async Task<IFile> GetFileCacheFile(CancellationToken cancellationToken)
    {
        var fileMetadataFilename = "Files.bin";
        var cachedFileStorable = await _metadataStorage.GetItemsAsync(cancellationToken: cancellationToken)
            .FirstOrDefaultAsync(x => x.Name == fileMetadataFilename, cancellationToken: cancellationToken);

        if (cachedFileStorable is not IFile cachedFile)
            cachedFile = await _metadataStorage.CreateFileAsync(fileMetadataFilename, overwrite: false, cancellationToken);

        return cachedFile;
    }

    internal async Task<Dictionary<string, Models.FileMetadata>?> TryGetFileMetadataCacheAsync(CancellationToken cancellationToken)
    {
        var cachedFile = await GetFileCacheFile(cancellationToken);
        using var fileCacheStream = await cachedFile.OpenStreamAsync(cancellationToken: cancellationToken);

        try
        {
            var cache = await FileMetadataRepoSerializer.Singleton.DeserializeAsync<Dictionary<string, Models.FileMetadata>>(fileCacheStream, cancellationToken);

            GarbageCollectMetadataReferences(cache.Values.ToList());
            return cache;
        }
        catch
        {
            return null;
        }
    }

    /// <inheritdoc />
    public ValueTask DisposeAsync()
    {
        _folderScanner.Dispose();
        return default;
    }
}
