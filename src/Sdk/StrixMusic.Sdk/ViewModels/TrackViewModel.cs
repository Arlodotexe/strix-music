using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.Input;
using OwlCore.Collections;
using StrixMusic.Sdk.Core.Data;
using StrixMusic.Sdk.MediaPlayback;

namespace StrixMusic.Sdk.Core.ViewModels
{
    /// <summary>
    /// Contains bindable information about an <see cref="ICoreTrack"/>
    /// </summary>
    public class TrackViewModel : MergeableObjectViewModel<ICoreTrack>, ICoreTrack, IArtistCollectionViewModel
    {
        private ICoreAlbum? _album;

        /// <summary>
        /// Creates a bindable wrapper around an <see cref="ICoreTrack"/>.
        /// </summary>
        /// <param name="track">The <see cref="ICoreTrack"/> to wrap.</param>
        public TrackViewModel(ICoreTrack track)
        {
            Model = track;

            if (Model.Album != null)
                Album = new AlbumViewModel(Model.Album);

            if (Model.RelatedItems != null)
                RelatedItems = new PlayableCollectionGroupViewModel(Model.RelatedItems);

            Artists = new SynchronizedObservableCollection<IArtistCollectionItem>();
            SourceCore = MainViewModel.GetLoadedCore(Model.SourceCore);

            PlayAsyncCommand = new AsyncRelayCommand(PlayAsync);
            PauseAsyncCommand = new AsyncRelayCommand(PlayAsync);
            ChangeNameAsyncCommand = new AsyncRelayCommand<string>(ChangeNameAsync);
            ChangeDescriptionAsyncCommand = new AsyncRelayCommand<string?>(ChangeDescriptionAsync);
            ChangeDurationAsyncCommand = new AsyncRelayCommand<TimeSpan>(ChangeDurationAsync);
            PopulateMoreArtistsCommand = new AsyncRelayCommand<int>(PopulateMoreArtistsAsync);

            AttachEvents();
        }

        private void AttachEvents()
        {
            Model.AlbumChanged += Track_AlbumChanged;
            Model.DescriptionChanged += Track_DescriptionChanged;
            Model.IsExplicitChanged += Track_IsExplicitChanged;
            Model.LanguageChanged += Track_LanguageChanged;
            Model.LyricsChanged += Track_LyricsChanged;
            Model.NameChanged += Track_NameChanged;
            Model.PlaybackStateChanged += Track_PlaybackStateChanged;
            Model.TrackNumberChanged += Track_TrackNumberChanged;
            Model.UrlChanged += Track_UrlChanged;
        }

        private void DetachEvents()
        {
            Model.AlbumChanged -= Track_AlbumChanged;
            Model.DescriptionChanged -= Track_DescriptionChanged;
            Model.IsExplicitChanged -= Track_IsExplicitChanged;
            Model.LanguageChanged -= Track_LanguageChanged;
            Model.LyricsChanged -= Track_LyricsChanged;
            Model.NameChanged -= Track_NameChanged;
            Model.PlaybackStateChanged -= Track_PlaybackStateChanged;
            Model.TrackNumberChanged -= Track_TrackNumberChanged;
            Model.UrlChanged -= Track_UrlChanged;
        }

        /// <inheritdoc />
        public event EventHandler<PlaybackState>? PlaybackStateChanged
        {
            add => Model.PlaybackStateChanged += value;

            remove => Model.PlaybackStateChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<ICoreAlbum?> AlbumChanged
        {
            add => Model.AlbumChanged += value;

            remove => Model.AlbumChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<int?> TrackNumberChanged
        {
            add => Model.TrackNumberChanged += value;

            remove => Model.TrackNumberChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<CultureInfo?> LanguageChanged
        {
            add => Model.LanguageChanged += value;

            remove => Model.LanguageChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<ICoreLyrics?> LyricsChanged
        {
            add => Model.LyricsChanged += value;

            remove => Model.LyricsChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<bool> IsExplicitChanged
        {
            add => Model.IsExplicitChanged += value;

            remove => Model.IsExplicitChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<string> NameChanged
        {
            add => Model.NameChanged += value;

            remove => Model.NameChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<string?> DescriptionChanged
        {
            add => Model.DescriptionChanged += value;

            remove => Model.DescriptionChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<Uri?> UrlChanged
        {
            add => Model.UrlChanged += value;

            remove => Model.UrlChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<TimeSpan>? DurationChanged
        {
            add => Model.DurationChanged += value;

            remove => Model.DurationChanged -= value;
        }

        private void Track_UrlChanged(object sender, Uri? e) => Url = e;

        private void Track_TrackNumberChanged(object sender, int? e) => TrackNumber = e;

        private void Track_PlaybackStateChanged(object sender, PlaybackState e) => PlaybackState = e;

        private void Track_NameChanged(object sender, string e) => Name = e;

        private void Track_LyricsChanged(object sender, ICoreLyrics? e) => Lyrics = e;

        private void Track_LanguageChanged(object sender, CultureInfo? e) => Language = e;

        private void Track_IsExplicitChanged(object sender, bool e) => IsExplicit = e;

        private void Track_DescriptionChanged(object sender, string? e) => Description = e;

        private void Track_AlbumChanged(object sender, ICoreAlbum? e) => Album = e != null ? new AlbumViewModel(e) : null;

        /// <summary>
        /// The wrapped model for this <see cref="TrackViewModel"/>.
        /// </summary>
        internal ICoreTrack Model { get; }

        /// <inheritdoc />
        public ICore[] SourceCore { get; }

        /// <summary>
        /// The artists for this track.
        /// </summary>
        public SynchronizedObservableCollection<IArtistCollectionItem> Artists { get; }

        /// <inheritdoc />
        public SynchronizedObservableCollection<string>? Genres => Model.Genres;

        /// <inheritdoc />
        public SynchronizedObservableCollection<IImage> Images => Model.Images;

        /// <inheritdoc />
        public TrackType Type => Model.Type;

        /// <inheritdoc />
        public int TotalArtistItemsCount => Model.TotalArtistItemsCount;

        /// <inheritdoc />
        public TimeSpan Duration => Model.Duration;

        /// <inheritdoc />
        public IPlayableCollectionGroup? RelatedItems { get; }

        /// <inheritdoc />
        public string Id => Model.Id;

        /// <inheritdoc />
        public string Name
        {
            get => Model.Name;
            set => SetProperty(() => Model.Name, value);
        }

        /// <inheritdoc />
        public Uri? Url
        {
            get => Model.Url;
            set => SetProperty(() => Model.Url, value);
        }

        /// <inheritdoc />
        public ICoreAlbum? Album
        {
            get => _album;
            set => SetProperty(ref _album, value);
        }

        /// <inheritdoc />
        public int? TrackNumber
        {
            get => Model.TrackNumber;
            set => SetProperty(() => Model.TrackNumber, value);
        }

        /// <inheritdoc/>
        public int? DiscNumber => Model.DiscNumber;

        /// <inheritdoc />
        public CultureInfo? Language
        {
            get => Model.Language;
            set => SetProperty(() => Model.Language, value);
        }

        /// <inheritdoc />
        public ICoreLyrics? Lyrics
        {
            get => Model.Lyrics;
            set => SetProperty(() => Model.Lyrics, value);
        }

        /// <inheritdoc />
        public bool IsExplicit
        {
            get => Model.IsExplicit;
            set => SetProperty(() => Model.IsExplicit, value);
        }

        /// <inheritdoc />
        public string? Description
        {
            get => Model.Description;
            set => SetProperty(() => Model.Description, value);
        }

        /// <inheritdoc />
        public PlaybackState PlaybackState
        {
            get => Model.PlaybackState;
            set => SetProperty(() => Model.PlaybackState, value);
        }

        /// <inheritdoc />
        public bool IsPlayAsyncSupported
        {
            get => Model.IsPlayAsyncSupported;
            set => SetProperty(() => Model.IsPlayAsyncSupported, value);
        }

        /// <inheritdoc />
        public bool IsPauseAsyncSupported
        {
            get => Model.IsPauseAsyncSupported;
            set => SetProperty(() => Model.IsPauseAsyncSupported, value);
        }

        /// <inheritdoc />
        public bool IsChangeNameAsyncSupported
        {
            get => Model.IsChangeNameAsyncSupported;
            set => SetProperty(() => Model.IsChangeNameAsyncSupported, value);
        }

        /// <inheritdoc />
        public bool IsChangeDescriptionAsyncSupported
        {
            get => Model.IsChangeDescriptionAsyncSupported;
            set => SetProperty(() => Model.IsChangeDescriptionAsyncSupported, value);
        }

        /// <inheritdoc />
        public bool IsChangeDurationAsyncSupported
        {
            get => Model.IsChangeDurationAsyncSupported;
            set => SetProperty(() => Model.IsChangeDurationAsyncSupported, value);
        }

        /// <inheritdoc />
        public bool IsChangeAlbumAsyncSupported
        {
            get => Model.IsChangeAlbumAsyncSupported;
            set => SetProperty(() => Model.IsChangeAlbumAsyncSupported, value);
        }

        /// <inheritdoc />
        public bool IsChangeTrackNumberAsyncSupported
        {
            get => Model.IsChangeTrackNumberAsyncSupported;
            set => SetProperty(() => Model.IsChangeTrackNumberAsyncSupported, value);
        }

        /// <inheritdoc />
        public bool IsChangeLanguageAsyncSupported
        {
            get => Model.IsChangeLanguageAsyncSupported;
            set => SetProperty(() => Model.IsChangeLanguageAsyncSupported, value);
        }

        /// <inheritdoc />
        public bool IsChangeLyricsAsyncSupported
        {
            get => Model.IsChangeLyricsAsyncSupported;
            set => SetProperty(() => Model.IsChangeLyricsAsyncSupported, value);
        }

        /// <inheritdoc />
        public bool IsChangeIsExplicitAsyncSupported
        {
            get => Model.IsChangeIsExplicitAsyncSupported;
            set => SetProperty(() => Model.IsChangeIsExplicitAsyncSupported, value);
        }

        /// <inheritdoc />
        public Task<bool> IsAddArtistSupported(int index) => Model.IsAddArtistSupported(index);

        /// <inheritdoc />
        public Task<bool> IsAddGenreSupported(int index) => Model.IsAddGenreSupported(index);

        /// <inheritdoc />
        public Task<bool> IsAddImageSupported(int index) => Model.IsAddImageSupported(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveImageSupported(int index) => Model.IsRemoveImageSupported(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveGenreSupported(int index) => Model.IsRemoveGenreSupported(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveArtistSupported(int index) => Model.IsRemoveArtistSupported(index);

        /// <inheritdoc />
        public Task ChangeAlbumAsync(ICoreAlbum? albums) => Model.ChangeAlbumAsync(albums);

        /// <inheritdoc />
        public Task ChangeTrackNumberAsync(int? trackNumber) => Model.ChangeTrackNumberAsync(trackNumber);

        /// <inheritdoc />
        public Task ChangeLanguageAsync(CultureInfo language) => Model.ChangeLanguageAsync(language);

        /// <inheritdoc />
        public Task ChangeLyricsAsync(ICoreLyrics? lyrics) => Model.ChangeLyricsAsync(lyrics);

        /// <inheritdoc />
        public Task ChangeIsExplicitAsync(bool isExplicit) => Model.ChangeIsExplicitAsync(isExplicit);

        /// <inheritdoc />
        public Task PauseAsync() => Model.PauseAsync();

        /// <inheritdoc />
        public Task PlayAsync() => Model.PlayAsync();

        /// <inheritdoc />
        public Task ChangeNameAsync(string name) => Model.ChangeNameAsync(name);

        /// <inheritdoc />
        public Task ChangeDescriptionAsync(string? description) => Model.ChangeDescriptionAsync(description);

        /// <inheritdoc />
        public Task ChangeDurationAsync(TimeSpan duration) => Model.ChangeDurationAsync(duration);

        /// <inheritdoc />
        public IAsyncEnumerable<IArtistCollectionItem> GetArtistsAsync(int limit, int offset) => Model.GetArtistsAsync(limit, offset);

        /// <inheritdoc />
        public Task AddArtistItemAsync(IArtistCollectionItem artist, int index) => Model.AddArtistItemAsync(artist, index);

        /// <inheritdoc />
        public Task RemoveArtistAsync(int index) => Model.RemoveArtistAsync(index);

        /// <inheritdoc />
        public Task PopulateMoreArtistsAsync(int limit)
        {
            // TODO
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public IAsyncRelayCommand<int> PopulateMoreArtistsCommand { get; }

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
    }
}
