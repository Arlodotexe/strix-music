using StrixMusic.Core.LocalFiles.Backing.Models;
using StrixMusic.Core.LocalFiles.Backing.Services;
using StrixMusic.Core.LocalFiles.Models;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StrixMusic.Core.LocalFiles.Models
{
    /// <summary>
    /// A LocalFileCore implementation of <see cref="ICoreSearchResults"/>.
    /// </summary>
    public class LocalFilesCoreSearchResults : LocalFilesCorePlayableCollectionGroupBase, ICoreSearchResults
    {
        private ArtistService _artistService;
        private TrackService _trackService;
        private AlbumService _albumService;

        private IEnumerable<TrackMetadata> _trackMetadatas;
        private IEnumerable<AlbumMetadata> _albumMetadatas;
        private IEnumerable<ArtistMetadata> _artistMetadatas;

        private readonly string _query = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalFilesCoreSearchResults"/> class.
        /// </summary>
        /// <param name="sourceCore">The core that created this object.</param>
        /// <param name="sourceCore">The core that created this object.</param>
        public LocalFilesCoreSearchResults(ICore sourceCore, string query)
            : base(sourceCore)
        {
            _trackService = SourceCore.GetService<TrackService>();
            _artistService = SourceCore.GetService<ArtistService>();
            _albumService = SourceCore.GetService<AlbumService>();
        }

        /// <inheritdoc />
        public sealed override string Id { get; protected set; }

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
        public async override IAsyncEnumerable<ICoreAlbumCollectionItem> GetAlbumItemsAsync(int limit, int offset)
        {
            _albumMetadatas = await _albumService.GetAlbumMetadata(0, int.MaxValue);

            _albumMetadatas = _albumMetadatas.Where(c => c.Title?.Equals(_query, StringComparison.OrdinalIgnoreCase) ?? false);

            foreach (var album in _albumMetadatas)
            {
                // just to test
                var tracks = await SourceCore.GetService<TrackService>().GetTracksByAlbumId(album.Id, 0, 1000);
                yield return new LocalFilesCoreAlbum(SourceCore, album, tracks.Count);
            }
        }

        /// <inheritdoc/>
        public async override IAsyncEnumerable<ICoreArtistCollectionItem> GetArtistItemsAsync(int limit, int offset)
        {
            _artistMetadatas = await _artistService.GetArtistMetadata(0, int.MaxValue);

            _albumMetadatas = _albumMetadatas.Where(c => c.Title?.Equals(_query, StringComparison.OrdinalIgnoreCase) ?? false);

            foreach (var album in _albumMetadatas)
            {
                // just to test
                var tracks = await SourceCore.GetService<TrackService>().GetTracksByAlbumId(album.Id, 0, 1000);
                yield return new LocalFilesCoreAlbum(SourceCore, album, tracks.Count);
            }
        }

        /// <inheritdoc/>
        public async override IAsyncEnumerable<ICoreTrack> GetTracksAsync(int limit, int offset = 0)
        {
            _trackMetadatas = await _trackService.GetTrackMetadata(0, int.MaxValue);

            _trackMetadatas = _trackMetadatas.Where(c => c.Title?.Equals(_query, StringComparison.OrdinalIgnoreCase) ?? false);

            foreach (var track in _trackMetadatas)
            {
                yield return new LocalFilesCoreTrack(SourceCore, track);
            }
        }
    }
}
