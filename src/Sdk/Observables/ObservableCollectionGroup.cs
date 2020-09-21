using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using OwlCore.Collections;
using StrixMusic.Sdk.Enums;
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

            Tracks = new SynchronizedObservableCollection<ITrack>(_collectionGroupBase.Tracks.Select(x => new ObservableTrack(x)));
            Playlists = new SynchronizedObservableCollection<IPlaylist>(_collectionGroupBase.Playlists.Select(x => new ObservablePlaylist(x)));
            Albums = new SynchronizedObservableCollection<IAlbum>(_collectionGroupBase.Albums.Select(x => new ObservableAlbum(x)));
            Artists = new SynchronizedObservableCollection<IArtist>(_collectionGroupBase.Artists.Select(x => new ObservableArtist(x)));
            Children = new SynchronizedObservableCollection<IPlayableCollectionGroup>(_collectionGroupBase.Children.Select(x => new ObservableCollectionGroup(x)));

            AttachEvents();
        }

        private void AttachEvents()
        {
            _collectionGroupBase.PlaybackStateChanged += CollectionGroupBase_PlaybackStateChanged;
            _collectionGroupBase.DescriptionChanged += CollectionGroupBase_DescriptionChanged;
            _collectionGroupBase.NameChanged += CollectionGroupBase_NameChanged;
            _collectionGroupBase.UrlChanged += CollectionGroupBase_UrlChanged;
        }

        private void DetachEvents()
        {
            _collectionGroupBase.PlaybackStateChanged -= CollectionGroupBase_PlaybackStateChanged;
            _collectionGroupBase.DescriptionChanged -= CollectionGroupBase_DescriptionChanged;
            _collectionGroupBase.NameChanged -= CollectionGroupBase_NameChanged;
            _collectionGroupBase.UrlChanged -= CollectionGroupBase_UrlChanged;
        }

        /// <inheritdoc />
        public string Id => _collectionGroupBase.Id;

        /// <inheritdoc />
        public ICore SourceCore { get; }

        /// <inheritdoc />
        public string Name
        {
            get => _collectionGroupBase.Name;
            private set => SetProperty(() => _collectionGroupBase.Name, value);
        }

        /// <inheritdoc />
        public TimeSpan Duration => _collectionGroupBase.Duration;

        /// <inheritdoc />
        public SynchronizedObservableCollection<IImage> Images => _collectionGroupBase.Images;

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
        public SynchronizedObservableCollection<IPlaylist> Playlists { get; }

        /// <inheritdoc />
        public SynchronizedObservableCollection<ITrack> Tracks { get; }

        /// <inheritdoc />
        public SynchronizedObservableCollection<IAlbum> Albums { get; }

        /// <inheritdoc />
        public SynchronizedObservableCollection<IArtist> Artists { get; }

        /// <inheritdoc />
        public SynchronizedObservableCollection<IPlayableCollectionGroup> Children { get; }

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
        public SynchronizedObservableCollection<bool> IsRemoveImageSupportedMap => _collectionGroupBase.IsRemoveImageSupportedMap;

        /// <inheritdoc />
        public SynchronizedObservableCollection<bool> IsRemovePlaylistSupportedMap => _collectionGroupBase.IsRemovePlaylistSupportedMap;

        /// <inheritdoc />
        public SynchronizedObservableCollection<bool> IsRemoveTrackSupportedMap => _collectionGroupBase.IsRemoveTrackSupportedMap;

        /// <inheritdoc />
        public SynchronizedObservableCollection<bool> IsRemoveAlbumSupportedMap => _collectionGroupBase.IsRemoveAlbumSupportedMap;

        /// <inheritdoc />
        public SynchronizedObservableCollection<bool> IsRemoveArtistSupportedMap => _collectionGroupBase.IsRemoveArtistSupportedMap;

        /// <inheritdoc />
        public SynchronizedObservableCollection<bool> IsRemoveChildSupportedMap => _collectionGroupBase.IsRemoveChildSupportedMap;

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
        public Task PopulateMorePlaylistsAsync(int limit) => _collectionGroupBase.PopulateMorePlaylistsAsync(limit);

        /// <inheritdoc />
        public Task PopulateMoreTracksAsync(int limit) => _collectionGroupBase.PopulateMoreTracksAsync(limit);

        /// <inheritdoc />
        public Task PopulateMoreAlbumsAsync(int limit) => _collectionGroupBase.PopulateMoreAlbumsAsync(limit);

        /// <inheritdoc />
        public Task PopulateMoreArtistsAsync(int limit) => _collectionGroupBase.PopulateMoreArtistsAsync(limit);

        /// <inheritdoc />
        public Task PopulateMoreChildrenAsync(int limit) => _collectionGroupBase.PopulateMoreChildrenAsync(limit);

        private void CollectionGroupBase_UrlChanged(object sender, Uri? e) => Url = e;

        private void CollectionGroupBase_NameChanged(object sender, string e) => Name = e;

        private void CollectionGroupBase_DescriptionChanged(object sender, string? e) => Description = e;

        private void CollectionGroupBase_PlaybackStateChanged(object sender, PlaybackState e) => PlaybackState = e;

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
