using System.Collections.Generic;
using Newtonsoft.Json;
using StrixMusic.Core.Dummy.Implementations;

namespace StrixMusic.Core.Dummy
{
    /// <summary>
    /// The root type for the Library stored in JSON.
    /// </summary>
    public class Library
    {
        /// <summary>
        /// The lists of tracks in the dummy core's library.
        /// </summary>
        [JsonProperty("tracks")]
        public List<DummyTrack>? Tracks { get; set; }

        /// <summary>
        /// The lists of albums in the dummy core's library.
        /// </summary>
        [JsonProperty("albums")]
        public List<DummyAlbum>? Albums { get; set; }

        /// <summary>
        /// The lists of artists in the dummy core's library.
        /// </summary>
        [JsonProperty("artists")]
        public List<DummyArtist>? Artists { get; set; }
    }
}
