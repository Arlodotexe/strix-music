using OwlCore.Events;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.MediaPlayback;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StrixMusic.Sdk.Tests.Mock.Core
{
    public abstract class MockCorePlayableCollectionGroupBase : ICorePlayableCollectionGroup
    {
        private int totalTrackCount;

        public MockCorePlayableCollectionGroupBase(ICore sourceCore, string id, string name)
        {
            SourceCore = sourceCore;
            Id = id;
            Name = name;
        }

        public int TotalPlaylistItemsCount { get; set; }

        public bool IsPlayPlaylistCollectionAsyncAvailable { get; set; }

        public bool IsPausePlaylistCollectionAsyncAvailable { get; set; }

        public int TotalTrackCount
        {
            get => totalTrackCount;
            set
            {
                totalTrackCount = value;
                TracksCountChanged?.Invoke(this, value);
            }
        }

        public bool IsPlayTrackCollectionAsyncAvailable { get; set; }

        public bool IsPauseTrackCollectionAsyncAvailable { get; set; }

        public int TotalAlbumItemsCount { get; set; }

        public bool IsPlayAlbumCollectionAsyncAvailable { get; set; }

        public bool IsPauseAlbumCollectionAsyncAvailable { get; set; }

        public int TotalArtistItemsCount { get; set; }

        public bool IsPlayArtistCollectionAsyncAvailable { get; set; }

        public bool IsPauseArtistCollectionAsyncAvailable { get; set; }

        public int TotalChildrenCount { get; set; }

        public DateTime? AddedAt { get; set; }

        public string Id { get; set; }

        public string Name { get; set; }

        public string? Description { get; set; }

        public DateTime? LastPlayed { get; set; }

        public PlaybackState PlaybackState { get; set; }

        public TimeSpan Duration { get; set; }

        public bool IsChangeNameAsyncAvailable { get; set; }

        public bool IsChangeDescriptionAsyncAvailable { get; set; }

        public bool IsChangeDurationAsyncAvailable { get; set; }

        public int TotalImageCount { get; set; }

        public int TotalUrlCount { get; set; }

        public ICore SourceCore { get; set; }

        public event CollectionChangedEventHandler<ICorePlaylistCollectionItem>? PlaylistItemsChanged;
        public event EventHandler<bool>? IsPlayPlaylistCollectionAsyncAvailableChanged;
        public event EventHandler<bool>? IsPausePlaylistCollectionAsyncAvailableChanged;
        public event EventHandler<int>? PlaylistItemsCountChanged;
        public event CollectionChangedEventHandler<ICoreTrack>? TracksChanged;
        public event EventHandler<bool>? IsPlayTrackCollectionAsyncAvailableChanged;
        public event EventHandler<bool>? IsPauseTrackCollectionAsyncAvailableChanged;
        public event EventHandler<int>? TracksCountChanged;
        public event CollectionChangedEventHandler<ICoreAlbumCollectionItem>? AlbumItemsChanged;
        public event EventHandler<bool>? IsPlayAlbumCollectionAsyncAvailableChanged;
        public event EventHandler<bool>? IsPauseAlbumCollectionAsyncAvailableChanged;
        public event EventHandler<int>? AlbumItemsCountChanged;
        public event CollectionChangedEventHandler<ICoreArtistCollectionItem>? ArtistItemsChanged;
        public event EventHandler<bool>? IsPlayArtistCollectionAsyncAvailableChanged;
        public event EventHandler<bool>? IsPauseArtistCollectionAsyncAvailableChanged;
        public event EventHandler<int>? ArtistItemsCountChanged;
        public event CollectionChangedEventHandler<ICoreImage>? ImagesChanged;
        public event CollectionChangedEventHandler<ICoreUrl>? UrlsChanged;
        public event CollectionChangedEventHandler<ICorePlayableCollectionGroup>? ChildItemsChanged;
        public event EventHandler<int>? ChildrenCountChanged;
        public event EventHandler<PlaybackState>? PlaybackStateChanged;
        public event EventHandler<string>? NameChanged;
        public event EventHandler<string?>? DescriptionChanged;
        public event EventHandler<TimeSpan>? DurationChanged;
        public event EventHandler<DateTime?>? LastPlayedChanged;
        public event EventHandler<bool>? IsChangeNameAsyncAvailableChanged;
        public event EventHandler<bool>? IsChangeDescriptionAsyncAvailableChanged;
        public event EventHandler<bool>? IsChangeDurationAsyncAvailableChanged;
        public event EventHandler<int>? ImagesCountChanged;
        public event EventHandler<int>? UrlsCountChanged;

        public Task AddAlbumItemAsync(ICoreAlbumCollectionItem album, int index)
        {
            throw new NotImplementedException();
        }

        public Task AddArtistItemAsync(ICoreArtistCollectionItem artist, int index)
        {
            throw new NotImplementedException();
        }

        public Task AddChildAsync(ICorePlayableCollectionGroup child, int index)
        {
            throw new NotImplementedException();
        }

        public Task AddImageAsync(ICoreImage image, int index)
        {
            throw new NotImplementedException();
        }

        public Task AddPlaylistItemAsync(ICorePlaylistCollectionItem playlist, int index)
        {
            throw new NotImplementedException();
        }

        public Task AddTrackAsync(ICoreTrack track, int index)
        {
            throw new NotImplementedException();
        }

        public Task AddUrlAsync(ICoreUrl url, int index)
        {
            throw new NotImplementedException();
        }

        public Task ChangeDescriptionAsync(string? description)
        {
            throw new NotImplementedException();
        }

        public Task ChangeDurationAsync(TimeSpan duration)
        {
            throw new NotImplementedException();
        }

        public Task ChangeNameAsync(string name)
        {
            throw new NotImplementedException();
        }

        public virtual IAsyncEnumerable<ICoreAlbumCollectionItem> GetAlbumItemsAsync(int limit, int offset)
        {
            throw new NotImplementedException();
        }

        public virtual IAsyncEnumerable<ICoreArtistCollectionItem> GetArtistItemsAsync(int limit, int offset)
        {
            throw new NotImplementedException();
        }

        public virtual IAsyncEnumerable<ICorePlayableCollectionGroup> GetChildrenAsync(int limit, int offset)
        {
            throw new NotImplementedException();
        }

        public virtual IAsyncEnumerable<ICoreImage> GetImagesAsync(int limit, int offset)
        {
            throw new NotImplementedException();
        }

        public virtual IAsyncEnumerable<ICorePlaylistCollectionItem> GetPlaylistItemsAsync(int limit, int offset)
        {
            throw new NotImplementedException();
        }

        public virtual IAsyncEnumerable<ICoreTrack> GetTracksAsync(int limit, int offset)
        {
            throw new NotImplementedException();
        }

        public virtual IAsyncEnumerable<ICoreUrl> GetUrlsAsync(int limit, int offset)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsAddAlbumItemAvailableAsync(int index)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsAddArtistItemAvailableAsync(int index)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsAddChildAvailableAsync(int index)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsAddImageAvailableAsync(int index)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsAddPlaylistItemAvailableAsync(int index)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsAddTrackAvailableAsync(int index)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsAddUrlAvailableAsync(int index)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsRemoveAlbumItemAvailableAsync(int index)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsRemoveArtistItemAvailableAsync(int index)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsRemoveChildAvailableAsync(int index)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsRemoveImageAvailableAsync(int index)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsRemovePlaylistItemAvailableAsync(int index)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsRemoveTrackAvailableAsync(int index)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsRemoveUrlAvailableAsync(int index)
        {
            throw new NotImplementedException();
        }

        public Task PauseAlbumCollectionAsync()
        {
            throw new NotImplementedException();
        }

        public Task PauseArtistCollectionAsync()
        {
            throw new NotImplementedException();
        }

        public Task PausePlayableCollectionGroupAsync()
        {
            throw new NotImplementedException();
        }

        public Task PausePlaylistCollectionAsync()
        {
            throw new NotImplementedException();
        }

        public Task PauseTrackCollectionAsync()
        {
            throw new NotImplementedException();
        }

        public Task PlayAlbumCollectionAsync(ICoreAlbumCollectionItem albumItem)
        {
            throw new NotImplementedException();
        }

        public Task PlayAlbumCollectionAsync()
        {
            throw new NotImplementedException();
        }

        public Task PlayArtistCollectionAsync(ICoreArtistCollectionItem artistItem)
        {
            throw new NotImplementedException();
        }

        public Task PlayArtistCollectionAsync()
        {
            throw new NotImplementedException();
        }

        public Task PlayPlayableCollectionGroupAsync(ICorePlayableCollectionGroup collectionGroup)
        {
            throw new NotImplementedException();
        }

        public Task PlayPlayableCollectionGroupAsync()
        {
            throw new NotImplementedException();
        }

        public Task PlayPlaylistCollectionAsync(ICorePlaylistCollectionItem playlistItem)
        {
            throw new NotImplementedException();
        }

        public Task PlayPlaylistCollectionAsync()
        {
            throw new NotImplementedException();
        }

        public Task PlayTrackCollectionAsync(ICoreTrack track)
        {
            throw new NotImplementedException();
        }

        public Task PlayTrackCollectionAsync()
        {
            throw new NotImplementedException();
        }

        public Task RemoveAlbumItemAsync(int index)
        {
            throw new NotImplementedException();
        }

        public Task RemoveArtistItemAsync(int index)
        {
            throw new NotImplementedException();
        }

        public Task RemoveChildAsync(int index)
        {
            throw new NotImplementedException();
        }

        public Task RemoveImageAsync(int index)
        {
            throw new NotImplementedException();
        }

        public Task RemovePlaylistItemAsync(int index)
        {
            throw new NotImplementedException();
        }

        public Task RemoveTrackAsync(int index)
        {
            throw new NotImplementedException();
        }

        public Task RemoveUrlAsync(int index)
        {
            throw new NotImplementedException();
        }

        public ValueTask DisposeAsync()
        {
            throw new NotImplementedException();
        }
    }
}
