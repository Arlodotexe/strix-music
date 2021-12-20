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
        private int totalChildrenCount;
        private int totalImageCount;
        private int totalUrlCount;
        private int totalAlbumItemsCount;
        private int totalArtistItemsCount;
        private int totalPlaylistItemsCount;
        private string _name = string.Empty;
        private string? description;
        private DateTime? lastPlayed;
        private PlaybackState playbackState;
        private TimeSpan duration;
        private bool isChangeDurationAsyncAvailable;
        private bool isChangeDescriptionAsyncAvailable;
        private bool isChangeNameAsyncAvailable;
        private bool isPlayAlbumCollectionAsyncAvailable;
        private bool isPauseAlbumCollectionAsyncAvailable;
        private bool isPlayPlaylistCollectionAsyncAvailable;
        private bool isPausePlaylistCollectionAsyncAvailable;
        private bool isPlayTrackCollectionAsyncAvailable;
        private bool isPauseTrackCollectionAsyncAvailable;
        private bool isPlayArtistCollectionAsyncAvailable;
        private bool isPauseArtistCollectionAsyncAvailable;

        public MockCorePlayableCollectionGroupBase(ICore sourceCore, string id, string name)
        {
            SourceCore = sourceCore;
            Id = id;
            Name = name;
            Description = $"Incredible description of {name}";
            PlaybackState = PlaybackState.Paused;
            LastPlayed = DateTime.Today;
            Duration = TimeSpan.FromMinutes(5);

            IsChangeNameAsyncAvailable = true;
            IsChangeDescriptionAsyncAvailable = true;
            IsChangeDurationAsyncAvailable = true;

            IsPlayAlbumCollectionAsyncAvailable = true;
            IsPauseAlbumCollectionAsyncAvailable = true;
            IsPlayArtistCollectionAsyncAvailable = true;
            IsPauseArtistCollectionAsyncAvailable = true;
            IsPlayPlaylistCollectionAsyncAvailable = true;
            IsPausePlaylistCollectionAsyncAvailable = true;
            IsPlayTrackCollectionAsyncAvailable = true;
            IsPauseTrackCollectionAsyncAvailable = true;

            TotalTrackCount = 5;
            TotalAlbumItemsCount = 5;
            TotalArtistItemsCount = 5;
            TotalPlaylistItemsCount = 5;
            TotalChildrenCount = 5;
            TotalImageCount = 5;
            TotalUrlCount = 5;
        }

        public int TotalPlaylistItemsCount
        {
            get => totalPlaylistItemsCount;
            set
            {
                totalPlaylistItemsCount = value;
                PlaylistItemsCountChanged?.Invoke(this, value);
            }
        }

        public bool IsPlayPlaylistCollectionAsyncAvailable
        {
            get => isPlayPlaylistCollectionAsyncAvailable;
            set
            {
                isPlayPlaylistCollectionAsyncAvailable = value;
                IsPlayPlaylistCollectionAsyncAvailableChanged?.Invoke(this, value);
            }
        }

        public bool IsPausePlaylistCollectionAsyncAvailable
        {
            get => isPausePlaylistCollectionAsyncAvailable;
            set
            {
                isPausePlaylistCollectionAsyncAvailable = value;
                IsPausePlaylistCollectionAsyncAvailableChanged?.Invoke(this, value);
            }
        }

        public int TotalTrackCount
        {
            get => totalTrackCount;
            set
            {
                totalTrackCount = value;
                TracksCountChanged?.Invoke(this, value);
            }
        }

        public bool IsPlayTrackCollectionAsyncAvailable
        {
            get => isPlayTrackCollectionAsyncAvailable;
            set
            {
                isPlayTrackCollectionAsyncAvailable = value;
                IsPlayTrackCollectionAsyncAvailableChanged?.Invoke(this, value);
            }
        }

        public bool IsPauseTrackCollectionAsyncAvailable
        {
            get => isPauseTrackCollectionAsyncAvailable;
            set
            {
                isPauseTrackCollectionAsyncAvailable = value;
                IsPauseTrackCollectionAsyncAvailableChanged?.Invoke(this, value);
            }
        }

        public int TotalAlbumItemsCount
        {
            get => totalAlbumItemsCount;
            set
            {
                totalAlbumItemsCount = value;
                AlbumItemsCountChanged?.Invoke(this, value);
            }
        }

        public bool IsPlayAlbumCollectionAsyncAvailable
        {
            get => isPlayAlbumCollectionAsyncAvailable;
            set
            {
                isPlayAlbumCollectionAsyncAvailable = value;
                IsPlayAlbumCollectionAsyncAvailableChanged?.Invoke(this, value);
            }
        }

        public bool IsPauseAlbumCollectionAsyncAvailable
        {
            get => isPauseAlbumCollectionAsyncAvailable;
            set
            {
                isPauseAlbumCollectionAsyncAvailable = value;
                IsPauseAlbumCollectionAsyncAvailableChanged?.Invoke(this, value);
            }
        }

        public int TotalArtistItemsCount
        {
            get => totalArtistItemsCount;
            set
            {
                totalArtistItemsCount = value;
                ArtistItemsCountChanged?.Invoke(this, value);
            }
        }

        public bool IsPlayArtistCollectionAsyncAvailable
        {
            get => isPlayArtistCollectionAsyncAvailable;
            set
            {
                isPlayArtistCollectionAsyncAvailable = value;
                IsPlayArtistCollectionAsyncAvailableChanged?.Invoke(this, value);
            }
        }

        public bool IsPauseArtistCollectionAsyncAvailable
        {
            get => isPauseArtistCollectionAsyncAvailable;
            set
            {
                isPauseArtistCollectionAsyncAvailable = value;
                IsPauseArtistCollectionAsyncAvailableChanged?.Invoke(this, value);
            }
        }

        public int TotalChildrenCount
        {
            get => totalChildrenCount;
            set
            {
                totalChildrenCount = value;
                ChildrenCountChanged?.Invoke(this, value);
            }
        }

        public DateTime? AddedAt { get; set; }

        public string Id { get; set; }

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                NameChanged?.Invoke(this, value);
            }
        }

        public string? Description
        {
            get => description;
            set
            {
                description = value;
                DescriptionChanged?.Invoke(this, value);
            }
        }

        public DateTime? LastPlayed
        {
            get => lastPlayed;
            set
            {
                lastPlayed = value;
                LastPlayedChanged?.Invoke(this, value);
            }
        }

        public PlaybackState PlaybackState
        {
            get => playbackState;
            set
            {
                playbackState = value;
                PlaybackStateChanged?.Invoke(this, value);
            }
        }

        public TimeSpan Duration
        {
            get => duration;
            set
            {
                duration = value;
                DurationChanged?.Invoke(this, value);
            }
        }

        public bool IsChangeNameAsyncAvailable
        {
            get => isChangeNameAsyncAvailable;
            set
            {
                isChangeNameAsyncAvailable = value;
                IsChangeNameAsyncAvailableChanged?.Invoke(this, value);
            }
        }

        public bool IsChangeDescriptionAsyncAvailable
        {
            get => isChangeDescriptionAsyncAvailable;
            set
            {
                isChangeDescriptionAsyncAvailable = value;
                IsChangeDescriptionAsyncAvailableChanged?.Invoke(this, value);
            }
        }

        public bool IsChangeDurationAsyncAvailable
        {
            get => isChangeDurationAsyncAvailable;
            set
            {
                isChangeDurationAsyncAvailable = value;
                IsChangeDurationAsyncAvailableChanged?.Invoke(this, value);
            }
        }

        public int TotalImageCount
        {
            get => totalImageCount;
            set
            {
                totalImageCount = value;
                ImagesCountChanged?.Invoke(this, value);
            }
        }

        public int TotalUrlCount
        {
            get => totalUrlCount;
            set
            {
                totalUrlCount = value;
                UrlsCountChanged?.Invoke(this, value);
            }
        }

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
            Description = description;
            return Task.CompletedTask;
        }

        public Task ChangeDurationAsync(TimeSpan duration)
        {
            Duration = duration;
            return Task.CompletedTask;
        }

        public Task ChangeNameAsync(string name)
        {
            Name = name;
            return Task.CompletedTask;
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
