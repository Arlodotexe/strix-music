// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Toolkit.Diagnostics;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Input;
using OwlCore;
using OwlCore.Events;
using OwlCore.Extensions;
using StrixMusic.Sdk.Extensions;
using StrixMusic.Sdk.MediaPlayback;
using StrixMusic.Sdk.Models;
using StrixMusic.Sdk.Models.Core;
using StrixMusic.Sdk.Models.Merged;
using StrixMusic.Sdk.ViewModels.Helpers;

namespace StrixMusic.Sdk.ViewModels
{
    /// <summary>
    /// A wrapper for <see cref="ITrackCollection"/> that contains props and methods for a ViewModel.
    /// </summary>
    public class TrackCollectionViewModel : ObservableObject, ISdkViewModel, ITrackCollectionViewModel, IImageCollectionViewModel
    {
        private readonly ITrackCollection _collection;

        private readonly SemaphoreSlim _populateTracksMutex = new SemaphoreSlim(1, 1);
        private readonly SemaphoreSlim _populateImagesMutex = new SemaphoreSlim(1, 1);
        private readonly SemaphoreSlim _populateUrlsMutex = new SemaphoreSlim(1, 1);

        /// <summary>
        /// Initializes a new instance of the <see cref="ITrackCollectionViewModel"/> class.
        /// </summary>
        /// <param name="root">The <see cref="MainViewModel"/> that this or the object that created this originated from.</param>
        /// <param name="collection">The base <see cref="ITrackCollection"/> containing properties about this class.</param>
        public TrackCollectionViewModel(MainViewModel root, ITrackCollection collection)
        {
            _collection = root.Plugins.ModelPlugins.TrackCollection.Execute(collection);
            Root = root;

            using (Threading.PrimaryContext)
            {
                Tracks = new ObservableCollection<TrackViewModel>();
                Images = new ObservableCollection<IImage>();
                Urls = new ObservableCollection<IUrl>();

                UnsortedTracks = new ObservableCollection<TrackViewModel>();
            }

            PopulateMoreTracksCommand = new AsyncRelayCommand<int>(PopulateMoreTracksAsync);
            PopulateMoreImagesCommand = new AsyncRelayCommand<int>(PopulateMoreImagesAsync);
            PopulateMoreUrlsCommand = new AsyncRelayCommand<int>(PopulateMoreUrlsAsync);

            PauseTrackCollectionAsyncCommand = new AsyncRelayCommand(PauseTrackCollectionAsync);
            PlayTrackCollectionAsyncCommand = new AsyncRelayCommand(PlayTrackCollectionAsync);

            PlayTrackAsyncCommand = new AsyncRelayCommand<ITrack>(x => _collection.PlayTrackCollectionAsync(x ?? ThrowHelper.ThrowArgumentNullException<ITrack>(nameof(x))));

            ChangeNameAsyncCommand = new AsyncRelayCommand<string>(ChangeNameInternalAsync);
            ChangeDescriptionAsyncCommand = new AsyncRelayCommand<string?>(ChangeDescriptionAsync);
            ChangeDurationAsyncCommand = new AsyncRelayCommand<TimeSpan>(ChangeDurationAsync);

            ChangeTrackCollectionSortingTypeCommand = new RelayCommand<TrackSortingType>(x => SortTrackCollection(x, CurrentTracksSortingDirection));
            ChangeTrackCollectionSortingDirectionCommand = new RelayCommand<SortDirection>(x => SortTrackCollection(CurrentTracksSortingType, x));

            InitImageCollectionAsyncCommand = new AsyncRelayCommand(InitImageCollectionAsync);
            InitTrackCollectionAsyncCommand = new AsyncRelayCommand(InitTrackCollectionAsync);

            SourceCores = collection.GetSourceCores<ICoreTrackCollection>().Select(root.GetLoadedCore).ToList();

            AttachEvents();
        }

        /// <inheritdoc />
        public Task InitAsync()
        {
            if (IsInitialized)
                return Task.CompletedTask;

            IsInitialized = true;

            return Task.WhenAll(InitImageCollectionAsync(), InitTrackCollectionAsync());
        }

        private void AttachEvents()
        {
            PlaybackStateChanged += OnPlaybackStateChanged;
            NameChanged += OnNameChanged;
            DescriptionChanged += OnDescriptionChanged;
            LastPlayedChanged += OnLastPlayedChanged;
            Flow.Catch<NotSupportedException>(() => DownloadInfoChanged += OnDownloadInfoChanged);

            IsPlayTrackCollectionAsyncAvailableChanged += OnIsPlayTrackCollectionAsyncAvailableChanged;
            IsPauseTrackCollectionAsyncAvailableChanged += OnIsPauseTrackCollectionAsyncAvailableChanged;

            IsChangeNameAsyncAvailableChanged += OnIsChangeNameAsyncAvailableChanged;
            IsChangeDurationAsyncAvailableChanged += OnIsChangeDurationAsyncAvailableChanged;
            IsChangeDescriptionAsyncAvailableChanged += OnIsChangeDescriptionAsyncAvailableChanged;

            TracksCountChanged += OnTrackItemsCountChanged;
            TracksChanged += TrackCollectionViewModel_TrackItemsChanged;
            ImagesChanged += TrackCollectionViewModel_ImagesChanged;
            ImagesCountChanged += TrackCollectionViewModel_ImagesCountChanged;
            UrlsChanged += TrackCollectionViewModel_UrlsChanged;
            UrlsCountChanged += TrackCollectionViewModel_UrlsCountChanged;
        }

        private void DetachEvents()
        {
            PlaybackStateChanged -= OnPlaybackStateChanged;
            NameChanged -= OnNameChanged;
            DescriptionChanged -= OnDescriptionChanged;
            LastPlayedChanged -= OnLastPlayedChanged;
            Flow.Catch<NotSupportedException>(() => DownloadInfoChanged -= OnDownloadInfoChanged);

            IsPlayTrackCollectionAsyncAvailableChanged -= OnIsPlayTrackCollectionAsyncAvailableChanged;
            IsPauseTrackCollectionAsyncAvailableChanged -= OnIsPauseTrackCollectionAsyncAvailableChanged;

            IsChangeNameAsyncAvailableChanged -= OnIsChangeNameAsyncAvailableChanged;
            IsChangeDurationAsyncAvailableChanged -= OnIsChangeDurationAsyncAvailableChanged;
            IsChangeDescriptionAsyncAvailableChanged -= OnIsChangeDescriptionAsyncAvailableChanged;

            TracksCountChanged -= OnTrackItemsCountChanged;
            TracksChanged -= TrackCollectionViewModel_TrackItemsChanged;
            ImagesChanged -= TrackCollectionViewModel_ImagesChanged;
            ImagesCountChanged -= TrackCollectionViewModel_ImagesCountChanged;
            UrlsChanged -= TrackCollectionViewModel_UrlsChanged;
            UrlsCountChanged -= TrackCollectionViewModel_UrlsCountChanged;
        }

        private void OnNameChanged(object sender, string e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(Name)));

        private void OnDescriptionChanged(object sender, string? e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(Description)));

        private void OnPlaybackStateChanged(object sender, PlaybackState e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(PlaybackState)));
        private void OnDownloadInfoChanged(object sender, DownloadInfo e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(DownloadInfo)));

        private void OnLastPlayedChanged(object sender, DateTime? e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(LastPlayed)));

        private void OnTrackItemsCountChanged(object sender, int e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(TotalTrackCount)));

        private void TrackCollectionViewModel_ImagesCountChanged(object sender, int e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(TotalImageCount)));

        private void TrackCollectionViewModel_UrlsCountChanged(object sender, int e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(TotalUrlCount)));

        private void OnIsChangeDescriptionAsyncAvailableChanged(object sender, bool e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(IsChangeDescriptionAsyncAvailable)));

        private void OnIsChangeDurationAsyncAvailableChanged(object sender, bool e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(IsChangeDurationAsyncAvailable)));

        private void OnIsChangeNameAsyncAvailableChanged(object sender, bool e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(IsChangeNameAsyncAvailable)));

        private void OnIsPauseTrackCollectionAsyncAvailableChanged(object sender, bool e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(IsPauseTrackCollectionAsyncAvailable)));

        private void OnIsPlayTrackCollectionAsyncAvailableChanged(object sender, bool e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(IsPlayTrackCollectionAsyncAvailable)));

        private void TrackCollectionViewModel_TrackItemsChanged(object sender, IReadOnlyList<CollectionChangedItem<ITrack>> addedItems, IReadOnlyList<CollectionChangedItem<ITrack>> removedItems)
        {
            _ = Threading.OnPrimaryThread(() =>
            {
                if (CurrentTracksSortingType == TrackSortingType.Unsorted)
                {
                    Tracks.ChangeCollection(addedItems, removedItems, x => new TrackViewModel(Root, x.Data));
                }
                else
                {
                    // Preventing index issues during tracks emission from the core, also making sure that unordered tracks updated. 
                    UnsortedTracks.ChangeCollection(addedItems, removedItems, x => new TrackViewModel(Root, x.Data));

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

        private void TrackCollectionViewModel_ImagesChanged(object sender, IReadOnlyList<CollectionChangedItem<IImage>> addedItems, IReadOnlyList<CollectionChangedItem<IImage>> removedItems)
        {
            _ = Threading.OnPrimaryThread(() =>
            {
                Images.ChangeCollection(addedItems, removedItems);
            });
        }

        private void TrackCollectionViewModel_UrlsChanged(object sender, IReadOnlyList<CollectionChangedItem<IUrl>> addedItems, IReadOnlyList<CollectionChangedItem<IUrl>> removedItems)
        {
            _ = Threading.OnPrimaryThread(() =>
            {
                Urls.ChangeCollection(addedItems, removedItems);
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
        public event EventHandler<DownloadInfo>? DownloadInfoChanged
        {
            add => _collection.DownloadInfoChanged += value;
            remove => _collection.DownloadInfoChanged -= value;
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
        public event EventHandler<int>? TracksCountChanged
        {
            add => _collection.TracksCountChanged += value;
            remove => _collection.TracksCountChanged -= value;
        }

        /// <inheritdoc />
        public event CollectionChangedEventHandler<ITrack>? TracksChanged
        {
            add => _collection.TracksChanged += value;
            remove => _collection.TracksChanged -= value;
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
        public event EventHandler<int>? UrlsCountChanged
        {
            add => _collection.UrlsCountChanged += value;
            remove => _collection.UrlsCountChanged -= value;
        }

        /// <inheritdoc />
        public event CollectionChangedEventHandler<IUrl>? UrlsChanged
        {
            add => _collection.UrlsChanged += value;
            remove => _collection.UrlsChanged -= value;
        }

        /// <inheritdoc />
        public bool IsInitialized { get; private set; }

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
        public string Name => _collection.Name;

        /// <inheritdoc />
        public int TotalTrackCount => _collection.TotalTrackCount;

        /// <inheritdoc />
        public int TotalImageCount => _collection.TotalImageCount;

        /// <inheritdoc />
        public int TotalUrlCount => _collection.TotalUrlCount;

        /// <inheritdoc />
        public string? Description => _collection.Description;

        /// <inheritdoc />
        public PlaybackState PlaybackState => _collection.PlaybackState;

        /// <inheritdoc />
        public DownloadInfo DownloadInfo => _collection.DownloadInfo;

        /// <inheritdoc />
        public TimeSpan Duration => _collection.Duration;

        /// <inheritdoc />
        public DateTime? LastPlayed => _collection.LastPlayed;

        /// <inheritdoc />
        public DateTime? AddedAt => _collection.AddedAt;

        /// <inheritdoc />
        public TrackSortingType CurrentTracksSortingType { get; private set; }

        /// <inheritdoc />
        public SortDirection CurrentTracksSortingDirection { get; private set; }

        /// <inheritdoc />
        public ObservableCollection<TrackViewModel> UnsortedTracks { get; }

        /// <inheritdoc />
        public ObservableCollection<TrackViewModel> Tracks { get; set; }

        /// <inheritdoc />
        public ObservableCollection<IImage> Images { get; }

        /// <inheritdoc />
        public ObservableCollection<IUrl> Urls { get; }

        /// <inheritdoc/>
        public MainViewModel Root { get; }

        /// <inheritdoc cref="IMerged{T}.SourceCores" />
        public IReadOnlyList<ICore> SourceCores { get; }

        /// <summary>
        /// The sources that were merged into this collection.
        /// </summary>
        public IReadOnlyList<ICoreTrackCollection> Sources => _collection.GetSources<ICoreTrackCollection>();

        /// <inheritdoc />
        IReadOnlyList<ICoreTrackCollection> IMerged<ICoreTrackCollection>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreImageCollection> IMerged<ICoreImageCollection>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreUrlCollection> IMerged<ICoreUrlCollection>.Sources => Sources;

        /// <inheritdoc />
        public Task<bool> IsAddTrackAvailableAsync(int index) => _collection.IsAddTrackAvailableAsync(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveTrackAvailableAsync(int index) => _collection.IsRemoveTrackAvailableAsync(index);

        /// <inheritdoc />
        public Task<bool> IsAddImageAvailableAsync(int index) => _collection.IsAddImageAvailableAsync(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveImageAvailableAsync(int index) => _collection.IsRemoveImageAvailableAsync(index);

        /// <inheritdoc />
        public Task<bool> IsAddUrlAvailableAsync(int index) => _collection.IsAddUrlAvailableAsync(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveUrlAvailableAsync(int index) => _collection.IsRemoveUrlAvailableAsync(index);

        /// <inheritdoc />
        public Task StartDownloadOperationAsync(DownloadOperation operation) => _collection.StartDownloadOperationAsync(operation);

        /// <inheritdoc />
        public Task ChangeNameAsync(string name) => ChangeNameInternalAsync(name);

        /// <inheritdoc />
        public Task AddTrackAsync(ITrack track, int index) => _collection.AddTrackAsync(track, index);

        /// <inheritdoc />
        public Task RemoveTrackAsync(int index) => _collection.RemoveTrackAsync(index);

        /// <inheritdoc />
        public Task AddImageAsync(IImage image, int index) => _collection.AddImageAsync(image, index);

        /// <inheritdoc />
        public Task RemoveImageAsync(int index) => _collection.RemoveImageAsync(index);

        /// <inheritdoc />
        public Task AddUrlAsync(IUrl image, int index) => _collection.AddUrlAsync(image, index);

        /// <inheritdoc />
        public Task RemoveUrlAsync(int index) => _collection.RemoveUrlAsync(index);

        /// <inheritdoc />
        public Task<IReadOnlyList<ITrack>> GetTracksAsync(int limit, int offset) => _collection.GetTracksAsync(limit, offset);

        /// <inheritdoc />
        public Task<IReadOnlyList<IImage>> GetImagesAsync(int limit, int offset) => _collection.GetImagesAsync(limit, offset);

        /// <inheritdoc />
        public Task<IReadOnlyList<IUrl>> GetUrlsAsync(int limit, int offset) => _collection.GetUrlsAsync(limit, offset);

        /// <inheritdoc />
        public Task PlayTrackCollectionAsync() => _collection.PlayTrackCollectionAsync();

        /// <inheritdoc />
        public Task PlayTrackCollectionAsync(ITrack track) => _collection.PlayTrackCollectionAsync(track);

        /// <inheritdoc />
        public Task PauseTrackCollectionAsync() => _collection.PauseTrackCollectionAsync();

        /// <inheritdoc />
        public Task ChangeDescriptionAsync(string? description) => _collection.ChangeDescriptionAsync(description);

        /// <inheritdoc />
        public Task ChangeDurationAsync(TimeSpan duration) => _collection.ChangeDurationAsync(duration);

        ///<inheritdoc />
        public void SortTrackCollection(TrackSortingType trackSorting, SortDirection sortDirection)
        {
            CurrentTracksSortingType = trackSorting;
            CurrentTracksSortingDirection = sortDirection;

            CollectionSorting.SortTracks(Tracks, trackSorting, sortDirection, UnsortedTracks);
        }

        /// <inheritdoc />
        public async Task PopulateMoreTracksAsync(int limit)
        {
            using (await Flow.EasySemaphore(_populateTracksMutex))
            {
                var items = await _collection.GetTracksAsync(limit, Tracks.Count);

                await Threading.OnPrimaryThread(() =>
                {
                    foreach (var item in items)
                    {
                        Tracks.Add(new TrackViewModel(Root, item));
                    }
                });
            }
        }

        /// <inheritdoc />
        public async Task PopulateMoreImagesAsync(int limit)
        {
            using (await Flow.EasySemaphore(_populateImagesMutex))
            {
                var items = await _collection.GetImagesAsync(limit, Images.Count);

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
        public async Task PopulateMoreUrlsAsync(int limit)
        {
            using (await Flow.EasySemaphore(_populateUrlsMutex))
            {
                var items = await _collection.GetUrlsAsync(limit, Urls.Count);

                await Threading.OnPrimaryThread(() =>
                {
                    foreach (var item in items)
                    {
                        Urls.Add(item);
                    }
                });
            }
        }

        /// <inheritdoc/>
        public Task InitImageCollectionAsync() => CollectionInit.ImageCollection(this);

        /// <inheritdoc/>
        public Task InitTrackCollectionAsync() => CollectionInit.TrackCollection(this);

        /// <inheritdoc />
        public IRelayCommand<TrackSortingType> ChangeTrackCollectionSortingTypeCommand { get; }

        /// <inheritdoc />
        public IRelayCommand<SortDirection> ChangeTrackCollectionSortingDirectionCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand<int> PopulateMoreTracksCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand<int> PopulateMoreImagesCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand<int> PopulateMoreUrlsCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand PlayTrackCollectionAsyncCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand<ITrack> PlayTrackAsyncCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand PauseTrackCollectionAsyncCommand { get; }

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

        /// <inheritdoc/>
        public IAsyncRelayCommand InitTrackCollectionAsyncCommand { get; }

        /// <inheritdoc/>
        public IAsyncRelayCommand InitImageCollectionAsyncCommand { get; }

        /// <inheritdoc />
        public bool Equals(ICoreTrackCollection other) => _collection.Equals(other);

        /// <inheritdoc />
        public bool Equals(ICoreImageCollection other) => _collection.Equals(other);

        /// <inheritdoc />
        public bool Equals(ICoreUrlCollection other) => _collection.Equals(other);

        private Task ChangeNameInternalAsync(string? name)
        {
            Guard.IsNotNull(name, nameof(name));
            return _collection.ChangeNameAsync(name);
        }

        /// <inheritdoc />
        public ValueTask DisposeAsync()
        {
            DetachEvents();
            return _collection.DisposeAsync();
        }
    }
}
