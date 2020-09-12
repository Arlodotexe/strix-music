using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Hqub.MusicBrainz.API;
using Microsoft.Extensions.DependencyInjection;
using StrixMusic.Sdk;
using StrixMusic.Sdk.Enums;
using StrixMusic.Sdk.Interfaces;

namespace StrixMusic.Core.MusicBrainz.Models
{
    /// <inheritdoc/>
    public class MusicBrainzSearchResults : MusicBrainzCollectionGroupBase, ISearchResults
    {
        private readonly MusicBrainzClient _musicBrainzClient;
        private readonly List<IAlbum> _albums;
        private readonly List<IArtist> _artists;
        private readonly List<ITrack> _tracks;

        /// <summary>
        /// Initializes a new instance of the <see cref="MusicBrainzSearchResults"/> class.
        /// </summary>
        /// <param name="sourceCore"></param>
        public MusicBrainzSearchResults(ICore sourceCore)
            : base(sourceCore)
        {
            _musicBrainzClient = SourceCore.CoreConfig.Services.GetService<MusicBrainzClient>();
            _albums = new List<IAlbum>();
            _artists = new List<IArtist>();
            _tracks = new List<ITrack>();
        }

        /// <inheritdoc/>
        public override Task ChangeDescriptionAsync(string? description)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public override Task ChangeDurationAsync(TimeSpan duration)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public override Task ChangeImagesAsync(IReadOnlyList<IImage> images)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public override Task ChangeNameAsync(string name)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public override Task PauseAsync()
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public override Task PlayAsync()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public async override Task<IReadOnlyList<IAlbum>> PopulateAlbumsAsync(int limit, int offset = 0)
        {
            var releases = await _musicBrainzClient.Releases.SearchAsync("*", limit, offset);
            Parallel.ForEach(releases, (release) =>
            {
                _albums.Add(new MusicBrainzAlbum(SourceCore, release));
            });

            return _albums;
        }

        /// <inheritdoc/>
        public async override Task<IReadOnlyList<IArtist>> PopulateArtistsAsync(int limit, int offset = 0)
        {
            var artists = await _musicBrainzClient.Artists.SearchAsync("*", limit, offset);
            Parallel.ForEach(artists, (artist) =>
            {
                _artists.Add(new MusicBrainzArtist(SourceCore, artist));
            });

            return _artists;
        }

        /// <inheritdoc/>
        public override Task<IReadOnlyList<IPlayableCollectionGroup>> PopulateChildrenAsync(int limit, int offset = 0)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override Task<IReadOnlyList<IPlaylist>> PopulatePlaylistsAsync(int limit, int offset = 0)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public async override Task<IReadOnlyList<ITrack>> PopulateTracksAsync(int limit, int offset = 0)
        {
            var recordings = await _musicBrainzClient.Recordings.SearchAsync("*", limit, offset);
            Parallel.ForEach(recordings, (recording) =>
            {
                _tracks.Add(new MusicBrainzTrack(SourceCore, recording));
            });

            return _tracks;
        }
    }
}
