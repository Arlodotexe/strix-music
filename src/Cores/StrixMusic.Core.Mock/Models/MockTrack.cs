using StrixMusic.CoreInterfaces;
using StrixMusic.CoreInterfaces.Enums;
using StrixMusic.CoreInterfaces.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;

namespace StrixMusic.Core.Mock.Models
{
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

        public string Type => _type;

        public IReadOnlyList<IArtist> Artists => _artists;

        public IAlbum Album => _albums;

        public DateTime? DatePublished => _datePublished;

        public IReadOnlyList<string> Genres => _genres;

        public int? TrackNumber => _trackNumber;

        public int? PlayCount => _playCount;

        public string Language => _language;

        public ILyrics Lyrics => _lyrics;

        public bool IsExplicit => _isExplicit;

        public ICore SourceCore => _sourceCore;

        public string Id => _id;

        public Uri Url => _url;

        public string Name => _name;

        public IReadOnlyList<IImage> Images => _images;

        public string Description => _description;

        public PlaybackState PlaybackState => _playBackState;

        public TimeSpan Duration =>_duration;

        public IReadOnlyList<IPlayableCollectionGroup> RelatedItems => _relatedItems;

        public int TotalRelatedItemsCount =>_totalRelatedItemsCount;

        TrackType ITrack.Type => throw new NotImplementedException();

        CultureInfo ITrack.Language => throw new NotImplementedException();

        public event EventHandler<CollectionChangedEventArgs<IArtist>> ArtistsChanged;
        public event EventHandler<CollectionChangedEventArgs<string>> GenresChanged;
        public event EventHandler<IAlbum> AlbumChanged;
        public event EventHandler<DateTime?> DatePublishedChanged;
        public event EventHandler<int?> TrackNumberChanged;
        public event EventHandler<int?> PlayCountChanged;
        public event EventHandler<string> LanguageChanged;
        public event EventHandler<ILyrics> LyricsChanged;
        public event EventHandler<bool> IsExplicitChanged;
        public event EventHandler<PlaybackState> PlaybackStateChanged;
        public event EventHandler<string> NameChanged;
        public event EventHandler<string> DescriptionChanged;
        public event EventHandler<Uri> UrlChanged;
        public event EventHandler<CollectionChangedEventArgs<IImage>> ImagesChanged;
        public event EventHandler<CollectionChangedEventArgs<IPlayableCollectionGroup>> RelatedItemsChanged;

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

        public Task PauseAsync()
        {
            throw new NotImplementedException();
        }

        public Task PlayAsync()
        {
            throw new NotImplementedException();
        }

        public Task PopulateRelatedItemsAsync(int limit, int offset = 0)
        {
            throw new NotImplementedException();
        }
    }
}
