using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Toolkit.Diagnostics;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using OwlCore.AbstractStorage;
using OwlCore.Extensions;
using StrixMusic.Sdk.Services.FileMetadataManager.MetadataScanner;
using StrixMusic.Sdk.Services.FileMetadataManager.Models;

namespace StrixMusic.Sdk.Services.FileMetadataManager
{
    /// <inheritdoc />
    public class FileMetadataManager : IFileMetadataManager
    {
        private readonly string _instanceId;
        private readonly FileMetadataScanner _fileMetadataScanner;

        /// <summary>
        /// Creates a new instance of <see cref="FileMetadataManager"/>.
        /// </summary>
        /// <param name="instanceId">A unique identifier that helps decide where the scanned data is stored.</param>
        /// <param name="rootFolder">The folder where data is scanned.</param>
        public FileMetadataManager(string instanceId, IFolderData rootFolder)
        {
            _instanceId = instanceId;

            _fileMetadataScanner = new FileMetadataScanner(rootFolder);

            Tracks = new TrackRepository(_fileMetadataScanner);
            Albums = new AlbumRepository(_fileMetadataScanner, Tracks);
            Artists = new ArtistRepository(_fileMetadataScanner, Tracks);
            Playlists = new PlaylistRepository();
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

        /// <inheritdoc />
        public bool IsInitialized { get; private set; }

        /// <inheritdoc />
        public AlbumRepository Albums { get; private set; }

        /// <inheritdoc />
        public ArtistRepository Artists { get; private set; }

        /// <inheritdoc />
        public PlaylistRepository Playlists { get; private set; }

        /// <inheritdoc />
        public TrackRepository Tracks { get; private set; }

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
        }

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

        /// <inheritdoc />
        public ValueTask DisposeAsync()
        {
            Albums.Dispose();
            Artists.Dispose();
            Playlists.Dispose();
            Tracks.Dispose();
            _fileMetadataScanner.Dispose();

            return default;
        }
    }
}