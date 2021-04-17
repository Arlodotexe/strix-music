using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Toolkit.Diagnostics;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Input;
using OwlCore;
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
    /// A wrapper for <see cref="ITrackCollection"/> that contains props and methods for a ViewModel.
    /// </summary>
    public class TrackCollectionViewModel : ObservableObject, ITrackCollectionViewModel, IImageCollectionViewModel
    {
        private readonly ITrackCollection _collection;
        private readonly IPlaybackHandlerService _playbackHandlerService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ITrackCollectionViewModel"/> class.
        /// </summary>
        /// <param name="collection">The base <see cref="ITrackCollection"/> containing properties about this class.</param>
        public TrackCollectionViewModel(ITrackCollection collection)
        {
            _collection = collection ?? throw new ArgumentNullException();

            using (Threading.PrimaryContext)
            {
                Images = new ObservableCollection<IImage>();
                Tracks = new ObservableCollection<TrackViewModel>();
            }

            PopulateMoreTracksCommand = new AsyncRelayCommand<int>(PopulateMoreTracksAsync);
            PopulateMoreImagesCommand = new AsyncRelayCommand<int>(PopulateMoreImagesAsync);

            PauseTrackCollectionAsyncCommand = new AsyncRelayCommand(PauseTrackCollectionAsync);
            PlayTrackCollectionAsyncCommand = new AsyncRelayCommand(PlayTrackCollectionAsync);

            PlayTrackAsyncCommand = new AsyncRelayCommand<ITrack>(PlayTrackCollectionInternalAsync);

            SourceCores = collection.GetSourceCores<ICoreTrackCollection>().Select(MainViewModel.GetLoadedCore).ToList();
            _playbackHandlerService = Ioc.Default.GetRequiredService<IPlaybackHandlerService>();

            AttachEvents();
        }

        private void AttachEvents()
        {
            PlaybackStateChanged += OnPlaybackStateChanged;
            NameChanged += OnNameChanged;
            DescriptionChanged += OnDescriptionChanged;
            UrlChanged += OnUrlChanged;
            LastPlayedChanged += OnLastPlayedChanged;

            IsPlayTrackCollectionAsyncAvailableChanged += OnIsPlayTrackCollectionAsyncAvailableChanged;
            IsPauseTrackCollectionAsyncAvailableChanged += OnIsPauseTrackCollectionAsyncAvailableChanged;

            IsChangeNameAsyncAvailableChanged += OnIsChangeNameAsyncAvailableChanged;
            IsChangeDurationAsyncAvailableChanged += OnIsChangeDurationAsyncAvailableChanged;
            IsChangeDescriptionAsyncAvailableChanged += OnIsChangeDescriptionAsyncAvailableChanged;

            TrackItemsCountChanged += OnTrackItemsCountChanged;
            TrackItemsChanged += TrackCollectionViewModel_TrackItemsChanged;
            ImagesChanged += TrackCollectionViewModel_ImagesChanged;
            ImagesCountChanged += TrackCollectionViewModel_ImagesCountChanged;
        }

        private void DetachEvents()
        {
            PlaybackStateChanged -= OnPlaybackStateChanged;
            NameChanged -= OnNameChanged;
            DescriptionChanged -= OnDescriptionChanged;
            UrlChanged -= OnUrlChanged;
            LastPlayedChanged -= OnLastPlayedChanged;

            IsPlayTrackCollectionAsyncAvailableChanged -= OnIsPlayTrackCollectionAsyncAvailableChanged;
            IsPauseTrackCollectionAsyncAvailableChanged -= OnIsPauseTrackCollectionAsyncAvailableChanged;

            IsChangeNameAsyncAvailableChanged -= OnIsChangeNameAsyncAvailableChanged;
            IsChangeDurationAsyncAvailableChanged -= OnIsChangeDurationAsyncAvailableChanged;
            IsChangeDescriptionAsyncAvailableChanged -= OnIsChangeDescriptionAsyncAvailableChanged;

            TrackItemsCountChanged -= OnTrackItemsCountChanged;
            TrackItemsChanged -= TrackCollectionViewModel_TrackItemsChanged;
            ImagesChanged -= TrackCollectionViewModel_ImagesChanged;
            ImagesCountChanged -= TrackCollectionViewModel_ImagesCountChanged;
        }

        private void OnUrlChanged(object sender, Uri? e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(Url)));

        private void OnNameChanged(object sender, string e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(Name)));

        private void OnDescriptionChanged(object sender, string? e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(Description)));

        private void OnPlaybackStateChanged(object sender, PlaybackState e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(PlaybackState)));

        private void OnLastPlayedChanged(object sender, DateTime? e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(LastPlayed)));

        private void OnTrackItemsCountChanged(object sender, int e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(TotalTracksCount)));

        private void TrackCollectionViewModel_ImagesCountChanged(object sender, int e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(TotalImageCount)));

        private void OnIsChangeDescriptionAsyncAvailableChanged(object sender, bool e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(IsChangeDescriptionAsyncAvailable)));

        private void OnIsChangeDurationAsyncAvailableChanged(object sender, bool e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(IsChangeDurationAsyncAvailable)));

        private void OnIsChangeNameAsyncAvailableChanged(object sender, bool e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(IsChangeNameAsyncAvailable)));

        private void OnIsPauseTrackCollectionAsyncAvailableChanged(object sender, bool e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(IsPauseTrackCollectionAsyncAvailable)));

        private void OnIsPlayTrackCollectionAsyncAvailableChanged(object sender, bool e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(IsPlayTrackCollectionAsyncAvailable)));

        private void TrackCollectionViewModel_ImagesChanged(object sender, IReadOnlyList<CollectionChangedItem<IImage>> addedItems, IReadOnlyList<CollectionChangedItem<IImage>> removedItems)
        {
            _ = Threading.OnPrimaryThread(() =>
            {
                Images.ChangeCollection(addedItems, removedItems);
            });
        }

        private void TrackCollectionViewModel_TrackItemsChanged(object sender, IReadOnlyList<CollectionChangedItem<ITrack>> addedItems, IReadOnlyList<CollectionChangedItem<ITrack>> removedItems)
        {
            _ = Threading.OnPrimaryThread(() =>
            {
                Tracks.ChangeCollection(addedItems, removedItems, item => new TrackViewModel(item.Data));
            });
        }

        /// <inheritdoc />
        public event EventHandler<bool>? IsPlayTrackCollectionAsyncAvailableChanged
        {
            add => _collection.IsPlayTrackCollectionAsyncAvailableChanged += value;
            remove => _collection.IsPlayTrackCollectionAsyncAvailableChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<bool>? IsPauseTrackCollectionAsyncAvailableChanged
        {
            add => _collection.IsPauseTrackCollectionAsyncAvailableChanged += value;
            remove => _collection.IsPauseTrackCollectionAsyncAvailableChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<PlaybackState>? PlaybackStateChanged
        {
            add => _collection.PlaybackStateChanged += value;
            remove => _collection.PlaybackStateChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<string>? NameChanged
        {
            add => _collection.NameChanged += value;
            remove => _collection.NameChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<string?>? DescriptionChanged
        {
            add => _collection.DescriptionChanged += value;
            remove => _collection.DescriptionChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<Uri?>? UrlChanged
        {
            add => _collection.UrlChanged += value;
            remove => _collection.UrlChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<TimeSpan>? DurationChanged
        {
            add => _collection.DurationChanged += value;
            remove => _collection.DurationChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<DateTime?>? LastPlayedChanged
        {
            add => _collection.LastPlayedChanged += value;
            remove => _collection.LastPlayedChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<bool>? IsChangeNameAsyncAvailableChanged
        {
            add => _collection.IsChangeNameAsyncAvailableChanged += value;
            remove => _collection.IsChangeNameAsyncAvailableChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<bool>? IsChangeDescriptionAsyncAvailableChanged
        {
            add => _collection.IsChangeDescriptionAsyncAvailableChanged += value;
            remove => _collection.IsChangeDescriptionAsyncAvailableChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<bool>? IsChangeDurationAsyncAvailableChanged
        {
            add => _collection.IsChangeDurationAsyncAvailableChanged += value;
            remove => _collection.IsChangeDurationAsyncAvailableChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<int>? TrackItemsCountChanged
        {
            add => _collection.TrackItemsCountChanged += value;
            remove => _collection.TrackItemsCountChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<int>? ImagesCountChanged
        {
            add => _collection.ImagesCountChanged += value;
            remove => _collection.ImagesCountChanged -= value;
        }

        /// <inheritdoc />
        public event CollectionChangedEventHandler<IImage>? ImagesChanged
        {
            add => _collection.ImagesChanged += value;
            remove => _collection.ImagesChanged -= value;
        }

        /// <inheritdoc />
        public event CollectionChangedEventHandler<ITrack>? TrackItemsChanged
        {
            add => _collection.TrackItemsChanged += value;
            remove => _collection.TrackItemsChanged -= value;
        }

        /// <inheritdoc />
        public string Id => _collection.Id;

        /// <inheritdoc />
        public bool IsPlayTrackCollectionAsyncAvailable => _collection.IsPlayTrackCollectionAsyncAvailable;

        /// <inheritdoc />
        public bool IsPauseTrackCollectionAsyncAvailable => _collection.IsPauseTrackCollectionAsyncAvailable;

        /// <inheritdoc />
        public bool IsChangeNameAsyncAvailable => _collection.IsChangeNameAsyncAvailable;

        /// <inheritdoc />
        public bool IsChangeDescriptionAsyncAvailable => _collection.IsChangeDescriptionAsyncAvailable;

        /// <inheritdoc />
        public bool IsChangeDurationAsyncAvailable => _collection.IsChangeDurationAsyncAvailable;

        /// <inheritdoc />
        public Uri? Url => _collection.Url;

        /// <inheritdoc />
        public string Name => _collection.Name;

        /// <inheritdoc />
        public int TotalTracksCount => _collection.TotalTracksCount;

        /// <inheritdoc />
        public int TotalImageCount => _collection.TotalImageCount;

        /// <inheritdoc />
        public string? Description => _collection.Description;

        /// <inheritdoc />
        public PlaybackState PlaybackState => _collection.PlaybackState;

        /// <inheritdoc />
        public TimeSpan Duration => _collection.Duration;

        /// <inheritdoc />
        public DateTime? LastPlayed => _collection.LastPlayed;

        /// <inheritdoc />
        public DateTime? AddedAt => _collection.AddedAt;

        /// <inheritdoc />
        public ObservableCollection<TrackViewModel> Tracks { get; }

        /// <inheritdoc />
        public ObservableCollection<IImage> Images { get; }

        /// <inheritdoc cref="IMerged{T}.SourceCores" />
        public IReadOnlyList<ICore> SourceCores { get; }

        /// <summary>
        /// The sources that were merged into this collection.
        /// </summary>
        public IReadOnlyList<ICoreTrackCollection> Sources => _collection.GetSources<ICoreTrackCollection>();

        /// <inheritdoc />
        IReadOnlyList<ICoreImageCollection> IMerged<ICoreImageCollection>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreTrackCollection> IMerged<ICoreTrackCollection>.Sources => Sources;

        /// <inheritdoc />
        public Task<bool> IsAddTrackAvailable(int index) => _collection.IsAddTrackAvailable(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveTrackAvailable(int index) => _collection.IsRemoveTrackAvailable(index);

        /// <inheritdoc />
        public Task<bool> IsAddImageAvailable(int index) => _collection.IsAddImageAvailable(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveImageAvailable(int index) => _collection.IsRemoveImageAvailable(index);

        /// <inheritdoc />
        public Task AddTrackAsync(ITrack track, int index) => _collection.AddTrackAsync(track, index);

        /// <inheritdoc />
        public Task RemoveTrackAsync(int index) => _collection.RemoveTrackAsync(index);

        /// <inheritdoc />
        public Task AddImageAsync(IImage image, int index) => _collection.AddImageAsync(image, index);

        /// <inheritdoc />
        public Task RemoveImageAsync(int index) => _collection.RemoveImageAsync(index);

        /// <inheritdoc />
        public Task<IReadOnlyList<ITrack>> GetTracksAsync(int limit, int offset) => _collection.GetTracksAsync(limit, offset);

        /// <inheritdoc />
        public Task<IReadOnlyList<IImage>> GetImagesAsync(int limit, int offset) => _collection.GetImagesAsync(limit, offset);

        /// <inheritdoc />
        public Task PlayTrackCollectionAsync()
        {
            return _playbackHandlerService.PlayAsync(this, _collection);
        }

        /// <inheritdoc />
        public Task PlayTrackCollectionAsync(ITrack track) => PlayTrackCollectionInternalAsync(track);

        /// <inheritdoc />
        public Task PauseTrackCollectionAsync()
        {
            return _collection.PauseTrackCollectionAsync();
        }

        /// <inheritdoc />
        public Task ChangeNameAsync(string name) => _collection.ChangeNameAsync(name);

        /// <inheritdoc />
        public Task ChangeDescriptionAsync(string? description) => _collection.ChangeDescriptionAsync(description);

        /// <inheritdoc />
        public Task ChangeDurationAsync(TimeSpan duration) => _collection.ChangeDurationAsync(duration);

        /// <inheritdoc />
        public async Task PopulateMoreTracksAsync(int limit)
        {
            var items = await _collection.GetTracksAsync(limit, Tracks.Count);

            _ = Threading.OnPrimaryThread(() =>
            {
                foreach (var item in items)
                {
                    Tracks.Add(new TrackViewModel(item));
                }
            });
        }

        /// <inheritdoc />
        public async Task PopulateMoreImagesAsync(int limit)
        {
            var items = await _collection.GetImagesAsync(limit, Images.Count);

            _ = Threading.OnPrimaryThread(() =>
            {
                foreach (var item in items)
                {
                    Images.Add(item);
                }
            });
        }

        /// <inheritdoc />
        public IAsyncRelayCommand<int> PopulateMoreTracksCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand<int> PopulateMoreImagesCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand PlayTrackCollectionAsyncCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand<ITrack> PlayTrackAsyncCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand PauseTrackCollectionAsyncCommand { get; }

        /// <inheritdoc />
        public bool Equals(ICoreImageCollection other) => _collection.Equals(other);

        /// <inheritdoc />
        public bool Equals(ICoreTrackCollection other) => _collection.Equals(other);

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

        /// <inheritdoc />
        private Task PlayTrackCollectionInternalAsync(ITrack? track)
        {
            Guard.IsNotNull(track, nameof(track));
            return _collection.PlayTrackCollectionAsync(track);
        }

        /// <inheritdoc />
        public ValueTask DisposeAsync()
        {
            DetachEvents();
            return _collection.DisposeAsync();
        }
    }
}