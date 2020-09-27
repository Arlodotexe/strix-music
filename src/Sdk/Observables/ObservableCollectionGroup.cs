using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using OwlCore.Collections;
using StrixMusic.Sdk.Enums;
using StrixMusic.Sdk.Events;
using StrixMusic.Sdk.Interfaces;

namespace StrixMusic.Sdk.Observables
{
    /// <summary>
    /// A bindable wrapper of the <see cref="IPlayableCollectionGroup"/>.
    /// </summary>
    public class ObservableCollectionGroup : ObservableObject, IPlayableCollectionGroup
    {
        private readonly IPlayableCollectionGroup _collectionGroupBase;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableCollectionGroup"/> class.
        /// </summary>
        /// <param name="collectionGroup">The base <see cref="IPlayableCollectionBase"/> containing properties about this class.</param>
        public ObservableCollectionGroup(IPlayableCollectionGroup collectionGroup)
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

            Tracks = new SynchronizedObservableCollection<ObservableTrack>();
            Playlists = new SynchronizedObservableCollection<ObservablePlaylist>();
            Albums = new SynchronizedObservableCollection<ObservableAlbum>();
            Artists = new SynchronizedObservableCollection<ObservableArtist>();
            Children = new SynchronizedObservableCollection<ObservableCollectionGroup>();

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

        private void AttachCollectionEvents()
        {
            _collectionGroupBase.TracksChanged += CollectionGroupBase_TracksChanged;
            _collectionGroupBase.AlbumsChanged += CollectionGroupBase_AlbumsChanged;
            _collectionGroupBase.ArtistsChanged += CollectionGroupBase_ArtistsChanged;
            _collectionGroupBase.PlaylistsChanged += CollectionGroupBase_PlaylistsChanged;
            _collectionGroupBase.ChildrenChanged += CollectionGroupBase_ChildrenChanged;
        }

        private void DetachCollectionEvents()
        {
            _collectionGroupBase.TracksChanged -= CollectionGroupBase_TracksChanged;
            _collectionGroupBase.AlbumsChanged -= CollectionGroupBase_AlbumsChanged;
            _collectionGroupBase.ArtistsChanged -= CollectionGroupBase_ArtistsChanged;
            _collectionGroupBase.PlaylistsChanged -= CollectionGroupBase_PlaylistsChanged;
            _collectionGroupBase.ChildrenChanged -= CollectionGroupBase_ChildrenChanged;
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

        private void CollectionGroupBase_ChildrenChanged(object sender, CollectionChangedEventArgs<IPlayableCollectionGroup> e)
        {
            foreach (var item in e.AddedItems)
            {
                Children.Insert(item.Index, new ObservableCollectionGroup(item.Data));
            }

            foreach (var item in e.RemovedItems)
            {
                Children.RemoveAt(item.Index);
            }
        }

        private void CollectionGroupBase_PlaylistsChanged(object sender, CollectionChangedEventArgs<IPlaylist> e)
        {
            foreach (var item in e.AddedItems)
            {
                Playlists.Insert(item.Index, new ObservablePlaylist(item.Data));
            }

            foreach (var item in e.RemovedItems)
            {
                Playlists.RemoveAt(item.Index);
            }
        }

        private void CollectionGroupBase_ArtistsChanged(object sender, CollectionChangedEventArgs<IArtist> e)
        {
            foreach (var item in e.AddedItems)
            {
                Artists.Insert(item.Index, new ObservableArtist(item.Data));
            }

            foreach (var item in e.RemovedItems)
            {
                Artists.RemoveAt(item.Index);
            }
        }

        private void CollectionGroupBase_AlbumsChanged(object sender, CollectionChangedEventArgs<IAlbum> e)
        {
            foreach (var item in e.AddedItems)
            {
                Albums.Insert(item.Index, new ObservableAlbum(item.Data));
            }

            foreach (var item in e.RemovedItems)
            {
                Albums.RemoveAt(item.Index);
            }
        }

        private void CollectionGroupBase_TracksChanged(object sender, CollectionChangedEventArgs<ITrack> e)
        {
            foreach (var item in e.AddedItems)
            {
                Tracks.Insert(item.Index, new ObservableTrack(item.Data));
            }

            foreach (var item in e.RemovedItems)
            {
                Tracks.RemoveAt(item.Index);
            }
        }

        /// <inheritdoc />
        public event EventHandler<CollectionChangedEventArgs<IPlaylist>>? PlaylistsChanged
        {
            add => _collectionGroupBase.PlaylistsChanged += value;
            remove => _collectionGroupBase.PlaylistsChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<CollectionChangedEventArgs<ITrack>>? TracksChanged
        {
            add => _collectionGroupBase.TracksChanged += value;
            remove => _collectionGroupBase.TracksChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<CollectionChangedEventArgs<IAlbum>>? AlbumsChanged
        {
            add => _collectionGroupBase.AlbumsChanged += value;
            remove => _collectionGroupBase.AlbumsChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<CollectionChangedEventArgs<IArtist>>? ArtistsChanged
        {
            add => _collectionGroupBase.ArtistsChanged += value;
            remove => _collectionGroupBase.ArtistsChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<CollectionChangedEventArgs<IPlayableCollectionGroup>>? ChildrenChanged
        {
            add => _collectionGroupBase.ChildrenChanged += value;
            remove => _collectionGroupBase.ChildrenChanged -= value;
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
        public SynchronizedObservableCollection<ObservablePlaylist> Playlists { get; }

        /// <summary>
        /// The tracks in this collection.
        /// </summary>
        public SynchronizedObservableCollection<ObservableTrack> Tracks { get; }

        /// <summary>
        /// The albums in this collection.
        /// </summary>
        public SynchronizedObservableCollection<ObservableAlbum> Albums { get; }

        /// <summary>
        /// The artists in this collection.
        /// </summary>
        public SynchronizedObservableCollection<ObservableArtist> Artists { get; }

        /// <summary>
        /// The nested <see cref="IPlayableCollectionGroup"/> items in this collection.
        /// </summary>
        public SynchronizedObservableCollection<ObservableCollectionGroup> Children { get; }

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
        public Task AddTrackAsync(IPlayableCollectionGroup track, int index) => _collectionGroupBase.AddTrackAsync(track, index);

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

        /// <summary>
        /// Populates the next set of playlists into the collection.
        /// </summary>
        /// <param name="limit">The number of items to load.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task PopulateMorePlaylistsAsync(int limit)
        {
            // TODO
            return Task.CompletedTask;
        }

        /// <summary>
        /// Populates the next set of tracks into the collection.
        /// </summary>
        /// <param name="limit">The number of items to load.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task PopulateMoreTracksAsync(int limit)
        {
            await foreach (var item in _collectionGroupBase.GetTracksAsync(limit, Tracks.Count))
            {
                Tracks.Add(new ObservableTrack(item));
            }
        }

        /// <summary>
        /// Populates the next set of albums into the collection.
        /// </summary>
        /// <param name="limit">The number of items to load.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task PopulateMoreAlbumsAsync(int limit)
        {
            // TODO
            return Task.CompletedTask;
        }

        /// <summary>
        /// Populates the next set of artists into the collection.
        /// </summary>
        /// <param name="limit">The number of items to load.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task PopulateMoreArtistsAsync(int limit)
        {
            // TODO
            return Task.CompletedTask;
        }

        /// <summary>
        /// Populates the next set of children items into the collection.
        /// </summary>
        /// <param name="limit">The number of items to load.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
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

        /// <summary>
        /// <inheritdoc cref="PopulateMoreTracksAsync"/>
        /// </summary>
        public IAsyncRelayCommand<int> PopulateMoreTracksCommand { get; }

        /// <summary>
        /// <inheritdoc cref="PopulateMorePlaylistsAsync"/>
        /// </summary>
        public IAsyncRelayCommand<int> PopulateMorePlaylistsCommand { get; }

        /// <summary>
        /// <inheritdoc cref="PopulateMoreAlbumsAsync"/>
        /// </summary>
        public IAsyncRelayCommand<int> PopulateMoreAlbumsCommand { get; }

        /// <summary>
        /// <inheritdoc cref="PopulateMoreArtistsAsync"/>
        /// </summary>
        public IAsyncRelayCommand<int> PopulateMoreArtistsCommand { get; }

        /// <summary>
        /// <inheritdoc cref="PopulateMoreChildrenAsync"/>
        /// </summary>
        public IAsyncRelayCommand<int> PopulateMoreChildrenCommand { get; }
    }
}
