using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Toolkit.Diagnostics;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Input;
using Nito.AsyncEx;
using OwlCore;
using OwlCore.Events;
using OwlCore.Extensions;
using StrixMusic.Sdk.Data;
using StrixMusic.Sdk.Data.Base;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.Data.Merged;
using StrixMusic.Sdk.Extensions;
using StrixMusic.Sdk.MediaPlayback;
using StrixMusic.Sdk.Services.Localization;
using StrixMusic.Sdk.Services.MediaPlayback;
using StrixMusic.Sdk.ViewModels.Helpers;
using StrixMusic.Sdk.ViewModels.Helpers.Sorting;

namespace StrixMusic.Sdk.ViewModels
{
    /// <summary>
    /// Contains bindable information about an <see cref="ICoreArtist"/>.
    /// </summary>
    public class ArtistViewModel : ObservableObject, IArtist, IAlbumCollectionViewModel, ITrackCollectionViewModel, IImageCollectionViewModel, IGenreCollectionViewModel, IUrlCollectionViewModel
    {
        private readonly IArtist _artist;
        private readonly IPlaybackHandlerService _playbackHandler;
        private readonly ILocalizationService _localizationService;

        private readonly AsyncLock _populateTracksMutex = new AsyncLock();
        private readonly AsyncLock _populateAlbumsMutex = new AsyncLock();
        private readonly AsyncLock _populateImagesMutex = new AsyncLock();
        private readonly AsyncLock _populateGenresMutex = new AsyncLock();
        private readonly AsyncLock _populateUrlsMutex = new AsyncLock();

        /// <summary>
        /// Initializes a new instance of the <see cref="ArtistViewModel"/> class.
        /// </summary>
        /// <param name="artist">The <see cref="IArtist"/> to wrap.</param>
        public ArtistViewModel(IArtist artist)
        {
            _artist = artist;

            SourceCores = _artist.GetSourceCores<ICoreArtist>().Select(MainViewModel.GetLoadedCore).ToList();

            _playbackHandler = Ioc.Default.GetRequiredService<IPlaybackHandlerService>();
            _localizationService = Ioc.Default.GetRequiredService<ILocalizationService>();

            if (_artist.RelatedItems != null)
                RelatedItems = new PlayableCollectionGroupViewModel(_artist.RelatedItems);

            using (Threading.PrimaryContext)
            {
                UnsortedTracks = new ObservableCollection<TrackViewModel>();
                UnsortedAlbums = new ObservableCollection<IAlbumCollectionItem>();
                Tracks = new ObservableCollection<TrackViewModel>();
                Albums = new ObservableCollection<IAlbumCollectionItem>();
                Images = new ObservableCollection<IImage>();
                Genres = new ObservableCollection<IGenre>();
                Urls = new ObservableCollection<IUrl>();
            }

            PlayTrackCollectionAsyncCommand = new AsyncRelayCommand(PlayTrackCollectionAsync);
            PauseTrackCollectionAsyncCommand = new AsyncRelayCommand(PauseTrackCollectionAsync);
            PlayAlbumCollectionAsyncCommand = new AsyncRelayCommand(PlayAlbumCollectionAsync);
            PauseAlbumCollectionAsyncCommand = new AsyncRelayCommand(PauseAlbumCollectionAsync);

            PlayTrackAsyncCommand = new AsyncRelayCommand<ITrack>(PlayTrackCollectionInternalAsync);
            PlayAlbumAsyncCommand = new AsyncRelayCommand<IAlbumCollectionItem>(PlayAlbumCollectionInternalAsync);

            ChangeNameAsyncCommand = new AsyncRelayCommand<string>(ChangeNameInternalAsync);
            ChangeDescriptionAsyncCommand = new AsyncRelayCommand<string?>(ChangeDescriptionAsync);
            ChangeDurationAsyncCommand = new AsyncRelayCommand<TimeSpan>(ChangeDurationAsync);

            PopulateMoreAlbumsCommand = new AsyncRelayCommand<int>(PopulateMoreAlbumsAsync);
            PopulateMoreTracksCommand = new AsyncRelayCommand<int>(PopulateMoreTracksAsync);
            PopulateMoreImagesCommand = new AsyncRelayCommand<int>(PopulateMoreImagesAsync);
            PopulateMoreGenresCommand = new AsyncRelayCommand<int>(PopulateMoreGenresAsync);
            PopulateMoreUrlsCommand = new AsyncRelayCommand<int>(PopulateMoreUrlsAsync);

            InitAlbumCollectionAsyncCommand = new AsyncRelayCommand(InitAlbumCollectionAsync);
            InitGenreCollectionAsyncCommand = new AsyncRelayCommand(InitGenreCollectionAsync);
            InitImageCollectionAsyncCommand = new AsyncRelayCommand(InitImageCollectionAsync);
            InitTrackCollectionAsyncCommand = new AsyncRelayCommand(InitTrackCollectionAsync);

            ChangeAlbumCollectionSortingTypeCommand = new RelayCommand<AlbumSortingType>(x => SortAlbumCollection(x, CurrentAlbumSortingDirection));
            ChangeAlbumCollectionSortingDirectionCommand = new RelayCommand<SortDirection>(x => SortAlbumCollection(CurrentAlbumSortingType, x));
            ChangeTrackCollectionSortingTypeCommand = new RelayCommand<TrackSortingType>(x => SortTrackCollection(x, CurrentTracksSortingDirection));
            ChangeTrackCollectionSortingDirectionCommand = new RelayCommand<SortDirection>(x => SortTrackCollection(CurrentTracksSortingType, x));

            AttachEvents();
        }

        private void AttachEvents()
        {
            PlaybackStateChanged += ArtistPlaybackStateChanged;
            DescriptionChanged += ArtistDescriptionChanged;
            NameChanged += ArtistNameChanged;
            LastPlayedChanged += OnLastPlayedChanged;

            IsPlayTrackCollectionAsyncAvailableChanged += OnIsPlayTrackCollectionAsyncAvailableChanged;
            IsPauseTrackCollectionAsyncAvailableChanged += OnIsPauseTrackCollectionAsyncAvailableChanged;
            IsPlayAlbumCollectionAsyncAvailableChanged += OnIsPlayAlbumCollectionAsyncAvailableChanged;
            IsPauseAlbumCollectionAsyncAvailableChanged += OnIsPauseAlbumCollectionAsyncAvailableChanged;

            IsChangeNameAsyncAvailableChanged += OnIsChangeNameAsyncAvailableChanged;
            IsChangeDurationAsyncAvailableChanged += OnIsChangeDurationAsyncAvailableChanged;
            IsChangeDescriptionAsyncAvailableChanged += OnIsChangeDescriptionAsyncAvailableChanged;

            AlbumItemsCountChanged += Artist_AlbumItemsCountChanged;
            TracksCountChanged += ArtistOnTrackItemsCountChanged;
            ImagesCountChanged += ArtistViewModel_ImagesCountChanged;
            GenresCountChanged += ArtistViewModel_GenresCountChanged;
            UrlsCountChanged += ArtistViewModel_UrlsCountChanged;

            AlbumItemsChanged += ArtistViewModel_AlbumItemsChanged;
            TracksChanged += ArtistViewModel_TrackItemsChanged;
            ImagesChanged += ArtistViewModel_ImagesChanged;
            GenresChanged += ArtistViewModel_GenresChanged;
            ImagesChanged += ArtistViewModel_ImagesChanged;
        }

        private void DetachEvents()
        {
            PlaybackStateChanged -= ArtistPlaybackStateChanged;
            DescriptionChanged -= ArtistDescriptionChanged;
            NameChanged -= ArtistNameChanged;
            LastPlayedChanged -= OnLastPlayedChanged;

            IsPlayTrackCollectionAsyncAvailableChanged -= OnIsPlayTrackCollectionAsyncAvailableChanged;
            IsPauseTrackCollectionAsyncAvailableChanged -= OnIsPauseTrackCollectionAsyncAvailableChanged;
            IsPlayAlbumCollectionAsyncAvailableChanged -= OnIsPlayAlbumCollectionAsyncAvailableChanged;
            IsPauseAlbumCollectionAsyncAvailableChanged -= OnIsPauseAlbumCollectionAsyncAvailableChanged;

            IsChangeNameAsyncAvailableChanged -= OnIsChangeNameAsyncAvailableChanged;
            IsChangeDurationAsyncAvailableChanged -= OnIsChangeDurationAsyncAvailableChanged;
            IsChangeDescriptionAsyncAvailableChanged -= OnIsChangeDescriptionAsyncAvailableChanged;

            AlbumItemsCountChanged -= Artist_AlbumItemsCountChanged;
            TracksCountChanged -= ArtistOnTrackItemsCountChanged;
            ImagesCountChanged -= ArtistViewModel_ImagesCountChanged;
            GenresCountChanged -= ArtistViewModel_GenresCountChanged;

            AlbumItemsChanged -= ArtistViewModel_AlbumItemsChanged;
            TracksChanged -= ArtistViewModel_TrackItemsChanged;
            ImagesChanged -= ArtistViewModel_ImagesChanged;
            GenresChanged -= ArtistViewModel_GenresChanged;
        }

        /// <inheritdoc />
        public event EventHandler<TimeSpan>? DurationChanged
        {
            add => _artist.DurationChanged += value;
            remove => _artist.DurationChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<DateTime?>? LastPlayedChanged
        {
            add => _artist.LastPlayedChanged += value;
            remove => _artist.LastPlayedChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<bool>? IsPlayTrackCollectionAsyncAvailableChanged
        {
            add => _artist.IsPlayTrackCollectionAsyncAvailableChanged += value;
            remove => _artist.IsPlayTrackCollectionAsyncAvailableChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<bool>? IsPauseTrackCollectionAsyncAvailableChanged
        {
            add => _artist.IsPauseTrackCollectionAsyncAvailableChanged += value;
            remove => _artist.IsPauseTrackCollectionAsyncAvailableChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<bool>? IsPlayAlbumCollectionAsyncAvailableChanged
        {
            add => _artist.IsPlayAlbumCollectionAsyncAvailableChanged += value;
            remove => _artist.IsPlayAlbumCollectionAsyncAvailableChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<bool>? IsPauseAlbumCollectionAsyncAvailableChanged
        {
            add => _artist.IsPauseAlbumCollectionAsyncAvailableChanged += value;
            remove => _artist.IsPauseAlbumCollectionAsyncAvailableChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<bool>? IsChangeNameAsyncAvailableChanged
        {
            add => _artist.IsChangeNameAsyncAvailableChanged += value;
            remove => _artist.IsChangeNameAsyncAvailableChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<bool>? IsChangeDescriptionAsyncAvailableChanged
        {
            add => _artist.IsChangeDescriptionAsyncAvailableChanged += value;
            remove => _artist.IsChangeDescriptionAsyncAvailableChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<bool>? IsChangeDurationAsyncAvailableChanged
        {
            add => _artist.IsChangeDurationAsyncAvailableChanged += value;
            remove => _artist.IsChangeDurationAsyncAvailableChanged -= value;
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
        public event EventHandler<string?>? DescriptionChanged
        {
            add => _artist.DescriptionChanged += value;
            remove => _artist.DescriptionChanged -= value;
        }

        /// <inheritdoc />
        public event CollectionChangedEventHandler<IAlbumCollectionItem>? AlbumItemsChanged
        {
            add => _artist.AlbumItemsChanged += value;
            remove => _artist.AlbumItemsChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<int>? AlbumItemsCountChanged
        {
            add => _artist.AlbumItemsCountChanged += value;
            remove => _artist.AlbumItemsCountChanged -= value;
        }

        /// <inheritdoc />
        public event CollectionChangedEventHandler<ITrack>? TracksChanged
        {
            add => _artist.TracksChanged += value;
            remove => _artist.TracksChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<int>? TracksCountChanged
        {
            add => _artist.TracksCountChanged += value;
            remove => _artist.TracksCountChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<int>? ImagesCountChanged
        {
            add => _artist.ImagesCountChanged += value;
            remove => _artist.ImagesCountChanged -= value;
        }

        /// <inheritdoc />
        public event CollectionChangedEventHandler<IImage>? ImagesChanged
        {
            add => _artist.ImagesChanged += value;
            remove => _artist.ImagesChanged -= value;
        }

        /// <inheritdoc/>
        public event CollectionChangedEventHandler<IGenre>? GenresChanged
        {
            add => _artist.GenresChanged += value;
            remove => _artist.GenresChanged -= value;
        }

        /// <inheritdoc/>
        public event EventHandler<int>? GenresCountChanged
        {
            add => _artist.GenresCountChanged += value;
            remove => _artist.GenresCountChanged -= value;
        }

        /// <inheritdoc />
        public event CollectionChangedEventHandler<IUrl>? UrlsChanged
        {
            add => _artist.UrlsChanged += value;
            remove => _artist.UrlsChanged -= value;
        }

        /// <inheritdoc/>
        public event EventHandler<int>? UrlsCountChanged
        {
            add => _artist.UrlsCountChanged += value;
            remove => _artist.UrlsCountChanged -= value;
        }

        private void ArtistNameChanged(object sender, string e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(Name)));

        private void ArtistDescriptionChanged(object sender, string? e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(Description)));

        private void ArtistPlaybackStateChanged(object sender, PlaybackState e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(PlaybackState)));

        private void ArtistOnTrackItemsCountChanged(object sender, int e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(TotalTrackCount)));

        private void Artist_AlbumItemsCountChanged(object sender, int e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(TotalAlbumItemsCount)));

        private void ArtistViewModel_ImagesCountChanged(object sender, int e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(TotalImageCount)));

        private void ArtistViewModel_UrlsCountChanged(object sender, int e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(TotalUrlCount)));

        private void ArtistViewModel_GenresCountChanged(object sender, int e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(TotalGenreCount)));

        private void OnLastPlayedChanged(object sender, DateTime? e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(LastPlayed)));

        private void OnIsChangeDescriptionAsyncAvailableChanged(object sender, bool e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(IsChangeDescriptionAsyncAvailable)));

        private void OnIsChangeDurationAsyncAvailableChanged(object sender, bool e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(IsChangeDurationAsyncAvailable)));

        private void OnIsChangeNameAsyncAvailableChanged(object sender, bool e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(IsChangeNameAsyncAvailable)));

        private void OnIsPauseTrackCollectionAsyncAvailableChanged(object sender, bool e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(IsPauseTrackCollectionAsyncAvailable)));

        private void OnIsPlayTrackCollectionAsyncAvailableChanged(object sender, bool e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(IsPlayTrackCollectionAsyncAvailable)));

        private void OnIsPauseAlbumCollectionAsyncAvailableChanged(object sender, bool e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(IsPauseAlbumCollectionAsyncAvailable)));

        private void OnIsPlayAlbumCollectionAsyncAvailableChanged(object sender, bool e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(IsPlayAlbumCollectionAsyncAvailable)));

        private async void ArtistViewModel_ImagesChanged(object sender, IReadOnlyList<CollectionChangedItem<IImage>> addedItems, IReadOnlyList<CollectionChangedItem<IImage>> removedItems)
        {
            await Threading.OnPrimaryThread(() =>
            {
                Images.ChangeCollection(addedItems, removedItems);
            });
        }

        private async void ArtistViewModel_GenresChanged(object sender, IReadOnlyList<CollectionChangedItem<IGenre>> addedItems, IReadOnlyList<CollectionChangedItem<IGenre>> removedItems)
        {
            await Threading.OnPrimaryThread(() =>
            {
                Genres.ChangeCollection(addedItems, removedItems);
            });
        }

        private async void ArtistViewModel_TrackItemsChanged(object sender, IReadOnlyList<CollectionChangedItem<ITrack>> addedItems, IReadOnlyList<CollectionChangedItem<ITrack>> removedItems)
        {
            await Threading.OnPrimaryThread(() =>
            {
                if (CurrentTracksSortingType == TrackSortingType.Unsorted)
                {
                    Tracks.ChangeCollection(addedItems, removedItems, x => new TrackViewModel(x.Data));
                }
                else
                {
                    // Preventing index issues during tracks emission from the core, also making sure that unordered tracks updated. 
                    UnsortedTracks.ChangeCollection(addedItems, removedItems, x => new TrackViewModel(x.Data));

                    // Avoiding direct assignment to prevent effect on UI.
                    foreach (var item in UnsortedTracks)
                    {
                        if (!Tracks.Contains(item))
                            Tracks.Add(item);
                    }

                    foreach (var item in Tracks)
                    {
                        if (!UnsortedTracks.Contains(item))
                            Tracks.Remove(item);
                    }

                    SortTrackCollection(CurrentTracksSortingType, CurrentTracksSortingDirection);
                }
            });
        }

        private async void ArtistViewModel_AlbumItemsChanged(object sender, IReadOnlyList<CollectionChangedItem<IAlbumCollectionItem>> addedItems, IReadOnlyList<CollectionChangedItem<IAlbumCollectionItem>> removedItems)
        {
            await Threading.OnPrimaryThread(() =>
            {
                if (CurrentAlbumSortingType == AlbumSortingType.Unsorted)
                {
                    Albums.ChangeCollection(addedItems, removedItems, item => item.Data switch
                    {
                        IAlbum album => new AlbumViewModel(album),
                        IAlbumCollection collection => new AlbumCollectionViewModel(collection),
                        _ => ThrowHelper.ThrowNotSupportedException<IAlbumCollectionItem>(
                            $"{item.Data.GetType()} not supported for adding to {GetType()}")
                    });
                }
                else
                {
                    // Preventing index issues during albums emission from the core, also making sure that unordered albums updated. 
                    UnsortedAlbums.ChangeCollection(addedItems, removedItems, item => item.Data switch
                    {
                        IAlbum album => new AlbumViewModel(album),
                        IAlbumCollection collection => new AlbumCollectionViewModel(collection),
                        _ => ThrowHelper.ThrowNotSupportedException<IAlbumCollectionItem>(
                            $"{item.Data.GetType()} not supported for adding to {GetType()}")
                    });

                    // Avoiding direct assignment to prevent effect on UI.
                    foreach (var item in UnsortedAlbums)
                    {
                        if (!Albums.Contains(item))
                            Albums.Add(item);
                    }

                    foreach (var item in Albums)
                    {
                        if (!UnsortedAlbums.Contains(item))
                            Albums.Remove(item);
                    }

                    SortAlbumCollection(CurrentAlbumSortingType, CurrentAlbumSortingDirection);
                }
            });
        }

        /// <inheritdoc cref="IMerged{T}.SourceCores" />
        public IReadOnlyList<ICore> SourceCores { get; }

        /// <summary>
        /// The sources that were merged to form this member.
        /// </summary>
        public IReadOnlyList<ICoreArtist> Sources => _artist.GetSources<ICoreArtist>();

        /// <inheritdoc />
        IReadOnlyList<ICoreArtist> IMerged<ICoreArtist>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreTrackCollection> IMerged<ICoreTrackCollection>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreAlbumCollection> IMerged<ICoreAlbumCollection>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreAlbumCollectionItem> IMerged<ICoreAlbumCollectionItem>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreArtistCollectionItem> IMerged<ICoreArtistCollectionItem>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreGenreCollection> IMerged<ICoreGenreCollection>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreImageCollection> IMerged<ICoreImageCollection>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreUrlCollection> IMerged<ICoreUrlCollection>.Sources => Sources;

        /// <inheritdoc />
        public string Id => _artist.Id;

        /// <inheritdoc cref="IPlayableBase.Duration" />
        public TimeSpan Duration => _artist.Duration;

        /// <inheritdoc />
        public DateTime? LastPlayed => _artist.LastPlayed;

        /// <inheritdoc />
        public DateTime? AddedAt => _artist.AddedAt;

        /// <inheritdoc />
        public bool IsInitialized { get; private set; }

        /// <inheritdoc />
        public IPlayableCollectionGroup? RelatedItems { get; }

        /// <inheritdoc />
        public TrackSortingType CurrentTracksSortingType { get; private set; }

        /// <inheritdoc />
        public SortDirection CurrentTracksSortingDirection { get; private set; }

        /// <inheritdoc />
        public AlbumSortingType CurrentAlbumSortingType { get; private set; }

        /// <inheritdoc />
        public SortDirection CurrentAlbumSortingDirection { get; private set; }

        /// <inheritdoc />
        public ObservableCollection<TrackViewModel> Tracks { get; set; }

        /// <inheritdoc />
        public ObservableCollection<TrackViewModel> UnsortedTracks { get; }

        /// <inheritdoc />
        public ObservableCollection<IAlbumCollectionItem> Albums { get; }

        ///<inheritdoc />
        public ObservableCollection<IAlbumCollectionItem> UnsortedAlbums { get; }

        /// <inheritdoc />
        public ObservableCollection<IImage> Images { get; }

        /// <inheritdoc />
        public ObservableCollection<IGenre> Genres { get; }

        /// <inheritdoc />
        public ObservableCollection<IUrl> Urls { get; }

        /// <inheritdoc />
        public string Name => _localizationService.LocalizeIfNullOrEmpty(_artist.Name, this);

        /// <inheritdoc />
        public int TotalAlbumItemsCount => _artist.TotalAlbumItemsCount;

        /// <inheritdoc />
        public int TotalTrackCount => _artist.TotalTrackCount;

        /// <inheritdoc />
        public int TotalImageCount => _artist.TotalImageCount;

        /// <inheritdoc />
        public int TotalGenreCount => _artist.TotalGenreCount;

        /// <inheritdoc />
        public int TotalUrlCount => _artist.TotalTrackCount;

        /// <inheritdoc />
        public string? Description => _artist.Description;

        /// <inheritdoc />
        public PlaybackState PlaybackState => _artist.PlaybackState;

        /// <inheritdoc />
        public bool IsPlayTrackCollectionAsyncAvailable => _artist.IsPlayTrackCollectionAsyncAvailable;

        /// <inheritdoc />
        public bool IsPauseTrackCollectionAsyncAvailable => _artist.IsPauseTrackCollectionAsyncAvailable;

        /// <inheritdoc />
        public bool IsPlayAlbumCollectionAsyncAvailable => _artist.IsPlayAlbumCollectionAsyncAvailable;

        /// <inheritdoc />
        public bool IsPauseAlbumCollectionAsyncAvailable => _artist.IsPauseAlbumCollectionAsyncAvailable;

        /// <inheritdoc />
        public bool IsChangeNameAsyncAvailable => _artist.IsChangeNameAsyncAvailable;

        /// <inheritdoc />
        public bool IsChangeDescriptionAsyncAvailable => _artist.IsChangeDescriptionAsyncAvailable;

        /// <inheritdoc />
        public bool IsChangeDurationAsyncAvailable => _artist.IsChangeDurationAsyncAvailable;

        /// <inheritdoc />
        public Task PlayTrackCollectionAsync() => _playbackHandler.PlayAsync((ITrackCollectionViewModel)this, _artist);

        /// <inheritdoc />
        public Task PlayAlbumCollectionAsync() => _playbackHandler.PlayAsync((IAlbumCollectionViewModel)this, _artist);

        /// <inheritdoc />
        public Task PauseTrackCollectionAsync() => _playbackHandler.PauseAsync();

        /// <inheritdoc />
        public Task PauseAlbumCollectionAsync() => _playbackHandler.PauseAsync();

        /// <inheritdoc />
        public Task ChangeNameAsync(string name) => ChangeNameInternalAsync(name);

        /// <inheritdoc />
        public Task ChangeDescriptionAsync(string? description) => _artist.ChangeDescriptionAsync(description);

        /// <inheritdoc />
        public Task ChangeDurationAsync(TimeSpan duration) => _artist.ChangeDurationAsync(duration);

        /// <inheritdoc />
        public Task<bool> IsAddAlbumItemAvailableAsync(int index) => _artist.IsAddAlbumItemAvailableAsync(index);

        /// <inheritdoc />
        public Task<bool> IsAddTrackAvailableAsync(int index) => _artist.IsAddTrackAvailableAsync(index);

        /// <inheritdoc />
        public Task<bool> IsAddGenreAvailableAsync(int index) => _artist.IsAddGenreAvailableAsync(index);

        /// <inheritdoc />
        public Task<bool> IsAddImageAvailableAsync(int index) => _artist.IsAddImageAvailableAsync(index);

        /// <inheritdoc />
        public Task<bool> IsAddUrlAvailableAsync(int index) => _artist.IsAddUrlAvailableAsync(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveTrackAvailableAsync(int index) => _artist.IsRemoveTrackAvailableAsync(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveAlbumItemAvailableAsync(int index) => _artist.IsRemoveAlbumItemAvailableAsync(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveGenreAvailableAsync(int index) => _artist.IsRemoveGenreAvailableAsync(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveImageAvailableAsync(int index) => _artist.IsRemoveImageAvailableAsync(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveUrlAvailableAsync(int index) => _artist.IsRemoveUrlAvailableAsync(index);

        /// <inheritdoc />
        public Task PlayAlbumCollectionAsync(IAlbumCollectionItem albumItem) => PlayAlbumCollectionInternalAsync(albumItem);

        /// <inheritdoc />
        public Task PlayTrackCollectionAsync(ITrack track) => PlayTrackCollectionInternalAsync(track);

        /// <inheritdoc />
        public Task<IReadOnlyList<ITrack>> GetTracksAsync(int limit, int offset) => _artist.GetTracksAsync(limit, offset);

        /// <inheritdoc />
        public Task<IReadOnlyList<IAlbumCollectionItem>> GetAlbumItemsAsync(int limit, int offset) => _artist.GetAlbumItemsAsync(limit, offset);

        /// <inheritdoc/>
        public Task<IReadOnlyList<IImage>> GetImagesAsync(int limit, int offset) => _artist.GetImagesAsync(limit, offset);

        /// <inheritdoc/>
        public Task<IReadOnlyList<IGenre>> GetGenresAsync(int limit, int offset) => _artist.GetGenresAsync(limit, offset);

        /// <inheritdoc/>
        public Task<IReadOnlyList<IUrl>> GetUrlsAsync(int limit, int offset) => _artist.GetUrlsAsync(limit, offset);

        /// <inheritdoc />
        public void SortAlbumCollection(AlbumSortingType albumSorting, SortDirection sortDirection)
        {
            CurrentAlbumSortingType = albumSorting;
            CurrentAlbumSortingDirection = sortDirection;

            CollectionSorting.SortAlbums(Albums, albumSorting, sortDirection, UnsortedAlbums);
        }

        /// <inheritdoc />
        public void SortTrackCollection(TrackSortingType trackSorting, SortDirection sortDirection)
        {
            CurrentTracksSortingType = trackSorting;
            CurrentTracksSortingDirection = sortDirection;

            CollectionSorting.SortTracks(Tracks, trackSorting, sortDirection, UnsortedTracks);
        }

        /// <inheritdoc />
        public async Task PopulateMoreAlbumsAsync(int limit)
        {
            using (await _populateAlbumsMutex.LockAsync())
            {
                var items = await _artist.GetAlbumItemsAsync(limit, Albums.Count);

                await Threading.OnPrimaryThread(() =>
                {
                    foreach (var item in items)
                    {
                        if (item is IAlbum album)
                        {
                            Albums.Add(new AlbumViewModel(album));
                        }
                    }
                });
            }
        }

        /// <inheritdoc />
        public async Task PopulateMoreTracksAsync(int limit)
        {
            using (await _populateTracksMutex.LockAsync())
            {
                var items = await GetTracksAsync(limit, Tracks.Count);

                await Threading.OnPrimaryThread(() =>
                {
                    foreach (var item in items)
                    {
                        Tracks.Add(new TrackViewModel(item));
                    }
                });
            }
        }

        /// <inheritdoc />
        public async Task PopulateMoreImagesAsync(int limit)
        {
            using (await _populateImagesMutex.LockAsync())
            {
                var items = await _artist.GetImagesAsync(limit, Images.Count);

                await Threading.OnPrimaryThread(() =>
                {
                    foreach (var item in items)
                    {
                        Images.Add(item);
                    }
                });
            }
        }

        /// <inheritdoc />
        public async Task PopulateMoreGenresAsync(int limit)
        {
            using (await _populateGenresMutex.LockAsync())
            {
                var items = await _artist.GetGenresAsync(limit, Genres.Count);

                await Threading.OnPrimaryThread(() =>
                {
                    foreach (var item in items)
                    {
                        Genres.Add(item);
                    }
                });
            }
        }

        /// <inheritdoc />
        public async Task PopulateMoreUrlsAsync(int limit)
        {
            using (await _populateUrlsMutex.LockAsync())
            {
                var items = await _artist.GetUrlsAsync(limit, Urls.Count);

                await Threading.OnPrimaryThread(() =>
                {
                    foreach (var item in items)
                    {
                        Urls.Add(item);
                    }
                });
            }
        }

        /// <inheritdoc />
        public Task AddTrackAsync(ITrack track, int index) => _artist.AddTrackAsync(track, index);

        /// <inheritdoc />
        public Task AddAlbumItemAsync(IAlbumCollectionItem album, int index) => _artist.AddAlbumItemAsync(album, index);

        /// <inheritdoc/>
        public Task AddGenreAsync(IGenre genre, int index) => _artist.AddGenreAsync(genre, index);

        /// <inheritdoc />
        public Task AddImageAsync(IImage image, int index) => _artist.AddImageAsync(image, index);

        /// <inheritdoc />
        public Task AddUrlAsync(IUrl image, int index) => _artist.AddUrlAsync(image, index);

        /// <inheritdoc />
        public Task RemoveTrackAsync(int index) => _artist.RemoveTrackAsync(index);

        /// <inheritdoc />
        public Task RemoveAlbumItemAsync(int index) => _artist.RemoveAlbumItemAsync(index);

        /// <inheritdoc/>
        public Task RemoveGenreAsync(int index) => _artist.RemoveGenreAsync(index);

        /// <inheritdoc />
        public Task RemoveImageAsync(int index) => _artist.RemoveImageAsync(index);

        /// <inheritdoc />
        public Task RemoveUrlAsync(int index) => _artist.RemoveUrlAsync(index);

        /// <inheritdoc />
        public IAsyncRelayCommand<IAlbumCollectionItem> PlayAlbumAsyncCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand PlayAlbumCollectionAsyncCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand PauseAlbumCollectionAsyncCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand<ITrack> PlayTrackAsyncCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand PlayTrackCollectionAsyncCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand PauseTrackCollectionAsyncCommand { get; }

        /// <inheritdoc />
        public IRelayCommand<TrackSortingType> ChangeTrackCollectionSortingTypeCommand { get; }

        /// <inheritdoc />
        public IRelayCommand<SortDirection> ChangeTrackCollectionSortingDirectionCommand { get; }

        /// <inheritdoc />
        public IRelayCommand<AlbumSortingType> ChangeAlbumCollectionSortingTypeCommand { get; }

        /// <inheritdoc />
        public IRelayCommand<SortDirection> ChangeAlbumCollectionSortingDirectionCommand { get; }

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
        public IAsyncRelayCommand<int> PopulateMoreTracksCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand<int> PopulateMoreAlbumsCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand<int> PopulateMoreGenresCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand<int> PopulateMoreImagesCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand<int> PopulateMoreUrlsCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand InitAlbumCollectionAsyncCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand InitTrackCollectionAsyncCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand InitGenreCollectionAsyncCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand InitImageCollectionAsyncCommand { get; }

        /// <inheritdoc />
        public Task InitImageCollectionAsync() => CollectionInit.ImageCollection(this);

        /// <inheritdoc />
        public Task InitAlbumCollectionAsync() => CollectionInit.AlbumCollection(this);

        /// <inheritdoc />
        public Task InitTrackCollectionAsync() => CollectionInit.TrackCollection(this);

        /// <inheritdoc />
        public Task InitGenreCollectionAsync() => CollectionInit.GenreCollection(this);

        /// <inheritdoc />
        public bool Equals(ICoreArtistCollectionItem other) => _artist.Equals(other);

        /// <inheritdoc />
        public bool Equals(ICoreAlbumCollectionItem other) => _artist.Equals(other);

        /// <inheritdoc />
        public bool Equals(ICoreAlbumCollection other) => _artist.Equals(other);

        /// <inheritdoc />
        public bool Equals(ICoreTrackCollection other) => _artist.Equals(other);

        /// <inheritdoc />
        public bool Equals(ICoreGenreCollection other) => _artist.Equals(other);

        /// <inheritdoc />
        public bool Equals(ICoreImageCollection other) => _artist.Equals(other);

        /// <inheritdoc />
        public bool Equals(ICoreUrlCollection other) => _artist.Equals(other);

        /// <inheritdoc />
        public bool Equals(ICoreArtist other) => _artist.Equals(other);

        /// <inheritdoc />
        public Task InitAsync()
        {
            if (IsInitialized)
                return Task.CompletedTask;

            IsInitialized = true;

            return Task.WhenAll(InitAlbumCollectionAsync(), InitGenreCollectionAsync(), InitImageCollectionAsync(), InitTrackCollectionAsync());
        }

        private Task ChangeNameInternalAsync(string? name)
        {
            Guard.IsNotNull(name, nameof(name));
            return _artist.ChangeNameAsync(name);
        }

        private Task PlayTrackCollectionInternalAsync(ITrack? track)
        {
            Guard.IsNotNull(track, nameof(track));
            return _playbackHandler.PlayAsync(track, this, _artist);
        }

        private Task PlayAlbumCollectionInternalAsync(IAlbumCollectionItem? albumItem)
        {
            Guard.IsNotNull(albumItem, nameof(albumItem));
            return _playbackHandler.PlayAsync(albumItem, this, this);
        }

        /// <inheritdoc />
        public ValueTask DisposeAsync()
        {
            DetachEvents();
            return _artist.DisposeAsync();
        }
    }
}
