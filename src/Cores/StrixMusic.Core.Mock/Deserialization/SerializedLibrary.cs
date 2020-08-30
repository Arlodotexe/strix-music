using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using StrixMusic.Core.Mock.Models;
using StrixMusic.CoreInterfaces;
using StrixMusic.CoreInterfaces.Enums;
using StrixMusic.CoreInterfaces.Interfaces;

namespace StrixMusic.Core.Mock.Deserialization
{
    /// <summary>
    /// The lists of tracks in the dummy core's library.
    /// </summary>
    public class SerializedLibrary : MockPlayableCollectionGroupBase
    {
        // Json will be here properites here.

        /// <summary>
        /// The lists of tracks in the dummy core's library.
        /// </summary>
        [JsonProperty("tracks")]
        public List<MockTrack>? TracksInfo { get; set; }

        /// <summary>
        /// The lists of albums in the dummy core's library.
        /// </summary>
        [JsonProperty("albums")]
        public List<MockAlbum>? AlbumsInfo { get; set; }

        /// <summary>
        /// The lists of artists in the dummy core's library.
        /// </summary>
        [JsonProperty("artists")]
        public List<MockArtist>? ArtistsInfo { get; set; }

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
        public override Task PopulateAlbumsAsync(int limit, int offset = 0)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override Task PopulateArtistsAsync(int limit, int offset = 0)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override Task PopulateChildrenAsync(int limit, int offset = 0)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override Task PopulatePlaylistsAsync(int limit, int offset = 0)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override Task PopulateTracksAsync(int limit, int offset = 0)
        {
            throw new NotImplementedException();
        }
    }
}
