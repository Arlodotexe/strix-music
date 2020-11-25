using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using OwlCore.Collections;
using OwlCore.Helpers;
using StrixMusic.Sdk.Data;
using StrixMusic.Sdk.Data.Base;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.Extensions;
using StrixMusic.Sdk.MediaPlayback;

namespace StrixMusic.Sdk.ViewModels
{
    /// <summary>
    /// An observable wrapper for a <see cref="IPlayableCollectionGroupBase"/>.
    /// </summary>
    public class PlayableCollectionGroupViewModel : ObservableObject, IPlayableCollectionGroup, IPlayableCollectionGroupChildrenViewModel, IAlbumCollectionViewModel, IArtistCollectionViewModel, ITrackCollectionViewModel, IPlaylistCollectionViewModel
    {
        private readonly IPlayableCollectionGroup _collectionGroup;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayableCollectionGroupViewModel"/> class.
        /// </summary>
        /// <param name="collectionGroup">The base <see cref="IPlayableCollectionGroup"/> containing properties about this class.</param>
        public PlayableCollectionGroupViewModel(IPlayableCollectionGroup collectionGroup)
        {
            _collectionGroup = collectionGroup ?? throw new ArgumentNullException(nameof(collectionGroup));

            SourceCores = _collectionGroup.GetSourceCores<ICorePlayableCollectionGroup>().Select(MainViewModel.GetLoadedCore).ToList();

            PauseAsyncCommand = new AsyncRelayCommand(PauseAsync);
            PlayAsyncCommand = new AsyncRelayCommand(PlayAsync);
            ChangeNameAsyncCommand = new AsyncRelayCommand<string>(ChangeNameAsync);
            ChangeDescriptionAsyncCommand = new AsyncRelayCommand<string?>(ChangeDescriptionAsync);
            ChangeDurationAsyncCommand = new AsyncRelayCommand<TimeSpan>(ChangeDurationAsync);

            PopulateMoreTracksCommand = new AsyncRelayCommand<int>(PopulateMoreTracksAsync);
            PopulateMorePlaylistsCommand = new AsyncRelayCommand<int>(PopulateMorePlaylistsAsync);
            PopulateMoreAlbumsCommand = new AsyncRelayCommand<int>(PopulateMoreAlbumsAsync);
            PopulateMoreArtistsCommand = new AsyncRelayCommand<int>(PopulateMoreArtistsAsync);
            PopulateMoreChildrenCommand = new AsyncRelayCommand<int>(PopulateMoreChildrenAsync);

            Tracks = Threading.InvokeOnUI(() => new SynchronizedObservableCollection<TrackViewModel>());
            Playlists = Threading.InvokeOnUI(() => new SynchronizedObservableCollection<IPlaylistCollectionItem>());
            Albums = Threading.InvokeOnUI(() => new SynchronizedObservableCollection<IAlbumCollectionItem>());
            Artists = Threading.InvokeOnUI(() => new SynchronizedObservableCollection<IArtistCollectionItem>());
            Children = Threading.InvokeOnUI(() => new SynchronizedObservableCollection<PlayableCollectionGroupViewModel>());

            AttachPropertyEvents();
        }

        private void AttachPropertyEvents()
        {
            _collectionGroup.PlaybackStateChanged += CollectionGroupPlaybackStateChanged;
            _collectionGroup.DescriptionChanged += CollectionGroupDescriptionChanged;
            _collectionGroup.NameChanged += CollectionGroupNameChanged;
            _collectionGroup.UrlChanged += CollectionGroupUrlChanged;

            _collectionGroup.AlbumItemsCountChanged += CollectionGroupOnAlbumItemsCountChanged;
            _collectionGroup.TrackItemsCountChanged += CollectionGroupOnTrackItemsCountChanged;
            _collectionGroup.ArtistItemsCountChanged += CollectionGroupOnArtistItemsCountChanged;
            _collectionGroup.PlaylistItemsCountChanged += CollectionGroupOnPlaylistItemsCountChanged;
            _collectionGroup.TotalChildrenCountChanged += CollectionGroupOnTotalChildrenCountChanged;
        }

        private void DetachPropertyEvents()
        {
            _collectionGroup.PlaybackStateChanged -= CollectionGroupPlaybackStateChanged;
            _collectionGroup.DescriptionChanged -= CollectionGroupDescriptionChanged;
            _collectionGroup.NameChanged -= CollectionGroupNameChanged;
            _collectionGroup.UrlChanged -= CollectionGroupUrlChanged;

            _collectionGroup.AlbumItemsCountChanged -= CollectionGroupOnAlbumItemsCountChanged;
            _collectionGroup.TrackItemsCountChanged -= CollectionGroupOnTrackItemsCountChanged;
            _collectionGroup.ArtistItemsCountChanged -= CollectionGroupOnArtistItemsCountChanged;
            _collectionGroup.PlaylistItemsCountChanged -= CollectionGroupOnPlaylistItemsCountChanged;
            _collectionGroup.TotalChildrenCountChanged -= CollectionGroupOnTotalChildrenCountChanged;
        }

        /// <inheritdoc />
        public event EventHandler<string> NameChanged
        {
            add => _collectionGroup.NameChanged += value;
            remove => _collectionGroup.NameChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<string?> DescriptionChanged
        {
            add => _collectionGroup.DescriptionChanged += value;
            remove => _collectionGroup.DescriptionChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<Uri?> UrlChanged
        {
            add => _collectionGroup.UrlChanged += value;
            remove => _collectionGroup.UrlChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<PlaybackState>? PlaybackStateChanged
        {
            add => _collectionGroup.PlaybackStateChanged += value;

            remove => _collectionGroup.PlaybackStateChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<TimeSpan>? DurationChanged
        {
            add => _collectionGroup.DurationChanged += value;

            remove => _collectionGroup.DurationChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<int> TrackItemsCountChanged
        {
            add => _collectionGroup.TrackItemsCountChanged += value;
            remove => _collectionGroup.TrackItemsCountChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<int> ArtistItemsCountChanged
        {
            add => _collectionGroup.ArtistItemsCountChanged += value;
            remove => _collectionGroup.ArtistItemsCountChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<int> AlbumItemsCountChanged
        {
            add => _collectionGroup.AlbumItemsCountChanged += value;
            remove => _collectionGroup.AlbumItemsCountChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<int> PlaylistItemsCountChanged
        {
            add => _collectionGroup.PlaylistItemsCountChanged += value;
            remove => _collectionGroup.PlaylistItemsCountChanged -= value;
        }

        private void CollectionGroupUrlChanged(object sender, Uri? e) => Url = e;

        private void CollectionGroupNameChanged(object sender, string e) => Name = e;

        private void CollectionGroupDescriptionChanged(object sender, string? e) => Description = e;

        private void CollectionGroupPlaybackStateChanged(object sender, PlaybackState e) => PlaybackState = e;

        private void CollectionGroupOnTotalChildrenCountChanged(object sender, int e) => TotalChildrenCount = e;

        private void CollectionGroupOnPlaylistItemsCountChanged(object sender, int e) => TotalPlaylistItemsCount = e;

        private void CollectionGroupOnArtistItemsCountChanged(object sender, int e) => TotalArtistItemsCount = e;

        private void CollectionGroupOnTrackItemsCountChanged(object sender, int e) => TotalTracksCount = e;

        private void CollectionGroupOnAlbumItemsCountChanged(object sender, int e) => TotalAlbumItemsCount = e;

        /// <inheritdoc />
        public string Id => _collectionGroup.Id;

        /// <inheritdoc cref="ISdkMember{T}" />
        public IReadOnlyList<ICore> SourceCores { get; }

        /// <summary>
        /// The merged sources for this item.
        /// </summary>
        public IReadOnlyList<ICorePlayableCollectionGroup> Sources => _collectionGroup.GetSources<ICorePlayableCollectionGroup>();

        /// <inheritdoc />
        IReadOnlyList<ICorePlayableCollectionGroup> ISdkMember<ICorePlayableCollectionGroup>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICorePlayableCollectionGroupChildren> ISdkMember<ICorePlayableCollectionGroupChildren>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreArtistCollection> ISdkMember<ICoreArtistCollection>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreArtistCollectionItem> ISdkMember<ICoreArtistCollectionItem>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreAlbumCollection> ISdkMember<ICoreAlbumCollection>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreAlbumCollectionItem> ISdkMember<ICoreAlbumCollectionItem>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreTrackCollection> ISdkMember<ICoreTrackCollection>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICorePlaylistCollection> ISdkMember<ICorePlaylistCollection>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICorePlaylistCollectionItem> ISdkMember<ICorePlaylistCollectionItem>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreImageCollection> ISdkMember<ICoreImageCollection>.Sources => Sources;

        /// <inheritdoc />
        public TimeSpan Duration => _collectionGroup.Duration;

        /// <inheritdoc />
        public SynchronizedObservableCollection<IImage> Images => _collectionGroup.Images;

        /// <inheritdoc />
        public SynchronizedObservableCollection<IPlaylistCollectionItem> Playlists { get; }

        /// <inheritdoc />
        public SynchronizedObservableCollection<TrackViewModel> Tracks { get; }

        /// <summary>
        /// The albums in this collection.
        /// </summary>
        public SynchronizedObservableCollection<IAlbumCollectionItem> Albums { get; }

        /// <inheritdoc />
        public SynchronizedObservableCollection<IArtistCollectionItem> Artists { get; }

        /// <summary>
        /// The nested <see cref="IPlayableCollectionGroupBase"/> items in this collection.
        /// </summary>
        public SynchronizedObservableCollection<PlayableCollectionGroupViewModel> Children { get; }

        /// <inheritdoc />
        public string Name
        {
            get => _collectionGroup.Name;
            private set => SetProperty(() => _collectionGroup.Name, value);
        }

        /// <inheritdoc />
        public int TotalPlaylistItemsCount
        {
            get => _collectionGroup.TotalPlaylistItemsCount;
            private set => SetProperty(() => _collectionGroup.TotalPlaylistItemsCount, value);
        }

        /// <inheritdoc />
        public int TotalTracksCount
        {
            get => _collectionGroup.TotalTracksCount;
            private set => SetProperty(() => _collectionGroup.TotalTracksCount, value);
        }

        /// <inheritdoc />
        public int TotalAlbumItemsCount
        {
            get => _collectionGroup.TotalAlbumItemsCount;
            private set => SetProperty(() => _collectionGroup.TotalAlbumItemsCount, value);
        }

        /// <inheritdoc />
        public int TotalArtistItemsCount
        {
            get => _collectionGroup.TotalArtistItemsCount;
            private set => SetProperty(() => _collectionGroup.TotalArtistItemsCount, value);
        }

        /// <inheritdoc />
        public int TotalChildrenCount
        {
            get => _collectionGroup.TotalChildrenCount;
            private set => SetProperty(() => _collectionGroup.TotalChildrenCount, value);
        }

        /// <inheritdoc />
        public Uri? Url
        {
            get => _collectionGroup.Url;
            private set => SetProperty(() => _collectionGroup.Url, value);
        }

        /// <inheritdoc />
        public string? Description
        {
            get => _collectionGroup.Description;
            private set => SetProperty(() => _collectionGroup.Description, value);
        }

        /// <inheritdoc />
        public PlaybackState PlaybackState
        {
            get => _collectionGroup.PlaybackState;
            private set => SetProperty(() => _collectionGroup.PlaybackState, value);
        }

        /// <inheritdoc />
        public event EventHandler<int> TotalChildrenCountChanged
        {
            add => _collectionGroup.TotalChildrenCountChanged += value;
            remove => _collectionGroup.TotalChildrenCountChanged -= value;
        }

        /// <inheritdoc />
        public bool IsPlayAsyncSupported => _collectionGroup.IsPlayAsyncSupported;

        /// <inheritdoc />
        public bool IsPauseAsyncSupported => _collectionGroup.IsPauseAsyncSupported;

        /// <inheritdoc />
        public bool IsChangeNameAsyncSupported => _collectionGroup.IsChangeNameAsyncSupported;

        /// <inheritdoc />
        public bool IsChangeDescriptionAsyncSupported => _collectionGroup.IsChangeDescriptionAsyncSupported;

        /// <inheritdoc />
        public bool IsChangeDurationAsyncSupported => _collectionGroup.IsChangeDurationAsyncSupported;

        /// <inheritdoc />
        public Task<bool> IsAddImageSupported(int index) => _collectionGroup.IsAddImageSupported(index);

        /// <inheritdoc />
        public Task<bool> IsAddPlaylistItemSupported(int index) => _collectionGroup.IsAddPlaylistItemSupported(index);

        /// <inheritdoc />
        public Task<bool> IsAddTrackSupported(int index) => _collectionGroup.IsAddTrackSupported(index);

        /// <inheritdoc />
        public Task<bool> IsAddAlbumItemSupported(int index) => _collectionGroup.IsAddAlbumItemSupported(index);

        /// <inheritdoc />
        public Task<bool> IsAddArtistSupported(int index) => _collectionGroup.IsAddArtistSupported(index);

        /// <inheritdoc />
        public Task<bool> IsAddChildSupported(int index) => _collectionGroup.IsAddChildSupported(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveImageSupported(int index) => _collectionGroup.IsRemoveImageSupported(index);

        /// <inheritdoc />
        public Task<bool> IsRemovePlaylistItemSupported(int index) => _collectionGroup.IsRemovePlaylistItemSupported(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveTrackSupported(int index) => _collectionGroup.IsRemoveTrackSupported(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveArtistSupported(int index) => _collectionGroup.IsRemoveArtistSupported(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveAlbumItemSupported(int index) => _collectionGroup.IsRemoveAlbumItemSupported(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveChildSupported(int index) => _collectionGroup.IsRemoveChildSupported(index);

        /// <inheritdoc />
        public Task PauseAsync() => _collectionGroup.PauseAsync();

        /// <inheritdoc />
        public Task PlayAsync() => _collectionGroup.PlayAsync();

        /// <inheritdoc />
        public Task ChangeNameAsync(string name) => _collectionGroup.ChangeNameAsync(name);

        /// <inheritdoc />
        public Task ChangeDescriptionAsync(string? description) => _collectionGroup.ChangeDescriptionAsync(description);

        /// <inheritdoc />
        public Task ChangeDurationAsync(TimeSpan duration) => _collectionGroup.ChangeDurationAsync(duration);

        /// <inheritdoc />
        public Task AddTrackAsync(ITrack track, int index) => _collectionGroup.AddTrackAsync(track, index);

        /// <inheritdoc />
        public Task AddArtistItemAsync(IArtistCollectionItem artist, int index) => _collectionGroup.AddArtistItemAsync(artist, index);

        /// <inheritdoc />
        public Task AddAlbumItemAsync(IAlbumCollectionItem album, int index) => _collectionGroup.AddAlbumItemAsync(album, index);

        /// <inheritdoc />
        public Task AddPlaylistItemAsync(IPlaylistCollectionItem playlist, int index) => _collectionGroup.AddPlaylistItemAsync(playlist, index);

        /// <inheritdoc />
        public Task AddChildAsync(IPlayableCollectionGroup child, int index) => _collectionGroup.AddChildAsync(child, index);

        /// <inheritdoc />
        public Task RemoveTrackAsync(int index) => _collectionGroup.RemoveTrackAsync(index);

        /// <inheritdoc />
        public Task RemoveArtistAsync(int index) => _collectionGroup.RemoveArtistAsync(index);

        /// <inheritdoc />
        public Task RemoveAlbumItemAsync(int index) => _collectionGroup.RemoveAlbumItemAsync(index);

        /// <inheritdoc />
        public Task RemovePlaylistItemAsync(int index) => _collectionGroup.RemovePlaylistItemAsync(index);

        /// <inheritdoc />
        public Task RemoveChildAsync(int index) => _collectionGroup.RemoveChildAsync(index);

        /// <inheritdoc />
        public Task<IReadOnlyList<IPlayableCollectionGroup>> GetChildrenAsync(int limit, int offset) => _collectionGroup.GetChildrenAsync(limit, offset);

        /// <inheritdoc />
        public Task<IReadOnlyList<IPlaylistCollectionItem>> GetPlaylistItemsAsync(int limit, int offset) => _collectionGroup.GetPlaylistItemsAsync(limit, offset);

        /// <inheritdoc />
        public Task<IReadOnlyList<ITrack>> GetTracksAsync(int limit, int offset = 0) => _collectionGroup.GetTracksAsync(limit, offset);

        /// <inheritdoc />
        public Task<IReadOnlyList<IAlbumCollectionItem>> GetAlbumItemsAsync(int limit, int offset) => _collectionGroup.GetAlbumItemsAsync(limit, offset);

        /// <inheritdoc />
        public Task<IReadOnlyList<IArtistCollectionItem>> GetArtistItemsAsync(int limit, int offset) => _collectionGroup.GetArtistItemsAsync(limit, offset);

        /// <inheritdoc />
        public async Task PopulateMorePlaylistsAsync(int limit)
        {
            foreach (var item in await _collectionGroup.GetPlaylistItemsAsync(limit, Playlists.Count))
            {
                switch (item)
                {
                    case IPlaylist playlist:
                        Playlists.Add(new PlaylistViewModel(playlist));
                        break;
                    case IPlaylistCollection collection:
                        Playlists.Add(new PlaylistCollectionViewModel(collection));
                        break;
                }
            }
        }

        /// <inheritdoc />
        public async Task PopulateMoreTracksAsync(int limit)
        {
            foreach (var item in await _collectionGroup.GetTracksAsync(limit, Tracks.Count))
            {
                Tracks.Add(new TrackViewModel(item));
            }
        }

        /// <inheritdoc />
        public async Task PopulateMoreAlbumsAsync(int limit)
        {
            foreach (var item in await _collectionGroup.GetAlbumItemsAsync(limit, Albums.Count))
            {
                switch (item)
                {
                    case IAlbum album:
                        Albums.Add(new AlbumViewModel(album));
                        break;
                    case IAlbumCollection collection:
                        Albums.Add(new AlbumCollectionViewModel(collection));
                        break;
                }
            }
        }

        /// <inheritdoc />
        public async Task PopulateMoreArtistsAsync(int limit)
        {
            foreach (var item in await _collectionGroup.GetArtistItemsAsync(limit, Artists.Count))
            {
                if (item is IArtist artist)
                {
                    Artists.Add(new ArtistViewModel(artist));
                }

                if (item is IArtistCollection collection)
                {
                    Artists.Add(new ArtistCollectionViewModel(collection));
                }
            }
        }

        /// <inheritdoc />
        public async Task PopulateMoreChildrenAsync(int limit)
        {
            foreach (var item in await _collectionGroup.GetChildrenAsync(limit, Albums.Count))
            {
                Children.Add(new PlayableCollectionGroupViewModel(item));
            }
        }

        /// <summary>
        /// Attempts to play the album.
        /// </summary>
        public IAsyncRelayCommand PlayAsyncCommand { get; }

        /// <summary>
        /// Attempts to pause the album, if playing.
        /// </summary>
        public IAsyncRelayCommand PauseAsyncCommand { get; }

        /// <summary>
        /// Attempts to change the name of the album, if supported.
        /// </summary>
        public IAsyncRelayCommand ChangeNameAsyncCommand { get; }

        /// <summary>
        /// Attempts to change the description of the album, if supported.
        /// </summary>
        public IAsyncRelayCommand ChangeDescriptionAsyncCommand { get; }

        /// <summary>
        /// Attempts to change the duration of the album, if supported.
        /// </summary>
        public IAsyncRelayCommand ChangeDurationAsyncCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand<int> PopulateMoreTracksCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand<int> PopulateMorePlaylistsCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand<int> PopulateMoreAlbumsCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand<int> PopulateMoreArtistsCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand<int> PopulateMoreChildrenCommand { get; }
    }
}
