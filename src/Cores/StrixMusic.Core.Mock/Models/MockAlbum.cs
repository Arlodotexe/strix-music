using StrixMusic.CoreInterfaces;
using StrixMusic.CoreInterfaces.Enums;
using StrixMusic.CoreInterfaces.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StrixMusic.Core.Mock.Models
{
    public class MockAlbum : IAlbum
    {
        #region Fields

        private IArtist _artist;
        private List<ITrack> _tracks = new List<ITrack>();
        private int _totalTracksCount;
        private ICore _sourceCore;
        private string _id;
        private Uri _url;
        private string _name;
        private List<IImage> _images = new List<IImage>();
        private string _description;
        private List<IPlayableCollectionGroup> _relatedItems = new List<IPlayableCollectionGroup>();
        private int _totalRelatedItemsCount;
        private int _duration;
        private PlaybackState _playbackState;

        #endregion

        public IArtist Artist => _artist;

        public IReadOnlyList<ITrack> Tracks => _tracks;

        public int TotalTracksCount => _totalTracksCount;

        public ICore SourceCore => _sourceCore;

        public string Id => _id;

        public Uri Url => _url;

        public string Name => _name;

        public IReadOnlyList<IImage> Images => _images;

        public string Description => _description;

        public PlaybackState PlaybackState => throw new NotImplementedException();

        public TimeSpan Duration => throw new NotImplementedException();

        public IReadOnlyList<IPlayableCollectionGroup> RelatedItems => throw new NotImplementedException();

        public int TotalRelatedItemsCount => _totalRelatedItemsCount;

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
