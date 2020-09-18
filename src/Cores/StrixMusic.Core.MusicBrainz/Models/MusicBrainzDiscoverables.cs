using System.Collections.Generic;
using System.Threading.Tasks;
using StrixMusic.Sdk.Interfaces;

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
        public override Task<IReadOnlyList<IAlbum>> PopulateAlbumsAsync(int limit, int offset = 0)
        {
            return Task.FromResult<IReadOnlyList<IAlbum>>(new List<IAlbum>());
        }

        /// <inheritdoc />
        public override Task<IReadOnlyList<IArtist>> PopulateArtistsAsync(int limit, int offset = 0)
        {
            return Task.FromResult<IReadOnlyList<IArtist>>(new List<IArtist>());
        }

        /// <inheritdoc />
        public override Task<IReadOnlyList<IPlayableCollectionGroup>> PopulateChildrenAsync(int limit, int offset = 0)
        {
            return Task.FromResult<IReadOnlyList<IPlayableCollectionGroup>>(new List<IPlayableCollectionGroup>());
        }

        /// <inheritdoc />
        public override Task<IReadOnlyList<IPlaylist>> PopulatePlaylistsAsync(int limit, int offset = 0)
        {
            return Task.FromResult<IReadOnlyList<IPlaylist>>(new List<IPlaylist>());
        }

        /// <inheritdoc />
        public override Task<IReadOnlyList<ITrack>> PopulateTracksAsync(int limit, int offset = 0)
        {
            return Task.FromResult<IReadOnlyList<ITrack>>(new List<ITrack>());
        }
    }
}