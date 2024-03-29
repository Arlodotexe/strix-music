﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using OwlCore.Extensions;
using StrixMusic.Cores.Storage.FileMetadata.Models;

namespace StrixMusic.Cores.Storage.FileMetadata.Repositories;

/// <summary>
/// The service that helps in interacting with album information.
/// </summary>
internal sealed class AlbumRepository : IAlbumRepository
{
    private readonly ConcurrentDictionary<string, AlbumMetadata> _inMemoryMetadata;
    private readonly SemaphoreSlim _initMutex;

    /// <summary>
    /// Creates a new instance of <see cref="AlbumRepository"/>.
    /// </summary>
    internal AlbumRepository()
    {
        _inMemoryMetadata = new ConcurrentDictionary<string, AlbumMetadata>();
        _initMutex = new SemaphoreSlim(1, 1);
    }

    /// <inheritdoc />
    public event EventHandler<IEnumerable<AlbumMetadata>>? MetadataUpdated;

    /// <inheritdoc />
    public event EventHandler<IEnumerable<AlbumMetadata>>? MetadataAdded;

    /// <inheritdoc />
    public event EventHandler<IEnumerable<AlbumMetadata>>? MetadataRemoved;

    /// <inheritdoc />
    public int GetItemCount() => _inMemoryMetadata.Count;

    /// <inheritdoc />
    public async Task AddOrUpdateAsync(params AlbumMetadata[] metadata)
    {
        var addedAlbums = new List<AlbumMetadata>();
        var updatedAlbums = new List<AlbumMetadata>();

        foreach (var item in metadata)
        {
            Guard.IsNotNull(item.Id, nameof(item.Id));

            var albumExists = true;

            var workingMetadata = _inMemoryMetadata.GetOrAdd(item.Id, key =>
            {
                albumExists = false;
                return item;
            });

            workingMetadata.ArtistIds ??= new HashSet<string>();
            workingMetadata.TrackIds ??= new HashSet<string>();
            workingMetadata.ImageIds ??= new HashSet<string>();
            item.ArtistIds ??= new HashSet<string>();
            item.TrackIds ??= new HashSet<string>();
            item.ImageIds ??= new HashSet<string>();
            item.ArtistIds ??= new HashSet<string>();
            item.TrackIds ??= new HashSet<string>();
            item.ImageIds ??= new HashSet<string>();

            Combine(workingMetadata.ArtistIds, item.ArtistIds);
            Combine(workingMetadata.TrackIds, item.TrackIds);
            Combine(workingMetadata.ImageIds, item.ImageIds);

            if (albumExists)
                updatedAlbums.Add(workingMetadata);
            else
                addedAlbums.Add(workingMetadata);
        }

        if (updatedAlbums.Count > 0)
            MetadataUpdated?.Invoke(this, updatedAlbums);

        if (addedAlbums.Count > 0)
            MetadataAdded?.Invoke(this, addedAlbums);
            
        void Combine(HashSet<string> originalData, HashSet<string> newIds)
        {
            foreach (var newId in newIds.ToArray())
                originalData.Add(newId);
        }
    }

    /// <inheritdoc />
    public async Task RemoveAsync(AlbumMetadata metadata)
    {
        Guard.IsNotNullOrWhiteSpace(metadata.Id, nameof(metadata.Id));

        var removed = _inMemoryMetadata.TryRemove(metadata.Id, out _);
        if (removed)
        {
            MetadataRemoved?.Invoke(this, metadata.IntoList());
        }
    }

    /// <inheritdoc />
    public Task<AlbumMetadata?> GetByIdAsync(string id)
    {
        _inMemoryMetadata.TryGetValue(id, out var metadata);
        return Task.FromResult<AlbumMetadata?>(metadata);
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<AlbumMetadata>> GetItemsAsync(int offset, int limit)
    {
        var allAlbums = _inMemoryMetadata.Values.ToList();

        if (limit == -1)
            return Task.FromResult<IReadOnlyList<AlbumMetadata>>(allAlbums);

        // If the offset exceeds the number of items we have, return nothing.
        if (offset >= allAlbums.Count)
            return Task.FromResult<IReadOnlyList<AlbumMetadata>>(new List<AlbumMetadata>());

        // If the total number of requested items exceeds the number of items we have, adjust the limit so it won't go out of range.
        if (offset + limit > allAlbums.Count)
            limit = allAlbums.Count - offset;

        return Task.FromResult<IReadOnlyList<AlbumMetadata>>(allAlbums.GetRange(offset, limit));
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<AlbumMetadata>> GetAlbumsByArtistId(string artistId, int offset, int limit)
    {
        var allArtists = await GetItemsAsync(offset, -1);
        var results = new List<AlbumMetadata>();

        foreach (var item in allArtists)
        {
            Guard.IsNotNull(item.ArtistIds, nameof(item.ArtistIds));
            Guard.IsGreaterThan(item.ArtistIds.Count, 0, nameof(item.ArtistIds.Count));

            if (item.ArtistIds.Contains(artistId))
                results.Add(item);
        }

        // If the offset exceeds the number of items we have, return nothing.
        if (offset >= results.Count)
            return new List<AlbumMetadata>();

        // If the total number of requested items exceeds the number of items we have, adjust the limit so it won't go out of range.
        if (offset + limit > results.Count)
            limit = results.Count - offset;

        return results.GetRange(offset, limit).ToList();
    }
}
