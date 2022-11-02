using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using OwlCore.Extensions;
using StrixMusic.Cores.Storage.FileMetadata.Models;

namespace StrixMusic.Cores.Storage.FileMetadata.Repositories;

/// <summary>
/// The service that helps in interacting with image information.
/// </summary>
internal class ImageRepository : IImageRepository
{
    private readonly ConcurrentDictionary<string, ImageMetadata> _inMemoryMetadata;

    /// <summary>
    /// Creates a new instance of <see cref="ImageRepository"/>.
    /// </summary>
    public ImageRepository()
    {
        _inMemoryMetadata = new ConcurrentDictionary<string, ImageMetadata>();
    }

    /// <inheritdoc/>
    public event EventHandler<IEnumerable<ImageMetadata>>? MetadataUpdated;

    /// <inheritdoc/>
    public event EventHandler<IEnumerable<ImageMetadata>>? MetadataRemoved;

    /// <inheritdoc/>
    public event EventHandler<IEnumerable<ImageMetadata>>? MetadataAdded;

    /// <inheritdoc />
    public int GetItemCount() => _inMemoryMetadata.Count;

    /// <inheritdoc />
    public Task AddOrUpdateAsync(params ImageMetadata[] metadata)
    {
        var addedImages = new List<ImageMetadata>();
        var updatedImages = new List<ImageMetadata>();
            
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

        if (addedImages.Count > 0 || updatedImages.Count > 0)
        {
            if (addedImages.Count > 0)
                MetadataAdded?.Invoke(this, addedImages);

            if (updatedImages.Count > 0)
                MetadataUpdated?.Invoke(this, updatedImages);
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task RemoveAsync(ImageMetadata imageMetadata)
    {
        Guard.IsNotNull(imageMetadata, nameof(imageMetadata));
        Guard.IsNotNullOrWhiteSpace(imageMetadata.Id, nameof(imageMetadata.Id));

        var removed = _inMemoryMetadata.TryRemove(imageMetadata.Id, out _);
        if (removed)
        {
            MetadataRemoved?.Invoke(this, imageMetadata.IntoList());
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task<ImageMetadata?> GetByIdAsync(string id)
    {
        var result = _inMemoryMetadata[id];
            
        return Task.FromResult<ImageMetadata?>(result);
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
}
