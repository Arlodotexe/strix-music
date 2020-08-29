using StrixMusic.CoreInterfaces;
using StrixMusic.CoreInterfaces.Enums;
using StrixMusic.CoreInterfaces.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StrixMusic.Core.Mock.Models
{
    public class MockArtist : IArtist
    {
        #region Fields

        private List<IAlbum> _albums = new List<IAlbum>();
        private List<ITrack> _itracks = new List<ITrack>();
        private List<IImage> _images = new List<IImage>();
        private List<IPlayableCollectionGroup> _relatedItems = new List<IPlayableCollectionGroup>();
        private string _id;
        private Uri _url;
        private string _name;
        private string _description;
        private TimeSpan _duration;
        private PlaybackState _playbackState;
        private TimeSpan _timeSpan;
        private int _totalTracksCount;
        private int _totalAlbumCount;

        #endregion

        public IReadOnlyList<IAlbum> Albums => _albums;

        public int TotalAlbumsCount => _totalTracksCount;

        public IReadOnlyList<ITrack> Tracks => _itracks;

        public int TotalTracksCount => _totalTracksCount;

        public ICore SourceCore => throw new NotImplementedException();

        public string Id => _id;

        public Uri Url => _url;

        public string Name => _name;

        public IReadOnlyList<IImage> Images => _images;

        public string Description => _description;

        public PlaybackState PlaybackState => _playbackState;

        public TimeSpan Duration => _duration;

        public IReadOnlyList<IPlayableCollectionGroup> RelatedItems => _relatedItems;

        public int TotalRelatedItemsCount => _totalTracksCount;

        public event EventHandler<CollectionChangedEventArgs<IAlbum>> AlbumsChanged;
        public event EventHandler<CollectionChangedEventArgs<ITrack>> TracksChanged;
        public event EventHandler<PlaybackState> PlaybackStateChanged;
        public event EventHandler<string> NameChanged;
        public event EventHandler<string> DescriptionChanged;
        public event EventHandler<Uri> UrlChanged;
        public event EventHandler<CollectionChangedEventArgs<IImage>> ImagesChanged;
        public event EventHandler<CollectionChangedEventArgs<IPlayableCollectionGroup>> RelatedItemsChanged;

        public Task PauseAsync()
        {
            throw new NotImplementedException();
        }

        public Task PlayAsync()
        {
            throw new NotImplementedException();
        }

        public Task PopulateAlbumsAsync(int limit, int offset = 0)
        {
            throw new NotImplementedException();
        }

        public Task PopulateRelatedItemsAsync(int limit, int offset = 0)
        {
            throw new NotImplementedException();
        }

        public Task PopulateTracksAsync(int limit, int offset = 0)
        {
            throw new NotImplementedException();
        }
    }
}
