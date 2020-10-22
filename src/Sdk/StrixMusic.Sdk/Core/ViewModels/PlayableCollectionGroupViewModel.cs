using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using OwlCore.Collections;
using OwlCore.Helpers;
using StrixMusic.Sdk.Core.Data;

namespace StrixMusic.Sdk.Core.ViewModels
{
    /// <summary>
    /// A bindable wrapper of the <see cref="IPlayableCollectionGroup"/>.
    /// </summary>
    public class PlayableCollectionGroupViewModel : ObservableObject, IPlayableCollectionGroup, IPlayableCollectionGroupChildrenViewModel, IAlbumCollectionViewModel, IArtistCollectionViewModel, ITrackCollectionViewModel, IPlaylistCollectionViewModel
    {
        private readonly IPlayableCollectionGroup _collectionGroupBase;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayableCollectionGroupViewModel"/> class.
        /// </summary>
        /// <param name="collectionGroup">The base <see cref="IPlayableCollectionBase"/> containing properties about this class.</param>
        public PlayableCollectionGroupViewModel(IPlayableCollectionGroup collectionGroup)
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
            Playlists = Threading.InvokeOnUI(() => new SynchronizedObservableCollection<PlaylistViewModel>());
            Albums = Threading.InvokeOnUI(() => new SynchronizedObservableCollection<AlbumViewModel>());
            Artists = Threading.InvokeOnUI(() => new SynchronizedObservableCollection<ArtistViewModel>());
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

        private void CollectionGroupBase_UrlChanged(object sender, Uri? e) => Url = e;

        private void CollectionGroupBase_NameChanged(object sender, string e) => Name = e;

        private void CollectionGroupBase_DescriptionChanged(object sender, string? e) => Description = e;

        private void CollectionGroupBase_PlaybackStateChanged(object sender, PlaybackState e) => PlaybackState = e;

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

        /// <inheritdoc />
        public string Id => _collectionGroupBase.Id;

        /// <inheritdoc />
        public ICore SourceCore { get; }

        /// <inheritdoc />
        public TimeSpan Duration => _collectionGroupBase.Duration;

        /// <inheritdoc />
        public int TotalPlaylistCount => _collectionGroupBase.TotalPlaylistCount;

        /// <inheritdoc />
        public int TotalTracksCount => _collectionGroupBase.TotalTracksCount;

        /// <inheritdoc />
        public int TotalAlbumsCount => _collectionGroupBase.TotalAlbumsCount;

        /// <inheritdoc />
        public int TotalArtistsCount => _collectionGroupBase.TotalArtistsCount;

        /// <inheritdoc />
        public int TotalChildrenCount => _collectionGroupBase.TotalChildrenCount;

        /// <inheritdoc />
        public SynchronizedObservableCollection<IImage> Images => _collectionGroupBase.Images;

        /// <summary>
        /// The playlists in this collection
        /// </summary>
        public SynchronizedObservableCollection<PlaylistViewModel> Playlists { get; }

        /// <inheritdoc />
        public SynchronizedObservableCollection<TrackViewModel> Tracks { get; }

        /// <summary>
        /// The albums in this collection.
        /// </summary>
        public SynchronizedObservableCollection<AlbumViewModel> Albums { get; }

        /// <inheritdoc />
        public SynchronizedObservableCollection<ArtistViewModel> Artists { get; }

        /// <summary>
        /// The nested <see cref="IPlayableCollectionGroup"/> items in this collection.
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
        public Task<bool> IsAddPlaylistSupported(int index) => _collectionGroupBase.IsAddPlaylistSupported(index);

        /// <inheritdoc />
        public Task<bool> IsAddTrackSupported(int index) => _collectionGroupBase.IsAddTrackSupported(index);

        /// <inheritdoc />
        public Task<bool> IsAddAlbumSupported(int index) => _collectionGroupBase.IsAddAlbumSupported(index);

        /// <inheritdoc />
        public Task<bool> IsAddArtistSupported(int index) => _collectionGroupBase.IsAddArtistSupported(index);

        /// <inheritdoc />
        public Task<bool> IsAddChildSupported(int index) => _collectionGroupBase.IsAddChildSupported(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveImageSupported(int index) => _collectionGroupBase.IsRemoveImageSupported(index);

        /// <inheritdoc />
        public Task<bool> IsRemovePlaylistSupported(int index) => _collectionGroupBase.IsRemovePlaylistSupported(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveTrackSupported(int index) => _collectionGroupBase.IsRemoveTrackSupported(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveArtistSupported(int index) => _collectionGroupBase.IsRemoveArtistSupported(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveAlbumSupported(int index) => _collectionGroupBase.IsRemoveAlbumSupported(index);

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
        public Task AddTrackAsync(ITrack track, int index) => _collectionGroupBase.AddTrackAsync(track, index);

        /// <inheritdoc />
        public Task AddArtistAsync(IArtist artist, int index) => _collectionGroupBase.AddArtistAsync(artist, index);

        /// <inheritdoc />
        public Task AddAlbumAsync(IAlbum album, int index) => _collectionGroupBase.AddAlbumAsync(album, index);

        /// <inheritdoc />
        public Task AddPlaylistAsync(IPlayableCollectionGroup playlist, int index) => _collectionGroupBase.AddPlaylistAsync(playlist, index);

        /// <inheritdoc />
        public Task AddChildAsync(IPlayableCollectionGroup child, int index) => _collectionGroupBase.AddChildAsync(child, index);

        /// <inheritdoc />
        public Task RemoveTrackAsync(int index) => _collectionGroupBase.RemoveTrackAsync(index);

        /// <inheritdoc />
        public Task RemoveArtistAsync(int index) => _collectionGroupBase.RemoveArtistAsync(index);

        /// <inheritdoc />
        public Task RemoveAlbumAsync(int index) => _collectionGroupBase.RemoveAlbumAsync(index);

        /// <inheritdoc />
        public Task RemovePlaylistAsync(int index) => _collectionGroupBase.RemovePlaylistAsync(index);

        /// <inheritdoc />
        public Task RemoveChildAsync(int index) => _collectionGroupBase.RemoveChildAsync(index);

        /// <inheritdoc />
        public IAsyncEnumerable<IPlayableCollectionGroup> GetChildrenAsync(int limit, int offset) => _collectionGroupBase.GetChildrenAsync(limit, offset);

        /// <inheritdoc />
        public IAsyncEnumerable<IPlaylist> GetPlaylistsAsync(int limit, int offset = 0) => _collectionGroupBase.GetPlaylistsAsync(limit, offset);

        /// <inheritdoc />
        public IAsyncEnumerable<ITrack> GetTracksAsync(int limit, int offset = 0) => _collectionGroupBase.GetTracksAsync(limit, offset);

        /// <inheritdoc />
        public IAsyncEnumerable<IAlbum> GetAlbumsAsync(int limit, int offset = 0) => _collectionGroupBase.GetAlbumsAsync(limit, offset);

        /// <inheritdoc />
        public IAsyncEnumerable<IArtist> GetArtistsAsync(int limit, int offset = 0) => _collectionGroupBase.GetArtistsAsync(limit, offset);

        /// <inheritdoc />
        public Task PopulateMorePlaylistsAsync(int limit)
        {
            // TODO
            return Task.CompletedTask;
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
            await foreach (var item in _collectionGroupBase.GetAlbumsAsync(limit, Albums.Count))
            {
                Albums.Add(new AlbumViewModel(item));
            }
        }

        /// <inheritdoc />
        public async Task PopulateMoreArtistsAsync(int limit)
        {
            await foreach (var item in _collectionGroupBase.GetArtistsAsync(limit, Albums.Count))
            {
                Artists.Add(new ArtistViewModel(item));
            }
        }

        /// <inheritdoc />
        public Task PopulateMoreChildrenAsync(int limit)
        {
            // TODO
            return Task.CompletedTask;
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
