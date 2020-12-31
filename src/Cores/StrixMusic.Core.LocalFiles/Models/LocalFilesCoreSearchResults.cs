using StrixMusic.Core.LocalFiles.Models;
using StrixMusic.Sdk.Data.Core;
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
        /// <summary>
        /// Initializes a new instance of the <see cref="LocalFilesCoreSearchResults"/> class.
        /// </summary>
        /// <param name="sourceCore">The core that created this object.</param>
        public LocalFilesCoreSearchResults(ICore sourceCore, string query)
            : base(sourceCore)
        {
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
        public override IAsyncEnumerable<ICoreAlbumCollectionItem> GetAlbumItemsAsync(int limit, int offset)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override IAsyncEnumerable<ICoreArtistCollectionItem> GetArtistItemsAsync(int limit, int offset)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override IAsyncEnumerable<ICoreTrack> GetTracksAsync(int limit, int offset = 0)
        {
            throw new NotImplementedException();
        }
    }
}
