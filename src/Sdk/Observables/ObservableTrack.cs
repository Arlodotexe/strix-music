using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.Input;
using OwlCore.Collections;
using StrixMusic.Sdk.Enums;
using StrixMusic.Sdk.Interfaces;

namespace StrixMusic.Sdk.Observables
{
    /// <summary>
    /// Contains bindable information about an <see cref="ITrack"/>
    /// </summary>
    public class ObservableTrack : ObservableMergeableObject<ITrack>, ITrack
    {
        private readonly ITrack _track;
        private IAlbum? _album;

        /// <summary>
        /// Creates a bindable wrapper around an <see cref="ITrack"/>.
        /// </summary>
        /// <param name="track">The <see cref="ITrack"/> to wrap.</param>
        public ObservableTrack(ITrack track)
        {
            _track = track;

            if (_track.Album != null)
                Album = new ObservableAlbum(_track.Album);

            if (_track.RelatedItems != null)
                RelatedItems = new ObservableCollectionGroup(_track.RelatedItems);

            Genres = new SynchronizedObservableCollection<string>(_track.Genres);
            Artists = new SynchronizedObservableCollection<IArtist>(_track.Artists.Select(x => new ObservableArtist(x)));
            Images = new SynchronizedObservableCollection<IImage>(_track.Images);
            SourceCore = MainViewModel.GetLoadedCore(_track.SourceCore);

            PlayAsyncCommand = new AsyncRelayCommand(PlayAsync);
            PauseAsyncCommand = new AsyncRelayCommand(PlayAsync);
            ChangeNameAsyncCommand = new AsyncRelayCommand<string>(ChangeNameAsync);
            ChangeDescriptionAsyncCommand = new AsyncRelayCommand<string?>(ChangeDescriptionAsync);
            ChangeDurationAsyncCommand = new AsyncRelayCommand<TimeSpan>(ChangeDurationAsync);

            AttachEvents();
        }

        private void AttachEvents()
        {
            _track.AlbumChanged += Track_AlbumChanged;
            _track.DescriptionChanged += Track_DescriptionChanged;
            _track.IsExplicitChanged += Track_IsExplicitChanged;
            _track.LanguageChanged += Track_LanguageChanged;
            _track.LyricsChanged += Track_LyricsChanged;
            _track.NameChanged += Track_NameChanged;
            _track.PlaybackStateChanged += Track_PlaybackStateChanged;
            _track.TrackNumberChanged += Track_TrackNumberChanged;
            _track.UrlChanged += Track_UrlChanged;
        }

        private void DetachEvents()
        {
            _track.AlbumChanged -= Track_AlbumChanged;
            _track.DescriptionChanged -= Track_DescriptionChanged;
            _track.IsExplicitChanged -= Track_IsExplicitChanged;
            _track.LanguageChanged -= Track_LanguageChanged;
            _track.LyricsChanged -= Track_LyricsChanged;
            _track.NameChanged -= Track_NameChanged;
            _track.PlaybackStateChanged -= Track_PlaybackStateChanged;
            _track.TrackNumberChanged -= Track_TrackNumberChanged;
            _track.UrlChanged -= Track_UrlChanged;
        }

        private void Track_UrlChanged(object sender, Uri? e) => Url = e;

        private void Track_TrackNumberChanged(object sender, int? e) => TrackNumber = e;

        private void Track_PlaybackStateChanged(object sender, PlaybackState e) => PlaybackState = e;

        private void Track_NameChanged(object sender, string e) => Name = e;

        private void Track_LyricsChanged(object sender, ILyrics? e) => Lyrics = e;

        private void Track_LanguageChanged(object sender, CultureInfo? e) => Language = e;

        private void Track_IsExplicitChanged(object sender, bool e) => IsExplicit = e;

        private void Track_DescriptionChanged(object sender, string? e) => Description = e;

        private void Track_AlbumChanged(object sender, IAlbum? e) => Album = e != null ? new ObservableAlbum(e) : null;

        /// <inheritdoc />
        public ICore SourceCore { get; }

        /// <inheritdoc />
        public SynchronizedObservableCollection<IArtist> Artists { get; }

        /// <inheritdoc />
        public SynchronizedObservableCollection<string> Genres { get; }

        /// <inheritdoc />
        public SynchronizedObservableCollection<IImage> Images { get; }

        /// <inheritdoc />
        public TrackType Type => _track.Type;

        /// <inheritdoc />
        public int TotalArtistsCount => _track.TotalArtistsCount;

        /// <inheritdoc />
        public TimeSpan Duration => _track.Duration;

        /// <inheritdoc />
        public IPlayableCollectionGroup? RelatedItems { get; }

        /// <inheritdoc />
        public string Id => _track.Id;

        /// <inheritdoc />
        public string Name
        {
            get => _track.Name;
            set => SetProperty(() => _track.Name, value);
        }

        /// <inheritdoc />
        public Uri? Url
        {
            get => _track.Url;
            set => SetProperty(() => _track.Url, value);
        }

        /// <inheritdoc />
        public IAlbum? Album
        {
            get => _album;
            set => SetProperty(ref _album, value);
        }

        /// <inheritdoc />
        public int? TrackNumber
        {
            get => _track.TrackNumber;
            set => SetProperty(() => _track.TrackNumber, value);
        }

        /// <inheritdoc />
        public CultureInfo? Language
        {
            get => _track.Language;
            set => SetProperty(() => _track.Language, value);
        }

        /// <inheritdoc />
        public ILyrics? Lyrics
        {
            get => _track.Lyrics;
            set => SetProperty(() => _track.Lyrics, value);
        }

        /// <inheritdoc />
        public bool IsExplicit
        {
            get => _track.IsExplicit;
            set => SetProperty(() => _track.IsExplicit, value);
        }

        /// <inheritdoc />
        public string? Description
        {
            get => _track.Description;
            set => SetProperty(() => _track.Description, value);
        }

        /// <inheritdoc />
        public PlaybackState PlaybackState
        {
            get => _track.PlaybackState;
            set => SetProperty(() => _track.PlaybackState, value);
        }

        /// <inheritdoc />
        public bool IsPlayAsyncSupported
        {
            get => _track.IsPlayAsyncSupported;
            set => SetProperty(() => _track.IsPlayAsyncSupported, value);
        }

        /// <inheritdoc />
        public bool IsPauseAsyncSupported
        {
            get => _track.IsPauseAsyncSupported;
            set => SetProperty(() => _track.IsPauseAsyncSupported, value);
        }

        /// <inheritdoc />
        public bool IsChangeNameAsyncSupported
        {
            get => _track.IsChangeNameAsyncSupported;
            set => SetProperty(() => _track.IsChangeNameAsyncSupported, value);
        }

        /// <inheritdoc />
        public bool IsChangeDescriptionAsyncSupported
        {
            get => _track.IsChangeDescriptionAsyncSupported;
            set => SetProperty(() => _track.IsChangeDescriptionAsyncSupported, value);
        }

        /// <inheritdoc />
        public bool IsChangeDurationAsyncSupported
        {
            get => _track.IsChangeDurationAsyncSupported;
            set => SetProperty(() => _track.IsChangeDurationAsyncSupported, value);
        }

        /// <inheritdoc />
        public bool IsChangeAlbumAsyncSupported
        {
            get => _track.IsChangeAlbumAsyncSupported;
            set => SetProperty(() => _track.IsChangeAlbumAsyncSupported, value);
        }

        /// <inheritdoc />
        public bool IsChangeTrackNumberAsyncSupported
        {
            get => _track.IsChangeTrackNumberAsyncSupported;
            set => SetProperty(() => _track.IsChangeTrackNumberAsyncSupported, value);
        }

        /// <inheritdoc />
        public bool IsChangeLanguageAsyncSupported
        {
            get => _track.IsChangeLanguageAsyncSupported;
            set => SetProperty(() => _track.IsChangeLanguageAsyncSupported, value);
        }

        /// <inheritdoc />
        public bool IsChangeLyricsAsyncSupported
        {
            get => _track.IsChangeLyricsAsyncSupported;
            set => SetProperty(() => _track.IsChangeLyricsAsyncSupported, value);
        }

        /// <inheritdoc />
        public bool IsChangeIsExplicitAsyncSupported
        {
            get => _track.IsChangeIsExplicitAsyncSupported;
            set => SetProperty(() => _track.IsChangeIsExplicitAsyncSupported, value);
        }

        /// <inheritdoc />
        public SynchronizedObservableCollection<bool> IsRemoveImageSupportedMap => _track.IsRemoveImageSupportedMap;

        /// <inheritdoc />
        public SynchronizedObservableCollection<bool> IsRemoveArtistSupportedMap => _track.IsRemoveArtistSupportedMap;

        /// <inheritdoc />
        public SynchronizedObservableCollection<bool> IsRemoveGenreSupportedMap => _track.IsRemoveGenreSupportedMap;

        /// <inheritdoc />
        public Task<bool> IsAddArtistSupported(int index) => _track.IsAddArtistSupported(index);

        /// <inheritdoc />
        public Task<bool> IsAddGenreSupported(int index) => _track.IsAddGenreSupported(index);

        /// <inheritdoc />
        public Task<bool> IsAddImageSupported(int index) => _track.IsAddImageSupported(index);

        /// <inheritdoc />
        public Task ChangeAlbumAsync(IAlbum? albums) => _track.ChangeAlbumAsync(albums);

        /// <inheritdoc />
        public Task ChangeTrackNumberAsync(int? trackNumber) => _track.ChangeTrackNumberAsync(trackNumber);

        /// <inheritdoc />
        public Task ChangeLanguageAsync(CultureInfo language) => _track.ChangeLanguageAsync(language);

        /// <inheritdoc />
        public Task ChangeLyricsAsync(ILyrics? lyrics) => _track.ChangeLyricsAsync(lyrics);

        /// <inheritdoc />
        public Task ChangeIsExplicitAsync(bool isExplicit) => _track.ChangeIsExplicitAsync(isExplicit);

        /// <inheritdoc />
        public Task PauseAsync() => _track.PauseAsync();

        /// <inheritdoc />
        public Task PlayAsync() => _track.PlayAsync();

        /// <inheritdoc />
        public Task ChangeNameAsync(string name) => _track.ChangeNameAsync(name);

        /// <inheritdoc />
        public Task ChangeDescriptionAsync(string? description) => _track.ChangeDescriptionAsync(description);

        /// <inheritdoc />
        public Task ChangeDurationAsync(TimeSpan duration) => _track.ChangeDurationAsync(duration);

        /// <inheritdoc />
        public event EventHandler<PlaybackState>? PlaybackStateChanged
        {
            add => _track.PlaybackStateChanged += value;

            remove => _track.PlaybackStateChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<IAlbum?> AlbumChanged
        {
            add => _track.AlbumChanged += value;

            remove => _track.AlbumChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<int?> TrackNumberChanged
        {
            add => _track.TrackNumberChanged += value;

            remove => _track.TrackNumberChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<CultureInfo?> LanguageChanged
        {
            add => _track.LanguageChanged += value;

            remove => _track.LanguageChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<ILyrics?> LyricsChanged
        {
            add => _track.LyricsChanged += value;

            remove => _track.LyricsChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<bool> IsExplicitChanged
        {
            add => _track.IsExplicitChanged += value;

            remove => _track.IsExplicitChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<string> NameChanged
        {
            add => _track.NameChanged += value;

            remove => _track.NameChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<string?> DescriptionChanged
        {
            add => _track.DescriptionChanged += value;

            remove => _track.DescriptionChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<Uri?> UrlChanged
        {
            add => _track.UrlChanged += value;

            remove => _track.UrlChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<TimeSpan>? DurationChanged
        {
            add => _track.DurationChanged += value;

            remove => _track.DurationChanged -= value;
        }

        /// <summary>
        /// Attempts to play the track.
        /// </summary>
        public IAsyncRelayCommand PlayAsyncCommand { get; }

        /// <summary>
        /// Attempts to pause the track, if playing.
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
        public Task PopulateMoreArtistsAsync(int limit) => _track.PopulateMoreArtistsAsync(limit);

        /// <inheritdoc />
        public IAsyncEnumerable<IArtist> GetArtistsAsync(int limit, int offset) => _track.GetArtistsAsync(limit, offset);
    }
}
