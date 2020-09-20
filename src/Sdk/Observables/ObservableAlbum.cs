using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.Input;
using StrixMusic.Sdk.Enums;
using StrixMusic.Sdk.Interfaces;

namespace StrixMusic.Sdk.Observables
{
    /// <summary>
    /// Contains bindable information about an <see cref="IAlbum"/>.
    /// </summary>
    public class ObservableAlbum : ObservableMergeableObject<IAlbum>, IAlbum
    {
        private readonly IAlbum _album;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableAlbum"/> class.
        /// </summary>
        /// <param name="album"><inheritdoc cref="IAlbum"/></param>
        public ObservableAlbum(IAlbum album)
        {
            _album = album;

            SourceCore = MainViewModel.GetLoadedCore(_album.SourceCore);

            Images = new ObservableCollection<IImage>(_album.Images);
            Tracks = new ObservableCollection<ITrack>(_album.Tracks.Select(x => new ObservableTrack(x)));

            if (_album.RelatedItems != null)
                RelatedItems = new ObservableCollectionGroup(_album.RelatedItems);

            _artist = new ObservableArtist(_album.Artist);

            PauseAsyncCommand = new AsyncRelayCommand(PauseAsync);
            PlayAsyncCommand = new AsyncRelayCommand(PlayAsync);
            ChangeNameAsyncCommand = new AsyncRelayCommand<string>(ChangeNameAsync);
            ChangeDescriptionAsyncCommand = new AsyncRelayCommand<string?>(ChangeDescriptionAsync);
            ChangeDurationAsyncCommand = new AsyncRelayCommand<TimeSpan>(ChangeDurationAsync);
            PopulateMoreTracksCommand = new AsyncRelayCommand<int>(PopulateMoreTracksAsync);

            AttachEvents();
        }

        private void AttachEvents()
        {
            _album.PlaybackStateChanged += Album_PlaybackStateChanged;
            _album.DescriptionChanged += Album_DescriptionChanged;
            _album.DatePublishedChanged += Album_DatePublishedChanged;
            _album.NameChanged += Album_NameChanged;
            _album.UrlChanged += Album_UrlChanged;
        }

        private void DetachEvents()
        {
            _album.PlaybackStateChanged -= Album_PlaybackStateChanged;
            _album.DescriptionChanged -= Album_DescriptionChanged;
            _album.DatePublishedChanged -= Album_DatePublishedChanged;
            _album.NameChanged -= Album_NameChanged;
            _album.UrlChanged -= Album_UrlChanged;
        }

        private void Album_UrlChanged(object sender, Uri? e)
        {
            Url = e;
        }

        private void Album_NameChanged(object sender, string e)
        {
            Name = e;
        }

        private void Album_DescriptionChanged(object sender, string? e)
        {
            Description = e;
        }

        private void Album_PlaybackStateChanged(object sender, PlaybackState e)
        {
            PlaybackState = e;
        }

        private void Album_DatePublishedChanged(object sender, DateTime? e)
        {
            DatePublished = e;
        }

        /// <inheritdoc />
        public ObservableCollection<ITrack> Tracks { get; }

        /// <inheritdoc />
        public int TotalTracksCount => _album.TotalTracksCount;

        private IArtist _artist;

        /// <inheritdoc />
        public IArtist Artist
        {
            get => _artist;
            set => SetProperty(ref _artist, value);
        }

        /// <inheritdoc />
        public string Id => _album.Id;

        /// <inheritdoc />
        public ICore SourceCore { get; }

        /// <inheritdoc />
        public TimeSpan Duration => _album.Duration;

        /// <inheritdoc />
        public string Name
        {
            get => _album.Name;
            private set => SetProperty(() => _album.Name, value);
        }

        /// <inheritdoc />
        public ObservableCollection<IImage> Images { get; }

        /// <inheritdoc />
        public Uri? Url
        {
            get => _album.Url;
            private set => SetProperty(() => _album.Url, value);
        }

        /// <inheritdoc />
        public DateTime? DatePublished
        {
            get => _album.DatePublished;
            set => SetProperty(() => _album.DatePublished, value);
        }

        /// <inheritdoc />
        public string? Description
        {
            get => _album.Description;
            private set => SetProperty(() => _album.Description, value);
        }

        /// <inheritdoc />
        public PlaybackState PlaybackState
        {
            get => _album.PlaybackState;
            private set => SetProperty(() => _album.PlaybackState, value);
        }

        /// <inheritdoc />
        public ObservableCollection<string>? Genres => _album.Genres;

        /// <inheritdoc />
        public bool IsPlayAsyncSupported
        {
            get => _album.IsPlayAsyncSupported;
            set => SetProperty(() => _album.IsPlayAsyncSupported, value);
        }

        /// <inheritdoc />
        public bool IsPauseAsyncSupported
        {
            get => _album.IsPauseAsyncSupported;
            set => SetProperty(() => _album.IsPauseAsyncSupported, value);
        }

        /// <inheritdoc />
        public bool IsChangeNameAsyncSupported
        {
            get => _album.IsChangeNameAsyncSupported;
            set => SetProperty(() => _album.IsChangeNameAsyncSupported, value);
        }

        /// <inheritdoc />
        public bool IsChangeDescriptionAsyncSupported
        {
            get => _album.IsChangeDescriptionAsyncSupported;
            set => SetProperty(() => _album.IsChangeDescriptionAsyncSupported, value);
        }

        /// <inheritdoc />
        public bool IsChangeDatePublishedAsyncSupported
        {
            get => _album.IsChangeDatePublishedAsyncSupported;
            set => SetProperty(() => _album.IsChangeDatePublishedAsyncSupported, value);
        }

        /// <inheritdoc />
        public bool IsChangeDurationAsyncSupported
        {
            get => _album.IsChangeDurationAsyncSupported;
            set => SetProperty(() => _album.IsChangeDurationAsyncSupported, value);
        }

        /// <inheritdoc />
        public ObservableCollection<bool> IsRemoveImageSupportedMap => _album.IsRemoveImageSupportedMap;

        /// <inheritdoc />
        public ObservableCollection<bool> IsRemoveGenreSupportedMap => _album.IsRemoveGenreSupportedMap;

        /// <inheritdoc />
        public ObservableCollection<bool> IsRemoveTrackSupportedMap => _album.IsRemoveTrackSupportedMap;

        /// <inheritdoc />
        public event EventHandler<PlaybackState>? PlaybackStateChanged
        {
            add => _album.PlaybackStateChanged += value;

            remove => _album.PlaybackStateChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<string>? NameChanged
        {
            add => _album.NameChanged += value;

            remove => _album.NameChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<string?> DescriptionChanged
        {
            add => _album.DescriptionChanged += value;

            remove => _album.DescriptionChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<Uri?> UrlChanged
        {
            add => _album.UrlChanged += value;

            remove => _album.UrlChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<TimeSpan>? DurationChanged
        {
            add => _album.DurationChanged += value;

            remove => _album.DurationChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<DateTime?> DatePublishedChanged
        {
            add => _album.DatePublishedChanged += value;

            remove => _album.DatePublishedChanged -= value;
        }

        /// <inheritdoc />
        public Task PauseAsync() => _album.PauseAsync();

        /// <inheritdoc />
        public IPlayableCollectionGroup? RelatedItems { get; }

        /// <inheritdoc />
        public Task PlayAsync() => _album.PlayAsync();

        /// <inheritdoc />
        public Task ChangeNameAsync(string name) => _album.ChangeNameAsync(name);

        /// <inheritdoc />
        public Task ChangeDescriptionAsync(string? description) => _album.ChangeDescriptionAsync(description);

        /// <inheritdoc />
        public Task ChangeDurationAsync(TimeSpan duration) => _album.ChangeDurationAsync(duration);

        /// <inheritdoc />
        public Task<bool> IsAddImageSupported(int index) => _album.IsAddImageSupported(index);

        /// <inheritdoc />
        public Task<bool> IsAddTrackSupported(int index) => _album.IsAddTrackSupported(index);

        /// <inheritdoc />
        public Task<bool> IsAddGenreSupported(int index) => _album.IsAddGenreSupported(index);

        /// <inheritdoc />
        public Task ChangeDatePublishedAsync(DateTime datePublished) => _album.ChangeDatePublishedAsync(datePublished);

        /// <inheritdoc />
        public IAsyncEnumerable<ITrack> GetTracksAsync(int limit, int offset) => _album.GetTracksAsync(limit, offset);

        /// <inheritdoc />
        public Task PopulateMoreTracksAsync(int limit) => _album.PopulateMoreTracksAsync(limit);

        /// <summary>
        /// <inheritdoc cref="PopulateMoreTracksAsync"/>
        /// </summary>
        public IAsyncRelayCommand<int> PopulateMoreTracksCommand { get; }

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
    }
}
