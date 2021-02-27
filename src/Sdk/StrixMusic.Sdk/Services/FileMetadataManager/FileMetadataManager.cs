using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Toolkit.Diagnostics;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Nito.AsyncEx;
using OwlCore;
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
        private readonly Queue<FileMetadata> _batchAddedMetadataToEmit = new Queue<FileMetadata>();
        private readonly Queue<FileMetadata> _batchUpdatedMetadataToEmit = new Queue<FileMetadata>();

        /// <summary>
        /// Creates a new instance of <see cref="FileMetadataManager"/>.
        /// </summary>
        /// <param name="instanceId">A unique identifier that helps decide where the scanned data is stored.</param>
        /// <param name="rootFolder">The folder where data is scanned.</param>
        public FileMetadataManager(string instanceId, IFolderData rootFolder)
        {
            _instanceId = instanceId;

            _fileMetadataScanner = new FileMetadataScanner(rootFolder);

            Albums = new AlbumRepository(_fileMetadataScanner);
            Artists = new ArtistRepository(_fileMetadataScanner);
            Tracks = new TrackRepository(_fileMetadataScanner);
            Playlists = new PlaylistRepository();

            AttachEvents();
        }

        private static async Task<IFolderData> GetDataStorageFolder(string instanceId)
        {
            var primaryFileSystemService = Ioc.Default.GetRequiredService<IFileSystemService>();

            var path = Path.Combine(primaryFileSystemService.RootFolder.Path, nameof(FileMetadataManager), instanceId);

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            var folderData = await primaryFileSystemService.GetFolderFromPathAsync(path);

            Guard.IsNotNull(folderData, nameof(folderData));

            return folderData;
        }

        private void AttachEvents()
        {
            _fileMetadataScanner.FileMetadataAdded += FileMetadataScanner_FileMetadataAdded;
            _fileMetadataScanner.FileMetadataUpdated += FileMetadataScanner_FileMetadataUpdated;
        }

        private void DetachEvents()
        {
            _fileMetadataScanner.FileMetadataAdded -= FileMetadataScanner_FileMetadataAdded;
            _fileMetadataScanner.FileMetadataUpdated -= FileMetadataScanner_FileMetadataUpdated;
        }

        private void FileMetadataScanner_FileMetadataUpdated(object sender, FileMetadata e)
        {
            lock (_batchUpdatedMetadataToEmit)
            {
                if (_batchUpdatedMetadataToEmit.Count >= 25)
                    BatchEmitUpdatedMetadata();
                else
                    _batchUpdatedMetadataToEmit.Enqueue(e);
            }
        }

        private async void FileMetadataScanner_FileMetadataAdded(object sender, FileMetadata e)
        {
            var fileMetadata = new FileMetadata();

            if (Tracks.AddOrSkipTrackMetadata(e.TrackMetadata))
                fileMetadata.TrackMetadata = e.TrackMetadata;

            if (Albums.AddOrSkipAlbumMetadata(e.AlbumMetadata))
                fileMetadata.AlbumMetadata = e.AlbumMetadata;

            if (Artists.AddOrSkipArtistMetadata(e.ArtistMetadata))
                fileMetadata.ArtistMetadata = e.ArtistMetadata;

            lock (_batchAddedMetadataToEmit)
            {
                if (_batchAddedMetadataToEmit.Count >= 25)
                    BatchEmitAddedMetadata();
                else
                    _batchAddedMetadataToEmit.Enqueue(fileMetadata);
            }
        }

        private void BatchEmitAddedMetadata()
        {
            while (_batchAddedMetadataToEmit.Count > 0)
            {
                var item = _batchAddedMetadataToEmit.Dequeue();
                FileMetadataAdded?.Invoke(this, item);
            }
        }

        private void BatchEmitUpdatedMetadata()
        {
            while (_batchUpdatedMetadataToEmit.Count > 0)
            {
                var item = _batchUpdatedMetadataToEmit.Dequeue();
                FileMetadataUpdated?.Invoke(this, item);
            }
        }

        ///<inheritdoc />
        public event EventHandler<FileMetadata>? FileMetadataAdded;

        ///<inheritdoc />
        public event EventHandler<FileMetadata>? FileMetadataUpdated;

        ///<inheritdoc />
        public event EventHandler<FileMetadata>? FileMetadataRemoved;

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
            if (IsInitialized)
                return;

            IsInitialized = true;

            var dataFolder = await GetDataStorageFolder(_instanceId);

            _fileMetadataScanner.CacheFolder = dataFolder;
            await _fileMetadataScanner.InitAsync();

            // Perform initialization tasks for all repos in parallel.
            var repositories = new IMetadataRepository[]
            {
                Albums,
                Artists,
                Tracks,
                Playlists,
            };

            await repositories.InParallel(x =>
            {
                x.SetDataFolder(dataFolder);
                return x.InitAsync();
            });

            ScanningCompleted?.Invoke(this, new EventArgs());
        }

        /// <inheritdoc />
        public ValueTask DisposeAsync()
        {
            DetachEvents();

            Albums.Dispose();
            Artists.Dispose();
            Playlists.Dispose();
            Tracks.Dispose();
            _fileMetadataScanner.Dispose();

            return default;
        }
    }
}