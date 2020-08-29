using StrixMusic.CoreInterfaces;
using StrixMusic.CoreInterfaces.Enums;
using StrixMusic.CoreInterfaces.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StrixMusic.Core.Mock.Implementations
{
    public class MockAlbum : IAlbum
    {
        public IArtist Artist => MockArtist;

        public MockArtist MockArtist { get; set; }

        public IReadOnlyList<ITrack> Tracks => throw new NotImplementedException();

        public int TotalTracksCount => throw new NotImplementedException();

        public ICore SourceCore => throw new NotImplementedException();

        public string Id => throw new NotImplementedException();

        public Uri Url => throw new NotImplementedException();

        public string Name => throw new NotImplementedException();

        public IReadOnlyList<IImage> Images => throw new NotImplementedException();

        public string Description => throw new NotImplementedException();

        public PlaybackState PlaybackState => throw new NotImplementedException();

        public TimeSpan Duration => throw new NotImplementedException();

        public IReadOnlyList<IPlayableCollectionGroup> RelatedItems => throw new NotImplementedException();

        public int TotalRelatedItemsCount => throw new NotImplementedException();

        public event EventHandler<CollectionChangedEventArgs<ITrack>> TracksChanged;
        public event EventHandler<PlaybackState> PlaybackStateChanged;
        public event EventHandler<string> NameChanged;
        public event EventHandler<string> DescriptionChanged;
        public event EventHandler<Uri> UrlChanged;
        public event EventHandler<CollectionChangedEventArgs<IImage>> ImagesChanged;
        public event EventHandler<CollectionChangedEventArgs<IPlayableCollectionGroup>> RelatedItemsChanged;

        public Task PlayAsync()
        {
            throw new NotImplementedException();
        }

        public Task PauseAsync()
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
