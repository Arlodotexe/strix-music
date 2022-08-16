// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using OwlCore.AbstractStorage;
using OwlCore.AbstractStorage.Scanners;
using OwlCore.Extensions;
using OwlCore.Services;
using StrixMusic.Sdk.FileMetadata.Repositories;
using StrixMusic.Sdk.FileMetadata.Scanners;

namespace StrixMusic.Sdk.FileMetadata
{
    /// <summary>
    /// Given an OwlCore.AbstractStorage implementation, this manages scanning and caching all the music metadata from files in folder, including child folders.
    /// </summary>
    public sealed class FileMetadataManager : IFileMetadataManager
    {
        private readonly List<IFileData> _knownFiles = new();
        private readonly IFileScanner _fileScanner;
        private readonly AudioMetadataScanner _audioMetadataScanner;
        private readonly PlaylistMetadataScanner _playlistMetadataScanner;
        private readonly IFolderData _rootFolderToScan;
        private readonly IFolderData _metadataStorage;
        private readonly IProgress<FileScanState>? _scanProgress;
        private CancellationTokenSource? _inProgressScanCancellationTokenSource;

        private FileScanState _scanState;

        /// <summary>
        /// Creates a new instance of <see cref="FileMetadataManager"/>.
        /// </summary>
        /// <param name="rootFolderToScan">The folder where data is scanned.</param>
        /// <param name="metadataStorage">The folder the metadata manager can persist metadata.</param>
        /// <param name="scanProgress">An <see cref="IProgress{T}"/> implementation that reports scan progress.</param>
        /// <param name="degreesOfParallelism">The number of files that are scanned concurrently.</param>
        public FileMetadataManager(IFolderData rootFolderToScan, IFolderData metadataStorage, IProgress<FileScanState>? scanProgress = null, int degreesOfParallelism = 2)
        {
            _fileScanner = new DepthFirstFileScanner(rootFolderToScan);
            _audioMetadataScanner = new AudioMetadataScanner(degreesOfParallelism);
            _playlistMetadataScanner = new PlaylistMetadataScanner();

            Images = new ImageRepository();
            Tracks = new TrackRepository();
            Albums = new AlbumRepository();
            AlbumArtists = new ArtistRepository(id: "AlbumArtists");
            TrackArtists = new ArtistRepository(id: "TrackArtists");
            Playlists = new PlaylistRepository(_playlistMetadataScanner);

            _rootFolderToScan = rootFolderToScan;
            _metadataStorage = metadataStorage;
            _scanProgress = scanProgress;
        }

        /// <inheritdoc />
        public async Task InitAsync(CancellationToken cancellationToken = default)
        {
            Guard.IsFalse(IsInitialized, nameof(IsInitialized));
            IsInitialized = true;

            Albums.SetDataFolder(_metadataStorage);
            AlbumArtists.SetDataFolder(_metadataStorage);
            TrackArtists.SetDataFolder(_metadataStorage);
            Tracks.SetDataFolder(_metadataStorage);
            Playlists.SetDataFolder(_metadataStorage);
            Images.SetDataFolder(_metadataStorage);

            if (!SkipRepoInit)
            {
                Logger.LogInformation($"Initializing repositories.");
                await Albums.InitAsync(cancellationToken);
                await AlbumArtists.InitAsync(cancellationToken);
                await Tracks.InitAsync(cancellationToken);
                await Playlists.InitAsync(cancellationToken);
                await Images.InitAsync(cancellationToken);
            }

            AttachEvents();
            Logger.LogInformation($"{nameof(InitAsync)} completed.");
        }

        /// <inheritdoc />
        public event EventHandler? ScanningStarted;

        /// <inheritdoc />
        public event EventHandler? ScanningCompleted;

        private void AttachEvents()
        {
            _fileScanner.FilesDiscovered += OnFilesDiscovered;
            _fileScanner.FileDiscoveryCompleted += FileScanner_FileScanCompleted;

            _audioMetadataScanner.FileMetadataAdded += AudioMetadataScanner_FileMetadataAdded;

            _audioMetadataScanner.FilesFoundChanged += FilesFoundChanged;
            _audioMetadataScanner.FilesProcessedChanged += FilesProcessedChanged;
            _playlistMetadataScanner.FilesFoundChanged += FilesFoundChanged;
            _playlistMetadataScanner.FilesProcessedChanged += FilesProcessedChanged;
        }

        private void DetachEvents()
        {
            _fileScanner.FilesDiscovered -= OnFilesDiscovered;
            _fileScanner.FileDiscoveryCompleted -= FileScanner_FileScanCompleted;

            _audioMetadataScanner.FileMetadataAdded -= AudioMetadataScanner_FileMetadataAdded;

            _audioMetadataScanner.FilesFoundChanged -= FilesFoundChanged;
            _audioMetadataScanner.FilesProcessedChanged -= FilesProcessedChanged;
            _playlistMetadataScanner.FilesFoundChanged -= FilesFoundChanged;
            _playlistMetadataScanner.FilesProcessedChanged -= FilesProcessedChanged;
        }

        private void FilesFoundChanged(object sender, int e)
        {
            ScanState = new FileScanState(ScanState.Stage, ScanState.FilesProcessed, e);
        }

        private void FilesProcessedChanged(object sender, int e)
        {
            ScanState = new FileScanState(ScanState.Stage, e, ScanState.FilesFound);
        }

        private async void FileScanner_FileScanCompleted(object sender, IEnumerable<IFileData> e)
        {
            Logger.LogInformation($"File scan completed");
            await RemoveMissingMetadatasAsync(e);
        }

        private async Task RemoveMissingMetadatasAsync(IEnumerable<IFileData> discoveredFiles)
        {
            Logger.LogInformation($"Pruning missing metadata.");
            var tracks = await Tracks.GetItemsAsync(0, -1);
            var removedTracks = tracks.Where(track => discoveredFiles.All(c => c.Path != track.Url)).ToList();

            foreach (var removedTrack in removedTracks)
            {
                await Tracks.RemoveAsync(removedTrack);

                if (removedTrack.ArtistIds != null)
                {
                    foreach (var artistId in removedTrack.ArtistIds)
                    {
                        var relatedArtist = await AlbumArtists.GetByIdAsync(artistId);

                        if (relatedArtist == null)
                            continue;

                        // Do not remove artists if it has more than 1 tracks.
                        if (relatedArtist.TrackIds?.Count == 1)
                            await AlbumArtists.RemoveAsync(relatedArtist);
                    }
                }

                if (removedTrack.AlbumId == null)
                    return;

                var relatedAlbum = await Albums.GetByIdAsync(removedTrack.AlbumId);
                if (relatedAlbum != null)
                {
                    // Do not remove album if it has more than 1 tracks.
                    if (relatedAlbum.TrackIds?.Count == 1)
                        await Albums.RemoveAsync(relatedAlbum);
                }
            }
        }

        private void OnFilesDiscovered(object sender, IEnumerable<IFileData> e)
        {
            _knownFiles.AddRange(e);
            var count = e.Count();
            Logger.LogInformation($"{count} files discovered");

            ScanState = new FileScanState(FileScanStage.FileDiscovery, ScanState.FilesProcessed, ScanState.FilesFound + count);
        }

        private async void AudioMetadataScanner_FileMetadataAdded(object sender, IEnumerable<Models.FileMetadata> e)
        {
            var fileMetadata = e as Models.FileMetadata[] ?? e.ToArray();

            var imageMetadata = fileMetadata.Where(x => x.ImageMetadata != null).SelectMany(x => x.ImageMetadata).ToArray();
            var trackMetadata = fileMetadata.Select(x => x.TrackMetadata).PruneNull().ToArray();
            var albumArtists = fileMetadata.Select(x => x.AlbumArtistMetadata).SelectMany(x => x).PruneNull().ToArray();
            var trackArtists = fileMetadata.Select(x => x.TrackArtistMetadata).SelectMany(x => x).PruneNull().ToArray();
            var albumMetadata = fileMetadata.Select(x => x.AlbumMetadata).PruneNull().ToArray();

            // Artists and Albums reference each other, so update repos in parallel
            // and cross your fingers that they internally add all data before emitting changed events 
            // and that one doesn't finish first.
            await Task.WhenAll(AlbumArtists.AddOrUpdateAsync(albumArtists), TrackArtists.AddOrUpdateAsync(trackArtists), Albums.AddOrUpdateAsync(albumMetadata));

            await Images.AddOrUpdateAsync(imageMetadata);
            await Tracks.AddOrUpdateAsync(trackMetadata);
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

        /// <inheritdoc />
        public IAlbumRepository Albums { get; }

        /// <inheritdoc />
        public IArtistRepository AlbumArtists { get; }

        /// <inheritdoc />
        public IArtistRepository TrackArtists { get; }

        /// <inheritdoc />
        public ITrackRepository Tracks { get; }

        /// <inheritdoc />
        public IPlaylistRepository Playlists { get; }

        /// <inheritdoc/>
        public IImageRepository Images { get; }

        /// <inheritdoc/>
        public async Task<Stream?> GetImageStreamById(string imageId)
        {
            var fileId = _audioMetadataScanner.GetFileIdFromImageId(imageId);

            // Check if we've seen the file already
            var targetFile = _knownFiles.FirstOrDefault(x => x.Id == fileId);
            if (targetFile is null)
            {
                // This method can't be used before a scan is kicked off in the core.
                // If we haven't seen the file yet, check if there's an active scan.
                if (_inProgressScanCancellationTokenSource is null)
                {
                    // If there's no active scan and we haven't seen the file, the file may not exist. Return nothing.
                    return null;
                }

                var taskCompletionSource = new TaskCompletionSource<IFileData?>();

                // Wait for active scan to find the file
                _fileScanner.FilesDiscovered += OnFileDiscovered;
                _fileScanner.FileDiscoveryCompleted += OnFileScanComplete;

                targetFile = await taskCompletionSource.Task;

                _fileScanner.FilesDiscovered -= OnFileDiscovered;
                _fileScanner.FileDiscoveryCompleted -= OnFileScanComplete;

                void OnFileDiscovered(object sender, IEnumerable<IFileData> e)
                {
                    Guard.IsNotNull(taskCompletionSource);
                    if (e.FirstOrDefault(x => x.Id == fileId) is { } file)
                        taskCompletionSource.SetResult(file);
                }

                void OnFileScanComplete(object sender, IEnumerable<IFileData> e)
                {
                    // If the scan ends and the file still hasn't been seen, return nothing.
                    Guard.IsNotNull(taskCompletionSource);
                    taskCompletionSource.SetResult(e.FirstOrDefault(x => x.Id == fileId));
                }
            }

            if (targetFile is null)
                return null;

            return await _audioMetadataScanner.GetImageStream(targetFile, imageId);
        }

        /// <inheritdoc />
        public bool SkipRepoInit { get; set; }

        /// <inheritdoc />
        public MetadataScanTypes ScanTypes
        {
            get => _audioMetadataScanner.ScanTypes;
            set => _audioMetadataScanner.ScanTypes = value;
        }

        /// <inheritdoc />
        public int DegreesOfParallelism => 2;

        /// <inheritdoc />
        public async Task ScanAsync(CancellationToken cancellationToken = default)
        {
            Logger.LogInformation($"Scan started");

            if (_inProgressScanCancellationTokenSource is not null)
            {
                _inProgressScanCancellationTokenSource.Cancel();
                _inProgressScanCancellationTokenSource.Dispose();
            }

            _inProgressScanCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            var currentToken = _inProgressScanCancellationTokenSource.Token;

            currentToken.ThrowIfCancellationRequested();

            if (!IsInitialized)
                await InitAsync(currentToken);

            ScanningStarted?.Invoke(this, EventArgs.Empty);

            if (!SkipRepoInit)
            {
                Logger.LogInformation($"Initializing repositories");
                await Albums.InitAsync(currentToken);
                await AlbumArtists.InitAsync(currentToken);
                await TrackArtists.InitAsync(currentToken);
                await Tracks.InitAsync(currentToken);
                await Playlists.InitAsync(currentToken);
                await Images.InitAsync(currentToken);
            }

            Logger.LogInformation($"Starting recursive file discovery in {_rootFolderToScan.Path}");
            ScanState = new FileScanState(FileScanStage.FileDiscovery, ScanState.FilesProcessed, ScanState.FilesFound);
            await _fileScanner.ScanFolderAsync(cancellationToken);

            cancellationToken.ThrowIfCancellationRequested();

            if (_knownFiles.Count == 0)
                return;

            Logger.LogInformation($"Starting metadata scan of audio files");
            ScanState = new FileScanState(FileScanStage.AudioFiles, ScanState.FilesProcessed, ScanState.FilesFound);
            var fileMetadata = await _audioMetadataScanner.ScanMusicFilesAsync(_knownFiles, currentToken).Select(item => item.Metadata).ToListAsync(cancellationToken: currentToken);

            currentToken.ThrowIfCancellationRequested();

            Logger.LogInformation($"Starting metadata scan of playlist files");
            ScanState = new FileScanState(FileScanStage.Playlists, ScanState.FilesProcessed, ScanState.FilesFound);
            await _playlistMetadataScanner.ScanPlaylistsAsync(_knownFiles, fileMetadata, currentToken).ToListAsync(cancellationToken: currentToken);

            currentToken.ThrowIfCancellationRequested();

            ScanState = new FileScanState(FileScanStage.Complete, ScanState.FilesProcessed, ScanState.FilesFound);
            ScanningCompleted?.Invoke(this, EventArgs.Empty);
            _inProgressScanCancellationTokenSource = null;
        }

        /// <inheritdoc />
        public ValueTask DisposeAsync()
        {
            Albums.Dispose();
            AlbumArtists.Dispose();
            TrackArtists.Dispose();
            Playlists.Dispose();
            Tracks.Dispose();
            Images.Dispose();

            _fileScanner.Dispose();
            _playlistMetadataScanner.Dispose();

            DetachEvents();
            return default;
        }
    }
}
