using Newtonsoft.Json;
using StrixMusic.CoreInterfaces;
using StrixMusic.CoreInterfaces.Enums;
using StrixMusic.CoreInterfaces.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StrixMusic.Core.Mock.Models
{
    /// <inheritdoc cref="IAlbum"/>
    public class MockAlbum : IAlbum
    {
        /// <inheritdoc/>
        [JsonProperty("id")]
        public string MockId { get; set; } = string.Empty;

        /// <inheritdoc/>
        [JsonProperty("name")]
        public string NameJson { get; set; } = string.Empty;

        /// <inheritdoc/>
        [JsonProperty("description")]
        public string MockDescription { get; set; } = string.Empty;

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
        public string Id => MockId;

        /// <inheritdoc/>
        public Uri Url => throw new NotImplementedException();

        /// <inheritdoc/>
        public string Name => NameJson;

        /// <inheritdoc/>
        public IReadOnlyList<IImage> Images => throw new NotImplementedException();

        /// <inheritdoc/>
        public string Description => MockDescription;

        /// <inheritdoc/>
        public PlaybackState PlaybackState => throw new NotImplementedException();

        /// <inheritdoc/>
        public TimeSpan Duration => throw new NotImplementedException();

        /// <inheritdoc/>
        public IReadOnlyList<ITrack> Tracks => throw new NotImplementedException();

        /// <inheritdoc/>
        public IPlayableCollectionGroup RelatedItems => throw new NotImplementedException();

        /// <inheritdoc/>
        public bool IsPlayAsyncSupported => throw new NotImplementedException();

        /// <inheritdoc/>
        public bool IsPauseAsyncSupported => throw new NotImplementedException();

        /// <inheritdoc/>
        public bool IsChangeNameAsyncSupported => throw new NotImplementedException();

        /// <inheritdoc/>
        public bool IsChangeImagesAsyncSupported => throw new NotImplementedException();

        /// <inheritdoc/>
        public bool IsChangeDescriptionAsyncSupported => throw new NotImplementedException();

        /// <inheritdoc/>
        public bool IsChangeDurationAsyncSupported => throw new NotImplementedException();

        /// <inheritdoc/>
        public event EventHandler<CollectionChangedEventArgs<ITrack>>? TracksChanged;

        /// <inheritdoc/>
        public event EventHandler<PlaybackState>? PlaybackStateChanged;

        /// <inheritdoc/>
        public event EventHandler<string>? NameChanged;

        /// <inheritdoc/>
        public event EventHandler<string?>? DescriptionChanged;

        /// <inheritdoc/>
        public event EventHandler<Uri?>? UrlChanged;

        /// <inheritdoc/>
        public event EventHandler<CollectionChangedEventArgs<IImage>>? ImagesChanged;

        /// <inheritdoc/>
        public event EventHandler<TimeSpan>? DurationChanged;

        /// <inheritdoc/>
        public Task ChangeDescriptionAsync(string? description)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task ChangeDurationAsync(TimeSpan duration)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task ChangeImagesAsync(IReadOnlyList<IImage> images)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task ChangeNameAsync(string name)
        {
            throw new NotImplementedException();
        }

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
        public Task<IReadOnlyList<ITrack>> PopulateTracksAsync(int limit, int offset)
        {
            return Task.FromResult(new List<ITrack>() as IReadOnlyList<ITrack>);
        }
    }
}
