using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Toolkit.Diagnostics;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Input;
using Nito.AsyncEx;
using OwlCore;
using OwlCore.Collections;
using OwlCore.Events;
using OwlCore.Extensions;
using StrixMusic.Sdk.Data;
using StrixMusic.Sdk.Data.Base;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.Data.Merged;
using StrixMusic.Sdk.Extensions;
using StrixMusic.Sdk.MediaPlayback;
using StrixMusic.Sdk.Services.MediaPlayback;

namespace StrixMusic.Sdk.ViewModels
{
    /// <summary>
    /// Contains bindable information about an <see cref="ITrack"/>
    /// </summary>
    public class TrackViewModel : ObservableObject, ITrack, IArtistCollectionViewModel, IImageCollectionViewModel
    {
        private readonly IPlaybackHandlerService _playbackHandler;

        private readonly AsyncLock _populateArtistsMutex = new AsyncLock();
        private readonly AsyncLock _populateImagesMutex = new AsyncLock();

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
                Images = new ObservableCollection<IImage>();
                Artists = new ObservableCollection<IArtistCollectionItem>();
            }

            PlayArtistCollectionAsyncCommand = new AsyncRelayCommand(PlayArtistCollectionAsync);
            PauseArtistCollectionAsyncCommand = new AsyncRelayCommand(PauseArtistCollectionAsync);

            PlayArtistAsyncCommand = new AsyncRelayCommand<IArtistCollectionItem>(PlayArtistCollectionInternalAsync);

            ChangeNameAsyncCommand = new AsyncRelayCommand<string>(ChangeNameInternalAsync);
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

            IsPlayArtistCollectionAsyncAvailableChanged += OnIsPlayArtistCollectionAsyncAvailableChanged;
            IsPauseArtistCollectionAsyncAvailableChanged += OnIsPauseArtistCollectionAsyncAvailableChanged;

            IsChangeNameAsyncAvailableChanged += OnIsChangeNameAsyncAvailableChanged;
            IsChangeDurationAsyncAvailableChanged += OnIsChangeDurationAsyncAvailableChanged;
            IsChangeDescriptionAsyncAvailableChanged += OnIsChangeDescriptionAsyncAvailableChanged;

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

            IsPlayArtistCollectionAsyncAvailableChanged -= OnIsPlayArtistCollectionAsyncAvailableChanged;
            IsPauseArtistCollectionAsyncAvailableChanged -= OnIsPauseArtistCollectionAsyncAvailableChanged;

            IsChangeNameAsyncAvailableChanged -= OnIsChangeNameAsyncAvailableChanged;
            IsChangeDurationAsyncAvailableChanged -= OnIsChangeDurationAsyncAvailableChanged;
            IsChangeDescriptionAsyncAvailableChanged -= OnIsChangeDescriptionAsyncAvailableChanged;

            ArtistItemsCountChanged -= ModelOnArtistItemsCountChanged;
            ArtistItemsChanged -= TrackViewModel_ArtistItemsChanged;
            ImagesCountChanged -= TrackViewModel_ImagesCountChanged;
            ImagesChanged -= TrackViewModel_ImagesChanged;
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
        public event EventHandler<bool>? IsPlayArtistCollectionAsyncAvailableChanged
        {
            add => Model.IsPlayArtistCollectionAsyncAvailableChanged += value;
            remove => Model.IsPlayArtistCollectionAsyncAvailableChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<bool>? IsPauseArtistCollectionAsyncAvailableChanged
        {
            add => Model.IsPauseArtistCollectionAsyncAvailableChanged += value;
            remove => Model.IsPauseArtistCollectionAsyncAvailableChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<bool>? IsChangeNameAsyncAvailableChanged
        {
            add => Model.IsChangeNameAsyncAvailableChanged += value;
            remove => Model.IsChangeNameAsyncAvailableChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<bool>? IsChangeDescriptionAsyncAvailableChanged
        {
            add => Model.IsChangeDescriptionAsyncAvailableChanged += value;
            remove => Model.IsChangeDescriptionAsyncAvailableChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<bool>? IsChangeDurationAsyncAvailableChanged
        {
            add => Model.IsChangeDurationAsyncAvailableChanged += value;
            remove => Model.IsChangeDurationAsyncAvailableChanged -= value;
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

        private void Track_UrlChanged(object sender, Uri? e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(Url)));

        private void Track_TrackNumberChanged(object sender, int? e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(TrackNumber)));

        private void Track_PlaybackStateChanged(object sender, PlaybackState e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(PlaybackState)));

        private void Track_NameChanged(object sender, string e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(Name)));

        private void Track_LyricsChanged(object sender, ILyrics? e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(Lyrics)));

        private void Track_LanguageChanged(object sender, CultureInfo? e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(Language)));

        private void Track_IsExplicitChanged(object sender, bool e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(IsExplicit)));

        private void Track_DescriptionChanged(object sender, string? e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(Description)));

        private void OnLastPlayedChanged(object sender, DateTime? e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(LastPlayed)));

        private void OnIsChangeDescriptionAsyncAvailableChanged(object sender, bool e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(IsChangeDescriptionAsyncAvailable)));

        private void OnIsChangeDurationAsyncAvailableChanged(object sender, bool e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(IsChangeDurationAsyncAvailable)));

        private void OnIsChangeNameAsyncAvailableChanged(object sender, bool e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(IsChangeNameAsyncAvailable)));

        private void OnIsPauseArtistCollectionAsyncAvailableChanged(object sender, bool e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(IsPauseArtistCollectionAsyncAvailable)));

        private void OnIsPlayArtistCollectionAsyncAvailableChanged(object sender, bool e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(IsPlayArtistCollectionAsyncAvailable)));

        private void Track_AlbumChanged(object sender, IAlbum? e)
        {
            Album = e is null ? null : new AlbumViewModel(e);
            _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(Album)));
        }

        private void ModelOnArtistItemsCountChanged(object sender, int e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(TotalArtistItemsCount)));

        private void TrackViewModel_ImagesCountChanged(object sender, int e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(TotalImageCount)));

        private void TrackViewModel_ImagesChanged(object sender, IReadOnlyList<CollectionChangedItem<IImage>> addedItems, IReadOnlyList<CollectionChangedItem<IImage>> removedItems)
        {
            _ = Threading.OnPrimaryThread(() =>
            {
                Images.ChangeCollection(addedItems, removedItems);
            });
        }

        private void TrackViewModel_ArtistItemsChanged(object sender, IReadOnlyList<CollectionChangedItem<IArtistCollectionItem>> addedItems, IReadOnlyList<CollectionChangedItem<IArtistCollectionItem>> removedItems)
        {
            _ = Threading.OnPrimaryThread(() =>
            {
                Artists.ChangeCollection(addedItems, removedItems, item => item.Data switch
                {
                    IArtist artist => new ArtistViewModel(artist),
                    IArtistCollection collection => new ArtistCollectionViewModel(collection),
                    _ => ThrowHelper.ThrowNotSupportedException<IArtistCollectionItem>($"{item.Data.GetType()} not supported for adding to {GetType()}")
                });
            });
        }

        /// <summary>
        /// The wrapped model for this <see cref="TrackViewModel"/>.
        /// </summary>
        internal ITrack Model { get; }

        /// <inheritdoc cref="IMerged{T}.SourceCores" />
        public IReadOnlyList<ICore> SourceCores { get; }

        /// <summary>
        /// The merged sources for this model.
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
        public ObservableCollection<IArtistCollectionItem> Artists { get; }

        /// <inheritdoc />
        public SynchronizedObservableCollection<string>? Genres => Model.Genres;

        /// <inheritdoc />
        public ObservableCollection<IImage> Images { get; }

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
        public bool IsPlayArtistCollectionAsyncAvailable => Model.IsPlayArtistCollectionAsyncAvailable;

        /// <inheritdoc />
        public bool IsPauseArtistCollectionAsyncAvailable => Model.IsPauseArtistCollectionAsyncAvailable;

        /// <inheritdoc />
        public bool IsChangeNameAsyncAvailable => Model.IsChangeNameAsyncAvailable;

        /// <inheritdoc />
        public bool IsChangeDescriptionAsyncAvailable => Model.IsChangeDescriptionAsyncAvailable;

        /// <inheritdoc />
        public bool IsChangeDurationAsyncAvailable => Model.IsChangeDurationAsyncAvailable;

        /// <inheritdoc />
        public bool IsChangeAlbumAsyncAvailable => Model.IsChangeAlbumAsyncAvailable;

        /// <inheritdoc />
        public bool IsChangeTrackNumberAsyncAvailable => Model.IsChangeTrackNumberAsyncAvailable;

        /// <inheritdoc />
        public bool IsChangeLanguageAsyncAvailable => Model.IsChangeLanguageAsyncAvailable;

        /// <inheritdoc />
        public bool IsChangeLyricsAsyncAvailable => Model.IsChangeLyricsAsyncAvailable;

        /// <inheritdoc />
        public bool IsChangeIsExplicitAsyncAvailable => Model.IsChangeIsExplicitAsyncAvailable;

        /// <inheritdoc />
        public Task<bool> IsAddArtistItemAvailable(int index) => Model.IsAddArtistItemAvailable(index);

        /// <inheritdoc />
        public Task<bool> IsAddGenreAvailable(int index) => Model.IsAddGenreAvailable(index);

        /// <inheritdoc />
        public Task<bool> IsAddImageAvailable(int index) => Model.IsAddImageAvailable(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveImageAvailable(int index) => Model.IsRemoveImageAvailable(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveGenreAvailable(int index) => Model.IsRemoveGenreAvailable(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveArtistItemAvailable(int index) => Model.IsRemoveArtistItemAvailable(index);

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
        public async Task PauseArtistCollectionAsync() => await _playbackHandler.PauseAsync();

        /// <inheritdoc />
        public Task ChangeNameAsync(string name) => ChangeNameInternalAsync(name);

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
        public async Task PlayArtistCollectionAsync()
        {
            await _playbackHandler.PlayAsync((IArtistCollectionViewModel)this, Model);
        }

        /// <inheritdoc />
        public Task PlayArtistCollectionAsync(IArtistCollectionItem artistItem) => PlayArtistCollectionInternalAsync(artistItem);

        /// <inheritdoc />
        public async Task PopulateMoreArtistsAsync(int limit)
        {
            using (await _populateArtistsMutex.LockAsync())
            {
                var items = await GetArtistItemsAsync(limit, Artists.Count);

                _ = Threading.OnPrimaryThread(() =>
                {
                    foreach (var item in items)
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
                });
            }
        }

        /// <inheritdoc />
        public async Task PopulateMoreImagesAsync(int limit)
        {
            using (await _populateImagesMutex.LockAsync())
            {
                var items = await GetImagesAsync(limit, Images.Count);

                _ = Threading.OnPrimaryThread(() =>
                {
                    foreach (var item in items)
                    {
                        Images.Add(item);
                    }
                });
            }
        }

        /// <inheritdoc />
        public IAsyncRelayCommand<int> PopulateMoreArtistsCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand PlayArtistCollectionAsyncCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand<IArtistCollectionItem> PlayArtistAsyncCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand PauseArtistCollectionAsyncCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand<int> PopulateMoreImagesCommand { get; }

        /// <summary>
        /// Command to change the name. It triggers <see cref="ChangeNameAsync"/>.
        /// </summary>
        public IAsyncRelayCommand<string> ChangeNameAsyncCommand { get; }

        /// <summary>
        /// Command to change the description. It triggers <see cref="ChangeDescriptionAsync"/>.
        /// </summary>
        public IAsyncRelayCommand<string?> ChangeDescriptionAsyncCommand { get; }

        /// <summary>
        /// Command to change the duration. It triggers <see cref="ChangeDurationAsync"/>.
        /// </summary>
        public IAsyncRelayCommand<TimeSpan> ChangeDurationAsyncCommand { get; }

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

        private Task ChangeNameInternalAsync(string? name)
        {
            Guard.IsNotNull(name, nameof(name));
            return Model.ChangeNameAsync(name);
        }

        private Task PlayArtistCollectionInternalAsync(IArtistCollectionItem? artistItem)
        {
            Guard.IsNotNull(artistItem, nameof(artistItem));
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public ValueTask DisposeAsync()
        {
            DetachEvents();
            return Model.DisposeAsync();
        }

        /// <inheritdoc />
        public Task InitAsync()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public bool IsInitialized { get; }
    }
}
