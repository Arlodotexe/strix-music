using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Toolkit.Diagnostics;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using OwlCore.AbstractStorage;
using OwlCore.AbstractUI.Models;
using OwlCore.Extensions;
using StrixMusic.Sdk.Services.FileMetadataManager.MetadataScanner;
using StrixMusic.Sdk.Services.FileMetadataManager.Models;
using StrixMusic.Sdk.Services.Notifications;

namespace StrixMusic.Sdk.Services.FileMetadataManager
{
    /// <inheritdoc cref="IFileMetadataManager" />
    public class FileMetadataManager : IFileMetadataManager, IDisposable
    {
        private readonly string _instanceId;
        private readonly FileMetadataScanner _fileMetadataScanner;
        private readonly PlaylistMetadataScanner _playlistMetadataScanner;
        private readonly INotificationService? _notificationService;
        private Notification? _filesScannedNotification;
        private Notification? _filesFoundNotification;
        private int _filesFound;
        private int _filesProcessed;
        private IFolderData _rootFolder;
        private AbstractProgressUIElement? _progressUIElement;

        /// <summary>
        /// Creates a new instance of <see cref="FileMetadataManager"/>.
        /// </summary>
        /// <param name="instanceId">A unique identifier that helps decide where the scanned data is stored.</param>
        /// <param name="rootFolder">The folder where data is scanned.</param>
        public FileMetadataManager(string instanceId, IFolderData rootFolder)
        {
            _instanceId = instanceId;

            _fileMetadataScanner = new FileMetadataScanner(rootFolder);
            _playlistMetadataScanner = new PlaylistMetadataScanner(rootFolder, _fileMetadataScanner);
            _notificationService = Ioc.Default.GetRequiredService<INotificationService>();

            Tracks = new TrackRepository(_fileMetadataScanner);
            Playlists = new PlaylistRepository(_playlistMetadataScanner);
            Albums = new AlbumRepository(_fileMetadataScanner, Tracks);
            Artists = new ArtistRepository(_fileMetadataScanner, Tracks);

            _rootFolder = rootFolder;
        }

        /// <inheritdoc />
        public async Task InitAsync()
        {
            Guard.IsFalse(IsInitialized, nameof(IsInitialized));
            IsInitialized = true;

            var dataFolder = await GetDataStorageFolder(_instanceId);

            _fileMetadataScanner.CacheFolder = dataFolder;

            Albums.SetDataFolder(dataFolder);
            Artists.SetDataFolder(dataFolder);
            Tracks.SetDataFolder(dataFolder);
            Playlists.SetDataFolder(dataFolder);

            await Albums.InitAsync();
            await Artists.InitAsync();
            await Tracks.InitAsync();
            await Playlists.InitAsync();

            AttachEvents();
        }

        private void AttachEvents()
        {
            _fileMetadataScanner.FilesFoundCountUpdated += FileMetadataScanner_FilesFoundCountUpdated;
            _fileMetadataScanner.FileDiscoveryCompleted += FileMetadataScanner_FileDiscoveryCompleted;
            _fileMetadataScanner.FilesProcessedCountUpdated += FileMetadataScanner_FilesProcessedCountUpdated;
            _playlistMetadataScanner.PlaylistMetadataScanCompleted += PlaylistMetadataScanner_PlaylistMetadataScanCompleted;
            _playlistMetadataScanner.PlaylistMetadataProcessedFileCountUpdated += PlaylistMetadataScanner_PlaylistMetadataProcessedFileCountUpdated;
            _fileMetadataScanner.FileDiscoveryStarted += FileMetadataScanner_FileDiscoveryStarted;
        }

        private void DetachEvents()
        {
            _fileMetadataScanner.FilesFoundCountUpdated -= FileMetadataScanner_FilesFoundCountUpdated;
            _fileMetadataScanner.FileDiscoveryCompleted -= FileMetadataScanner_FileDiscoveryCompleted;
            _fileMetadataScanner.FilesProcessedCountUpdated -= FileMetadataScanner_FilesProcessedCountUpdated;
            _playlistMetadataScanner.PlaylistMetadataScanCompleted -= PlaylistMetadataScanner_PlaylistMetadataScanCompleted;
            _playlistMetadataScanner.PlaylistMetadataProcessedFileCountUpdated -= PlaylistMetadataScanner_PlaylistMetadataProcessedFileCountUpdated;
            _fileMetadataScanner.FileDiscoveryStarted -= FileMetadataScanner_FileDiscoveryStarted;
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

        private void FileMetadataScanner_FileDiscoveryStarted(object sender, EventArgs e)
        {
            _filesFoundNotification = RaiseStructureNotification();
        }

        private void FileMetadataScanner_FilesFoundCountUpdated(object sender, EventArgs e)
        {
            FilesFound++;
        }

        private void FileMetadataScanner_FileDiscoveryCompleted(object sender, System.Collections.Generic.IEnumerable<IFileData> e)
        {
            Guard.IsNotNull(_filesFoundNotification, nameof(_filesFoundNotification));

            _filesFoundNotification.Dismiss();

            _filesScannedNotification = RaiseProcessingNotification();
        }

        private void PlaylistMetadataScanner_PlaylistMetadataProcessedFileCountUpdated(object sender, EventArgs e) => FilesProcessed++;

        private void PlaylistMetadataScanner_PlaylistMetadataScanCompleted(object sender, EventArgs e)
        {
            Guard.IsNotNull(_filesScannedNotification, nameof(_filesScannedNotification));

            _filesScannedNotification.Dismiss();
        }

        private void FileMetadataScanner_FilesProcessedCountUpdated(object sender, EventArgs e) => FilesProcessed++;

        /// <inheritdoc />
        public event EventHandler? ScanningStarted;

        /// <inheritdoc />
        public event EventHandler? ScanningCompleted;

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

                UpdateFilesScanNotification();
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
        public async Task StartScan()
        {
            ScanningStarted?.Invoke(this, EventArgs.Empty);

            await Albums.InitAsync();
            await Artists.InitAsync();
            await Tracks.InitAsync();
            await Playlists.InitAsync();

            await _fileMetadataScanner.InitAsync();

            ScanningCompleted?.Invoke(this, EventArgs.Empty);
        }

        private void UpdateFilesScanNotification()
        {
            Guard.IsNotNull(_filesScannedNotification, nameof(_filesScannedNotification));

            _filesScannedNotification.AbstractUIElementGroup.Subtitle = $"Scanned {FilesProcessed}/{FilesFound} files in {_rootFolder.Path}";
        }

        private void UpdateFilesFoundNotification()
        {
            Guard.IsNotNull(_filesFoundNotification, nameof(_filesFoundNotification));

            _filesFoundNotification.AbstractUIElementGroup.Subtitle = $"Found {FilesFound} in {_rootFolder.Path}";
        }

        private Notification RaiseStructureNotification()
        {
            Guard.IsNotNull(_notificationService, nameof(_notificationService));

            static string NewGuid() => Guid.NewGuid().ToString();

            var elementGroup = new AbstractUIElementGroup(NewGuid())
            {
                Title = "Looking for files",
                Subtitle = $"Found {FilesFound} in {_rootFolder.Path}",
                Items = { new AbstractProgressUIElement(NewGuid(), null) },
            };

            return _notificationService.RaiseNotification(elementGroup);
        }

        private Notification RaiseProcessingNotification()
        {
            Guard.IsNotNull(_notificationService, nameof(_notificationService));

            static string NewGuid() => Guid.NewGuid().ToString();

            _progressUIElement = new AbstractProgressUIElement(NewGuid(), FilesProcessed, FilesFound);

            var elementGroup = new AbstractUIElementGroup(NewGuid())
            {
                Title = "Scanning library",
                Subtitle = $"Scanned {FilesProcessed}/{FilesFound} files in {_rootFolder.Path}",
                Items = { _progressUIElement },
            };

            return _notificationService.RaiseNotification(elementGroup);
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

            _fileMetadataScanner.Dispose();
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