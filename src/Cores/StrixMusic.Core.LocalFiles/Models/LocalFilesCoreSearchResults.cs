using StrixMusic.Core.LocalFiles.Models;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StrixMusic.Sdk.Services.FileMetadataManager;
using StrixMusic.Sdk.Services.FileMetadataManager.Models;

namespace StrixMusic.Core.LocalFiles.Models
{
    /// <summary>
    /// A LocalFileCore implementation of <see cref="ICoreSearchResults"/>.
    /// </summary>
    public class LocalFilesCoreSearchResults : LocalFilesCorePlayableCollectionGroupBase, ICoreSearchResults
    {
        private readonly string _query = string.Empty;

        private IEnumerable<TrackMetadata> _trackMetadatas;
        private IEnumerable<AlbumMetadata> _albumMetadatas;
        private IEnumerable<ArtistMetadata> _artistMetadatas;

        private IFileMetadataManager _fileMetadataManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalFilesCoreSearchResults"/> class.
        /// </summary>
        /// <param name="sourceCore">The core that created this object.</param>
        /// <param name="sourceCore">The core that created this object.</param>
        public LocalFilesCoreSearchResults(ICore sourceCore, string query)
            : base(sourceCore)
        {
            _albumMetadatas = new List<AlbumMetadata>();
            _trackMetadatas = new List<TrackMetadata>();
            _artistMetadatas = new List<ArtistMetadata>();

            _query = query;

            _fileMetadataManager = SourceCore.GetService<IFileMetadataManager>();
        }

        /// <inheritdoc />
        public sealed override string Id { get; protected set; } = string.Empty;

        /// <inheritdoc />
        public override Uri? Url { get; protected set; } = null;

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
        public override int TotalTracksCount { get; internal set; }

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
            _albumMetadatas = await _fileMetadataManager.Albums.GetAlbumMetadata(0, int.MaxValue);

            _albumMetadatas = _albumMetadatas.Where(c => c.Title?.Equals(_query, StringComparison.OrdinalIgnoreCase) ?? false);

            foreach (var album in _albumMetadatas)
            {
                yield return new LocalFilesCoreAlbum(SourceCore, album, album.TrackIds?.Count ?? 0);
            }
        }

        /// <inheritdoc/>
        public override async IAsyncEnumerable<ICoreArtistCollectionItem> GetArtistItemsAsync(int limit, int offset)
        {
            _artistMetadatas = await _fileMetadataManager.Artists.GetArtistMetadata(0, int.MaxValue);

            _artistMetadatas = _artistMetadatas.Where(c => c.Name?.Equals(_query, StringComparison.OrdinalIgnoreCase) ?? false);

            foreach (var artist in _artistMetadatas)
            {
                yield return new LocalFilesCoreArtist(SourceCore, artist, artist.TrackIds?.Count ?? 0);
            }
        }

        /// <inheritdoc/>
        public override async IAsyncEnumerable<ICoreTrack> GetTracksAsync(int limit, int offset = 0)
        {
            _trackMetadatas = await _fileMetadataManager.Tracks.GetTrackMetadata(0, int.MaxValue);

            _trackMetadatas = _trackMetadatas.Where(c => c.Title?.Equals(_query, StringComparison.OrdinalIgnoreCase) ?? false);

            foreach (var track in _trackMetadatas)
            {
                yield return new LocalFilesCoreTrack(SourceCore, track);
            }
        }
    }
}
