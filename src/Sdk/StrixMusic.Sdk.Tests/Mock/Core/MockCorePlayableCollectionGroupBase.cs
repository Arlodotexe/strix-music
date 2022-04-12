using OwlCore.Events;
using OwlCore.Extensions;
using StrixMusic.Sdk.Models.Core;
using StrixMusic.Sdk.MediaPlayback;
using StrixMusic.Sdk.Tests.Mock.Core.Items;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace StrixMusic.Sdk.Tests.Mock.Core
{
    public abstract class MockCorePlayableCollectionGroupBase : ICorePlayableCollectionGroup
    {
        private List<ICoreAlbumCollectionItem> _albums = new List<ICoreAlbumCollectionItem>();
        private List<ICoreArtistCollectionItem> _artists = new List<ICoreArtistCollectionItem>();
        private List<ICorePlaylistCollectionItem> _playlists = new List<ICorePlaylistCollectionItem>();
        private List<ICoreTrack> _tracks = new List<ICoreTrack>();
        private List<ICorePlayableCollectionGroup> _children = new List<ICorePlayableCollectionGroup>();
        private List<ICoreImage> _images = new List<ICoreImage>();
        private List<ICoreUrl> _urls = new List<ICoreUrl>();

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
            PlaybackState = PlaybackState.Loaded;
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

        // Must be done outside the ctor to avoid Stackoverflow exceptions.
        public void PopulateMockItems()
        {
            for (int i = _tracks.Count; i < TotalTrackCount; i++)
                _tracks.Add(new MockCoreTrack(SourceCore, $"{i}", $"Track {i}"));

            for (int i = _albums.Count; i < TotalAlbumItemsCount; i++)
                _albums.Add(new MockCoreAlbum(SourceCore, $"{i}", $"Album {i}"));

            for (int i = _artists.Count; i < TotalArtistItemsCount; i++)
                _artists.Add(new MockCoreArtist(SourceCore, $"{i}", $"Artist {i}"));

            for (int i = _playlists.Count; i < TotalPlaylistItemsCount; i++)
                _playlists.Add(new MockCorePlaylist(SourceCore, $"{i}", $"Playlist {i}"));

            for (int i = _images.Count; i < TotalImageCount; i++)
                _images.Add(new MockCoreImage(SourceCore, new Uri($"https://picsum.photos/seed/picsum{i}/200/300")));

            for (int i = _urls.Count; i < TotalUrlCount; i++)
                _urls.Add(new MockCoreUrl(SourceCore, new Uri($"https://picsum.photos/seed/picsum{i}/200/300"), $"Url {i}"));

            for (int i = _children.Count; i < TotalChildrenCount; i++)
                _children.Add(new MockCorePlayableCollectionGroup(SourceCore, $"{i}", $"Child {i}"));
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

        public Task ChangeDescriptionAsync(string? description, CancellationToken cancellationToken = default)
        {
            Description = description;
            return Task.CompletedTask;
        }

        public Task ChangeDurationAsync(TimeSpan duration, CancellationToken cancellationToken = default)
        {
            Duration = duration;
            return Task.CompletedTask;
        }

        public Task ChangeNameAsync(string name, CancellationToken cancellationToken = default)
        {
            Name = name;
            return Task.CompletedTask;
        }

        public virtual async IAsyncEnumerable<ICoreAlbumCollectionItem> GetAlbumItemsAsync(int limit, int offset, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            await Task.Yield();

            for (int i = 0; i < limit; i++)
                yield return MockCoreItemFactory.CreateAlbum(SourceCore);
        }

        public virtual async IAsyncEnumerable<ICoreArtistCollectionItem> GetArtistItemsAsync(int limit, int offset, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            await Task.Yield();

            for (int i = 0; i < limit; i++)
                yield return MockCoreItemFactory.CreateArtist(SourceCore);
        }

        public virtual async IAsyncEnumerable<ICorePlayableCollectionGroup> GetChildrenAsync(int limit, int offset, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            await Task.Yield();

            for (int i = 0; i < limit; i++)
                yield return MockCoreItemFactory.CreatePlayableCollectionGroup(SourceCore);
        }

        public virtual async IAsyncEnumerable<ICoreImage> GetImagesAsync(int limit, int offset, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            await Task.Yield();

            for (int i = 0; i < limit; i++)
                yield return MockCoreItemFactory.CreateImage(SourceCore);
        }

        public virtual async IAsyncEnumerable<ICorePlaylistCollectionItem> GetPlaylistItemsAsync(int limit, int offset, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            await Task.Yield();

            for (int i = 0; i < limit; i++)
                yield return MockCoreItemFactory.CreatePlaylist(SourceCore);
        }

        public virtual async IAsyncEnumerable<ICoreTrack> GetTracksAsync(int limit, int offset, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            await Task.Yield();

            for (int i = 0; i < limit; i++)
                yield return MockCoreItemFactory.CreateTrack(SourceCore);
        }

        public virtual async IAsyncEnumerable<ICoreUrl> GetUrlsAsync(int limit, int offset, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            await Task.Yield();

            for (int i = 0; i < limit; i++)
                yield return MockCoreItemFactory.CreateUrl(SourceCore);
        }

        public Task<bool> IsAddAlbumItemAvailableAsync(int index, CancellationToken cancellationToken = default) => Task.FromResult(index % 2 == 0);

        public Task<bool> IsAddArtistItemAvailableAsync(int index, CancellationToken cancellationToken = default) => Task.FromResult(index % 2 == 0);

        public Task<bool> IsAddChildAvailableAsync(int index, CancellationToken cancellationToken = default) => Task.FromResult(index % 2 == 0);

        public Task<bool> IsAddImageAvailableAsync(int index, CancellationToken cancellationToken = default) => Task.FromResult(index % 2 == 0);

        public Task<bool> IsAddPlaylistItemAvailableAsync(int index, CancellationToken cancellationToken = default) => Task.FromResult(index % 2 == 0);

        public Task<bool> IsAddTrackAvailableAsync(int index, CancellationToken cancellationToken = default) => Task.FromResult(index % 2 == 0);

        public Task<bool> IsAddUrlAvailableAsync(int index, CancellationToken cancellationToken = default) => Task.FromResult(index % 2 == 0);

        public Task<bool> IsRemoveAlbumItemAvailableAsync(int index, CancellationToken cancellationToken = default) => Task.FromResult(index % 2 == 0);

        public Task<bool> IsRemoveArtistItemAvailableAsync(int index, CancellationToken cancellationToken = default) => Task.FromResult(index % 2 == 0);

        public Task<bool> IsRemoveChildAvailableAsync(int index, CancellationToken cancellationToken = default) => Task.FromResult(index % 2 == 0);

        public Task<bool> IsRemoveImageAvailableAsync(int index, CancellationToken cancellationToken = default) => Task.FromResult(index % 2 == 0);

        public Task<bool> IsRemovePlaylistItemAvailableAsync(int index, CancellationToken cancellationToken = default) => Task.FromResult(index % 2 == 0);

        public Task<bool> IsRemoveTrackAvailableAsync(int index, CancellationToken cancellationToken = default) => Task.FromResult(index % 2 == 0);

        public Task<bool> IsRemoveUrlAvailableAsync(int index, CancellationToken cancellationToken = default) => Task.FromResult(index % 2 == 0);

        public Task PauseAlbumCollectionAsync(CancellationToken cancellationToken = default)
        {
            PlaybackState = PlaybackState.Paused;
            return Task.CompletedTask;
        }

        public Task PauseArtistCollectionAsync(CancellationToken cancellationToken = default)
        {
            PlaybackState = PlaybackState.Paused;
            return Task.CompletedTask;
        }

        public Task PausePlayableCollectionGroupAsync(CancellationToken cancellationToken = default)
        {
            PlaybackState = PlaybackState.Paused;
            return Task.CompletedTask;
        }

        public Task PausePlaylistCollectionAsync(CancellationToken cancellationToken = default)
        {
            PlaybackState = PlaybackState.Paused;
            return Task.CompletedTask;
        }

        public Task PauseTrackCollectionAsync(CancellationToken cancellationToken = default)
        {
            PlaybackState = PlaybackState.Paused;
            return Task.CompletedTask;
        }

        public Task PlayAlbumCollectionAsync(ICoreAlbumCollectionItem albumItem, CancellationToken cancellationToken = default)
        {
            PlaybackState = PlaybackState.Playing;
            Name = albumItem.Name;
            return Task.CompletedTask;
        }

        public Task PlayAlbumCollectionAsync(CancellationToken cancellationToken = default)
        {
            PlaybackState = PlaybackState.Playing;
            return Task.CompletedTask;
        }

        public Task PlayArtistCollectionAsync(ICoreArtistCollectionItem artistItem, CancellationToken cancellationToken = default)
        {
            PlaybackState = PlaybackState.Playing;
            Name = artistItem.Name;
            return Task.CompletedTask;
        }

        public Task PlayArtistCollectionAsync(CancellationToken cancellationToken = default)
        {
            PlaybackState = PlaybackState.Playing;
            return Task.CompletedTask;
        }

        public Task PlayPlayableCollectionGroupAsync(ICorePlayableCollectionGroup collectionGroup, CancellationToken cancellationToken = default)
        {
            PlaybackState = PlaybackState.Playing;
            Name = collectionGroup.Name;
            return Task.CompletedTask;
        }

        public Task PlayPlayableCollectionGroupAsync(CancellationToken cancellationToken = default)
        {
            PlaybackState = PlaybackState.Playing;
            return Task.CompletedTask;
        }

        public Task PlayPlaylistCollectionAsync(ICorePlaylistCollectionItem playlistItem, CancellationToken cancellationToken = default)
        {
            PlaybackState = PlaybackState.Playing;
            Name = playlistItem.Name;
            return Task.CompletedTask;
        }

        public Task PlayPlaylistCollectionAsync(CancellationToken cancellationToken = default)
        {
            PlaybackState = PlaybackState.Playing;
            return Task.CompletedTask;
        }

        public Task PlayTrackCollectionAsync(ICoreTrack track, CancellationToken cancellationToken = default)
        {
            PlaybackState = PlaybackState.Playing;
            Name = track.Name;
            return Task.CompletedTask;
        }

        public Task PlayTrackCollectionAsync(CancellationToken cancellationToken = default)
        {
            PlaybackState = PlaybackState.Playing;
            return Task.CompletedTask;
        }

        public Task AddAlbumItemAsync(ICoreAlbumCollectionItem album, int index, CancellationToken cancellationToken = default)
        {
            _albums.InsertOrAdd(index, album);
            TotalAlbumItemsCount++;
            AlbumItemsChanged?.Invoke(this, new CollectionChangedItem<ICoreAlbumCollectionItem>(album, index).IntoList(), new List<CollectionChangedItem<ICoreAlbumCollectionItem>>());

            return Task.CompletedTask;
        }

        public Task AddArtistItemAsync(ICoreArtistCollectionItem artist, int index, CancellationToken cancellationToken = default)
        {
            _artists.InsertOrAdd(index, artist);
            TotalArtistItemsCount++;
            ArtistItemsChanged?.Invoke(this, new CollectionChangedItem<ICoreArtistCollectionItem>(artist, index).IntoList(), new List<CollectionChangedItem<ICoreArtistCollectionItem>>());

            return Task.CompletedTask;
        }

        public Task AddChildAsync(ICorePlayableCollectionGroup child, int index, CancellationToken cancellationToken = default)
        {
            _children.InsertOrAdd(index, child);
            TotalChildrenCount++;
            ChildItemsChanged?.Invoke(this, new CollectionChangedItem<ICorePlayableCollectionGroup>(child, index).IntoList(), new List<CollectionChangedItem<ICorePlayableCollectionGroup>>());

            return Task.CompletedTask;
        }

        public Task AddImageAsync(ICoreImage image, int index, CancellationToken cancellationToken = default)
        {
            _images.InsertOrAdd(index, image);
            TotalImageCount++;
            ImagesChanged?.Invoke(this, new CollectionChangedItem<ICoreImage>(image, index).IntoList(), new List<CollectionChangedItem<ICoreImage>>());

            return Task.CompletedTask;
        }

        public Task AddPlaylistItemAsync(ICorePlaylistCollectionItem playlist, int index, CancellationToken cancellationToken = default)
        {
            _playlists.InsertOrAdd(index, playlist);
            TotalPlaylistItemsCount++;
            PlaylistItemsChanged?.Invoke(this, new CollectionChangedItem<ICorePlaylistCollectionItem>(playlist, index).IntoList(), new List<CollectionChangedItem<ICorePlaylistCollectionItem>>());

            return Task.CompletedTask;
        }

        public Task AddTrackAsync(ICoreTrack track, int index, CancellationToken cancellationToken = default)
        {
            _tracks.InsertOrAdd(index, track);
            TotalTrackCount++;
            TracksChanged?.Invoke(this, new CollectionChangedItem<ICoreTrack>(track, index).IntoList(), new List<CollectionChangedItem<ICoreTrack>>());

            return Task.CompletedTask;
        }

        public Task AddUrlAsync(ICoreUrl url, int index, CancellationToken cancellationToken = default)
        {
            _urls.InsertOrAdd(index, url);
            TotalUrlCount++;
            UrlsChanged?.Invoke(this, new CollectionChangedItem<ICoreUrl>(url, index).IntoList(), new List<CollectionChangedItem<ICoreUrl>>());

            return Task.CompletedTask;
        }

        public Task RemoveAlbumItemAsync(int index, CancellationToken cancellationToken = default)
        {
            _albums.RemoveAt(0);
            TotalAlbumItemsCount--;

            return Task.CompletedTask;
        }

        public Task RemoveArtistItemAsync(int index, CancellationToken cancellationToken = default)
        {
            _artists.RemoveAt(index);
            TotalArtistItemsCount--;

            return Task.CompletedTask;
        }

        public Task RemoveChildAsync(int index, CancellationToken cancellationToken = default)
        {
            _children.RemoveAt(index);
            TotalChildrenCount--;

            return Task.CompletedTask;
        }

        public Task RemoveImageAsync(int index, CancellationToken cancellationToken = default)
        {
            _images.RemoveAt(index);
            TotalImageCount--;

            return Task.CompletedTask;
        }

        public Task RemovePlaylistItemAsync(int index, CancellationToken cancellationToken = default)
        {
            _playlists.RemoveAt(index);
            TotalPlaylistItemsCount--;

            return Task.CompletedTask;
        }

        public Task RemoveTrackAsync(int index, CancellationToken cancellationToken = default)
        {
            _tracks.RemoveAt(index);
            TotalTrackCount--;

            return Task.CompletedTask;
        }

        public Task RemoveUrlAsync(int index, CancellationToken cancellationToken = default)
        {
            _urls.RemoveAt(index);
            TotalUrlCount--;

            return Task.CompletedTask;
        }

        public ValueTask DisposeAsync()
        {
            throw new NotImplementedException();
        }
    }
}
