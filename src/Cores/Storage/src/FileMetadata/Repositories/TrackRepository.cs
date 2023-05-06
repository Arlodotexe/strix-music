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
/// The service that helps in interacting with track information.
/// </summary>
internal sealed class TrackRepository : ITrackRepository
{
    private readonly ConcurrentDictionary<string, TrackMetadata> _inMemoryMetadata;

    /// <summary>
    /// Creates a new instance of <see cref="TrackRepository"/>.
    /// </summary>
    public TrackRepository()
    {
        _inMemoryMetadata = new ConcurrentDictionary<string, TrackMetadata>();
    }

    /// <inheritdoc />
    public event EventHandler<IEnumerable<TrackMetadata>>? MetadataUpdated;

    /// <inheritdoc />
    public event EventHandler<IEnumerable<TrackMetadata>>? MetadataAdded;

    /// <inheritdoc />
    public event EventHandler<IEnumerable<TrackMetadata>>? MetadataRemoved;

    /// <inheritdoc />
    public int GetItemCount() => _inMemoryMetadata.Count;

    /// <inheritdoc />
    public Task AddOrUpdateAsync(params TrackMetadata[] trackMetadata)
    {
        var addedTracks = new List<TrackMetadata>();
        var updatedTracks = new List<TrackMetadata>();

        // Iterate through FileMetadata and store in memory.
        // Updates and additions are tracked separately and emitted as events after all metadata has been processed.
        foreach (var item in trackMetadata)
        {
            Guard.IsNotNullOrWhiteSpace(item.Id, nameof(item.Id));

            var trackExists = true;

            var workingMetadata = _inMemoryMetadata.GetOrAdd(item.Id, key =>
            {
                trackExists = false;
                return item;
            });

            workingMetadata.ArtistIds ??= new HashSet<string>();
            workingMetadata.ImageIds ??= new HashSet<string>();
            item.ArtistIds ??= new HashSet<string>();
            item.ImageIds ??= new HashSet<string>();

            Combine(workingMetadata.ArtistIds, item.ArtistIds);
            Combine(workingMetadata.ImageIds, item.ImageIds);
                
            if (trackExists)
                updatedTracks.Add(workingMetadata);
            else
                addedTracks.Add(workingMetadata);
        }

        if (addedTracks.Count > 0 || updatedTracks.Count > 0)
        {
            if (addedTracks.Count > 0)
                MetadataAdded?.Invoke(this, addedTracks);

            if (updatedTracks.Count > 0)
                MetadataUpdated?.Invoke(this, updatedTracks);
        }

        void Combine(HashSet<string> originalData, HashSet<string> newIds)
        {
            foreach (var newId in newIds)
                originalData.Add(newId);
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task RemoveAsync(TrackMetadata trackMetadata)
    {
        Guard.IsNotNullOrWhiteSpace(trackMetadata.Id, nameof(trackMetadata.Id));

        var removed = _inMemoryMetadata.TryRemove(trackMetadata.Id, out _);
        if (removed)
        {
            MetadataRemoved?.Invoke(this, trackMetadata.IntoList());
        }
    }

    /// <inheritdoc />
    public Task<TrackMetadata?> GetByIdAsync(string id)
    {
        _inMemoryMetadata.TryGetValue(id, out var trackMetadata);

        return Task.FromResult<TrackMetadata?>(trackMetadata);
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<TrackMetadata>> GetItemsAsync(int offset, int limit)
    {
        var allTracks = _inMemoryMetadata.Values.OrderBy(c => c.TrackNumber).GroupBy(x => x.DiscNumber).SelectMany(x => x).ToList();

        if (limit == -1)
            return Task.FromResult<IReadOnlyList<TrackMetadata>>(allTracks);

        // If the offset exceeds the number of items we have, return nothing.
        if (offset >= allTracks.Count)
            return Task.FromResult<IReadOnlyList<TrackMetadata>>(new List<TrackMetadata>());

        // If the total number of requested items exceeds the number of items we have, adjust the limit so it won't go out of range.
        if (offset + limit > allTracks.Count)
            limit = allTracks.Count - offset;

        return Task.FromResult<IReadOnlyList<TrackMetadata>>(allTracks.GetRange(offset, limit));
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<TrackMetadata>> GetTracksByArtistId(string artistId, int offset, int limit)
    {
        var allTracks = await GetItemsAsync(offset, -1);
        var results = new List<TrackMetadata>();

        foreach (var item in allTracks)
        {
            Guard.IsNotNull(item.ArtistIds, nameof(item.ArtistIds));
            Guard.IsGreaterThan(item.ArtistIds.Count, 0, nameof(item.ArtistIds.Count));

            if (item.ArtistIds.Contains(artistId))
                results.Add(item);
        }

        // If the offset exceeds the number of items we have, return nothing.
        if (offset >= results.Count)
            return new List<TrackMetadata>();

        // If the total number of requested items exceeds the number of items we have, adjust the limit so it won't go out of range.
        if (offset + limit > results.Count)
            limit = results.Count - offset;

        return results.GetRange(offset, limit).ToList();
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<TrackMetadata>> GetTracksByAlbumId(string albumId, int offset, int limit)
    {
        var results = new List<TrackMetadata>();
        var allTracks = await GetItemsAsync(offset, -1);

        foreach (var item in allTracks)
        {
            Guard.IsNotNull(item.AlbumId, nameof(item.AlbumId));

            if (item.AlbumId == albumId)
                results.Add(item);
        }

        // If the offset exceeds the number of items we have, return nothing.
        if (offset >= results.Count)
            return new List<TrackMetadata>();

        // If the total number of requested items exceeds the number of items we have, adjust the limit so it won't go out of range.
        if (offset + limit > results.Count)
            limit = results.Count - offset;

        return results.Skip(offset).Take(limit).ToList();
    }
}
