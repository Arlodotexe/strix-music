using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using OwlCore.Collections;
using OwlCore.Helpers;
using StrixMusic.Sdk.Data;
using StrixMusic.Sdk.Data.Base;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.MediaPlayback;

namespace StrixMusic.Sdk.ViewModels
{
    /// <summary>
    /// An observable wrapper for a <see cref="IPlayableCollectionGroupBase"/>.
    /// </summary>
    public class PlayableCollectionGroupViewModel : ObservableObject, IPlayableCollectionGroupBase, IPlayableCollectionGroupChildrenViewModel, IAlbumCollectionViewModel, IArtistCollectionViewModel, ITrackCollectionViewModel, IPlaylistCollectionViewModel
    {
        private readonly IPlayableCollectionGroupBase _collectionGroupBase;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayableCollectionGroupViewModel"/> class.
        /// </summary>
        /// <param name="collectionGroup">The base <see cref="IPlayableCollectionBase"/> containing properties about this class.</param>
        public PlayableCollectionGroupViewModel(IPlayableCollectionGroupBase collectionGroup)
        {
            _collectionGroupBase = collectionGroup ?? throw new ArgumentNullException(nameof(collectionGroup));

            SourceCore = MainViewModel.GetLoadedCore(_collectionGroupBase.SourceCore);

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
            Playlists = Threading.InvokeOnUI(() => new SynchronizedObservableCollection<ICorePlaylistCollectionItem>());
            Albums = Threading.InvokeOnUI(() => new SynchronizedObservableCollection<ICoreAlbumCollectionItem>());
            Artists = Threading.InvokeOnUI(() => new SynchronizedObservableCollection<ICoreArtistCollectionItem>());
            Children = Threading.InvokeOnUI(() => new SynchronizedObservableCollection<PlayableCollectionGroupViewModel>());

            AttachPropertyEvents();
        }

        private void AttachPropertyEvents()
        {
            _collectionGroupBase.PlaybackStateChanged += CollectionGroupBase_PlaybackStateChanged;
            _collectionGroupBase.DescriptionChanged += CollectionGroupBase_DescriptionChanged;
            _collectionGroupBase.NameChanged += CollectionGroupBase_NameChanged;
            _collectionGroupBase.UrlChanged += CollectionGroupBase_UrlChanged;
        }

        private void DetachPropertyEvents()
        {
            _collectionGroupBase.PlaybackStateChanged -= CollectionGroupBase_PlaybackStateChanged;
            _collectionGroupBase.DescriptionChanged -= CollectionGroupBase_DescriptionChanged;
            _collectionGroupBase.NameChanged -= CollectionGroupBase_NameChanged;
            _collectionGroupBase.UrlChanged -= CollectionGroupBase_UrlChanged;
        }

        /// <inheritdoc />
        public event EventHandler<string> NameChanged
        {
            add => _collectionGroupBase.NameChanged += value;
            remove => _collectionGroupBase.NameChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<string?> DescriptionChanged
        {
            add => _collectionGroupBase.DescriptionChanged += value;
            remove => _collectionGroupBase.DescriptionChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<Uri?> UrlChanged
        {
            add => _collectionGroupBase.UrlChanged += value;
            remove => _collectionGroupBase.UrlChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<PlaybackState>? PlaybackStateChanged
        {
            add => _collectionGroupBase.PlaybackStateChanged += value;

            remove => _collectionGroupBase.PlaybackStateChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<TimeSpan>? DurationChanged
        {
            add => _collectionGroupBase.DurationChanged += value;

            remove => _collectionGroupBase.DurationChanged -= value;
        }

        private void CollectionGroupBase_UrlChanged(object sender, Uri? e) => Url = e;

        private void CollectionGroupBase_NameChanged(object sender, string e) => Name = e;

        private void CollectionGroupBase_DescriptionChanged(object sender, string? e) => Description = e;

        private void CollectionGroupBase_PlaybackStateChanged(object sender, PlaybackState e) => PlaybackState = e;

        /// <inheritdoc />
        public string Id => _collectionGroupBase.Id;

        /// <inheritdoc />
        public ICore SourceCore { get; }

        /// <inheritdoc />
        public TimeSpan Duration => _collectionGroupBase.Duration;

        /// <inheritdoc />
        public int TotalPlaylistItemsCount => _collectionGroupBase.TotalPlaylistItemsCount;

        /// <inheritdoc />
        public int TotalTracksCount => _collectionGroupBase.TotalTracksCount;

        /// <inheritdoc />
        public int TotalAlbumItemsCount => _collectionGroupBase.TotalAlbumItemsCount;

        /// <inheritdoc />
        public int TotalArtistItemsCount => _collectionGroupBase.TotalArtistItemsCount;

        /// <inheritdoc />
        public int TotalChildrenCount => _collectionGroupBase.TotalChildrenCount;

        /// <inheritdoc />
        public SynchronizedObservableCollection<ICoreImage> Images => _collectionGroupBase.Images;

        /// <inheritdoc />
        public SynchronizedObservableCollection<ICorePlaylistCollectionItem> Playlists { get; }

        /// <inheritdoc />
        public SynchronizedObservableCollection<TrackViewModel> Tracks { get; }

        /// <summary>
        /// The albums in this collection.
        /// </summary>
        public SynchronizedObservableCollection<ICoreAlbumCollectionItem> Albums { get; }

        /// <inheritdoc />
        public SynchronizedObservableCollection<ICoreArtistCollectionItem> Artists { get; }

        /// <summary>
        /// The nested <see cref="IPlayableCollectionGroupBase"/> items in this collection.
        /// </summary>
        public SynchronizedObservableCollection<PlayableCollectionGroupViewModel> Children { get; }

        /// <inheritdoc />
        public string Name
        {
            get => _collectionGroupBase.Name;
            private set => SetProperty(() => _collectionGroupBase.Name, value);
        }

        /// <inheritdoc />
        public Uri? Url
        {
            get => _collectionGroupBase.Url;
            private set => SetProperty(() => _collectionGroupBase.Url, value);
        }

        /// <inheritdoc />
        public string? Description
        {
            get => _collectionGroupBase.Description;
            private set => SetProperty(() => _collectionGroupBase.Description, value);
        }

        /// <inheritdoc />
        public PlaybackState PlaybackState
        {
            get => _collectionGroupBase.PlaybackState;
            private set => SetProperty(() => _collectionGroupBase.PlaybackState, value);
        }

        /// <inheritdoc />
        public bool IsPlayAsyncSupported => _collectionGroupBase.IsPlayAsyncSupported;

        /// <inheritdoc />
        public bool IsPauseAsyncSupported => _collectionGroupBase.IsPauseAsyncSupported;

        /// <inheritdoc />
        public bool IsChangeNameAsyncSupported => _collectionGroupBase.IsChangeNameAsyncSupported;

        /// <inheritdoc />
        public bool IsChangeDescriptionAsyncSupported => _collectionGroupBase.IsChangeDescriptionAsyncSupported;

        /// <inheritdoc />
        public bool IsChangeDurationAsyncSupported => _collectionGroupBase.IsChangeDurationAsyncSupported;

        /// <inheritdoc />
        public Task<bool> IsAddImageSupported(int index) => _collectionGroupBase.IsAddImageSupported(index);

        /// <inheritdoc />
        public Task<bool> IsAddPlaylistItemSupported(int index) => _collectionGroupBase.IsAddPlaylistItemSupported(index);

        /// <inheritdoc />
        public Task<bool> IsAddTrackSupported(int index) => _collectionGroupBase.IsAddTrackSupported(index);

        /// <inheritdoc />
        public Task<bool> IsAddAlbumItemSupported(int index) => _collectionGroupBase.IsAddAlbumItemSupported(index);

        /// <inheritdoc />
        public Task<bool> IsAddArtistSupported(int index) => _collectionGroupBase.IsAddArtistSupported(index);

        /// <inheritdoc />
        public Task<bool> IsAddChildSupported(int index) => _collectionGroupBase.IsAddChildSupported(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveImageSupported(int index) => _collectionGroupBase.IsRemoveImageSupported(index);

        /// <inheritdoc />
        public Task<bool> IsRemovePlaylistItemSupported(int index) => _collectionGroupBase.IsRemovePlaylistItemSupported(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveTrackSupported(int index) => _collectionGroupBase.IsRemoveTrackSupported(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveArtistSupported(int index) => _collectionGroupBase.IsRemoveArtistSupported(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveAlbumItemSupported(int index) => _collectionGroupBase.IsRemoveAlbumItemSupported(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveChildSupported(int index) => _collectionGroupBase.IsRemoveChildSupported(index);

        /// <inheritdoc />
        public Task PauseAsync() => _collectionGroupBase.PauseAsync();

        /// <inheritdoc />
        public Task PlayAsync() => _collectionGroupBase.PlayAsync();

        /// <inheritdoc />
        public Task ChangeNameAsync(string name) => _collectionGroupBase.ChangeNameAsync(name);

        /// <inheritdoc />
        public Task ChangeDescriptionAsync(string? description) => _collectionGroupBase.ChangeDescriptionAsync(description);

        /// <inheritdoc />
        public Task ChangeDurationAsync(TimeSpan duration) => _collectionGroupBase.ChangeDurationAsync(duration);

        /// <inheritdoc />
        public Task AddTrackAsync(ICoreTrack track, int index) => _collectionGroupBase.AddTrackAsync(track, index);

        /// <inheritdoc />
        public Task AddArtistItemAsync(ICoreArtistCollectionItem artist, int index) => _collectionGroupBase.AddArtistItemAsync(artist, index);

        /// <inheritdoc />
        public Task AddAlbumItemAsync(ICoreAlbumCollectionItem album, int index) => _collectionGroupBase.AddAlbumItemAsync(album, index);

        /// <inheritdoc />
        public Task AddPlaylistItemAsync(ICorePlaylistCollectionItem playlist, int index) => _collectionGroupBase.AddPlaylistItemAsync(playlist, index);

        /// <inheritdoc />
        public Task AddChildAsync(IPlayableCollectionGroupBase child, int index) => _collectionGroupBase.AddChildAsync(child, index);

        /// <inheritdoc />
        public Task RemoveTrackAsync(int index) => _collectionGroupBase.RemoveTrackAsync(index);

        /// <inheritdoc />
        public Task RemoveArtistAsync(int index) => _collectionGroupBase.RemoveArtistAsync(index);

        /// <inheritdoc />
        public Task RemoveAlbumItemAsync(int index) => _collectionGroupBase.RemoveAlbumItemAsync(index);

        /// <inheritdoc />
        public Task RemovePlaylistItemAsync(int index) => _collectionGroupBase.RemovePlaylistItemAsync(index);

        /// <inheritdoc />
        public Task RemoveChildAsync(int index) => _collectionGroupBase.RemoveChildAsync(index);

        /// <inheritdoc />
        public IAsyncEnumerable<IPlayableCollectionGroupBase> GetChildrenAsync(int limit, int offset) => _collectionGroupBase.GetChildrenAsync(limit, offset);

        /// <inheritdoc />
        public IAsyncEnumerable<ICorePlaylistCollectionItem> GetPlaylistItemsAsync(int limit, int offset) => _collectionGroupBase.GetPlaylistItemsAsync(limit, offset);

        /// <inheritdoc />
        public IAsyncEnumerable<ICoreTrack> GetTracksAsync(int limit, int offset = 0) => _collectionGroupBase.GetTracksAsync(limit, offset);

        /// <inheritdoc />
        public IAsyncEnumerable<ICoreAlbumCollectionItem> GetAlbumItemsAsync(int limit, int offset) => _collectionGroupBase.GetAlbumItemsAsync(limit, offset);

        /// <inheritdoc />
        public IAsyncEnumerable<ICoreArtistCollectionItem> GetArtistItemsAsync(int limit, int offset) => _collectionGroupBase.GetArtistItemsAsync(limit, offset);

        /// <inheritdoc />
        public async Task PopulateMorePlaylistsAsync(int limit)
        {
            await foreach (var item in _collectionGroupBase.GetPlaylistItemsAsync(limit, Playlists.Count))
            {
                switch (item)
                {
                    case ICorePlaylist playlist:
                        Playlists.Add(new PlaylistViewModel(playlist));
                        break;
                    case ICorePlaylistCollection collection:
                        Playlists.Add(new PlaylistCollectionViewModel(collection));
                        break;
                }
            }
        }

        /// <inheritdoc />
        public async Task PopulateMoreTracksAsync(int limit)
        {
            await foreach (var item in _collectionGroupBase.GetTracksAsync(limit, Tracks.Count))
            {
                Tracks.Add(new TrackViewModel(item));
            }
        }

        /// <inheritdoc />
        public async Task PopulateMoreAlbumsAsync(int limit)
        {
            await foreach (var item in _collectionGroupBase.GetAlbumItemsAsync(limit, Albums.Count))
            {
                switch (item)
                {
                    case ICoreAlbum album:
                        Albums.Add(new AlbumViewModel(album));
                        break;
                    case ICoreAlbumCollection collection:
                        Albums.Add(new AlbumCollectionViewModel(collection));
                        break;
                }
            }
        }

        /// <inheritdoc />
        public async Task PopulateMoreArtistsAsync(int limit)
        {
            await foreach (var item in _collectionGroupBase.GetArtistItemsAsync(limit, Artists.Count))
            {
                if (item is ICoreArtist artist)
                {
                    Artists.Add(new ArtistViewModel(artist));
                }

                if (item is ICoreArtistCollection collection)
                {
                    Artists.Add(new ArtistCollectionViewModel(collection));
                }
            }
        }

        /// <inheritdoc />
        public async Task PopulateMoreChildrenAsync(int limit)
        {
            await foreach (var item in _collectionGroupBase.GetChildrenAsync(limit, Albums.Count))
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
