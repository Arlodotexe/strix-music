using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StrixMusic.Cores.Storage.FileMetadata.Models;

namespace StrixMusic.Cores.Storage.FileMetadata.Repositories;

/// <summary>
/// A repository that provides access to metadata scanned from a file.
/// </summary>
internal interface IMetadataRepository<TFileMetadata>
    where TFileMetadata : class, IFileMetadata
{
    /// <summary>
    /// Returns the number of items currently loaded in the repository.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public int GetItemCount();

    /// <summary>
    /// Adds a metadata to the repo, or updates an existing metadata.
    /// </summary>
    /// <param name="metadata">The metadata to add.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public Task AddOrUpdateAsync(TFileMetadata[] metadata);

    /// <summary>
    /// Removes metadata from the repository.
    /// </summary>
    /// <param name="metadata">The metadata to remove.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentException">The specified <paramref name="metadata"/> does not exist in the repository.</exception>
    public Task RemoveAsync(TFileMetadata metadata);

    /// <summary>
    /// Gets metadata by the given <paramref name="id"/>.
    /// </summary>
    /// <param name="id">The id of some metadata in the repository..</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation. Value is the metadata if found, otherwise null.</returns>
    public Task<TFileMetadata?> GetByIdAsync(string id);

    /// <summary>
    /// Gets all <see cref="TrackMetadata"/> over the file system.
    /// </summary>
    /// <param name="offset">The starting index for retrieving items.</param>
    /// <param name="limit">The maximum number of items to return.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public Task<IReadOnlyList<TFileMetadata>> GetItemsAsync(int offset, int limit);

    /// <summary>
    /// Raised when metadata is updated.
    /// </summary>
    public event EventHandler<IEnumerable<TFileMetadata>>? MetadataUpdated;

    /// <summary>
    /// Raised metadata is removed.
    /// </summary>
    public event EventHandler<IEnumerable<TFileMetadata>>? MetadataRemoved;

    /// <summary>
    /// Raised when new metadata is added.
    /// </summary>
    public event EventHandler<IEnumerable<TFileMetadata>>? MetadataAdded;
}
