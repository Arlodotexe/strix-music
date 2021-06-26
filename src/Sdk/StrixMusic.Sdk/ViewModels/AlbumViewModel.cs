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
using OwlCore.Collections;
using OwlCore.Events;
using OwlCore.Extensions;
using StrixMusic.Sdk.Data;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.Data.Merged;
using StrixMusic.Sdk.Extensions;
using StrixMusic.Sdk.MediaPlayback;
using StrixMusic.Sdk.Services.MediaPlayback;
using StrixMusic.Sdk.ViewModels.Helpers;

namespace StrixMusic.Sdk.ViewModels
{
    /// <summary>
    /// Contains bindable information about an <see cref="IAlbum"/>.
    /// </summary>
    public class AlbumViewModel : ObservableObject, IAlbum, IArtistCollectionViewModel, ITrackCollectionViewModel, IImageCollectionViewModel
    {
        private readonly IAlbum _album;
        private readonly IPlaybackHandlerService _playbackHandler;

        private readonly AsyncLock _populateTracksMutex = new AsyncLock();
        private readonly AsyncLock _populateArtistsMutex = new AsyncLock();
        private readonly AsyncLock _populateImagesMutex = new AsyncLock();

        /// <summary>
        /// Initializes a new instance of the <see cref="AlbumViewModel"/> class.
        /// </summary>
        /// <param name="album"><inheritdoc cref="IAlbum"/></param>
        public AlbumViewModel(IAlbum album)
        {
            _album = album;

            SourceCores = _album.GetSourceCores<ICoreAlbum>().Select(MainViewModel.GetLoadedCore).ToList();

            _playbackHandler = Ioc.Default.GetRequiredService<IPlaybackHandlerService>();

            using (Threading.PrimaryContext)
            {
                Images = new ObservableCollection<IImage>();
                Tracks = new ObservableCollection<TrackViewModel>();
                Artists = new ObservableCollection<IArtistCollectionItem>();
                UnsortedTracks = new ObservableCollection<TrackViewModel>();
            }

            if (_album.RelatedItems != null)
                RelatedItems = new PlayableCollectionGroupViewModel(_album.RelatedItems);

            PauseTrackCollectionAsyncCommand = new AsyncRelayCommand(PauseTrackCollectionAsync);
            PlayTrackCollectionAsyncCommand = new AsyncRelayCommand(PlayTrackCollectionAsync);
            PauseArtistCollectionAsyncCommand = new AsyncRelayCommand(PauseArtistCollectionAsync);
            PlayArtistCollectionAsyncCommand = new AsyncRelayCommand(PlayArtistCollectionAsync);

            PlayTrackAsyncCommand = new AsyncRelayCommand<ITrack>(PlayTrackCollectionInternalAsync);
            PlayArtistAsyncCommand = new AsyncRelayCommand<IArtistCollectionItem>(PlayArtistCollectionInternalAsync);

            ChangeNameAsyncCommand = new AsyncRelayCommand<string>(ChangeNameInternalAsync);
            ChangeDescriptionAsyncCommand = new AsyncRelayCommand<string?>(ChangeDescriptionAsync);
            ChangeDurationAsyncCommand = new AsyncRelayCommand<TimeSpan>(ChangeDurationAsync);
            PopulateMoreTracksCommand = new AsyncRelayCommand<int>(PopulateMoreTracksAsync);
            PopulateMoreImagesCommand = new AsyncRelayCommand<int>(PopulateMoreImagesAsync);
            PopulateMoreArtistsCommand = new AsyncRelayCommand<int>(PopulateMoreImagesAsync);
            SortTrackCollectionCommand = new RelayCommand<TracksSortType>(SortTrackCollection);

            AttachEvents();
        }

        private void AttachEvents()
        {
            PlaybackStateChanged += AlbumPlaybackStateChanged;
            DescriptionChanged += AlbumDescriptionChanged;
            DatePublishedChanged += AlbumDatePublishedChanged;
            NameChanged += AlbumNameChanged;
            UrlChanged += AlbumUrlChanged;

            IsPlayTrackCollectionAsyncAvailableChanged += OnIsPlayTrackCollectionAsyncAvailableChanged;
            IsPauseTrackCollectionAsyncAvailableChanged += OnIsPauseTrackCollectionAsyncAvailableChanged;
            IsPlayArtistCollectionAsyncAvailableChanged += OnIsPlayArtistCollectionAsyncAvailableChanged;
            IsPauseArtistCollectionAsyncAvailableChanged += OnIsPauseArtistCollectionAsyncAvailableChanged;
            IsChangeNameAsyncAvailableChanged += OnIsChangeNameAsyncAvailableChanged;
            IsChangeDurationAsyncAvailableChanged += OnIsChangeDurationAsyncAvailableChanged;
            IsChangeDescriptionAsyncAvailableChanged += OnIsChangeDescriptionAsyncAvailableChanged;

            TrackItemsCountChanged += AlbumOnTrackItemsCountChanged;
            TrackItemsChanged += AlbumViewModel_TrackItemsChanged;
            ArtistItemsCountChanged += OnArtistItemsCountChanged;
            ArtistItemsChanged += OnArtistItemsChanged;
            ImagesCountChanged += AlbumViewModel_ImagesCountChanged;
            ImagesChanged += AlbumViewModel_ImagesChanged;
            LastPlayedChanged += OnLastPlayedChanged;
        }

        private void DetachEvents()
        {
            PlaybackStateChanged -= AlbumPlaybackStateChanged;
            DescriptionChanged -= AlbumDescriptionChanged;
            DatePublishedChanged -= AlbumDatePublishedChanged;
            NameChanged -= AlbumNameChanged;
            UrlChanged -= AlbumUrlChanged;

            IsPlayTrackCollectionAsyncAvailableChanged -= OnIsPlayTrackCollectionAsyncAvailableChanged;
            IsPauseTrackCollectionAsyncAvailableChanged -= OnIsPauseTrackCollectionAsyncAvailableChanged;
            IsPlayArtistCollectionAsyncAvailableChanged -= OnIsPlayArtistCollectionAsyncAvailableChanged;
            IsPauseArtistCollectionAsyncAvailableChanged -= OnIsPauseArtistCollectionAsyncAvailableChanged;
            IsChangeNameAsyncAvailableChanged -= OnIsChangeNameAsyncAvailableChanged;
            IsChangeDurationAsyncAvailableChanged -= OnIsChangeDurationAsyncAvailableChanged;
            IsChangeDescriptionAsyncAvailableChanged -= OnIsChangeDescriptionAsyncAvailableChanged;

            TrackItemsCountChanged += AlbumOnTrackItemsCountChanged;
            TrackItemsChanged -= AlbumViewModel_TrackItemsChanged;
            ArtistItemsCountChanged -= OnArtistItemsCountChanged;
            ArtistItemsChanged -= OnArtistItemsChanged;
            ImagesCountChanged -= AlbumViewModel_ImagesCountChanged;
            ImagesChanged -= AlbumViewModel_ImagesChanged;
            LastPlayedChanged -= OnLastPlayedChanged;
        }

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
        public event EventHandler<string?>? DescriptionChanged
        {
            add => _album.DescriptionChanged += value;
            remove => _album.DescriptionChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<Uri?>? UrlChanged
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
        public event EventHandler<DateTime?>? LastPlayedChanged
        {
            add => _album.LastPlayedChanged += value;
            remove => _album.LastPlayedChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<bool>? IsChangeNameAsyncAvailableChanged
        {
            add => _album.IsChangeNameAsyncAvailableChanged += value;
            remove => _album.IsChangeNameAsyncAvailableChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<bool>? IsChangeDescriptionAsyncAvailableChanged
        {
            add => _album.IsChangeDescriptionAsyncAvailableChanged += value;
            remove => _album.IsChangeDescriptionAsyncAvailableChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<bool>? IsChangeDurationAsyncAvailableChanged
        {
            add => _album.IsChangeDurationAsyncAvailableChanged += value;
            remove => _album.IsChangeDurationAsyncAvailableChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<DateTime?>? DatePublishedChanged
        {
            add => _album.DatePublishedChanged += value;

            remove => _album.DatePublishedChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<bool>? IsPauseTrackCollectionAsyncAvailableChanged
        {
            add => _album.IsPauseTrackCollectionAsyncAvailableChanged += value;
            remove => _album.IsPauseTrackCollectionAsyncAvailableChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<bool>? IsPlayTrackCollectionAsyncAvailableChanged
        {
            add => _album.IsPlayTrackCollectionAsyncAvailableChanged += value;
            remove => _album.IsPlayTrackCollectionAsyncAvailableChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<bool>? IsPlayArtistCollectionAsyncAvailableChanged
        {
            add => _album.IsPlayArtistCollectionAsyncAvailableChanged += value;
            remove => _album.IsPlayArtistCollectionAsyncAvailableChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<int>? TrackItemsCountChanged
        {
            add => _album.TrackItemsCountChanged += value;
            remove => _album.TrackItemsCountChanged -= value;
        }

        /// <inheritdoc />
        public event CollectionChangedEventHandler<ITrack>? TrackItemsChanged
        {
            add => _album.TrackItemsChanged += value;
            remove => _album.TrackItemsChanged -= value;
        }

        /// <inheritdoc />
        public event CollectionChangedEventHandler<IImage>? ImagesChanged
        {
            add => _album.ImagesChanged += value;
            remove => _album.ImagesChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<int>? ImagesCountChanged
        {
            add => _album.ImagesCountChanged += value;
            remove => _album.ImagesCountChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<bool>? IsPauseArtistCollectionAsyncAvailableChanged
        {
            add => _album.IsPauseArtistCollectionAsyncAvailableChanged += value;
            remove => _album.IsPauseArtistCollectionAsyncAvailableChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<int>? ArtistItemsCountChanged
        {
            add => _album.ArtistItemsCountChanged += value;
            remove => _album.ArtistItemsCountChanged -= value;
        }

        /// <inheritdoc />
        public event CollectionChangedEventHandler<IArtistCollectionItem>? ArtistItemsChanged
        {
            add => _album.ArtistItemsChanged += value;
            remove => _album.ArtistItemsChanged -= value;
        }

        private void AlbumUrlChanged(object sender, Uri? e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(Url)));

        private void AlbumNameChanged(object sender, string e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(Name)));

        private void AlbumDescriptionChanged(object sender, string? e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(Description)));

        private void AlbumPlaybackStateChanged(object sender, PlaybackState e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(PlaybackState)));

        private void AlbumDatePublishedChanged(object sender, DateTime? e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(DatePublished)));

        private void AlbumOnTrackItemsCountChanged(object sender, int e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(TotalTracksCount)));

        private void AlbumViewModel_ImagesCountChanged(object sender, int e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(TotalImageCount)));

        private void OnArtistItemsCountChanged(object sender, int e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(TotalArtistItemsCount)));

        private void OnLastPlayedChanged(object sender, DateTime? e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(LastPlayed)));

        private void OnIsChangeDescriptionAsyncAvailableChanged(object sender, bool e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(IsChangeDescriptionAsyncAvailable)));

        private void OnIsChangeDurationAsyncAvailableChanged(object sender, bool e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(IsChangeDurationAsyncAvailable)));

        private void OnIsChangeNameAsyncAvailableChanged(object sender, bool e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(IsChangeNameAsyncAvailable)));

        private void OnIsPauseTrackCollectionAsyncAvailableChanged(object sender, bool e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(IsPauseTrackCollectionAsyncAvailable)));

        private void OnIsPlayTrackCollectionAsyncAvailableChanged(object sender, bool e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(IsPlayTrackCollectionAsyncAvailable)));

        private void OnIsPauseArtistCollectionAsyncAvailableChanged(object sender, bool e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(IsPauseArtistCollectionAsyncAvailable)));

        private void OnIsPlayArtistCollectionAsyncAvailableChanged(object sender, bool e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(IsPlayArtistCollectionAsyncAvailable)));

        private void AlbumViewModel_TrackItemsChanged(object sender, IReadOnlyList<CollectionChangedItem<ITrack>> addedItems, IReadOnlyList<CollectionChangedItem<ITrack>> removedItems)
        {
            _ = Threading.OnPrimaryThread(() =>
            {
                Tracks.ChangeCollection(addedItems, removedItems, x => new TrackViewModel(x.Data));
            });
        }

        private void OnArtistItemsChanged(object sender, IReadOnlyList<CollectionChangedItem<IArtistCollectionItem>> addedItems, IReadOnlyList<CollectionChangedItem<IArtistCollectionItem>> removedItems)
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

        private void AlbumViewModel_ImagesChanged(object sender, IReadOnlyList<CollectionChangedItem<IImage>> addedItems, IReadOnlyList<CollectionChangedItem<IImage>> removedItems)
        {
            _ = Threading.OnPrimaryThread(() =>
            {
                Images.ChangeCollection(addedItems, removedItems);
            });
        }

        /// <inheritdoc />
        public string Id => _album.Id;

        /// <inheritdoc cref="IMerged{T}.SourceCores" />
        public IReadOnlyList<ICore> SourceCores { get; }

        /// <summary>
        /// The merged sources for this album.
        /// </summary>
        public IReadOnlyList<ICoreAlbum> Sources => _album.GetSources<ICoreAlbum>();

        /// <summary>
        /// The merged sources for this album.
        /// </summary>
        IReadOnlyList<ICoreAlbum> IMerged<ICoreAlbum>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreImageCollection> IMerged<ICoreImageCollection>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreTrackCollection> IMerged<ICoreTrackCollection>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreArtistCollection> IMerged<ICoreArtistCollection>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreArtistCollectionItem> IMerged<ICoreArtistCollectionItem>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreAlbumCollectionItem> IMerged<ICoreAlbumCollectionItem>.Sources => Sources;

        /// <inheritdoc />
        public TimeSpan Duration => _album.Duration;

        /// <inheritdoc />
        public DateTime? LastPlayed => _album.LastPlayed;

        /// <inheritdoc />
        public DateTime? AddedAt => _album.AddedAt;

        /// <inheritdoc />
        public IPlayableCollectionGroup? RelatedItems { get; }

        /// <inheritdoc />
        public ObservableCollection<IImage> Images { get; }

        /// <inheritdoc />
        public SynchronizedObservableCollection<string>? Genres => _album.Genres;

        /// <summary>
        /// The tracks for this album.
        /// </summary>
        public ObservableCollection<TrackViewModel> Tracks { get; private set; }

        /// <inheritdoc />
        public ObservableCollection<TrackViewModel> UnsortedTracks { get; private set; }

        /// <inheritdoc />
        public ObservableCollection<IArtistCollectionItem> Artists { get; }

        /// <inheritdoc />
        public string Name => _album.Name;

        /// <inheritdoc />
        public int TotalTracksCount => _album.TotalTracksCount;

        /// <inheritdoc />
        public int TotalArtistItemsCount => _album.TotalArtistItemsCount;

        /// <inheritdoc />
        public int TotalImageCount => _album.TotalImageCount;

        /// <inheritdoc />
        public bool IsPlayTrackCollectionAsyncAvailable => _album.IsPlayTrackCollectionAsyncAvailable;

        /// <inheritdoc />
        public bool IsPauseTrackCollectionAsyncAvailable => _album.IsPauseTrackCollectionAsyncAvailable;

        /// <inheritdoc />
        public bool IsPlayArtistCollectionAsyncAvailable => _album.IsPlayArtistCollectionAsyncAvailable;

        /// <inheritdoc />
        public bool IsPauseArtistCollectionAsyncAvailable => _album.IsPauseArtistCollectionAsyncAvailable;

        /// <inheritdoc />
        public Task PlayTrackCollectionAsync()
        {
            return _playbackHandler.PlayAsync((ITrackCollectionViewModel)this, _album);
        }

        /// <inheritdoc />
        public Task PauseTrackCollectionAsync()
        {
            return _album.PauseTrackCollectionAsync();
        }

        /// <inheritdoc />
        public Task PlayArtistCollectionAsync()
        {
            return _playbackHandler.PlayAsync((IArtistCollectionViewModel)this, _album);
        }

        /// <inheritdoc />
        public Task PauseArtistCollectionAsync()
        {
            return _playbackHandler.PauseAsync();
        }

        /// <inheritdoc />
        public Task PlayTrackCollectionAsync(ITrack track) => PlayTrackCollectionInternalAsync(track);

        /// <inheritdoc />
        public Task PlayArtistCollectionAsync(IArtistCollectionItem artistItem) => PlayArtistCollectionInternalAsync(artistItem);

        /// <inheritdoc />
        public Uri? Url => _album.Url;

        /// <inheritdoc />
        public DateTime? DatePublished => _album.DatePublished;

        /// <inheritdoc />
        public string? Description => _album.Description;

        /// <inheritdoc />
        public PlaybackState PlaybackState => _album.PlaybackState;

        /// <inheritdoc />
        public bool IsChangeNameAsyncAvailable => _album.IsChangeNameAsyncAvailable;

        /// <inheritdoc />
        public bool IsChangeDescriptionAsyncAvailable => _album.IsChangeDescriptionAsyncAvailable;

        /// <inheritdoc />
        public bool IsChangeDatePublishedAsyncAvailable => _album.IsChangeDatePublishedAsyncAvailable;

        /// <inheritdoc />
        public bool IsChangeDurationAsyncAvailable => _album.IsChangeDurationAsyncAvailable;

        /// <inheritdoc />
        public Task ChangeNameAsync(string name) => ChangeNameInternalAsync(name);

        /// <inheritdoc />
        public Task ChangeDescriptionAsync(string? description) => _album.ChangeDescriptionAsync(description);

        /// <inheritdoc />
        public Task ChangeDurationAsync(TimeSpan duration) => _album.ChangeDurationAsync(duration);

        /// <inheritdoc />
        public Task<bool> IsAddImageAvailable(int index) => _album.IsAddImageAvailable(index);

        /// <inheritdoc />
        public Task<bool> IsAddTrackAvailable(int index) => _album.IsAddTrackAvailable(index);

        /// <inheritdoc />
        public Task<bool> IsAddArtistItemAvailable(int index) => _album.IsAddArtistItemAvailable(index);

        /// <inheritdoc />
        public Task<bool> IsAddGenreAvailable(int index) => _album.IsAddGenreAvailable(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveImageAvailable(int index) => _album.IsRemoveImageAvailable(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveGenreAvailable(int index) => _album.IsRemoveGenreAvailable(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveTrackAvailable(int index) => _album.IsRemoveTrackAvailable(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveArtistItemAvailable(int index) => _album.IsRemoveArtistItemAvailable(index);

        /// <inheritdoc />
        public Task ChangeDatePublishedAsync(DateTime datePublished) => _album.ChangeDatePublishedAsync(datePublished);

        /// <inheritdoc />
        public Task AddTrackAsync(ITrack track, int index) => _album.AddTrackAsync(track, index);

        /// <inheritdoc />
        public Task RemoveTrackAsync(int index) => _album.RemoveTrackAsync(index);

        /// <inheritdoc />
        public Task AddImageAsync(IImage image, int index) => _album.AddImageAsync(image, index);

        /// <inheritdoc />
        public Task RemoveImageAsync(int index) => _album.RemoveImageAsync(index);

        /// <inheritdoc />
        public Task AddArtistItemAsync(IArtistCollectionItem artist, int index) => _album.AddArtistItemAsync(artist, index);

        /// <inheritdoc />
        public Task RemoveArtistItemAsync(int index) => _album.RemoveArtistItemAsync(index);

        ///<inheritdoc />
        public void SortTrackCollection(TracksSortType tracksSortType)
        {
            TracksHelper.SortTracks(Tracks, tracksSortType, UnsortedTracks);

            OnPropertyChanged(nameof(Tracks)); // letting UI know that the order has changed.
        }

        /// <inheritdoc />
        public Task<IReadOnlyList<ITrack>> GetTracksAsync(int limit, int offset) => _album.GetTracksAsync(limit, offset);

        /// <inheritdoc />
        public async Task PopulateMoreTracksAsync(int limit)
        {
            using (await _populateTracksMutex.LockAsync())
            {
                foreach (var item in await _album.GetTracksAsync(limit, Tracks.Count))
                {
                    Tracks.Add(new TrackViewModel(item));
                }
            }
        }

        /// <inheritdoc />
        public Task<IReadOnlyList<IImage>> GetImagesAsync(int limit, int offset)
        {
            return _album.GetImagesAsync(limit, offset);
        }

        /// <inheritdoc />
        public Task<IReadOnlyList<IArtistCollectionItem>> GetArtistItemsAsync(int limit, int offset)
        {
            return _album.GetArtistItemsAsync(limit, offset);
        }

        /// <inheritdoc />
        public async Task PopulateMoreImagesAsync(int limit)
        {
            using (await _populateImagesMutex.LockAsync())
            {
                var items = await _album.GetImagesAsync(limit, Images.Count);

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
        public async Task PopulateMoreArtistsAsync(int limit)
        {
            using (await _populateArtistsMutex.LockAsync())
            {
                var items = await _album.GetArtistItemsAsync(limit, Artists.Count);

                _ = Threading.OnPrimaryThread(() =>
                {
                    foreach (var item in items)
                    {
                        if (item is IArtist artist)
                        {
                            Artists.Add(new ArtistViewModel(artist));
                        }
                    }
                });
            }
        }

        /// <inheritdoc />
        public IAsyncRelayCommand<int> PopulateMoreImagesCommand { get; }

        /// <inheritdoc />
        public RelayCommand<TracksSortType> SortTrackCollectionCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand<int> PopulateMoreTracksCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand PlayTrackCollectionAsyncCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand<ITrack> PlayTrackAsyncCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand PauseTrackCollectionAsyncCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand<int> PopulateMoreArtistsCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand PlayArtistCollectionAsyncCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand<IArtistCollectionItem> PlayArtistAsyncCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand PauseArtistCollectionAsyncCommand { get; }

        /// <summary>
        /// Attempts to change the name of the album, if supported.
        /// </summary>
        public IAsyncRelayCommand<string> ChangeNameAsyncCommand { get; }

        /// <summary>
        /// Attempts to change the description of the album, if supported.
        /// </summary>
        public IAsyncRelayCommand<string?> ChangeDescriptionAsyncCommand { get; }

        /// <summary>
        /// Attempts to change the duration of the album, if supported.
        /// </summary>
        public IAsyncRelayCommand<TimeSpan> ChangeDurationAsyncCommand { get; }

        /// <inheritdoc />
        public bool Equals(ICoreAlbumCollectionItem other) => _album.Equals(other);

        /// <inheritdoc />
        public bool Equals(ICoreArtistCollectionItem other) => _album.Equals(other);

        /// <inheritdoc />
        public bool Equals(ICoreImageCollection other) => _album.Equals(other);

        /// <inheritdoc />
        public bool Equals(ICoreArtistCollection other) => _album.Equals(other);

        /// <inheritdoc />
        public bool Equals(ICoreTrackCollection other) => _album.Equals(other);

        /// <inheritdoc />
        public bool Equals(ICoreAlbum other) => _album.Equals(other);

        /// <inheritdoc />
        public async Task InitAsync()
        {
            if (IsInitialized)
                return;

            IsInitialized = true;

            await CollectionInit.TrackCollection(this);
        }

        /// <inheritdoc />
        public bool IsInitialized { get; private set; }

        private Task PlayArtistCollectionInternalAsync(IArtistCollectionItem? artistItem)
        {
            Guard.IsNotNull(artistItem, nameof(artistItem));
            return _playbackHandler.PlayAsync(artistItem, this, this);
        }

        private Task PlayTrackCollectionInternalAsync(ITrack? track)
        {
            Guard.IsNotNull(track, nameof(track));
            return _playbackHandler.PlayAsync(track, this, this);
        }

        private Task ChangeNameInternalAsync(string? name)
        {
            Guard.IsNotNull(name, nameof(name));
            return _album.ChangeNameAsync(name);
        }

        /// <inheritdoc />
        public ValueTask DisposeAsync()
        {
            DetachEvents();
            return _album.DisposeAsync();
        }
    }
}
