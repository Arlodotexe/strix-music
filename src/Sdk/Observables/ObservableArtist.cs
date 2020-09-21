using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.Input;
using OwlCore.Collections;
using StrixMusic.Sdk.Enums;
using StrixMusic.Sdk.Interfaces;

namespace StrixMusic.Sdk.Observables
{
    /// <summary>
    /// Contains bindable information about an <see cref="IArtist"/>.
    /// </summary>
    public class ObservableArtist : ObservableMergeableObject<IArtist>, IArtist
    {
        private readonly IArtist _artist;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableArtist"/> class.
        /// </summary>
        /// <param name="artist">The <see cref="IArtist"/> to wrap.</param>
        public ObservableArtist(IArtist artist)
        {
            _artist = artist;

            SourceCore = MainViewModel.GetLoadedCore(_artist.SourceCore);

            if (_artist.RelatedItems != null)
                RelatedItems = new ObservableCollectionGroup(_artist.RelatedItems);

            Tracks = new SynchronizedObservableCollection<ITrack>(_artist.Tracks.Select(x => new ObservableTrack(x)));
            Albums = new SynchronizedObservableCollection<IAlbum>(_artist.Albums.Select(x => new ObservableAlbum(x)));
            Images = new SynchronizedObservableCollection<IImage>(_artist.Images);

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

        private void Artist_UrlChanged(object sender, Uri? e)
        {
            Url = e;
        }

        private void Artist_NameChanged(object sender, string e)
        {
            Name = e;
        }

        private void Artist_DescriptionChanged(object sender, string? e)
        {
            Description = e;
        }

        /// <inheritdoc />
        public event EventHandler<TimeSpan>? DurationChanged
        {
            add => _artist.DurationChanged += value;

            remove => _artist.DurationChanged -= value;
        }

        private void Artist_PlaybackStateChanged(object sender, PlaybackState e)
        {
            PlaybackState = e;
        }

        /// <inheritdoc />
        public SynchronizedObservableCollection<IAlbum> Albums { get; }

        /// <inheritdoc />
        public int TotalAlbumsCount => _artist.TotalAlbumsCount;

        /// <inheritdoc />
        public SynchronizedObservableCollection<ITrack> Tracks { get; }

        /// <inheritdoc />
        public int TotalTracksCount => _artist.TotalTracksCount;

        /// <inheritdoc />
        public Uri? Url
        {
            get => _artist.Url;
            private set => SetProperty(() => _artist.Url, value);
        }

        /// <inheritdoc />
        public ICore SourceCore { get; }

        /// <inheritdoc />
        public string Id => _artist.Id;

        /// <inheritdoc />
        public string Name
        {
            get => _artist.Name;
            private set => SetProperty(() => _artist.Name, value);
        }

        /// <inheritdoc />
        public SynchronizedObservableCollection<IImage> Images { get; }

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
        public SynchronizedObservableCollection<string>? Genres => _artist.Genres;

        /// <inheritdoc />
        public SynchronizedObservableCollection<bool> IsRemoveImageSupportedMap => _artist.IsRemoveImageSupportedMap;

        /// <inheritdoc />
        public SynchronizedObservableCollection<bool> IsRemoveAlbumSupportedMap => _artist.IsRemoveAlbumSupportedMap;

        /// <inheritdoc />
        public SynchronizedObservableCollection<bool> IsRemoveTrackSupportedMap => _artist.IsRemoveTrackSupportedMap;

        /// <inheritdoc />
        public SynchronizedObservableCollection<bool> IsRemoveGenreSupportedMap => _artist.IsRemoveGenreSupportedMap;

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

        /// <inheritdoc />
        public Task PlayAsync() => _artist.PlayAsync();

        /// <inheritdoc />
        public Task PauseAsync() => _artist.PauseAsync();

        /// <inheritdoc cref="IPlayable.Duration" />
        public TimeSpan Duration => _artist.Duration;

        /// <inheritdoc />
        public IPlayableCollectionGroup? RelatedItems { get; }

        /// <inheritdoc />
        public Task ChangeNameAsync(string name) => _artist.ChangeNameAsync(name);

        /// <inheritdoc />
        public Task ChangeDescriptionAsync(string? description) => _artist.ChangeDescriptionAsync(description);

        /// <inheritdoc />
        public Task ChangeDurationAsync(TimeSpan duration) => _artist.ChangeDurationAsync(duration);

        /// <inheritdoc />
        public Task<bool> IsAddImageSupported(int index) => _artist.IsAddImageSupported(index);

        /// <inheritdoc />
        public Task<bool> IsAddAlbumSupported(int index) => _artist.IsAddAlbumSupported(index);

        /// <inheritdoc />
        public Task<bool> IsAddTrackSupported(int index) => _artist.IsAddTrackSupported(index);

        /// <inheritdoc />
        public Task<bool> IsAddGenreSupported(int index) => _artist.IsAddGenreSupported(index);

        /// <inheritdoc />
        public IAsyncEnumerable<IAlbum> GetAlbumsAsync(int limit, int offset) => _artist.GetAlbumsAsync(limit, offset);

        /// <inheritdoc />
        public IAsyncEnumerable<ITrack> GetTracksAsync(int limit, int offset) => _artist.GetTracksAsync(limit, offset);

        /// <inheritdoc />
        public Task PopulateMoreAlbumsAsync(int limit) => _artist.PopulateMoreAlbumsAsync(limit);

        /// <inheritdoc />
        public Task PopulateMoreTracksAsync(int limit) => _artist.PopulateMoreTracksAsync(limit);

        /// <summary>
        /// <inheritdoc cref="PopulateMoreAlbumsAsync"/>
        /// </summary>
        public IAsyncRelayCommand<int> PopulateMoreAlbumsCommand { get; }

        /// <summary>
        /// <inheritdoc cref="PopulateMoreTracksAsync"/>
        /// </summary>
        public IAsyncRelayCommand<int> PopulateMoreTracksCommand { get; }

        /// <summary>
        /// Attempts to play the artist.
        /// </summary>
        public IAsyncRelayCommand PlayAsyncCommand { get; }

        /// <summary>
        /// Attempts to pause the artist, if playing.
        /// </summary>
        public IAsyncRelayCommand PauseAsyncCommand { get; }

        /// <summary>
        /// Attempts to change the name of the artist, if supported.
        /// </summary>
        public IAsyncRelayCommand ChangeNameAsyncCommand { get; }

        /// <summary>
        /// Attempts to change the description of the artist, if supported.
        /// </summary>
        public IAsyncRelayCommand ChangeDescriptionAsyncCommand { get; }

        /// <summary>
        /// Attempts to change the duration of the artist, if supported.
        /// </summary>
        public IAsyncRelayCommand ChangeDurationAsyncCommand { get; }
    }
}
