// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using OwlCore.AbstractStorage;
using OwlCore.AbstractStorage.Scanners;
using OwlCore.AbstractUI.Models;
using OwlCore.Extensions;
using OwlCore.Services;
using StrixMusic.Sdk.AppModels;
using StrixMusic.Sdk.FileMetadata.Repositories;
using StrixMusic.Sdk.FileMetadata.Scanners;
using StrixMusic.Sdk.Services;

namespace StrixMusic.Sdk.FileMetadata
{
    /// <summary>
    /// Given an OwlCore.AbstractStorage implementation, this manages scanning and caching all the music metadata from files in folder, including child folders.
    /// </summary>
    public sealed class FileMetadataManager : IFileMetadataManager
    {
        private static string NewGuid() => Guid.NewGuid().ToString();

        private readonly IFileScanner _fileScanner;
        private readonly AudioMetadataScanner _audioMetadataScanner;
        private readonly PlaylistMetadataScanner _playlistMetadataScanner;
        private readonly INotificationService? _notificationService;
        private readonly IFolderData _rootFolderToScan;
        private readonly IFolderData _metadataStorage;

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
        /// <param name="rootFolderToScan">The folder where data is scanned.</param>
        /// <param name="metadataStorage">The folder the metadata manager can persist metadata.</param>
        /// <param name="notificationService">An optional notification service for notifying the user with dynamic data.</param>
        public FileMetadataManager(IFolderData rootFolderToScan, IFolderData metadataStorage, INotificationService? notificationService = null)
        {
            _notificationService = notificationService;
            _fileScanner = new DepthFirstFileScanner(rootFolderToScan);
            _audioMetadataScanner = new AudioMetadataScanner(this);
            _playlistMetadataScanner = new PlaylistMetadataScanner(this, _audioMetadataScanner, _fileScanner);

            Images = new ImageRepository();
            Tracks = new TrackRepository();
            Albums = new AlbumRepository();
            Artists = new ArtistRepository();
            Playlists = new PlaylistRepository(_playlistMetadataScanner);

            _rootFolderToScan = rootFolderToScan;
            _metadataStorage = metadataStorage;
        }

        /// <inheritdoc />
        public async Task InitAsync(CancellationToken cancellationToken = default)
        {
            Guard.IsFalse(IsInitialized, nameof(IsInitialized));
            IsInitialized = true;

            Logger.LogInformation($"Setting up repository data location to {_metadataStorage.Path}");
            _audioMetadataScanner.CacheFolder = _metadataStorage;

            Albums.SetDataFolder(_metadataStorage);
            Artists.SetDataFolder(_metadataStorage);
            Tracks.SetDataFolder(_metadataStorage);
            Playlists.SetDataFolder(_metadataStorage);
            Images.SetDataFolder(_metadataStorage);

            if (!SkipRepoInit)
            {
                Logger.LogInformation($"Initializing repositories.");
                await Albums.InitAsync(cancellationToken);
                await Artists.InitAsync(cancellationToken);
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

        private void OnFilesDiscovered(object sender, IEnumerable<IFileData> e)
        {
            var count = e.Count();
            Logger.LogInformation($"{count} files discovered");

            FilesFound += count;
        }

        private async void AudioMetadataScanner_FileMetadataAdded(object sender, IEnumerable<Models.FileMetadata> e)
        {
            var fileMetadata = e as Models.FileMetadata[] ?? e.ToArray();

            var imageMetadata = fileMetadata.Where(x => x.ImageMetadata != null).SelectMany(x => x.ImageMetadata).ToArray();
            var trackMetadata = fileMetadata.Select(x => x.TrackMetadata).PruneNull().ToArray();
            var artistMetadatas = fileMetadata.Select(x => x.ArtistMetadataCollection).PruneNull().ToArray();
            var albumMetadata = fileMetadata.Select(x => x.AlbumMetadata).PruneNull().ToArray();

            // Artists and albums reference each other, so update repos in parallel
            // and cross your fingers that they internally add all data before emitting changed events 
            // and that one doesn't finish first.

            foreach (var artist in artistMetadatas)
            {
                Guard.IsNotNull(artist, nameof(artist));
                await Task.WhenAll(Artists.AddOrUpdateAsync(artist.PruneNull().ToArray()), Albums.AddOrUpdateAsync(albumMetadata));
            }

            await Images.AddOrUpdateAsync(imageMetadata);
            await Tracks.AddOrUpdateAsync(trackMetadata);
        }

        /// <inheritdoc />
        public bool IsInitialized { get; private set; }

        /// <summary>
        /// Gets the number of found files
        /// </summary>
        public int FilesFound
        {
            get => _filesFound;
            internal set
            {
                _filesFound = value;
                if (_progressUIElement != null)
                    _progressUIElement.Maximum = value;

                UpdateFilesFoundNotification();
            }
        }

        /// <summary>
        /// Gets the number of processed files.
        /// </summary>
        public int FilesProcessed
        {
            get => _filesProcessed;
            internal set
            {
                _filesProcessed = value;

                if (_progressUIElement != null)
                    _progressUIElement.Value = value;

                UpdateFilesScannedNotification();
            }
        }

        /// <inheritdoc />
        public IAlbumRepository Albums { get; }

        /// <inheritdoc />
        public IArtistRepository Artists { get; }

        /// <inheritdoc />
        public IPlaylistRepository Playlists { get; }

        /// <inheritdoc />
        public ITrackRepository Tracks { get; }

        /// <inheritdoc/>
        public IImageRepository Images { get; }

        /// <inheritdoc />
        public bool SkipRepoInit { get; set; }

        /// <inheritdoc />
        public MetadataScanTypes ScanTypes { get; set; } = MetadataScanTypes.TagLib | MetadataScanTypes.FileProperties;

        /// <inheritdoc />
        public int DegreesOfParallelism { get; set; } = 2;

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

            DismissNotifs();

            if (!IsInitialized)
                await InitAsync(currentToken);

            ScanningStarted?.Invoke(this, EventArgs.Empty);

            if (!SkipRepoInit)
            {
                Logger.LogInformation($"Initializing repositories");
                await Albums.InitAsync(currentToken);
                await Artists.InitAsync(currentToken);
                await Tracks.InitAsync(currentToken);
                await Playlists.InitAsync(currentToken);
                await Images.InitAsync(currentToken);
            }

            CancelIfNeeded();
            FilesFound = 0;

            Logger.LogInformation($"Starting recursive file discovery in {_rootFolderToScan.Path}");

            var findingFilesNotif = RaiseFileDiscoveryNotification();
            var discoveredFiles = await _fileScanner.ScanFolderAsync(currentToken);
            var filesToScan = discoveredFiles as IFileData[] ?? discoveredFiles.ToArray();
            findingFilesNotif?.Dismiss();

            CancelIfNeeded();
            if (filesToScan.Length == 0)
                return;

            FilesProcessed = 0;

            Logger.LogInformation($"Starting metadata scan of audio files");

            _currentScanningType = FileScanningType.AudioFiles;
            var scanningMusicNotif = RaiseProcessingNotification();
            var fileMetadata = await _audioMetadataScanner.ScanMusicFilesAsync(filesToScan, currentToken);
            scanningMusicNotif?.Dismiss();

            CancelIfNeeded();

            Logger.LogInformation($"Starting metadata scan of playlist files");

            _currentScanningType = FileScanningType.Playlists;
            var scanningPlaylistsNotif = RaiseProcessingNotification();
            await _playlistMetadataScanner.ScanPlaylists(filesToScan, fileMetadata, currentToken);
            scanningPlaylistsNotif?.Dismiss();

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

            _filesScannedNotification.AbstractUICollection.Subtitle = $"Scanned {FilesProcessed}/{FilesFound} in {_rootFolderToScan.Path}";
        }

        private void UpdateFilesFoundNotification()
        {
            if (_filesFoundNotification is null)
                return;

            _filesFoundNotification.AbstractUICollection.Subtitle = $"Found {FilesFound} in {_rootFolderToScan.Path}";
        }

        private Notification? RaiseFileDiscoveryNotification()
        {
            if (_notificationService is null)
                return null;

            var elementGroup = new AbstractUICollection(NewGuid())
            {
                Title = "Discovering files",
                Subtitle = $"Found {FilesFound} in {_rootFolderToScan.Path}",
            };

            elementGroup.Add(new AbstractProgressIndicator(NewGuid(), true));

            return _filesFoundNotification = _notificationService.RaiseNotification(elementGroup);
        }

        private Notification? RaiseProcessingNotification()
        {
            if (_notificationService is null)
                return null;

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
                Subtitle = $"Scanned {FilesProcessed}/{FilesFound} in {_rootFolderToScan.Path}",
            };

            elementGroup.Add(_progressUIElement);

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
