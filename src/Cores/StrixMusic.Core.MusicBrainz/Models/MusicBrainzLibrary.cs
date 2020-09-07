using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Threading.Tasks;
using Hqub.MusicBrainz.API;
using Newtonsoft.Json;
using StrixMusic.Sdk.Interfaces;

namespace StrixMusic.Core.MusicBrainz.Models
{
    /// <summary>
    /// The lists of tracks in the dummy core's library.
    /// </summary>
    public class MusicBrainzLibrary : MusicBrainzCollectionGroupBase, ILibrary
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="MusicBrainzLibrary"/> class.
        /// </summary>
        /// <param name="sourceCore"></param>
        public MusicBrainzLibrary(ICore sourceCore)
            : base(sourceCore)
        {
            SourceCore = sourceCore;
        }

        /// <inheritdoc/>
        public override Task ChangeDescriptionAsync(string? description)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override Task ChangeDurationAsync(TimeSpan duration)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override Task ChangeImagesAsync(IReadOnlyList<IImage> images)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override Task ChangeNameAsync(string name)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override Task PauseAsync()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override Task PlayAsync()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override async Task<IReadOnlyList<IAlbum>> PopulateAlbumsAsync(int limit, int offset = 0)
        {
            var artists = await new MusicBrainzClient().Releases.SearchAsync("*", limit, offset);
            var list = new List<IAlbum>();
            foreach (var item in artists.Items)
            {
               // list.Add(new MusicBrainzAlbum(item, SourceCore, new MusicBrainzArtist(null)));
            }

            return list;
        }

        /// <inheritdoc/>
        public override async Task<IReadOnlyList<IArtist>> PopulateArtistsAsync(int limit, int offset = 0)
        {
            var artists = await new MusicBrainzClient().Artists.SearchAsync("*", limit, offset);
            var list = new List<IArtist>();
            foreach (var item in artists.Items)
            {
                list.Add(new MusicBrainzArtist(item, SourceCore));
            }

            return list;
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
            var recordings = await new MusicBrainzClient().Recordings.SearchAsync("*", limit, offset);
            var list = new List<ITrack>();
            foreach (var item in recordings.Items)
            {
                list.Add(new MusicBrainzTrack(item, SourceCore, new MusicBrainzAlbum(item.Releases.FirstOrDefault(), SourceCore, new MusicBrainzArtist(item.Releases.FirstOrDefault().Relations.FirstOrDefault().Artist, SourceCore))));
            }

            return list;
        }
    }
}
