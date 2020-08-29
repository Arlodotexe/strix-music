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
    public class SerializedLibrary : IPlayableCollectionBase
    {
        /// <summary>
        /// The lists of tracks in the dummy core's library.
        /// </summary>
        [JsonProperty("tracks")]
        public List<MockTrack>? Tracks { get; set; }

        /// <summary>
        /// The lists of albums in the dummy core's library.
        /// </summary>
        [JsonProperty("albums")]
        public List<MockAlbum>? Albums { get; set; }

        /// <summary>
        /// The lists of artists in the dummy core's library.
        /// </summary>
        [JsonProperty("artists")]
        public List<MockArtist>? Artists { get; set; }

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
        public TimeSpan Duration => throw new NotImplementedException();

        /// <inheritdoc/>
        public event EventHandler<PlaybackState>? PlaybackStateChanged;

        /// <inheritdoc/>
        public event EventHandler<CollectionChangedEventArgs<IImage>>? ImagesChanged;

        /// <inheritdoc/>
        public event EventHandler<string>? NameChanged
        {
            add
            {
                NameChanged += value;
            }

            remove
            {
                NameChanged -= value;
            }
        }

        /// <inheritdoc/>
        public event EventHandler<string?> DescriptionChanged
        {
            add
            {
                NameChanged += value;
            }

            remove
            {
                NameChanged -= value;
            }
        }

        /// <inheritdoc/>
        public event EventHandler<Uri?> UrlChanged
        {
            add
            {
                UrlChanged += value;
            }

            remove
            {
                UrlChanged -= value;
            }
        }

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
