using System;
using System.Collections.Generic;
using System.Linq;
using OwlCore.Collections;
using StrixMusic.Sdk.Interfaces;

namespace StrixMusic.Core.MusicBrainz.Models
{
    /// <summary>
    /// The recently played items for the <see cref="MusicBrainzCore"/>.
    /// </summary>
    /// <remarks>MusicBrainz has no playback mechanism, so collections should never return anything.</remarks>
    public class MusicBrainzRecentlyPlayed : MusicBrainzCollectionGroupBase, IRecentlyPlayed
    {
        /// <inheritdoc />
        public MusicBrainzRecentlyPlayed(ICore sourceCore)
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
        public override SynchronizedObservableCollection<IImage> Images { get; protected set; } = new SynchronizedObservableCollection<IImage>();

        /// <inheritdoc />
        public override string? Description { get; protected set; } = null;

        /// <inheritdoc />
        public override int TotalChildrenCount { get; internal set; } = 0;

        /// <inheritdoc />
        public override int TotalPlaylistCount { get; internal set; } = 0;

        /// <inheritdoc />
        public override int TotalTracksCount { get; internal set; } = 0;

        /// <inheritdoc />
        public override int TotalAlbumsCount { get; internal set; } = 0;

        /// <inheritdoc />
        public override int TotalArtistsCount { get; internal set; } = 0;

        /// <inheritdoc />
        public override IAsyncEnumerable<IAlbum> GetAlbumsAsync(int limit, int offset = 0)
        {
            return AsyncEnumerable.Empty<IAlbum>();
        }

        /// <inheritdoc />
        public override IAsyncEnumerable<IArtist> GetArtistsAsync(int limit, int offset = 0)
        {
            return AsyncEnumerable.Empty<IArtist>();
        }

        /// <inheritdoc />
        public override IAsyncEnumerable<IPlayableCollectionGroup> GetChildrenAsync(int limit, int offset = 0)
        {
            return AsyncEnumerable.Empty<IPlayableCollectionGroup>();
        }

        /// <inheritdoc />
        public override IAsyncEnumerable<IPlaylist> GetPlaylistsAsync(int limit, int offset = 0)
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