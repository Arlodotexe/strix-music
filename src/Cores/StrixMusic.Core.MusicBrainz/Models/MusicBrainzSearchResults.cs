using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hqub.MusicBrainz.API;
using Microsoft.Extensions.DependencyInjection;
using StrixMusic.Sdk.Interfaces;

namespace StrixMusic.Core.MusicBrainz.Models
{
    /// <inheritdoc cref="ISearchResults" />
    public class MusicBrainzSearchResults : MusicBrainzCollectionGroupBase, ISearchResults
    {
        private readonly MusicBrainzClient _musicBrainzClient;
        private readonly List<IAlbum> _albums;
        private readonly List<IArtist> _artists;
        private readonly List<ITrack> _tracks;
        private readonly string _query;

        /// <summary>
        /// Initializes a new instance of the <see cref="MusicBrainzSearchResults"/> class.
        /// </summary>
        /// <param name="sourceCore">The core that created this object.</param>
        /// <param name="query">The search query for these results.</param>
        public MusicBrainzSearchResults(ICore sourceCore, string query)
            : base(sourceCore)
        {
            _musicBrainzClient = SourceCore.CoreConfig.Services.GetService<MusicBrainzClient>();

            _query = query;

            // TODO: Add playlists and folders
            _albums = new List<IAlbum>();
            _artists = new List<IArtist>();
            _tracks = new List<ITrack>();
        }

        /// <inheritdoc/>
        public override async Task<IReadOnlyList<IAlbum>> PopulateAlbumsAsync(int limit, int offset = 0)
        {
            var releases = await _musicBrainzClient.Releases.SearchAsync(_query, limit, offset);

            foreach (var release in releases)
            {
                _albums.AddRange(release.Media.Select(x => new MusicBrainzAlbum(SourceCore, release, x)));
            }

            return _albums;
        }

        /// <inheritdoc/>
        public override async Task<IReadOnlyList<IArtist>> PopulateArtistsAsync(int limit, int offset = 0)
        {
            var artists = await _musicBrainzClient.Artists.SearchAsync(_query, limit, offset);

            foreach (var item in artists)
            {
                _artists.Add(new MusicBrainzArtist(SourceCore, item));
            }

            return _artists;
        }

        /// <inheritdoc/>
        public override Task<IReadOnlyList<IPlayableCollectionGroup>> PopulateChildrenAsync(int limit, int offset = 0)
        {
            return Task.FromResult(Children);
        }

        /// <inheritdoc/>
        public override Task<IReadOnlyList<IPlaylist>> PopulatePlaylistsAsync(int limit, int offset = 0)
        {
            return Task.FromResult(Playlists);
        }

        /// <inheritdoc/>
        public override async Task<IReadOnlyList<ITrack>> PopulateTracksAsync(int limit, int offset = 0)
        {
            var recordings = await _musicBrainzClient.Recordings.SearchAsync(_query, limit, offset);

            foreach (var recording in recordings)
            {
                foreach (var release in recording.Releases)
                {
                    foreach (var medium in release.Media)
                    {
                        foreach (var track in medium.Tracks)
                        {
                            _tracks.Add(new MusicBrainzTrack(SourceCore, track, new MusicBrainzAlbum(SourceCore, release, medium)));
                        }
                    }
                }
            }

            return _tracks;
        }
    }
}
