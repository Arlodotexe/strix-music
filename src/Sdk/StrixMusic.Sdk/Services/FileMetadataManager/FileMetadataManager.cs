using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Toolkit.Diagnostics;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using OwlCore.AbstractStorage;
using OwlCore.AbstractStorage.Scanners;
using OwlCore.AbstractUI.Models;
using OwlCore.Extensions;
using StrixMusic.Sdk.Services.FileMetadataManager.MetadataScanner;
using StrixMusic.Sdk.Services.FileMetadataManager.Models;
using StrixMusic.Sdk.Services.FileMetadataManager.Repositories;
using StrixMusic.Sdk.Services.Notifications;

namespace StrixMusic.Sdk.Services.FileMetadataManager
{
    /// <inheritdoc cref="IFileMetadataManager" />
    public class FileMetadataManager : IFileMetadataManager
    {
        private static string NewGuid() => Guid.NewGuid().ToString();

        private readonly string _instanceId;
        private readonly IFileScanner _fileScanner;
        private readonly AudioMetadataScanner _audioMetadataScanner;
        private readonly PlaylistMetadataScanner _playlistMetadataScanner;
        private readonly INotificationService? _notificationService;
        private readonly IFolderData _rootFolder;

        private CancellationTokenSource? _inProgressScanCancellationTokenSource;
        private Notification? _filesScannedNotification;
        private Notification? _filesFoundNotification;
        private AbstractProgressIndicator? _progressUIElement;

        private FileScanningType _currentScanningType;
        private int _filesFound;
        private int _filesProcessed;

        /// <summary>
        /// Creates a new instance of <see cref="FileMetadataManager"/>.
        /// </summary>
        /// <param name="instanceId">A unique identifier that helps decide where the scanned data is stored.</param>
        /// <param name="rootFolder">The folder where data is scanned.</param>
        public FileMetadataManager(string instanceId, IFolderData rootFolder)
        {
            _instanceId = instanceId;

            _notificationService = Ioc.Default.GetRequiredService<INotificationService>();
            _fileScanner = new DepthFirstFileScanner(rootFolder);
            _audioMetadataScanner = new AudioMetadataScanner(this);
            _playlistMetadataScanner = new PlaylistMetadataScanner(this, _audioMetadataScanner, _fileScanner);

            Images = new ImageRepository();
            Tracks = new TrackRepository();
            Albums = new AlbumRepository();
            Artists = new ArtistRepository();
            Playlists = new PlaylistRepository(_playlistMetadataScanner);

            _rootFolder = rootFolder;
        }

        /// <inheritdoc />
        public async Task InitAsync()
        {
            Guard.IsFalse(IsInitialized, nameof(IsInitialized));
            IsInitialized = true;

            var dataFolder = await GetDataStorageFolder(_instanceId);

            _audioMetadataScanner.CacheFolder = dataFolder;

            Albums.SetDataFolder(dataFolder);
            Artists.SetDataFolder(dataFolder);
            Tracks.SetDataFolder(dataFolder);
            Playlists.SetDataFolder(dataFolder);
            Images.SetDataFolder(dataFolder);

            if (!SkipRepoInit)
            {
                await Albums.InitAsync();
                await Artists.InitAsync();
                await Tracks.InitAsync();
                await Playlists.InitAsync();
                await Images.InitAsync();
            }

            AttachEvents();
        }

        private static async Task<IFolderData> GetDataStorageFolder(string instanceId)
        {
            var primaryFileSystemService = Ioc.Default.GetRequiredService<IFileSystemService>();

            var path = Path.Combine(primaryFileSystemService.RootFolder.Path, instanceId, nameof(FileMetadataManager));

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            var folderData = await primaryFileSystemService.GetFolderFromPathAsync(path);

            Guard.IsNotNull(folderData, nameof(folderData));

            return folderData;
        }

        /// <inheritdoc />
        public event EventHandler? ScanningStarted;

        /// <inheritdoc />
        public event EventHandler? ScanningCompleted;

        private void AttachEvents()
        {
            _fileScanner.FilesDiscovered += OnFilesDiscovered;
            _audioMetadataScanner.FileMetadataAdded += AudioMetadataScanner_FileMetadataAdded;
            _fileScanner.FileDiscoveryCompleted += FileScanner_FileScanCompleted;
        }

        private void DetachEvents()
        {
            _fileScanner.FilesDiscovered -= OnFilesDiscovered;
            _audioMetadataScanner.FileMetadataAdded -= AudioMetadataScanner_FileMetadataAdded;
            _fileScanner.FileDiscoveryCompleted -= FileScanner_FileScanCompleted;
        }

        private async void FileScanner_FileScanCompleted(object sender, IEnumerable<IFileData> e)
        {
            await RemoveMissingMetadatasAsync(e);
        }

        private async Task RemoveMissingMetadatasAsync(IEnumerable<IFileData> e)
        {
            var tracks = await Tracks.GetItemsAsync(0, -1);
            var removedTracks = new List<TrackMetadata>();

            var discoveredFiles = e;

            foreach (var track in tracks)
            {
                if (!discoveredFiles.Any(c => new Uri(c.Path).AbsoluteUri == track.Url?.AbsoluteUri))
                {
                    removedTracks.Add(track);
                }
            }

            foreach (var removedTrack in removedTracks)
            {
                await Tracks.RemoveAsync(removedTrack);

                if (removedTrack.ArtistIds != null)
                {
                    foreach (var artistId in removedTrack.ArtistIds)
                    {
                        var relatedArtist = await Artists.GetByIdAsync(artistId);

                        if (relatedArtist == null)
                            continue;

                        // Do not remove artists if it has more than 1 tracks.
                        if (relatedArtist.TrackIds?.Count == 1)
                            await Artists.RemoveAsync(relatedArtist);
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

        private void OnFilesDiscovered(object sender, System.Collections.Generic.IEnumerable<IFileData> e)
        {
            FilesFound += e.Count();
        }

        private async void AudioMetadataScanner_FileMetadataAdded(object sender, IEnumerable<FileMetadata> e)
        {
            var fileMetadata = e as FileMetadata[] ?? e.ToArray();

            var imageMetadata = fileMetadata.Where(x => x.ImageMetadata != null).SelectMany(x => x.ImageMetadata).ToArray();
            var trackMetadata = fileMetadata.Select(x => x.TrackMetadata).PruneNull().ToArray();
            var artistMetadata = fileMetadata.Select(x => x.ArtistMetadata).PruneNull().ToArray();
            var albumMetadata = fileMetadata.Select(x => x.AlbumMetadata).PruneNull().ToArray();

            await Images.AddOrUpdateAsync(imageMetadata);
            await Tracks.AddOrUpdateAsync(trackMetadata);

            // Artists and albums reference each other, so update repos in parallel
            // and cross your fingers that they internally add all data before emitting changed events 
            // and that one doesn't finish first.
            await Task.WhenAll(Artists.AddOrUpdateAsync(artistMetadata), Albums.AddOrUpdateAsync(albumMetadata));
        }

        /// <inheritdoc />
        public bool IsInitialized { get; private set; }

        /// <summary>
        /// Keeps the count of found files
        /// </summary>
        public int FilesFound
        {
            get => _filesFound;
            set
            {
                _filesFound = value;
                if (_progressUIElement != null)
                    _progressUIElement.Maximum = value;

                UpdateFilesFoundNotification();
            }
        }

        /// <summary>
        /// Keeps the count of processed file.
        /// </summary>
        public int FilesProcessed
        {
            get => _filesProcessed;
            set
            {
                _filesProcessed = value;

                if (_progressUIElement != null)
                    _progressUIElement.Value = value;

                UpdateFilesScannedNotification();
            }
        }

        /// <inheritdoc />
        public AlbumRepository Albums { get; }

        /// <inheritdoc />
        public ArtistRepository Artists { get; }

        /// <inheritdoc />
        public PlaylistRepository Playlists { get; }

        /// <inheritdoc />
        public TrackRepository Tracks { get; }

        /// <inheritdoc/>
        public ImageRepository Images { get; private set; }

        /// <inheritdoc />
        public bool SkipRepoInit { get; set; }

        /// <inheritdoc />
        public MetadataScanTypes ScanTypes { get; set; } = MetadataScanTypes.TagLib | MetadataScanTypes.FileProperties;

        /// <inheritdoc />
        public int DegreesOfParallelism { get; set; } = 2;

        /// <inheritdoc />
        public async Task StartScan()
        {
            if (!(_inProgressScanCancellationTokenSource is null))
            {
                _inProgressScanCancellationTokenSource.Cancel();
                _inProgressScanCancellationTokenSource.Dispose();
            }

            _inProgressScanCancellationTokenSource = new CancellationTokenSource();
            var currentToken = _inProgressScanCancellationTokenSource.Token;

            DismissNotifs();

            if (!IsInitialized)
                await InitAsync();

            ScanningStarted?.Invoke(this, EventArgs.Empty);

            if (!SkipRepoInit)
            {
                await Albums.InitAsync();
                await Artists.InitAsync();
                await Tracks.InitAsync();
                await Playlists.InitAsync();
                await Images.InitAsync();
            }

            CancelIfNeeded();
            FilesFound = 0;

            var findingFilesNotif = RaiseFileDiscoveryNotification();
            var discoveredFiles = await _fileScanner.ScanFolderAsync(currentToken);
            var filesToScan = discoveredFiles as IFileData[] ?? discoveredFiles.ToArray();
            findingFilesNotif.Dismiss();

            CancelIfNeeded();
            FilesProcessed = 0;

            _currentScanningType = FileScanningType.AudioFiles;
            var scanningMusicNotif = RaiseProcessingNotification();
            var fileMetadata = await _audioMetadataScanner.ScanMusicFilesAsync(filesToScan, currentToken);
            scanningMusicNotif.Dismiss();

            CancelIfNeeded();

            _currentScanningType = FileScanningType.Playlists;
            var scanningPlaylistsNotif = RaiseProcessingNotification();
            await _playlistMetadataScanner.ScanPlaylists(filesToScan, fileMetadata, currentToken);
            scanningPlaylistsNotif.Dismiss();

            CancelIfNeeded();

            ScanningCompleted?.Invoke(this, EventArgs.Empty);

            void CancelIfNeeded()
            {
                if (currentToken.IsCancellationRequested)
                {
                    DismissNotifs();
                    currentToken.ThrowIfCancellationRequested();
                }
            }

            void DismissNotifs()
            {
                _filesScannedNotification?.Dismiss();
                _filesFoundNotification?.Dismiss();
            }
        }

        private void UpdateFilesScannedNotification()
        {
            if (_filesScannedNotification is null)
                return;

            _filesScannedNotification.AbstractUICollection.Subtitle = $"Scanned {FilesProcessed}/{FilesFound} in {_rootFolder.Path}";
        }

        private void UpdateFilesFoundNotification()
        {
            if (_filesFoundNotification is null)
                return;

            _filesFoundNotification.AbstractUICollection.Subtitle = $"Found {FilesFound} in {_rootFolder.Path}";
        }

        private Notification RaiseFileDiscoveryNotification()
        {
            Guard.IsNotNull(_notificationService, nameof(_notificationService));

            var elementGroup = new AbstractUICollection(NewGuid())
            {
                Title = "Discovering files",
                Subtitle = $"Found {FilesFound} in {_rootFolder.Path}",
                Items = new AbstractProgressIndicator(NewGuid(), null).IntoList(),
            };

            return _filesFoundNotification = _notificationService.RaiseNotification(elementGroup);
        }

        private Notification RaiseProcessingNotification()
        {
            Guard.IsNotNull(_notificationService, nameof(_notificationService));

            _progressUIElement = new AbstractProgressIndicator(NewGuid(), FilesProcessed, FilesFound);

            var scanningTypeStr = _currentScanningType switch
            {
                FileScanningType.AudioFiles => "music",
                FileScanningType.Playlists => "playlists",
                _ => ThrowHelper.ThrowArgumentOutOfRangeException<string>(),
            };

            var elementGroup = new AbstractUICollection(NewGuid())
            {
                Title = $"Scanning {scanningTypeStr}",
                Subtitle = $"Scanned {FilesProcessed}/{FilesFound} in {_rootFolder.Path}",
                Items = _progressUIElement.IntoList(),
            };

            return _filesScannedNotification = _notificationService.RaiseNotification(elementGroup);
        }

        /// <inheritdoc />
        public ValueTask DisposeAsync()
        {
            Albums.Dispose();
            Artists.Dispose();
            Playlists.Dispose();
            Tracks.Dispose();
            Images.Dispose();

            _fileScanner.Dispose();
            _playlistMetadataScanner.Dispose();

            _filesFoundNotification?.Dismiss();
            _filesScannedNotification?.Dismiss();

            DetachEvents();
            return default;
        }
    }
}