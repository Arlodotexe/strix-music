using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using StrixMusic.Core.Dummy.Implementations;
using StrixMusic.CoreInterfaces.Enums;
using StrixMusic.CoreInterfaces.Interfaces;

namespace StrixMusic.Core.Dummy.Deserialization
{
    /// <summary>
    /// A <see cref="class"/> 
    /// </summary>
    public class SerializedLibrary : IPlayableCollectionBase
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

        /// <inheritdoc/>
        public string Id => throw new NotImplementedException();

        /// <inheritdoc/>
        public ICore SourceCore => throw new NotImplementedException();

        /// <inheritdoc/>
        public string Name => throw new NotImplementedException();

        /// <inheritdoc/>
        public IReadOnlyList<IImage> Images => throw new NotImplementedException();

        /// <inheritdoc/>
        public Uri Url => throw new NotImplementedException();

        /// <inheritdoc/>
        public string? Description => throw new NotImplementedException();

        /// <inheritdoc/>
        public IUserProfile? Owner => throw new NotImplementedException();

        /// <inheritdoc/>
        public PlaybackState PlaybackState => throw new NotImplementedException();

        /// <inheritdoc/>
        public ITrack? PlayingTrack => throw new NotImplementedException();

        /// <inheritdoc/>
        public event EventHandler<PlaybackState>? PlaybackStateChanged;

        /// <inheritdoc/>
        public void Pause()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task PauseAsync()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void Play()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task PlayAsync()
        {
            throw new NotImplementedException();
        }
    }
}
