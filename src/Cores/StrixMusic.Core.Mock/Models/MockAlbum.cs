using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using StrixMusic.CoreInterfaces;
using StrixMusic.CoreInterfaces.Enums;
using StrixMusic.CoreInterfaces.Interfaces;

namespace StrixMusic.Core.Mock.Models
{
    /// <inheritdoc cref="IAlbum"/>
    public class MockAlbum : IAlbum
    {
        private IArtist _artist;
        private List<ITrack> _tracks = new List<ITrack>();
        private int _totalTracksCount;
        private ICore _sourceCore;
        private string _id;
        private Uri _url;
        private string _name;
        private List<IImage> _images = new List<IImage>();
        private string _description;
        private int _duration;
        private PlaybackState _playbackState;

        /// <summary>
        /// Init MockAlbum
        /// </summary>
        public MockAlbum()
        {
            _totalTracksCount = 10;
            _id = "33";
            _url = new Uri("http://test.com");
            _name = "Test name";
            _description = "Test description";
            _playbackState = PlaybackState.None;
        }

        /// <inheritdoc cref="IAlbum.Artist"/>
        public IArtist Artist => _artist;

        /// <inheritdoc cref="IAlbum.Tracks"/>
        public IReadOnlyList<ITrack> Tracks => _tracks;

        /// <inheritdoc cref="IAlbum.TotalTracksCount"/>
        public int TotalTracksCount => _totalTracksCount;

        /// <inheritdoc cref="IAlbum.SourceCore"/>
        public ICore SourceCore => _sourceCore;

        /// <inheritdoc cref="IAlbum.Id"/>
        public string Id => _id;

        /// <inheritdoc cref="IAlbum.Url"/>
        public Uri Url => _url;

        /// <inheritdoc cref="IAlbum.Name"/>
        public string Name => _name;

        /// <inheritdoc cref="IAlbum.Images"/>
        public IReadOnlyList<IImage> Images => _images;


        /// <inheritdoc cref="IAlbum.Description"/>
        public string Description => _description;


        /// <inheritdoc cref="IAlbum.PlaybackState"/>
        public PlaybackState PlaybackState => throw new NotImplementedException();


        /// <inheritdoc cref="IAlbum.Duration"/>
        public TimeSpan Duration => throw new NotImplementedException();

        /// <inheritdoc cref="IAlbum.RelatedItems"/>
        public IPlayableCollectionGroup RelatedItems => throw new NotImplementedException();

        /// <inheritdoc cref="IAlbum.TracksChanged"/>
        public event EventHandler<CollectionChangedEventArgs<ITrack>> TracksChanged;

        /// <inheritdoc cref="IAlbum.PlaybackStateChanged"/>
        public event EventHandler<PlaybackState> PlaybackStateChanged;

        /// <inheritdoc cref="IAlbum.PlaybackStateChanged"/>
        public event EventHandler<string> NameChanged;

        /// <inheritdoc cref="IAlbum.DescriptionChanged"/>
        public event EventHandler<string> DescriptionChanged;

        /// <inheritdoc cref="IAlbum.UrlChanged"/>
        public event EventHandler<Uri> UrlChanged;

        /// <inheritdoc cref="IAlbum.ImagesChanged"/>
        public event EventHandler<CollectionChangedEventArgs<IImage>> ImagesChanged;

        /// <inheritdoc cref="IAlbum.PauseAsync"/>
        public Task PauseAsync()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc cref="IAlbum.PlayAsync"/>
        public Task PlayAsync()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc cref="IAlbum.PopulateTracksAsync"/>
        public Task<IReadOnlyList<ITrack>> PopulateTracksAsync(int limit, int offset = 0)
        {
            _tracks = new List<ITrack>()
            {
                new MockTrack(),
                new MockTrack(),
                new MockTrack(),
                new MockTrack(),
            };
            return Task.FromResult(Tracks);
        }
    }
}
