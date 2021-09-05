using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Toolkit.Diagnostics;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.Extensions;
using StrixMusic.Sdk.Services.FileMetadataManager;
using StrixMusic.Sdk.Services.FileMetadataManager.Models;

namespace StrixMusic.Core.LocalFiles.Models
{
    /// <summary>
    /// A LocalFileCore implementation of <see cref="ICoreSearchResults"/>.
    /// </summary>
    public class LocalFilesCoreSearchResults : LocalFilesCorePlayableCollectionGroupBase, ICoreSearchResults
    {
        private readonly string _query;
        private readonly IFileMetadataManager _fileMetadataManager;

        private IEnumerable<TrackMetadata> _trackMetadata;
        private IEnumerable<AlbumMetadata> _albumMetadata;
        private IEnumerable<ArtistMetadata> _artistMetadata;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalFilesCoreSearchResults"/> class.
        /// </summary>
        /// <param name="sourceCore">The core that created this object.</param>
        /// <param name="query">The query that was given to produce these results.</param>
        public LocalFilesCoreSearchResults(ICore sourceCore, string query)
            : base(sourceCore)
        {
            _albumMetadata = new List<AlbumMetadata>();
            _trackMetadata = new List<TrackMetadata>();
            _artistMetadata = new List<ArtistMetadata>();

            _query = query;

            _fileMetadataManager = SourceCore.GetService<IFileMetadataManager>();
        }

        /// <inheritdoc />
        public sealed override string Id { get; protected set; } = string.Empty;

        /// <inheritdoc />
        public override string Name { get; protected set; } = "Search Results";

        /// <inheritdoc />
        public override string? Description { get; protected set; } = null;

        /// <inheritdoc />
        public override int TotalChildrenCount { get; internal set; }

        /// <inheritdoc />
        public override int TotalPlaylistItemsCount { get; internal set; }

        /// <inheritdoc />
        public override int TotalArtistItemsCount { get; internal set; }

        /// <inheritdoc />
        public override int TotalAlbumItemsCount { get; internal set; }

        /// <inheritdoc />
        public override int TotalTrackCount { get; internal set; }

        /// <inheritdoc/>
        public override IAsyncEnumerable<ICorePlayableCollectionGroup> GetChildrenAsync(int limit, int offset = 0)
        {
            return AsyncEnumerable.Empty<ICorePlayableCollectionGroup>();
        }

        /// <inheritdoc/>
        public override IAsyncEnumerable<ICorePlaylistCollectionItem> GetPlaylistItemsAsync(int limit, int offset)
        {
            return AsyncEnumerable.Empty<ICorePlaylist>();
        }

        /// <inheritdoc/>
        public override async IAsyncEnumerable<ICoreAlbumCollectionItem> GetAlbumItemsAsync(int limit, int offset)
        {
            _albumMetadata = await _fileMetadataManager.Albums.GetAlbums(0, int.MaxValue);

            _albumMetadata = _albumMetadata.Where(c => c.Title?.Equals(_query, StringComparison.OrdinalIgnoreCase) ?? false);

            foreach (var album in _albumMetadata)
            {
                Guard.IsNotNull(album.Id, nameof(album.Id));

                yield return new LocalFilesCoreAlbum(SourceCore, album);
            }
        }

        /// <inheritdoc/>
        public override async IAsyncEnumerable<ICoreArtistCollectionItem> GetArtistItemsAsync(int limit, int offset)
        {
            _artistMetadata = await _fileMetadataManager.Artists.GetArtists(0, int.MaxValue);

            _artistMetadata = _artistMetadata.Where(c => c.Name?.Equals(_query, StringComparison.OrdinalIgnoreCase) ?? false);

            foreach (var artist in _artistMetadata)
            {
                yield return new LocalFilesCoreArtist(SourceCore, artist);
            }
        }

        /// <inheritdoc/>
        public override async IAsyncEnumerable<ICoreTrack> GetTracksAsync(int limit, int offset = 0)
        {
            _trackMetadata = await _fileMetadataManager.Tracks.GetTracks(0, int.MaxValue);

            _trackMetadata = _trackMetadata.Where(c => c.Title?.Equals(_query, StringComparison.OrdinalIgnoreCase) ?? false);

            foreach (var track in _trackMetadata)
            {
                yield return new LocalFilesCoreTrack(SourceCore, track);
            }
        }

        /// <inheritdoc/>
        public override IAsyncEnumerable<ICoreUrl> GetUrlsAsync(int limit, int offset = 0)
        {
            return AsyncEnumerable.Empty<ICoreUrl>();
        }
    }
}
