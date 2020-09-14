using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hqub.MusicBrainz.API;
using Microsoft.Extensions.DependencyInjection;
using StrixMusic.Core.MusicBrainz.Statics;
using StrixMusic.Sdk.Interfaces;

namespace StrixMusic.Core.MusicBrainz.Models
{
    /// <summary>
    /// The lists of tracks in the dummy core's library.
    /// </summary>
    public class MusicBrainzLibrary : MusicBrainzCollectionGroupBase, ILibrary
    {
        private readonly MusicBrainzClient _musicBrainzClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="MusicBrainzLibrary"/> class.
        /// </summary>
        /// <param name="sourceCore"></param>
        public MusicBrainzLibrary(ICore sourceCore)
            : base(sourceCore)
        {
            SourceCore = sourceCore;
            _musicBrainzClient = SourceCore.CoreConfig.Services.GetService<MusicBrainzClient>();
        }

        /// <inheritdoc/>
        public override async Task<IReadOnlyList<IAlbum>> PopulateAlbumsAsync(int limit, int offset = 0)
        {
            var albums = await _musicBrainzClient.Releases.SearchAsync("*", limit, offset);
            var list = new List<IAlbum>();

            foreach (var release in albums.Items)
            {
                list.AddRange(release.Media.Select(medium => new MusicBrainzAlbum(SourceCore, release, medium)));
            }

            return list;
        }

        /// <inheritdoc/>
        public override async Task<IReadOnlyList<IArtist>> PopulateArtistsAsync(int limit, int offset = 0)
        {
            var artists = await _musicBrainzClient.Artists.SearchAsync("*", limit, offset);

            return artists.Items.Select(item => new MusicBrainzArtist(SourceCore, item)).Cast<IArtist>().ToList();
        }

        /// <inheritdoc/>
        public override Task<IReadOnlyList<IPlayableCollectionGroup>> PopulateChildrenAsync(int limit, int offset = 0)
        {
            return Task.FromResult<IReadOnlyList<IPlayableCollectionGroup>>(new List<IPlayableCollectionGroup>());
        }

        /// <inheritdoc/>
        public override Task<IReadOnlyList<IPlaylist>> PopulatePlaylistsAsync(int limit, int offset = 0)
        {
            return Task.FromResult<IReadOnlyList<IPlaylist>>(new List<IPlaylist>());
        }

        /// <inheritdoc/>
        public override async Task<IReadOnlyList<ITrack>> PopulateTracksAsync(int limit, int offset = 0)
        {
            var recordings = await _musicBrainzClient.Recordings.SearchAsync($"*&{RelationshipQueries.RecordingsQuery}", limit, offset);

            return recordings.Items.Select(item => new MusicBrainzTrack(SourceCore, item)).Cast<ITrack>().ToList();
        }
    }
}
