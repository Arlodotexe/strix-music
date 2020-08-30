using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using StrixMusic.CoreInterfaces;
using StrixMusic.CoreInterfaces.Enums;
using StrixMusic.CoreInterfaces.Interfaces;

namespace StrixMusic.Core.Mock.Models
{
    /// <inheritdoc cref="ITrack"/>
    public class MockTrack : ITrack
    {
        private string _type;
        private List<IArtist> _artists = new List<IArtist>();
        private IAlbum _albums;
        private DateTime? _datePublished;
        private List<string> _genres = new List<string>();
        private int? _trackNumber;
        private int? _playCount;
        private string _language;
        private ILyrics _lyrics;
        private bool _isExplicit;
        private ICore _sourceCore;
        private string _id;
        private Uri _url;
        private string _name;
        private List<IImage> _images = new List<IImage>();
        private string _description;
        private PlaybackState _playBackState;
        private TimeSpan _duration;
        private List<IPlayableCollectionGroup> _relatedItems = new List<IPlayableCollectionGroup>();
        private int _totalRelatedItemsCount;

        ///<summary>
        /// Init Track
        /// </summary>
        public MockTrack()
        {
            _playCount = 10;
            _language = "en";
            _isExplicit = false;
            _id = "45";
            _url = new Uri("http://test.com");
            _playCount = 12;
            _duration = TimeSpan.FromMinutes(1);
        }

        /// <inheritdoc cref="ITrack.Type"/>
        public string Type => _type;

        /// <inheritdoc cref="ITrack.Artists"/>
        public IReadOnlyList<IArtist> Artists => _artists;

        /// <inheritdoc cref="ITrack.Album"/>
        public IAlbum Album => _albums;

        /// <inheritdoc cref="ITrack.DatePublished"/>
        public DateTime? DatePublished => _datePublished;

        /// <inheritdoc cref="ITrack.Genres"/>
        public IReadOnlyList<string> Genres => _genres;

        /// <inheritdoc cref="ITrack.TrackNumber"/>
        public int? TrackNumber => _trackNumber;

        /// <inheritdoc cref="ITrack.PlayCount"/>
        public int? PlayCount => _playCount;

        /// <inheritdoc cref="ITrack.Language"/>
        public string Language => _language;

        /// <inheritdoc cref="ITrack.Lyrics"/>
        public ILyrics Lyrics => _lyrics;

        /// <inheritdoc cref="ITrack.IsExplicit"/>
        public bool IsExplicit => _isExplicit;

        /// <inheritdoc cref="ITrack.SourceCore"/>
        public ICore SourceCore => _sourceCore;

        /// <inheritdoc cref="ITrack.Id"/>
        public string Id => _id;

        /// <inheritdoc cref="ITrack.Url"/>
        public Uri Url => _url;

        /// <inheritdoc cref="ITrack.Name"/>
        public string Name => _name;

        /// <inheritdoc cref="ITrack.Images"/>
        public IReadOnlyList<IImage> Images => _images;

        /// <inheritdoc cref="ITrack.Description"/>
        public string Description => _description;

        /// <inheritdoc cref="ITrack.PlaybackState"/>
        public PlaybackState PlaybackState => _playBackState;

        /// <inheritdoc cref="ITrack.Duration"/>
        public TimeSpan Duration => _duration;

        /// <inheritdoc cref="ITrack.RelatedItems"/>
        public IReadOnlyList<IPlayableCollectionGroup> RelatedItems => _relatedItems;

        /// <inheritdoc cref="ITrack.TotalRelatedItemsCount"/>
        public int TotalRelatedItemsCount => _totalRelatedItemsCount;

        /// <inheritdoc cref="ITrack.Type"/>
        TrackType ITrack.Type => throw new NotImplementedException();

        /// <inheritdoc cref="ITrack.Language"/>
        CultureInfo ITrack.Language => throw new NotImplementedException();

        /// <inheritdoc cref="ITrack.ArtistsChanged"/>
        public event EventHandler<CollectionChangedEventArgs<IArtist>> ArtistsChanged;

        /// <inheritdoc cref="ITrack.GenresChanged"/>
        public event EventHandler<CollectionChangedEventArgs<string>> GenresChanged;

        /// <inheritdoc cref="ITrack.AlbumChanged"/>
        public event EventHandler<IAlbum> AlbumChanged;

        /// <inheritdoc cref="ITrack.DatePublishedChanged"/>
        public event EventHandler<DateTime?> DatePublishedChanged;

        /// <inheritdoc cref="ITrack.TrackNumberChanged"/>
        public event EventHandler<int?> TrackNumberChanged;

        /// <inheritdoc cref="ITrack.PlayCountChanged"/>
        public event EventHandler<int?> PlayCountChanged;

        /// <inheritdoc cref="ITrack.LanguageChanged"/>
        public event EventHandler<string> LanguageChanged;

        /// <inheritdoc cref="ITrack.LyricsChanged"/>
        public event EventHandler<ILyrics> LyricsChanged;

        /// <inheritdoc cref="ITrack.IsExplicitChanged"/>
        public event EventHandler<bool> IsExplicitChanged;

        /// <inheritdoc cref="ITrack.PlaybackStateChanged"/>
        public event EventHandler<PlaybackState> PlaybackStateChanged;

        /// <inheritdoc cref="ITrack.NameChanged"/>
        public event EventHandler<string> NameChanged;

        /// <inheritdoc cref="ITrack.DescriptionChanged"/>
        public event EventHandler<string> DescriptionChanged;

        /// <inheritdoc cref="ITrack.UrlChanged"/>
        public event EventHandler<Uri> UrlChanged;

        /// <inheritdoc cref="ITrack.ImagesChanged"/>
        public event EventHandler<CollectionChangedEventArgs<IImage>> ImagesChanged;

        /// <inheritdoc cref="ITrack.RelatedItemsChanged"/>
        public event EventHandler<CollectionChangedEventArgs<IPlayableCollectionGroup>> RelatedItemsChanged;

        /// <inheritdoc cref="ITrack.LanguageChanged"/>
        event EventHandler<CultureInfo> ITrack.LanguageChanged
        {
            add
            {
                throw new NotImplementedException();
            }

            remove
            {
                throw new NotImplementedException();
            }
        }

        /// <inheritdoc cref="ITrack.PauseAsync"/>
        public Task PauseAsync()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc cref="ITrack.PlayAsync"/>
        public Task PlayAsync()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc cref="ITrack.PopulateRelatedItemsAsync"/>
        public Task PopulateRelatedItemsAsync(int limit, int offset = 0)
        {
            throw new NotImplementedException();
        }
    }
}
