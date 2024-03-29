﻿using System.Collections.Generic;
using System.Threading.Tasks;
using StrixMusic.Cores.Storage.FileMetadata.Models;

namespace StrixMusic.Cores.Storage.FileMetadata.Repositories;

/// <summary>
/// Provides storage for album metadata.
/// </summary>
internal interface IAlbumRepository : IMetadataRepository<AlbumMetadata>
{
    /// <summary>
    /// Gets the filtered albums by artists ids.
    /// </summary>
    /// <param name="artistId">The artist Id.</param>
    /// <param name="offset">The starting index for retrieving items.</param>
    /// <param name="limit">The maximum number of items to return.</param>
    /// <returns>The filtered <see cref="IReadOnlyList{AlbumMetadata}"/>>.</returns>
    public Task<IReadOnlyList<AlbumMetadata>> GetAlbumsByArtistId(string artistId, int offset, int limit);
}