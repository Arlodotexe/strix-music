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
/// The service that helps in interacting with artist information.
/// </summary>
internal sealed class ArtistRepository : IArtistRepository
{
    private readonly ConcurrentDictionary<string, ArtistMetadata> _inMemoryMetadata;

    /// <summary>
    /// Creates a new instance of <see cref="ArtistRepository"/>.
    /// </summary>
    internal ArtistRepository(string id)
    {
        Id = id;
        _inMemoryMetadata = new ConcurrentDictionary<string, ArtistMetadata>();
    }

    /// <inheritdoc />
    public event EventHandler<IEnumerable<ArtistMetadata>>? MetadataUpdated;

    /// <inheritdoc />
    public event EventHandler<IEnumerable<ArtistMetadata>>? MetadataAdded;

    /// <inheritdoc />
    public event EventHandler<IEnumerable<ArtistMetadata>>? MetadataRemoved;

    /// <inheritdoc />
    public string Id { get; }

    /// <inheritdoc />
    public int GetItemCount() => _inMemoryMetadata.Count;

    /// <inheritdoc />
    public Task AddOrUpdateAsync(params ArtistMetadata[] metadata)
    {
        var addedArtists = new List<ArtistMetadata>();
        var updatedArtists = new List<ArtistMetadata>();

        foreach (var item in metadata)
        {
            Guard.IsNotNullOrWhiteSpace(item.Id, nameof(item.Id));

            var artistExists = true;
            var workingMetadata = _inMemoryMetadata.GetOrAdd(item.Id, key =>
            {
                artistExists = false;
                return item;
            });

            workingMetadata.AlbumIds ??= new HashSet<string>();
            workingMetadata.TrackIds ??= new HashSet<string>();
            workingMetadata.ImageIds ??= new HashSet<string>();
            item.AlbumIds ??= new HashSet<string>();
            item.TrackIds ??= new HashSet<string>();
            item.ImageIds ??= new HashSet<string>();

            Combine(workingMetadata.AlbumIds, item.AlbumIds);
            Combine(workingMetadata.TrackIds, item.TrackIds);
            Combine(workingMetadata.ImageIds, item.ImageIds);

            if (artistExists)
                updatedArtists.Add(workingMetadata);
            else
                addedArtists.Add(workingMetadata);
        }

        if (updatedArtists.Count > 0)
            MetadataUpdated?.Invoke(this, updatedArtists);

        if (addedArtists.Count > 0)
            MetadataAdded?.Invoke(this, addedArtists);

        void Combine(HashSet<string> originalData, HashSet<string> newIds)
        {
            foreach (var newId in newIds.ToArray())
                originalData.Add(newId);
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task RemoveAsync(ArtistMetadata metadata)
    {
        Guard.IsNotNullOrWhiteSpace(metadata.Id, nameof(metadata.Id));

        var removed = _inMemoryMetadata.TryRemove(metadata.Id, out _);
        if (removed)
        {
            MetadataRemoved?.Invoke(this, metadata.IntoList());
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task<ArtistMetadata?> GetByIdAsync(string id)
    {
        _inMemoryMetadata.TryGetValue(id, out var metadata);
        return Task.FromResult<ArtistMetadata?>(metadata);
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<ArtistMetadata>> GetItemsAsync(int offset, int limit)
    {
        var allArtists = _inMemoryMetadata.Values.ToList();

        if (limit == -1)
            return Task.FromResult<IReadOnlyList<ArtistMetadata>>(allArtists);

        // If the offset exceeds the number of items we have, return nothing.
        if (offset >= allArtists.Count)
            return Task.FromResult<IReadOnlyList<ArtistMetadata>>(new List<ArtistMetadata>());

        // If the total number of requested items exceeds the number of items we have, adjust the limit so it won't go out of range.
        if (offset + limit > allArtists.Count)
            limit = allArtists.Count - offset;

        return Task.FromResult<IReadOnlyList<ArtistMetadata>>(allArtists.GetRange(offset, limit));
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<ArtistMetadata>> GetArtistsByAlbumId(string albumId, int offset, int limit)
    {
        var allArtists = await GetItemsAsync(offset, -1);
        var results = new List<ArtistMetadata>();

        foreach (var item in allArtists)
        {
            Guard.IsNotNull(item.AlbumIds, nameof(item.AlbumIds));
            Guard.IsGreaterThan(item.AlbumIds.Count, 0, nameof(item.AlbumIds.Count));

            if (item.AlbumIds.Contains(albumId))
                results.Add(item);
        }

        // If the offset exceeds the number of items we have, return nothing.
        if (offset >= results.Count)
            return new List<ArtistMetadata>();

        // If the total number of requested items exceeds the number of items we have, adjust the limit so it won't go out of range.
        if (offset + limit > results.Count)
            limit = results.Count - offset;

        return results.GetRange(offset, limit).ToList();
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<ArtistMetadata>> GetArtistsByTrackId(string trackId, int offset, int limit)
    {
        var allArtists = await GetItemsAsync(0, -1);
        var results = new List<ArtistMetadata>();

        foreach (var item in allArtists)
        {
            Guard.IsNotNull(item.TrackIds, nameof(item.TrackIds));
            Guard.IsGreaterThan(item.TrackIds.Count, 0, nameof(item.TrackIds.Count));

            if (item.TrackIds.Contains(trackId))
                results.Add(item);
        }

        // If the offset exceeds the number of items we have, return nothing.
        if (offset >= results.Count)
            return new List<ArtistMetadata>();

        // If the total number of requested items exceeds the number of items we have, adjust the limit so it won't go out of range.
        if (offset + limit > results.Count)
            limit = results.Count - offset;

        return results.GetRange(offset, limit).ToList();
    }
}
