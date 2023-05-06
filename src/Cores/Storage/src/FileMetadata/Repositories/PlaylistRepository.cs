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
/// The service that helps in interacting with playlist information.
/// </summary>
internal sealed class PlaylistRepository : IPlaylistRepository
{
    private readonly ConcurrentDictionary<string, PlaylistMetadata> _inMemoryMetadata;

    /// <summary>
    /// Creates a new instance of <see cref="PlaylistRepository"/>.
    /// </summary>
    internal PlaylistRepository()
    {
        _inMemoryMetadata = new ConcurrentDictionary<string, PlaylistMetadata>();
    }

    /// <inheritdoc />
    public event EventHandler<IEnumerable<PlaylistMetadata>>? MetadataUpdated;

    /// <inheritdoc />
    public event EventHandler<IEnumerable<PlaylistMetadata>>? MetadataAdded;

    /// <inheritdoc />
    public event EventHandler<IEnumerable<PlaylistMetadata>>? MetadataRemoved;

    /// <inheritdoc />
    public int GetItemCount() => _inMemoryMetadata.Count;

    /// <inheritdoc />
    public Task AddOrUpdateAsync(params PlaylistMetadata[] playlistMetadata)
    {
        var updatedItems = new List<PlaylistMetadata>();
        var newItems = new List<PlaylistMetadata>();

        foreach (var item in playlistMetadata)
        {
            Guard.IsNotNullOrWhiteSpace(item.Id, nameof(item.Id));

            var playlistExists = true;

            var workingMetadata = _inMemoryMetadata.GetOrAdd(item.Id, key =>
            {
                playlistExists = false;
                return item;
            });

            if (playlistExists)
            {
                workingMetadata.TrackIds ??= new HashSet<string>();
                item.TrackIds ??= new HashSet<string>();
                    
                Combine(workingMetadata.TrackIds, item.TrackIds);
                updatedItems.Add(item);
            }
            else
                newItems.Add(item);
        }

        if (newItems.Count > 0)
            MetadataAdded?.Invoke(this, newItems);

        if (updatedItems.Count > 0)
            MetadataUpdated?.Invoke(this, updatedItems);
            
        void Combine(HashSet<string> originalData, HashSet<string> newIds)
        {
            foreach (var newId in newIds)
                originalData.Add(newId);
        }
            
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task RemoveAsync(PlaylistMetadata playlistMetadata)
    {
        Guard.IsNotNullOrWhiteSpace(playlistMetadata.Id, nameof(playlistMetadata.Id));

        var removed = _inMemoryMetadata.TryRemove(playlistMetadata.Id, out _);
        if (removed)
        {
            MetadataRemoved?.Invoke(this, playlistMetadata.IntoList());
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task<PlaylistMetadata?> GetByIdAsync(string id)
    {
        _inMemoryMetadata.TryGetValue(id, out var metadata);

        return Task.FromResult<PlaylistMetadata?>(metadata);
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<PlaylistMetadata>> GetItemsAsync(int offset, int limit)
    {
        var allPlaylists = _inMemoryMetadata.Values.ToList();

        if (limit == -1)
            return Task.FromResult<IReadOnlyList<PlaylistMetadata>>(allPlaylists);

        // If the offset exceeds the number of items we have, return nothing.
        if (offset >= allPlaylists.Count)
            return Task.FromResult<IReadOnlyList<PlaylistMetadata>>(new List<PlaylistMetadata>());

        // If the total number of requested items exceeds the number of items we have, adjust the limit so it won't go out of range.
        if (offset + limit > allPlaylists.Count)
            limit = allPlaylists.Count - offset;

        return Task.FromResult<IReadOnlyList<PlaylistMetadata>>(allPlaylists.GetRange(offset, limit));
    }
}
