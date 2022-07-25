// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using OwlCore;
using OwlCore.AbstractStorage;
using OwlCore.Extensions;
using StrixMusic.Sdk.FileMetadata.Models;

namespace StrixMusic.Sdk.FileMetadata.Repositories
{
    /// <summary>
    /// The service that helps in interacting with image information.
    /// </summary>
    public class ImageRepository : IImageRepository
    {
        private const string DATA_FILENAME = "Images.bin";

        private readonly ConcurrentDictionary<string, ImageMetadata> _inMemoryMetadata;
        private readonly SemaphoreSlim _storageMutex;
        private readonly SemaphoreSlim _initMutex;
        private readonly string _debouncerId;

        private IFolderData? _folderData;

        /// <summary>
        /// Creates a new instance of <see cref="ImageRepository"/>.
        /// </summary>
        public ImageRepository()
        {
            _inMemoryMetadata = new ConcurrentDictionary<string, ImageMetadata>();
            _storageMutex = new SemaphoreSlim(1, 1);
            _initMutex = new SemaphoreSlim(1, 1);
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
        public async Task InitAsync(CancellationToken cancellationToken = default)
        {
            using var initMutexReleaseRegistration = cancellationToken.Register(() => _initMutex.Release());
            await _initMutex.WaitAsync(cancellationToken);
            if (IsInitialized)
            {
                _initMutex.Release();
                return;
            }

            try
            {
                await LoadDataFromDiskAsync(cancellationToken);
            }
            catch (JsonException)
            {
                // ignored
            }

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

        /// <inheritdoc />
        public async Task AddOrUpdateAsync(params ImageMetadata[] metadata)
        {
            var addedImages = new List<ImageMetadata>();
            var updatedImages = new List<ImageMetadata>();

            await _storageMutex.WaitAsync();

            var isUpdate = false;

            foreach (var item in metadata)
            {
                Guard.IsNotNullOrWhiteSpace(item.Id, nameof(item.Id));

                _inMemoryMetadata.AddOrUpdate(
                    item.Id,
                    addValueFactory: id =>
                    {
                        isUpdate = false;
                        return item;
                    },
                    updateValueFactory: (id, existing) =>
                    {
                        isUpdate = true;
                        return item;
                    });

                if (isUpdate)
                    updatedImages.Add(item);
                else
                    addedImages.Add(item);
            }

            _storageMutex.Release();

            if (addedImages.Count > 0 || updatedImages.Count > 0)
            {
                _ = CommitChangesAsync();

                if (addedImages.Count > 0)
                    MetadataAdded?.Invoke(this, addedImages);

                if (updatedImages.Count > 0)
                    MetadataUpdated?.Invoke(this, updatedImages);
            }
        }

        /// <inheritdoc/>
        public async Task RemoveAsync(ImageMetadata imageMetadata)
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
        public async Task<ImageMetadata?> GetByIdAsync(string id)
        {
            await _storageMutex.WaitAsync();

            var result = _inMemoryMetadata[id];

            _storageMutex.Release();

            return result;
        }

        /// <inheritdoc/>
        public Task<IReadOnlyList<ImageMetadata>> GetItemsAsync(int offset, int limit)
        {
            var allImages = _inMemoryMetadata.Values.ToList();

            if (limit == -1)
                return Task.FromResult<IReadOnlyList<ImageMetadata>>(allImages);

            // If the offset exceeds the number of items we have, return nothing.
            if (offset >= allImages.Count)
                return Task.FromResult<IReadOnlyList<ImageMetadata>>(new List<ImageMetadata>());

            // If the total number of requested items exceeds the number of items we have, adjust the limit so it won't go out of range.
            if (offset + limit > allImages.Count)
                limit = allImages.Count - offset;

            return Task.FromResult<IReadOnlyList<ImageMetadata>>(allImages.GetRange(offset, limit));
        }

        private async Task LoadDataFromDiskAsync(CancellationToken cancellationToken)
        {
            Guard.IsEmpty((ICollection<KeyValuePair<string, ImageMetadata>>)_inMemoryMetadata, nameof(_inMemoryMetadata));
            Guard.IsNotNull(_folderData, nameof(_folderData));

            var fileData = await _folderData.CreateFileAsync(DATA_FILENAME, CreationCollisionOption.OpenIfExists);

            Guard.IsNotNull(fileData, nameof(fileData));

            using var stream = await fileData.GetStreamAsync(FileAccessMode.ReadWrite);
            cancellationToken.ThrowIfCancellationRequested();

            var data = await FileMetadataRepoSerializer.Singleton.DeserializeAsync<List<ImageMetadata>>(stream);
            cancellationToken.ThrowIfCancellationRequested();

            using var mutexReleaseOnCancelRegistration = cancellationToken.Register(() => _storageMutex.Release());
            await _storageMutex.WaitAsync(cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();

            foreach (var item in data ?? Enumerable.Empty<ImageMetadata>())
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
            using var serializedStream = await FileMetadataRepoSerializer.Singleton.SerializeAsync(_inMemoryMetadata.Values.DistinctBy(x => x.Id).ToList());

            var fileData = await _folderData.CreateFileAsync(DATA_FILENAME, CreationCollisionOption.OpenIfExists);
            using var fileStream = await fileData.GetStreamAsync(FileAccessMode.ReadWrite);
            fileStream.Position = 0;
            serializedStream.Position = 0;

            await serializedStream.CopyToAsync(fileStream);

            _storageMutex.Release();
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            _initMutex.Dispose();
            _storageMutex.Dispose();
        }
    }
}
