using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MessagePack;
using Microsoft.Toolkit.Diagnostics;

using OwlCore;
using OwlCore.AbstractStorage;
using OwlCore.Extensions;
using StrixMusic.Sdk.Services.FileMetadataManager.MetadataScanner;
using StrixMusic.Sdk.Services.FileMetadataManager.Models;

namespace StrixMusic.Sdk.Services.FileMetadataManager.Repositories
{
    /// <summary>
    /// The service that helps in interacting with image information.
    /// </summary>
    public class ImageRepository : IImageRepository
    {
        private const string IMAGE_DATA_FILENAME = "ImageData.bin";

        private readonly ConcurrentDictionary<string, ImageMetadata> _inMemoryMetadata;
        private readonly SemaphoreSlim _storageMutex;
        private readonly SemaphoreSlim _initMutex;
        private readonly AudioMetadataScanner _audioMetadataScanner;
        private readonly string _debouncerId;

        private IFolderData? _folderData;

        /// <summary>
        /// Creates a new instance of <see cref="ImageRepository"/>.
        /// </summary>
        public ImageRepository(AudioMetadataScanner audioMetadataScanner)
        {
            Guard.IsNotNull(audioMetadataScanner, nameof(audioMetadataScanner));

            _inMemoryMetadata = new ConcurrentDictionary<string, ImageMetadata>();
            _storageMutex = new SemaphoreSlim(1, 1);
            _initMutex = new SemaphoreSlim(1, 1);
            _audioMetadataScanner = audioMetadataScanner;
            _debouncerId = Guid.NewGuid().ToString();
        }

        /// <inheritdoc/>
        public bool IsInitialized { get; private set; }

        /// <inheritdoc/>
        public event EventHandler<IEnumerable<ImageMetadata>>? MetadataUpdated;

        /// <inheritdoc/>
        public event EventHandler<IEnumerable<ImageMetadata>>? MetadataRemoved;

        /// <inheritdoc/>
        public event EventHandler<IEnumerable<ImageMetadata>>? MetadataAdded;

        /// <inheritdoc/>
        public async Task InitAsync()
        {
            await _initMutex.WaitAsync();
            if (IsInitialized)
            {
                _initMutex.Release();
                return;
            }

            await LoadDataFromDiskAsync();

            IsInitialized = true;
            _initMutex.Release();
        }

        /// <inheritdoc/>
        public Task<int> GetItemCount()
        {
            return Task.FromResult(_inMemoryMetadata.Count);
        }

        /// <inheritdoc/>
        public void SetDataFolder(IFolderData rootFolder)
        {
            _folderData = rootFolder;
        }

        /// <inheritdoc/>
        public async Task AddOrUpdateImageAsync(ImageMetadata imageMetadata)
        {
            Guard.IsNotNull(imageMetadata, nameof(imageMetadata));
            Guard.IsNotNullOrWhiteSpace(imageMetadata.Id, nameof(imageMetadata.Id));

            var isUpdate = false;

            await _storageMutex.WaitAsync();

            _inMemoryMetadata.AddOrUpdate(
                imageMetadata.Id,
                addValueFactory: id =>
                {
                    isUpdate = false;
                    return imageMetadata;
                },
                updateValueFactory: (id, existing) =>
                {
                    isUpdate = true;
                    return imageMetadata;
                });

            _storageMutex.Release();

            _ = CommitChangesAsync();

            if (isUpdate)
                MetadataUpdated?.Invoke(this, imageMetadata.IntoList());
            else
                MetadataAdded?.Invoke(this, imageMetadata.IntoList());
        }

        /// <inheritdoc/>
        public async Task RemoveImageAsync(ImageMetadata imageMetadata)
        {
            Guard.IsNotNull(imageMetadata, nameof(imageMetadata));
            Guard.IsNotNullOrWhiteSpace(imageMetadata.Id, nameof(imageMetadata.Id));

            await _storageMutex.WaitAsync();

            var removed = _inMemoryMetadata.TryRemove(imageMetadata.Id, out _);
            
            _storageMutex.Release();

            if (removed)
            {
                _ = CommitChangesAsync();
                MetadataRemoved?.Invoke(this, imageMetadata.IntoList());
            }
        }

        /// <inheritdoc/>
        public Task<ImageMetadata?> GetImageByIdAsync(string id)
        {
            _inMemoryMetadata.TryGetValue(id, out var metadata);
            return Task.FromResult<ImageMetadata?>(metadata);
        }

        /// <inheritdoc/>
        public Task<IReadOnlyList<ImageMetadata?>> GetImagesByIdsAsync(IEnumerable<string> ids)
        {
            Guard.IsNotNull(ids, nameof(ids));

            var metadataValues = new ImageMetadata?[ids.Count()];

            var i = 0;
            foreach (var id in ids)
            {
                _inMemoryMetadata.TryGetValue(id, out var metadata);
                metadataValues[i++] = metadata;
            }

            return Task.FromResult<IReadOnlyList<ImageMetadata?>>(metadataValues);
        }

        private async Task LoadDataFromDiskAsync()
        {
            Guard.IsEmpty((ICollection<KeyValuePair<string, ImageMetadata>>)_inMemoryMetadata, nameof(_inMemoryMetadata));
            Guard.IsNotNull(_folderData, nameof(_folderData));

            var fileData = await _folderData.CreateFileAsync(IMAGE_DATA_FILENAME, CreationCollisionOption.OpenIfExists);

            Guard.IsNotNull(fileData, nameof(fileData));

            using var stream = await fileData.GetStreamAsync(FileAccessMode.ReadWrite);
            var bytes = await stream.ToBytesAsync();

            if (bytes.Length == 0)
                return;

            var data = MessagePackSerializer.Deserialize<List<ImageMetadata>>(bytes, MessagePack.Resolvers.ContractlessStandardResolver.Options);

            await _storageMutex.WaitAsync();

            foreach (var item in data)
            {
                Guard.IsNotNullOrWhiteSpace(item?.Id, nameof(item.Id));

                if (!_inMemoryMetadata.TryAdd(item.Id, item))
                    ThrowHelper.ThrowInvalidOperationException($"Item already added to {nameof(_inMemoryMetadata)}");
            }

            _storageMutex.Release();
        }

        private async Task CommitChangesAsync()
        {
            if (!await Flow.Debounce(_debouncerId, TimeSpan.FromSeconds(5)) || _inMemoryMetadata.IsEmpty)
                return;

            await _storageMutex.WaitAsync();

            Guard.IsNotNull(_folderData, nameof(_folderData));
            var bytes = MessagePackSerializer.Serialize(_inMemoryMetadata.Values.ToList(), MessagePack.Resolvers.ContractlessStandardResolver.Options);

            var fileData = await _folderData.CreateFileAsync(IMAGE_DATA_FILENAME, CreationCollisionOption.OpenIfExists);
            await fileData.WriteAllBytesAsync(bytes);

            _storageMutex.Release();
        }

        private void ReleaseUnmanagedResources()
        {
            // TODO
        }

        /// <inheritdoc cref="Dispose()"/>
        protected virtual void Dispose(bool disposing)
        {
            Guard.IsTrue(IsInitialized, nameof(IsInitialized));

            ReleaseUnmanagedResources();
            if (disposing)
            {
                _inMemoryMetadata.Clear();
                _storageMutex.Dispose();
            }

            IsInitialized = false;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
