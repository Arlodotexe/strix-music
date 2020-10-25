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
    /// Contains bindable information about an <see cref="IArtist"/>.
    /// </summary>
    public class ArtistViewModel : MergeableObjectViewModel<IArtist>, IArtist, IAlbumCollectionViewModel, ITrackCollectionViewModel
    {
        private readonly IArtist _artist;

        /// <summary>
        /// Initializes a new instance of the <see cref="ArtistViewModel"/> class.
        /// </summary>
        /// <param name="artist">The <see cref="IArtist"/> to wrap.</param>
        public ArtistViewModel(IArtist artist)
        {
            _artist = artist;

            SourceCore = MainViewModel.GetLoadedCore(_artist.SourceCore);

            if (_artist.RelatedItems != null)
                RelatedItems = new PlayableCollectionGroupViewModel(_artist.RelatedItems);

            Tracks = new SynchronizedObservableCollection<TrackViewModel>();
            Albums = new SynchronizedObservableCollection<IAlbumCollectionItem>();

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
            _artist.PlaybackStateChanged += Artist_PlaybackStateChanged;
            _artist.DescriptionChanged += Artist_DescriptionChanged;
            _artist.NameChanged += Artist_NameChanged;
            _artist.UrlChanged += Artist_UrlChanged;
        }

        private void DetachEvents()
        {
            _artist.PlaybackStateChanged -= Artist_PlaybackStateChanged;
            _artist.DescriptionChanged -= Artist_DescriptionChanged;
            _artist.NameChanged -= Artist_NameChanged;
            _artist.UrlChanged -= Artist_UrlChanged;
        }

        /// <inheritdoc />
        public event EventHandler<TimeSpan>? DurationChanged
        {
            add => _artist.DurationChanged += value;

            remove => _artist.DurationChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<PlaybackState>? PlaybackStateChanged
        {
            add => _artist.PlaybackStateChanged += value;

            remove => _artist.PlaybackStateChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<string>? NameChanged
        {
            add => _artist.NameChanged += value;

            remove => _artist.NameChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<string?> DescriptionChanged
        {
            add => _artist.DescriptionChanged += value;

            remove => _artist.DescriptionChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<Uri?> UrlChanged
        {
            add => _artist.UrlChanged += value;

            remove => _artist.UrlChanged -= value;
        }

        private void Artist_UrlChanged(object sender, Uri? e) => Url = e;

        private void Artist_NameChanged(object sender, string e) => Name = e;

        private void Artist_DescriptionChanged(object sender, string? e) => Description = e;

        private void Artist_PlaybackStateChanged(object sender, PlaybackState e) => PlaybackState = e;

        /// <inheritdoc />
        public ICore SourceCore { get; }

        /// <inheritdoc />
        public string Id => _artist.Id;

        /// <inheritdoc />
        public int TotalAlbumItemsCount => _artist.TotalAlbumItemsCount;

        /// <inheritdoc />
        public int TotalTracksCount => _artist.TotalTracksCount;

        /// <inheritdoc cref="IPlayable.Duration" />
        public TimeSpan Duration => _artist.Duration;

        /// <inheritdoc />
        public IPlayableCollectionGroup? RelatedItems { get; }

        /// <summary>
        /// The artistViewModel's albums.
        /// </summary>
        public SynchronizedObservableCollection<IAlbumCollectionItem> Albums { get; }

        /// <summary>
        /// The tracks released by this artistViewModel.
        /// </summary>
        public SynchronizedObservableCollection<TrackViewModel> Tracks { get; }

        /// <inheritdoc />
        public SynchronizedObservableCollection<IImage> Images => _artist.Images;

        /// <inheritdoc />
        public SynchronizedObservableCollection<string>? Genres => _artist.Genres;

        /// <inheritdoc />
        public string Name
        {
            get => _artist.Name;
            private set => SetProperty(() => _artist.Name, value);
        }

        /// <inheritdoc />
        public Uri? Url
        {
            get => _artist.Url;
            private set => SetProperty(() => _artist.Url, value);
        }

        /// <inheritdoc />
        public string? Description
        {
            get => _artist.Description;
            private set => SetProperty(() => _artist.Description, value);
        }

        /// <inheritdoc />
        public PlaybackState PlaybackState
        {
            get => _artist.PlaybackState;
            private set => SetProperty(() => _artist.PlaybackState, value);
        }

        /// <inheritdoc />
        public bool IsPlayAsyncSupported
        {
            get => _artist.IsPlayAsyncSupported;
            set => SetProperty(() => _artist.IsPlayAsyncSupported, value);
        }

        /// <inheritdoc />
        public bool IsPauseAsyncSupported
        {
            get => _artist.IsPauseAsyncSupported;
            set => SetProperty(() => _artist.IsPauseAsyncSupported, value);
        }

        /// <inheritdoc />
        public bool IsChangeNameAsyncSupported
        {
            get => _artist.IsChangeNameAsyncSupported;
            set => SetProperty(() => _artist.IsChangeNameAsyncSupported, value);
        }

        /// <inheritdoc />
        public bool IsChangeDescriptionAsyncSupported
        {
            get => _artist.IsChangeDescriptionAsyncSupported;
            set => SetProperty(() => _artist.IsChangeDescriptionAsyncSupported, value);
        }

        /// <inheritdoc />
        public bool IsChangeDurationAsyncSupported
        {
            get => _artist.IsChangeDurationAsyncSupported;
            set => SetProperty(() => _artist.IsChangeDurationAsyncSupported, value);
        }

        /// <inheritdoc />
        public Task PlayAsync() => _artist.PlayAsync();

        /// <inheritdoc />
        public Task PauseAsync() => _artist.PauseAsync();

        /// <inheritdoc />
        public Task ChangeNameAsync(string name) => _artist.ChangeNameAsync(name);

        /// <inheritdoc />
        public Task ChangeDescriptionAsync(string? description) => _artist.ChangeDescriptionAsync(description);

        /// <inheritdoc />
        public Task ChangeDurationAsync(TimeSpan duration) => _artist.ChangeDurationAsync(duration);

        /// <inheritdoc />
        public Task<bool> IsAddImageSupported(int index) => _artist.IsAddImageSupported(index);

        /// <inheritdoc />
        public Task<bool> IsAddAlbumItemSupported(int index) => _artist.IsAddAlbumItemSupported(index);

        /// <inheritdoc />
        public Task<bool> IsAddTrackSupported(int index) => _artist.IsAddTrackSupported(index);

        /// <inheritdoc />
        public Task<bool> IsAddGenreSupported(int index) => _artist.IsAddGenreSupported(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveImageSupported(int index) => _artist.IsRemoveImageSupported(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveTrackSupported(int index) => _artist.IsRemoveTrackSupported(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveAlbumItemSupported(int index) => _artist.IsRemoveAlbumItemSupported(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveGenreSupported(int index) => _artist.IsRemoveGenreSupported(index);

        /// <inheritdoc />
        public IAsyncEnumerable<IAlbumCollectionItem> GetAlbumItemsAsync(int limit, int offset) => _artist.GetAlbumItemsAsync(limit, offset);

        /// <inheritdoc />
        public IAsyncEnumerable<ITrack> GetTracksAsync(int limit, int offset) => _artist.GetTracksAsync(limit, offset);

        /// <inheritdoc />
        public async Task PopulateMoreAlbumsAsync(int limit)
        {
            await foreach (var item in _artist.GetAlbumItemsAsync(limit, Albums.Count))
            {
                if (item is IAlbum album)
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
        public Task AddTrackAsync(ITrack track, int index) => _artist.AddTrackAsync(track, index);

        /// <inheritdoc />
        public Task AddAlbumItemAsync(IAlbumCollectionItem album, int index) => _artist.AddAlbumItemAsync(album, index);

        /// <inheritdoc />
        public Task RemoveTrackAsync(int index) => _artist.RemoveTrackAsync(index);

        /// <inheritdoc />
        public Task RemoveAlbumItemAsync(int index) => _artist.RemoveAlbumItemAsync(index);

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
