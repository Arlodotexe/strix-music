using StrixMusic.Sdk.Data.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StrixMusic.Core.External.Models
{
    /// <summary>
    /// The recently played items for the <see cref="ExternalCore"/>.
    /// </summary>
    public class ExternalCoreRecentlyPlayed : ExternalCorePlayableCollectionGroupBase, ICoreRecentlyPlayed
    {
        /// <inheritdoc />
        public ExternalCoreRecentlyPlayed(ICore sourceCore)
            : base(sourceCore)
        {
        }

        /// <inheritdoc />
        public override string Id { get; protected set; } = "recentlyPlayed";

        /// <inheritdoc />
        public override Uri? Url { get; protected set; } = null;

        /// <inheritdoc />
        public override string Name { get; protected set; } = "Recently Played";

        /// <inheritdoc />
        public override string? Description { get; protected set; } = null;

        /// <inheritdoc />
        public override int TotalChildrenCount { get; internal set; } = 0;

        /// <inheritdoc />
        public override int TotalPlaylistItemsCount { get; internal set; } = 0;

        /// <inheritdoc />
        public override int TotalTracksCount { get; internal set; } = 0;

        /// <inheritdoc />
        public override int TotalAlbumItemsCount { get; internal set; } = 0;

        /// <inheritdoc />
        public override int TotalArtistItemsCount { get; internal set; } = 0;

        /// <inheritdoc />
        public override IAsyncEnumerable<ICoreAlbumCollectionItem> GetAlbumItemsAsync(int limit, int offset)
        {
            return AsyncEnumerable.Empty<ICoreAlbum>();
        }

        /// <inheritdoc />
        public override IAsyncEnumerable<ICoreArtistCollectionItem> GetArtistItemsAsync(int limit, int offset)
        {
            return AsyncEnumerable.Empty<ICoreArtist>();
        }

        /// <inheritdoc />
        public override IAsyncEnumerable<ICorePlayableCollectionGroup> GetChildrenAsync(int limit, int offset = 0)
        {
            return AsyncEnumerable.Empty<ICorePlayableCollectionGroup>();
        }

        /// <inheritdoc />
        public override IAsyncEnumerable<ICorePlaylistCollectionItem> GetPlaylistItemsAsync(int limit, int offset)
        {
            return AsyncEnumerable.Empty<ICorePlaylist>();
        }

        /// <inheritdoc />
        public override IAsyncEnumerable<ICoreTrack> GetTracksAsync(int limit, int offset = 0)
        {
            return AsyncEnumerable.Empty<ICoreTrack>();
        }
    }
}
