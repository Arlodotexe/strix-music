using System;
using System.Collections.Generic;
using System.Linq;
using OwlCore.Collections;
using StrixMusic.Sdk.Core.Data;

namespace StrixMusic.Core.MusicBrainz.Models
{
    /// <summary>
    /// Discoverable music for the <see cref="MusicBrainzCore"/>.
    /// </summary>
    public class MusicBrainzDiscoverables : MusicBrainzCollectionGroupBase, IDiscoverables
    {
        /// <inheritdoc />
        public MusicBrainzDiscoverables(ICore sourceCore)
            : base(sourceCore)
        {
        }

        /// <inheritdoc />
        public override string Id { get; protected set; } = "discoverables";

        /// <inheritdoc />
        public override Uri? Url { get; protected set; } = null;

        /// <inheritdoc />
        public override string Name { get; protected set; } = "Discoverables";

        /// <inheritdoc />
        public override SynchronizedObservableCollection<IImage> Images { get; protected set; } = new SynchronizedObservableCollection<IImage>();

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
        public override IAsyncEnumerable<IAlbumCollectionItem> GetAlbumItemsAsync(int limit, int offset)
        {
            return AsyncEnumerable.Empty<IAlbum>();
        }

        /// <inheritdoc />
        public override IAsyncEnumerable<IArtistCollectionItem> GetArtistsAsync(int limit, int offset)
        {
            return AsyncEnumerable.Empty<IArtist>();
        }

        /// <inheritdoc />
        public override IAsyncEnumerable<IPlayableCollectionGroup> GetChildrenAsync(int limit, int offset = 0)
        {
            return AsyncEnumerable.Empty<IPlayableCollectionGroup>();
        }

        /// <inheritdoc />
        public override IAsyncEnumerable<IPlaylistCollectionItem> GetPlaylistItemsAsync(int limit, int offset)
        {
            return AsyncEnumerable.Empty<IPlaylist>();
        }

        /// <inheritdoc />
        public override IAsyncEnumerable<ITrack> GetTracksAsync(int limit, int offset = 0)
        {
            return AsyncEnumerable.Empty<ITrack>();
        }
    }
}