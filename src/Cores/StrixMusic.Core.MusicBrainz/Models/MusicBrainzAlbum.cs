using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hqub.MusicBrainz.API.Entities;
using Newtonsoft.Json;
using StrixMusic.Sdk;
using StrixMusic.Sdk.Enums;
using StrixMusic.Sdk.Interfaces;

namespace StrixMusic.Core.MusicBrainz.Models
{
    /// <inheritdoc cref="IAlbum"/>
    public class MusicBrainzAlbum : IAlbum
    {
        private readonly Release _release;

        private readonly List<ITrack> _albums;

        private readonly IArtist _artist;

        /// <summary>
        /// Initializes a new instance of the <see cref="MusicBrainzAlbum"/> class.
        /// </summary>
        /// <param name="release"></param>
        /// <param name="sourceCore"></param>
        public MusicBrainzAlbum(Release release, ICore sourceCore, IArtist artist)
        {
            SourceCore = sourceCore;
            _release = release;
            _artist = artist;
            _albums = new List<ITrack>();
        }

        /// <inheritdoc/>
        public IArtist Artist => _artist;

        /// <inheritdoc/>
        public int TotalTracksCount => throw new NotImplementedException();

        /// <inheritdoc/>
        public ICore SourceCore { get; }

        /// <inheritdoc/>
        public string Id => _release.Id;

        /// <inheritdoc/>
        public Uri Url => throw new NotImplementedException();

        /// <inheritdoc/>
        public string Name => _release.Title;

        /// <inheritdoc/>
        public IReadOnlyList<IImage> Images => throw new NotImplementedException();

        /// <inheritdoc/>
        public string Description => _release.TextRepresentation.Script;

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
