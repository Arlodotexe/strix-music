using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Toolkit.Diagnostics;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using OwlCore.AbstractStorage;
using OwlCore.AbstractUI.Models;
using StrixMusic.Sdk.Services.FileMetadataManager.MetadataScanner;
using StrixMusic.Sdk.Services.Notifications;

namespace StrixMusic.Sdk.Services.FileMetadataManager
{
    /// <summary>
    /// The type of files being scanned.
    /// </summary>
    public enum FileScanningType
    {
        /// <summary>
        /// Not currently scanning files.
        /// </summary>
        None,

        /// <summary>
        /// Files containing raw audio.
        /// </summary>
        AudioFiles,

        /// <summary>
        /// Files containing playlist metadata.
        /// </summary>
        Playlists
    }

    /// <inheritdoc cref="IFileMetadataManager" />
    public class FileMetadataManager : IFileMetadataManager, IDisposable
    {
        private static string NewGuid() => Guid.NewGuid().ToString();

        private readonly string _instanceId;
        private readonly IFileScanner _fileScanner;
        private readonly AudioMetadataScanner _audioMetadataScanner;
        private readonly PlaylistMetadataScanner _playlistMetadataScanner;
        private readonly INotificationService? _notificationService;

        private Notification? _filesScannedNotification;
        private Notification? _filesFoundNotification;
        private AbstractProgressUIElement? _progressUIElement;

        private FileScanningType _currentScanningType;
        private IFolderData _rootFolder;
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
            _audioMetadataScanner = new AudioMetadataScanner(_fileScanner, this);
            _playlistMetadataScanner = new PlaylistMetadataScanner(this, _audioMetadataScanner, _fileScanner);

            Tracks = new TrackRepository(_audioMetadataScanner);
            Playlists = new PlaylistRepository(_playlistMetadataScanner);
            Albums = new AlbumRepository(_audioMetadataScanner, Tracks);
            Artists = new ArtistRepository(_audioMetadataScanner, Tracks);

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

            if (!SkipRepoInit)
            {
                await Albums.InitAsync();
                await Artists.InitAsync();
                await Tracks.InitAsync();
                await Playlists.InitAsync();
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
        }

        private void DetachEvents()
        {
            _fileScanner.FilesDiscovered -= OnFilesDiscovered;
        }

        private void OnFilesDiscovered(object sender, System.Collections.Generic.IEnumerable<IFileData> e)
        {
            FilesFound += e.Count();
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
        public AlbumRepository Albums { get; private set; }

        /// <inheritdoc />
        public ArtistRepository Artists { get; private set; }

        /// <inheritdoc />
        public PlaylistRepository Playlists { get; private set; }

        /// <inheritdoc />
        public TrackRepository Tracks { get; private set; }

        /// <inheritdoc />
        public bool SkipRepoInit { get; set; }

        /// <inheritdoc />
        public async Task StartScan()
        {
            if (!IsInitialized)
                await InitAsync();

            ScanningStarted?.Invoke(this, EventArgs.Empty);

            if (!SkipRepoInit)
            {
                await Albums.InitAsync();
                await Artists.InitAsync();
                await Tracks.InitAsync();
                await Playlists.InitAsync();
            }

            var findingFilesNotif = RaiseFileDiscoveryNotification();
            var discoveredFiles = await _fileScanner.ScanFolder(_rootFolder);
            findingFilesNotif.Dismiss();

            _currentScanningType = FileScanningType.AudioFiles;
            var scanningMusicNotif = RaiseProcessingNotification();
            var fileMetadata = await _audioMetadataScanner.ScanMusicFiles(discoveredFiles);
            scanningMusicNotif.Dismiss();


            _currentScanningType = FileScanningType.Playlists;
            var scanningPlaylistsNotif = RaiseProcessingNotification();
            await _playlistMetadataScanner.ScanPlaylists(discoveredFiles, fileMetadata);
            scanningPlaylistsNotif.Dismiss();

            ScanningCompleted?.Invoke(this, EventArgs.Empty);
        }

        private void UpdateFilesScannedNotification()
        {
            Guard.IsNotNull(_filesScannedNotification, nameof(_filesScannedNotification));

            _filesScannedNotification.AbstractUIElementGroup.Subtitle = $"Scanned {FilesProcessed}/{FilesFound} in {_rootFolder.Path}";
        }

        private void UpdateFilesFoundNotification()
        {
            Guard.IsNotNull(_filesFoundNotification, nameof(_filesFoundNotification));

            _filesFoundNotification.AbstractUIElementGroup.Subtitle = $"Found {FilesFound} in {_rootFolder.Path}";
        }

        private Notification RaiseFileDiscoveryNotification()
        {
            Guard.IsNotNull(_notificationService, nameof(_notificationService));

            var elementGroup = new AbstractUIElementGroup(NewGuid())
            {
                Title = "Discovering files",
                Subtitle = $"Found {FilesFound} in {_rootFolder.Path}",
                Items = { new AbstractProgressUIElement(NewGuid(), null) },
            };

            return _filesFoundNotification = _notificationService.RaiseNotification(elementGroup);
        }

        private Notification RaiseProcessingNotification()
        {
            Guard.IsNotNull(_notificationService, nameof(_notificationService));

            _progressUIElement = new AbstractProgressUIElement(NewGuid(), FilesProcessed, FilesFound);

            var scanningTypeStr = _currentScanningType switch
            {
                FileScanningType.AudioFiles => "music",
                FileScanningType.Playlists => "playlists",
                _ => ThrowHelper.ThrowArgumentOutOfRangeException<string>(),
            };

            var elementGroup = new AbstractUIElementGroup(NewGuid())
            {
                Title = $"Scanning {scanningTypeStr}",
                Subtitle = $"Scanned {FilesProcessed}/{FilesFound} in {_rootFolder.Path}",
                Items = { _progressUIElement },
            };

            return _filesScannedNotification = _notificationService.RaiseNotification(elementGroup);
        }

        private void ReleaseUnmanagedResources()
        {
            DetachEvents();
        }

        /// <inheritdoc />
        public ValueTask DisposeAsync()
        {
            Albums.Dispose();
            Artists.Dispose();
            Playlists.Dispose();
            Tracks.Dispose();

            _fileScanner.Dispose();
            _playlistMetadataScanner.Dispose();

            _filesFoundNotification?.Dismiss();
            _filesScannedNotification?.Dismiss();

            return default;
        }

        /// <inheritdoc cref="Dispose()"/>
        protected virtual void Dispose(bool disposing)
        {
            if (!IsInitialized)
                return;

            if (disposing)
            {
                // dispose any objects you created here
                ReleaseUnmanagedResources();
            }

            IsInitialized = false;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc />
        ~FileMetadataManager()
        {
            Dispose(false);
        }
    }
}