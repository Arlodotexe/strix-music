using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.Input;
using OwlCore.Collections;
using StrixMusic.Sdk.Core.Data;
using StrixMusic.Sdk.MediaPlayback;

namespace StrixMusic.Sdk.Core.ViewModels
{
    /// <summary>
    /// Contains bindable information about an <see cref="ICoreArtist"/>.
    /// </summary>
    public class ArtistViewModel : MergeableObjectViewModel<ICoreArtist>, ICoreArtist, IAlbumCollectionViewModel, ITrackCollectionViewModel
    {
        private readonly ICoreArtist _coreArtist;

        /// <summary>
        /// Initializes a new instance of the <see cref="ArtistViewModel"/> class.
        /// </summary>
        /// <param name="coreArtist">The <see cref="ICoreArtist"/> to wrap.</param>
        public ArtistViewModel(ICoreArtist coreArtist)
        {
            _coreArtist = coreArtist;

            SourceCore = MainViewModel.GetLoadedCore(_coreArtist.SourceCore);

            if (_coreArtist.RelatedItems != null)
                RelatedItems = new PlayableCollectionGroupViewModel(_coreArtist.RelatedItems);

            Tracks = new SynchronizedObservableCollection<TrackViewModel>();
            Albums = new SynchronizedObservableCollection<ICoreAlbumCollectionItem>();

            PlayAsyncCommand = new AsyncRelayCommand(PlayAsync);
            PauseAsyncCommand = new AsyncRelayCommand(PauseAsync);
            ChangeNameAsyncCommand = new AsyncRelayCommand<string>(ChangeNameAsync);
            ChangeDescriptionAsyncCommand = new AsyncRelayCommand<string?>(ChangeDescriptionAsync);
            ChangeDurationAsyncCommand = new AsyncRelayCommand<TimeSpan>(ChangeDurationAsync);
            PopulateMoreAlbumsCommand = new AsyncRelayCommand<int>(PopulateMoreAlbumsAsync);
            PopulateMoreTracksCommand = new AsyncRelayCommand<int>(PopulateMoreTracksAsync);

            AttachEvents();
        }

        private void AttachEvents()
        {
            _coreArtist.PlaybackStateChanged += CoreArtistPlaybackStateChanged;
            _coreArtist.DescriptionChanged += CoreArtistDescriptionChanged;
            _coreArtist.NameChanged += CoreArtistNameChanged;
            _coreArtist.UrlChanged += CoreArtistUrlChanged;
        }

        private void DetachEvents()
        {
            _coreArtist.PlaybackStateChanged -= CoreArtistPlaybackStateChanged;
            _coreArtist.DescriptionChanged -= CoreArtistDescriptionChanged;
            _coreArtist.NameChanged -= CoreArtistNameChanged;
            _coreArtist.UrlChanged -= CoreArtistUrlChanged;
        }

        /// <inheritdoc />
        public event EventHandler<TimeSpan>? DurationChanged
        {
            add => _coreArtist.DurationChanged += value;

            remove => _coreArtist.DurationChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<PlaybackState>? PlaybackStateChanged
        {
            add => _coreArtist.PlaybackStateChanged += value;

            remove => _coreArtist.PlaybackStateChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<string>? NameChanged
        {
            add => _coreArtist.NameChanged += value;

            remove => _coreArtist.NameChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<string?> DescriptionChanged
        {
            add => _coreArtist.DescriptionChanged += value;

            remove => _coreArtist.DescriptionChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<Uri?> UrlChanged
        {
            add => _coreArtist.UrlChanged += value;

            remove => _coreArtist.UrlChanged -= value;
        }

        private void CoreArtistUrlChanged(object sender, Uri? e) => Url = e;

        private void CoreArtistNameChanged(object sender, string e) => Name = e;

        private void CoreArtistDescriptionChanged(object sender, string? e) => Description = e;

        private void CoreArtistPlaybackStateChanged(object sender, PlaybackState e) => PlaybackState = e;

        /// <inheritdoc />
        public ICore SourceCore { get; }

        /// <inheritdoc />
        public string Id => _coreArtist.Id;

        /// <inheritdoc />
        public int TotalAlbumItemsCount => _coreArtist.TotalAlbumItemsCount;

        /// <inheritdoc />
        public int TotalTracksCount => _coreArtist.TotalTracksCount;

        /// <inheritdoc cref="IPlayable.Duration" />
        public TimeSpan Duration => _coreArtist.Duration;

        /// <inheritdoc />
        public IPlayableCollectionGroupBase? RelatedItems { get; }

        /// <summary>
        /// The artistViewModel's albums.
        /// </summary>
        public SynchronizedObservableCollection<ICoreAlbumCollectionItem> Albums { get; }

        /// <summary>
        /// The tracks released by this artistViewModel.
        /// </summary>
        public SynchronizedObservableCollection<TrackViewModel> Tracks { get; }

        /// <inheritdoc />
        public SynchronizedObservableCollection<ICoreImage> Images => _coreArtist.Images;

        /// <inheritdoc />
        public SynchronizedObservableCollection<string>? Genres => _coreArtist.Genres;

        /// <inheritdoc />
        public string Name
        {
            get => _coreArtist.Name;
            private set => SetProperty(() => _coreArtist.Name, value);
        }

        /// <inheritdoc />
        public Uri? Url
        {
            get => _coreArtist.Url;
            private set => SetProperty(() => _coreArtist.Url, value);
        }

        /// <inheritdoc />
        public string? Description
        {
            get => _coreArtist.Description;
            private set => SetProperty(() => _coreArtist.Description, value);
        }

        /// <inheritdoc />
        public PlaybackState PlaybackState
        {
            get => _coreArtist.PlaybackState;
            private set => SetProperty(() => _coreArtist.PlaybackState, value);
        }

        /// <inheritdoc />
        public bool IsPlayAsyncSupported
        {
            get => _coreArtist.IsPlayAsyncSupported;
            set => SetProperty(() => _coreArtist.IsPlayAsyncSupported, value);
        }

        /// <inheritdoc />
        public bool IsPauseAsyncSupported
        {
            get => _coreArtist.IsPauseAsyncSupported;
            set => SetProperty(() => _coreArtist.IsPauseAsyncSupported, value);
        }

        /// <inheritdoc />
        public bool IsChangeNameAsyncSupported
        {
            get => _coreArtist.IsChangeNameAsyncSupported;
            set => SetProperty(() => _coreArtist.IsChangeNameAsyncSupported, value);
        }

        /// <inheritdoc />
        public bool IsChangeDescriptionAsyncSupported
        {
            get => _coreArtist.IsChangeDescriptionAsyncSupported;
            set => SetProperty(() => _coreArtist.IsChangeDescriptionAsyncSupported, value);
        }

        /// <inheritdoc />
        public bool IsChangeDurationAsyncSupported
        {
            get => _coreArtist.IsChangeDurationAsyncSupported;
            set => SetProperty(() => _coreArtist.IsChangeDurationAsyncSupported, value);
        }

        /// <inheritdoc />
        public Task PlayAsync() => _coreArtist.PlayAsync();

        /// <inheritdoc />
        public Task PauseAsync() => _coreArtist.PauseAsync();

        /// <inheritdoc />
        public Task ChangeNameAsync(string name) => _coreArtist.ChangeNameAsync(name);

        /// <inheritdoc />
        public Task ChangeDescriptionAsync(string? description) => _coreArtist.ChangeDescriptionAsync(description);

        /// <inheritdoc />
        public Task ChangeDurationAsync(TimeSpan duration) => _coreArtist.ChangeDurationAsync(duration);

        /// <inheritdoc />
        public Task<bool> IsAddImageSupported(int index) => _coreArtist.IsAddImageSupported(index);

        /// <inheritdoc />
        public Task<bool> IsAddAlbumItemSupported(int index) => _coreArtist.IsAddAlbumItemSupported(index);

        /// <inheritdoc />
        public Task<bool> IsAddTrackSupported(int index) => _coreArtist.IsAddTrackSupported(index);

        /// <inheritdoc />
        public Task<bool> IsAddGenreSupported(int index) => _coreArtist.IsAddGenreSupported(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveImageSupported(int index) => _coreArtist.IsRemoveImageSupported(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveTrackSupported(int index) => _coreArtist.IsRemoveTrackSupported(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveAlbumItemSupported(int index) => _coreArtist.IsRemoveAlbumItemSupported(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveGenreSupported(int index) => _coreArtist.IsRemoveGenreSupported(index);

        /// <inheritdoc />
        public IAsyncEnumerable<ICoreAlbumCollectionItem> GetAlbumItemsAsync(int limit, int offset) => _coreArtist.GetAlbumItemsAsync(limit, offset);

        /// <inheritdoc />
        public IAsyncEnumerable<ICoreTrack> GetTracksAsync(int limit, int offset) => _coreArtist.GetTracksAsync(limit, offset);

        /// <inheritdoc />
        public async Task PopulateMoreAlbumsAsync(int limit)
        {
            await foreach (var item in _coreArtist.GetAlbumItemsAsync(limit, Albums.Count))
            {
                if (item is ICoreAlbum album)
                {
                    Albums.Add(new AlbumViewModel(album));
                }
            }

            OnPropertyChanged(nameof(TotalAlbumItemsCount));
        }

        /// <inheritdoc />
        public Task PopulateMoreTracksAsync(int limit)
        {
            // TODO
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task AddTrackAsync(ICoreTrack track, int index) => _coreArtist.AddTrackAsync(track, index);

        /// <inheritdoc />
        public Task AddAlbumItemAsync(ICoreAlbumCollectionItem album, int index) => _coreArtist.AddAlbumItemAsync(album, index);

        /// <inheritdoc />
        public Task RemoveTrackAsync(int index) => _coreArtist.RemoveTrackAsync(index);

        /// <inheritdoc />
        public Task RemoveAlbumItemAsync(int index) => _coreArtist.RemoveAlbumItemAsync(index);

        /// <inheritdoc />
        public IAsyncRelayCommand<int> PopulateMoreAlbumsCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand<int> PopulateMoreTracksCommand { get; }

        /// <summary>
        /// Attempts to play the artistViewModel.
        /// </summary>
        public IAsyncRelayCommand PlayAsyncCommand { get; }

        /// <summary>
        /// Attempts to pause the artistViewModel, if playing.
        /// </summary>
        public IAsyncRelayCommand PauseAsyncCommand { get; }

        /// <summary>
        /// Attempts to change the name of the artistViewModel, if supported.
        /// </summary>
        public IAsyncRelayCommand ChangeNameAsyncCommand { get; }

        /// <summary>
        /// Attempts to change the description of the artistViewModel, if supported.
        /// </summary>
        public IAsyncRelayCommand ChangeDescriptionAsyncCommand { get; }

        /// <summary>
        /// Attempts to change the duration of the artistViewModel, if supported.
        /// </summary>
        public IAsyncRelayCommand ChangeDurationAsyncCommand { get; }
    }
}
