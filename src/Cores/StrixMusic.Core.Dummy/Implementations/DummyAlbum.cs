using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using StrixMusic.CoreInterfaces.Enums;
using StrixMusic.CoreInterfaces.Interfaces;

namespace StrixMusic.Core.Dummy.Implementations
{
    /// <inheritdoc/>
    public class DummyAlbum : IAlbum
    {
        /// <inheritdoc/>
        [JsonProperty("id")]
        public string Id { get; set; } = string.Empty;

        /// <inheritdoc/>
        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

        /// <inheritdoc/>
        [JsonIgnore]
        public IReadOnlyList<ITrack> Tracks => DummyTracks;

        /// <summary>
        /// List of full <see cref="DummyTrack"/>s to be used within the DummyCore.
        /// </summary>
        [JsonIgnore]
        public List<DummyTrack> DummyTracks { get; set; } = new List<DummyTrack>();

        /// <summary>
        /// List of the Ids of <see cref="DummyTrack"/>s on the <see cref="DummyAlbum"/>.
        /// </summary>
        [JsonProperty("track_ids")]
        public IEnumerable<string>? TrackIds { get; set; }

        /// <inheritdoc/>
        [JsonIgnore]
        public IArtist Artist => DummyArtist!;

        /// <summary>
        /// The full <see cref="DummyArtist"/> of the album.
        /// </summary>
        [JsonIgnore]
        public DummyArtist? DummyArtist { get; set; }

        /// <summary>
        /// The Id of the album's artist.
        /// </summary>
        [JsonProperty("artist_id")]
        public string ArtistId { get; set; } = string.Empty;

        /// <inheritdoc/>
        [JsonProperty("image_url")]
        public Uri? CoverImageUri { get; set; }

        /// <inheritdoc/>
        public IReadOnlyList<IImage> Images => throw new NotImplementedException();

        /// <inheritdoc/>
        public Uri Url => throw new NotImplementedException();

        /// <inheritdoc/>
        public string Description => throw new NotImplementedException();

        /// <inheritdoc/>
        public IUserProfile Owner => throw new NotImplementedException();

        /// <inheritdoc/>
        public PlaybackState State => throw new NotImplementedException();

        /// <inheritdoc/>
        public ITrack PlayingTrack => throw new NotImplementedException();

        /// <inheritdoc/>
        public ICore SourceCore { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        /// <inheritdoc/>
        public int TotalTrackCount => TrackIds?.Count() ?? 0;

        /// <inheritdoc/>
        public int TotalCount => throw new NotImplementedException();

        /// <inheritdoc/>
        public void Play()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void Pause()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task PopulateTracks(int limit, int offset = 0)
        {
            throw new NotImplementedException();
        }
    }
}
