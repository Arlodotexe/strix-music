using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Toolkit.Diagnostics;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Input;
using OwlCore;
using OwlCore.Collections;
using OwlCore.Events;
using StrixMusic.Sdk.Data;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.Data.Merged;
using StrixMusic.Sdk.Extensions;
using StrixMusic.Sdk.MediaPlayback;
using StrixMusic.Sdk.MediaPlayback.LocalDevice;
using StrixMusic.Sdk.Services.MediaPlayback;

namespace StrixMusic.Sdk.ViewModels
{
    /// <summary>
    /// Contains bindable information about an <see cref="ITrack"/>
    /// </summary>
    public class TrackViewModel : ObservableObject, ITrack, IArtistCollectionViewModel, IImageCollectionViewModel
    {
        private readonly IPlaybackHandlerService _playbackHandler;

        /// <summary>
        /// Creates a bindable wrapper around an <see cref="ITrack"/>.
        /// </summary>
        /// <param name="track">The <see cref="ITrack"/> to wrap.</param>
        public TrackViewModel(ITrack track)
        {
            Model = track;

            SourceCores = Model.GetSourceCores<ICoreTrack>().Select(MainViewModel.GetLoadedCore).ToList();

            _playbackHandler = Ioc.Default.GetRequiredService<IPlaybackHandlerService>();

            if (Model.Album != null)
                Album = new AlbumViewModel(Model.Album);

            if (Model.RelatedItems != null)
                RelatedItems = new PlayableCollectionGroupViewModel(Model.RelatedItems);

            using (Threading.PrimaryContext)
            {
                Images = new SynchronizedObservableCollection<IImage>();
                Artists = new SynchronizedObservableCollection<IArtistCollectionItem>();
            }

            PlayAsyncCommand = new AsyncRelayCommand(PlayAsync);
            PauseAsyncCommand = new AsyncRelayCommand(PauseAsync);
            ChangeNameAsyncCommand = new AsyncRelayCommand<string>(ChangeNameAsync);
            ChangeDescriptionAsyncCommand = new AsyncRelayCommand<string?>(ChangeDescriptionAsync);
            ChangeDurationAsyncCommand = new AsyncRelayCommand<TimeSpan>(ChangeDurationAsync);

            PopulateMoreArtistsCommand = new AsyncRelayCommand<int>(PopulateMoreArtistsAsync);
            PopulateMoreImagesCommand = new AsyncRelayCommand<int>(PopulateMoreImagesAsync);

            AttachEvents();
        }

        private void AttachEvents()
        {
            AlbumChanged += Track_AlbumChanged;
            DescriptionChanged += Track_DescriptionChanged;
            IsExplicitChanged += Track_IsExplicitChanged;
            LanguageChanged += Track_LanguageChanged;
            LyricsChanged += Track_LyricsChanged;
            NameChanged += Track_NameChanged;
            PlaybackStateChanged += Track_PlaybackStateChanged;
            TrackNumberChanged += Track_TrackNumberChanged;
            UrlChanged += Track_UrlChanged;
            LastPlayedChanged += OnLastPlayedChanged;

            ArtistItemsCountChanged += ModelOnArtistItemsCountChanged;
            ArtistItemsChanged += TrackViewModel_ArtistItemsChanged;
            ImagesCountChanged += TrackViewModel_ImagesCountChanged;
            ImagesChanged += TrackViewModel_ImagesChanged;
        }

        private void DetachEvents()
        {
            AlbumChanged -= Track_AlbumChanged;
            DescriptionChanged -= Track_DescriptionChanged;
            IsExplicitChanged -= Track_IsExplicitChanged;
            LanguageChanged -= Track_LanguageChanged;
            LyricsChanged -= Track_LyricsChanged;
            NameChanged -= Track_NameChanged;
            PlaybackStateChanged -= Track_PlaybackStateChanged;
            TrackNumberChanged -= Track_TrackNumberChanged;
            UrlChanged -= Track_UrlChanged;
            LastPlayedChanged -= OnLastPlayedChanged;

            ArtistItemsCountChanged += ModelOnArtistItemsCountChanged;
        }

        /// <inheritdoc />
        public event EventHandler<PlaybackState>? PlaybackStateChanged
        {
            add => Model.PlaybackStateChanged += value;

            remove => Model.PlaybackStateChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<IAlbum?>? AlbumChanged
        {
            add => Model.AlbumChanged += value;

            remove => Model.AlbumChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<int?>? TrackNumberChanged
        {
            add => Model.TrackNumberChanged += value;

            remove => Model.TrackNumberChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<CultureInfo?>? LanguageChanged
        {
            add => Model.LanguageChanged += value;

            remove => Model.LanguageChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<ILyrics?>? LyricsChanged
        {
            add => Model.LyricsChanged += value;

            remove => Model.LyricsChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<bool>? IsExplicitChanged
        {
            add => Model.IsExplicitChanged += value;

            remove => Model.IsExplicitChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<string>? NameChanged
        {
            add => Model.NameChanged += value;

            remove => Model.NameChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<string?>? DescriptionChanged
        {
            add => Model.DescriptionChanged += value;

            remove => Model.DescriptionChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<Uri?>? UrlChanged
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

        /// <inheritdoc />
        public event EventHandler<DateTime?>? LastPlayedChanged
        {
            add => Model.LastPlayedChanged += value;

            remove => Model.LastPlayedChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<int>? ArtistItemsCountChanged
        {
            add => Model.ArtistItemsCountChanged += value;
            remove => Model.ArtistItemsCountChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<int>? ImagesCountChanged
        {
            add => Model.ImagesCountChanged += value;
            remove => Model.ImagesCountChanged -= value;
        }

        /// <inheritdoc />
        public event CollectionChangedEventHandler<IImage>? ImagesChanged
        {
            add => Model.ImagesChanged += value;
            remove => Model.ImagesChanged -= value;
        }

        /// <inheritdoc />
        public event CollectionChangedEventHandler<IArtistCollectionItem>? ArtistItemsChanged
        {
            add => Model.ArtistItemsChanged += value;
            remove => Model.ArtistItemsChanged -= value;
        }

        private void Track_UrlChanged(object sender, Uri? e) => OnPropertyChanged(nameof(Url));

        private void Track_TrackNumberChanged(object sender, int? e) => OnPropertyChanged(nameof(TrackNumber));

        private void Track_PlaybackStateChanged(object sender, PlaybackState e) => OnPropertyChanged(nameof(PlaybackState));

        private void Track_NameChanged(object sender, string e) => OnPropertyChanged(nameof(Name));

        private void Track_LyricsChanged(object sender, ILyrics? e) => OnPropertyChanged(nameof(Lyrics));

        private void Track_LanguageChanged(object sender, CultureInfo? e) => OnPropertyChanged(nameof(Language));

        private void Track_IsExplicitChanged(object sender, bool e) => OnPropertyChanged(nameof(IsExplicit));

        private void Track_DescriptionChanged(object sender, string? e) => OnPropertyChanged(nameof(Description));

        private void OnLastPlayedChanged(object sender, DateTime? e) => OnPropertyChanged(nameof(LastPlayed));

        private void Track_AlbumChanged(object sender, IAlbum? e)
        {
            Album = e is null ? null : new AlbumViewModel(e);
            OnPropertyChanged(nameof(Album));
        }

        private void ModelOnArtistItemsCountChanged(object sender, int e) => OnPropertyChanged(nameof(TotalArtistItemsCount));

        private void TrackViewModel_ImagesCountChanged(object sender, int e) => OnPropertyChanged(nameof(TotalImageCount));

        private void TrackViewModel_ImagesChanged(object sender, IReadOnlyList<CollectionChangedEventItem<IImage>> addedItems, IReadOnlyList<CollectionChangedEventItem<IImage>> removedItems)
        {
            foreach (var item in addedItems)
            {
                Images.Insert(item.Index, item.Data);
            }

            foreach (var item in removedItems)
            {
                Guard.IsInRangeFor(item.Index, (IReadOnlyList<IImage>)Images, nameof(Images));
                Images.RemoveAt(item.Index);
            }
        }

        private void TrackViewModel_ArtistItemsChanged(object sender, IReadOnlyList<CollectionChangedEventItem<IArtistCollectionItem>> addedItems, IReadOnlyList<CollectionChangedEventItem<IArtistCollectionItem>> removedItems)
        {
            foreach (var item in addedItems)
            {
                switch (item.Data)
                {
                    case IArtist artist:
                        Artists.Insert(item.Index, new ArtistViewModel(artist));
                        break;
                    case IArtistCollection collection:
                        Artists.Insert(item.Index, new ArtistCollectionViewModel(collection));
                        break;
                    default:
                        ThrowHelper.ThrowNotSupportedException($"{item.Data.GetType()} not supported for adding to {GetType()}");
                        break;
                }
            }

            foreach (var item in removedItems)
            {
                Guard.IsInRangeFor(item.Index, (IReadOnlyList<IArtistCollectionItem>)Artists, nameof(Artists));
                Artists.RemoveAt(item.Index);
            }
        }

        /// <summary>
        /// The wrapped model for this <see cref="TrackViewModel"/>.
        /// </summary>
        internal ITrack Model { get; }

        /// <inheritdoc cref="IMerged{T}.SourceCores" />
        public IReadOnlyList<ICore> SourceCores { get; }

        /// <summary>
        /// The merged sources for this model
        /// </summary>
        public IReadOnlyList<ICoreTrack> Sources => Model.GetSources<ICoreTrack>();

        /// <inheritdoc />
        IReadOnlyList<ICoreImageCollection> IMerged<ICoreImageCollection>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreGenreCollection> IMerged<ICoreGenreCollection>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreArtistCollection> IMerged<ICoreArtistCollection>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreArtistCollectionItem> IMerged<ICoreArtistCollectionItem>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreTrack> IMerged<ICoreTrack>.Sources => Sources;

        /// <summary>
        /// The artists for this track.
        /// </summary>
        public SynchronizedObservableCollection<IArtistCollectionItem> Artists { get; }

        /// <inheritdoc />
        public SynchronizedObservableCollection<string>? Genres => Model.Genres;

        /// <inheritdoc />
        public SynchronizedObservableCollection<IImage> Images { get; }

        /// <inheritdoc />
        public TrackType Type => Model.Type;

        /// <inheritdoc />
        public TimeSpan Duration => Model.Duration;

        /// <inheritdoc />
        public DateTime? AddedAt => Model.AddedAt;

        /// <inheritdoc />
        public DateTime? LastPlayed => Model.LastPlayed;

        /// <inheritdoc />
        public IPlayableCollectionGroup? RelatedItems { get; }

        /// <inheritdoc />
        public string Id => Model.Id;

        /// <inheritdoc />
        public string Name => Model.Name;

        /// <inheritdoc />
        public int TotalArtistItemsCount => Model.TotalArtistItemsCount;

        /// <inheritdoc />
        public int TotalImageCount => Model.TotalImageCount;

        /// <inheritdoc />
        public Uri? Url => Model.Url;

        /// <inheritdoc cref="ITrack.Album" />
        public AlbumViewModel? Album { get; private set; }

        /// <inheritdoc />
        IAlbum? ITrack.Album => Model.Album;

        /// <inheritdoc />
        public int? TrackNumber => Model.TrackNumber;

        /// <inheritdoc/>
        public int? DiscNumber => Model.DiscNumber;

        /// <inheritdoc />
        public CultureInfo? Language => Model.Language;

        /// <inheritdoc />
        public ILyrics? Lyrics => Model.Lyrics;

        /// <inheritdoc />
        public bool IsExplicit => Model.IsExplicit;

        /// <inheritdoc />
        public string? Description => Model.Description;

        /// <inheritdoc />
        public PlaybackState PlaybackState => Model.PlaybackState;

        /// <inheritdoc />
        public bool IsPlayAsyncAvailable => Model.IsPlayAsyncAvailable;

        /// <inheritdoc />
        public bool IsPauseAsyncAvailable => Model.IsPauseAsyncAvailable;

        /// <inheritdoc />
        public bool IsChangeNameAsyncAvailable => Model.IsChangeNameAsyncAvailable;

        /// <inheritdoc />
        public bool IsChangeDescriptionAsyncAvailable => Model.IsChangeDescriptionAsyncAvailable;

        /// <inheritdoc />
        public bool IsChangeDurationAsyncAvailable => Model.IsChangeDurationAsyncAvailable;

        /// <inheritdoc />
        public bool IsChangeAlbumAsyncSupported => Model.IsChangeAlbumAsyncSupported;

        /// <inheritdoc />
        public bool IsChangeTrackNumberAsyncSupported => Model.IsChangeTrackNumberAsyncSupported;

        /// <inheritdoc />
        public bool IsChangeLanguageAsyncSupported => Model.IsChangeLanguageAsyncSupported;

        /// <inheritdoc />
        public bool IsChangeLyricsAsyncSupported => Model.IsChangeLyricsAsyncSupported;

        /// <inheritdoc />
        public bool IsChangeIsExplicitAsyncSupported => Model.IsChangeIsExplicitAsyncSupported;

        /// <inheritdoc />
        public Task<bool> IsAddArtistItemSupported(int index) => Model.IsAddArtistItemSupported(index);

        /// <inheritdoc />
        public Task<bool> IsAddGenreSupported(int index) => Model.IsAddGenreSupported(index);

        /// <inheritdoc />
        public Task<bool> IsAddImageSupported(int index) => Model.IsAddImageSupported(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveImageSupported(int index) => Model.IsRemoveImageSupported(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveGenreSupported(int index) => Model.IsRemoveGenreSupported(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveArtistItemSupported(int index) => Model.IsRemoveArtistItemSupported(index);

        /// <inheritdoc />
        public Task ChangeAlbumAsync(IAlbum? album) => Model.ChangeAlbumAsync(album);

        /// <inheritdoc />
        public Task ChangeTrackNumberAsync(int? trackNumber) => Model.ChangeTrackNumberAsync(trackNumber);

        /// <inheritdoc />
        public Task ChangeLanguageAsync(CultureInfo language) => Model.ChangeLanguageAsync(language);

        /// <inheritdoc />
        public Task ChangeLyricsAsync(ILyrics? lyrics) => Model.ChangeLyricsAsync(lyrics);

        /// <inheritdoc />
        public Task ChangeIsExplicitAsync(bool isExplicit) => Model.ChangeIsExplicitAsync(isExplicit);

        /// <inheritdoc />
        public Task PauseAsync() => Model.PauseAsync();

        /// <inheritdoc />
        public async Task PlayAsync()
        {
            var activeDevice = MainViewModel.Singleton?.ActiveDevice;

            if (activeDevice?.Type == DeviceType.Remote)
            {
                await Model.PlayAsync();
                return;
            }

            if (_playbackHandler.PlaybackState == PlaybackState.Paused)
            {
                await _playbackHandler.ResumeAsync();
            }
            else
            {
                await _playbackHandler.PauseAsync();

                // The actual playback
                _playbackHandler.ClearNext();
                _playbackHandler.ClearPrevious();

                // TODO
            }

            if (MainViewModel.Singleton?.LocalDevice?.Model is StrixDevice device)
            {
                device.SetPlaybackData(Model, Model);
            }

            // Play the first thing in the queue.
            await _playbackHandler.PlayFromNext(0);
        }

        /// <inheritdoc />
        public Task ChangeNameAsync(string name) => Model.ChangeNameAsync(name);

        /// <inheritdoc />
        public Task ChangeDescriptionAsync(string? description) => Model.ChangeDescriptionAsync(description);

        /// <inheritdoc />
        public Task ChangeDurationAsync(TimeSpan duration) => Model.ChangeDurationAsync(duration);

        /// <inheritdoc />
        public Task<IReadOnlyList<IArtistCollectionItem>> GetArtistItemsAsync(int limit, int offset) => Model.GetArtistItemsAsync(limit, offset);

        /// <inheritdoc />
        public Task<IReadOnlyList<IImage>> GetImagesAsync(int limit, int offset) => Model.GetImagesAsync(limit, offset);

        /// <inheritdoc />
        public Task AddImageAsync(IImage image, int index) => Model.AddImageAsync(image, index);

        /// <inheritdoc />
        public Task RemoveImageAsync(int index) => Model.RemoveImageAsync(index);

        /// <inheritdoc />
        public Task AddArtistItemAsync(IArtistCollectionItem artist, int index) => Model.AddArtistItemAsync(artist, index);

        /// <inheritdoc />
        public Task RemoveArtistItemAsync(int index) => Model.RemoveArtistItemAsync(index);

        /// <inheritdoc />
        public async Task PopulateMoreArtistsAsync(int limit)
        {
            foreach (var item in await GetArtistItemsAsync(limit, Artists.Count))
            {
                if (item is IArtist artist)
                {
                    Artists.Add(new ArtistViewModel(artist));
                }

                if (item is IArtistCollection collection)
                {
                    Artists.Add(new ArtistCollectionViewModel(collection));
                }
            }
        }

        /// <inheritdoc />
        public async Task PopulateMoreImagesAsync(int limit)
        {
            foreach (var item in await GetImagesAsync(limit, Images.Count))
            {
                Images.Add(item);
            }
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

        /// <inheritdoc />
        public IAsyncRelayCommand<int> PopulateMoreImagesCommand { get; }

        /// <inheritdoc />
        public bool Equals(ICoreArtistCollectionItem other) => Model.Equals(other);

        /// <inheritdoc />
        public bool Equals(ICoreImageCollection other) => Model.Equals(other);

        /// <inheritdoc />
        public bool Equals(ICoreArtistCollection other) => Model.Equals(other);

        /// <inheritdoc />
        public bool Equals(ICoreGenreCollection other) => Model.Equals(other);

        /// <inheritdoc />
        public bool Equals(ICoreTrack other) => Model.Equals(other);
    }
}
