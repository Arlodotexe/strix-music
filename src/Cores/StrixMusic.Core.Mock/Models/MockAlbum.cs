using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using StrixMusic.CoreInterfaces;
using StrixMusic.CoreInterfaces.Enums;
using StrixMusic.CoreInterfaces.Interfaces;

namespace StrixMusic.Core.Mock.Models
{
    /// <inheritdoc cref="IAlbum"/>
    public class MockAlbum : IAlbum
    {

        /// <inheritdoc/>
        [JsonProperty("id")]
        public string IdJson { get; set; } = string.Empty;

        /// <inheritdoc/>
        [JsonProperty("name")]
        public string NameJson { get; set; } = string.Empty;

        /// <inheritdoc/>
        [JsonIgnore]
        public IReadOnlyList<MockTrack> Tracks => DummyTracks;

        /// <inheritdoc/>
        [JsonIgnore]
        public List<MockTrack> DummyTracks { get; set; } = new List<MockTrack>();

        /// <inheritdoc/>
        [JsonProperty("track_ids")]
        public IEnumerable<string>? TrackIds { get; set; }

        /// <inheritdoc/>
        public IArtist Artist => throw new NotImplementedException();

        /// <inheritdoc/>
        public int TotalTracksCount => throw new NotImplementedException();

        /// <inheritdoc/>
        public ICore SourceCore => throw new NotImplementedException();

        /// <inheritdoc/>
        public string Id => throw new NotImplementedException();

        /// <inheritdoc/>
        public Uri Url => throw new NotImplementedException();

        /// <inheritdoc/>
        public string Name => throw new NotImplementedException();

        /// <inheritdoc/>
        public IReadOnlyList<IImage> Images => throw new NotImplementedException();

        /// <inheritdoc/>
        public string Description => throw new NotImplementedException();

        /// <inheritdoc/>
        public PlaybackState PlaybackState => throw new NotImplementedException();

        /// <inheritdoc/>
        public TimeSpan Duration => throw new NotImplementedException();

        /// <inheritdoc/>
        public IReadOnlyList<IPlayableCollectionGroup> RelatedItems => throw new NotImplementedException();

        /// <inheritdoc/>
        public int TotalRelatedItemsCount => throw new NotImplementedException();

        /// <inheritdoc/>
        IReadOnlyList<ITrack> ITrackCollection.Tracks => throw new NotImplementedException();

        /// <inheritdoc/>
        public event EventHandler<CollectionChangedEventArgs<ITrack>> TracksChanged;

        /// <inheritdoc/>
        public event EventHandler<PlaybackState> PlaybackStateChanged;

        /// <inheritdoc/>
        public event EventHandler<string> NameChanged;

        /// <inheritdoc/>
        public event EventHandler<string> DescriptionChanged;

        /// <inheritdoc/>
        public event EventHandler<Uri> UrlChanged;

        /// <inheritdoc/>
        public event EventHandler<CollectionChangedEventArgs<IImage>> ImagesChanged;

        /// <inheritdoc/>
        public event EventHandler<CollectionChangedEventArgs<IPlayableCollectionGroup>> RelatedItemsChanged;

        /// <inheritdoc/>
        public Task PauseAsync()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task PlayAsync()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task PopulateRelatedItemsAsync(int limit, int offset = 0)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task PopulateTracksAsync(int limit, int offset = 0)
        {
            throw new NotImplementedException();
        }
    }
}
