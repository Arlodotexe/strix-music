// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OwlCore;
using OwlCore.Events;
using OwlCore.Extensions;
using StrixMusic.Sdk.AdapterModels;
using StrixMusic.Sdk.AppModels;
using StrixMusic.Sdk.CoreModels;
using StrixMusic.Sdk.Extensions;
using StrixMusic.Sdk.MediaPlayback;
using StrixMusic.Sdk.ViewModels.Helpers;

namespace StrixMusic.Sdk.ViewModels
{
    /// <summary>
    /// A ViewModel for <see cref="IAlbumCollection"/>.
    /// </summary>
    public sealed class AlbumCollectionViewModel : ObservableObject, ISdkViewModel, IAlbumCollectionViewModel, IUrlCollectionViewModel, IImageCollectionViewModel
    {
        private readonly IAlbumCollection _collection;

        private readonly SemaphoreSlim _populateAlbumsMutex = new(1, 1);
        private readonly SemaphoreSlim _populateImagesMutex = new(1, 1);
        private readonly SemaphoreSlim _populateUrlsMutex = new(1, 1);
        private readonly SynchronizationContext _syncContext;

        /// <summary>
        /// Creates a new instance of <see cref="AlbumCollectionViewModel"/>.
        /// </summary>
        /// <param name="root">The <see cref="MainViewModel"/> that this or the object that created this originated from.</param>
        /// <param name="collection">The <see cref="IAlbumCollection"/> to wrap around.</param>
        public AlbumCollectionViewModel(MainViewModel root, IAlbumCollection collection)
        {
            _syncContext = SynchronizationContext.Current;
            Root = root;
            _collection = collection;

            Albums = new ObservableCollection<IAlbumCollectionItem>();
            UnsortedAlbums = new ObservableCollection<IAlbumCollectionItem>();
            Images = new ObservableCollection<IImage>();
            Urls = new ObservableCollection<IUrl>();

            SourceCores = _collection.GetSourceCores<ICoreAlbumCollection>().Select(root.GetLoadedCore).ToList();

            PopulateMoreAlbumsCommand = new AsyncRelayCommand<int>(PopulateMoreAlbumsAsync);
            PopulateMoreImagesCommand = new AsyncRelayCommand<int>(PopulateMoreImagesAsync);
            PopulateMoreUrlsCommand = new AsyncRelayCommand<int>(PopulateMoreUrlsAsync);

            PauseAlbumCollectionAsyncCommand = new AsyncRelayCommand(PauseAlbumCollectionAsync);
            PlayAlbumCollectionAsyncCommand = new AsyncRelayCommand(PlayAlbumCollectionAsync);

            ChangeNameAsyncCommand = new AsyncRelayCommand<string?>(ChangeNameInternalAsync);
            ChangeDescriptionAsyncCommand = new AsyncRelayCommand<string?>(ChangeDescriptionAsync);
            ChangeDurationAsyncCommand = new AsyncRelayCommand<TimeSpan>(ChangeDurationAsync);

            ChangeAlbumCollectionSortingTypeCommand = new RelayCommand<AlbumSortingType>(x => SortAlbumCollection(x, CurrentAlbumSortingDirection));
            ChangeAlbumCollectionSortingDirectionCommand = new RelayCommand<SortDirection>(x => SortAlbumCollection(CurrentAlbumSortingType, x));

            InitAlbumCollectionAsyncCommand = new AsyncRelayCommand(InitAlbumCollectionAsync);
            InitImageCollectionAsyncCommand = new AsyncRelayCommand(InitImageCollectionAsync);

            PlayAlbumAsyncCommand = new AsyncRelayCommand<IAlbumCollectionItem>((x, y) => _collection.PlayAlbumCollectionAsync(x ?? ThrowHelper.ThrowArgumentNullException<IAlbumCollectionItem>(nameof(x)), y));

            AttachEvents();
        }

        private void AttachEvents()
        {
            PlaybackStateChanged += OnPlaybackStateChanged;
            NameChanged += OnNameChanged;
            DescriptionChanged += OnDescriptionChanged;
            LastPlayedChanged += OnLastPlayedChanged;
            Flow.Catch<NotSupportedException>(() => DownloadInfoChanged += OnDownloadInfoChanged);

            IsPlayAlbumCollectionAsyncAvailableChanged += OnIsPlayAlbumCollectionAsyncAvailableChanged;
            IsPauseAlbumCollectionAsyncAvailableChanged += OnIsPauseAlbumCollectionAsyncAvailableChanged;
            IsChangeNameAsyncAvailableChanged += OnIsChangeNameAsyncAvailableChanged;
            IsChangeDurationAsyncAvailableChanged += OnIsChangeDurationAsyncAvailableChanged;
            IsChangeDescriptionAsyncAvailableChanged += OnIsChangeDescriptionAsyncAvailableChanged;

            AlbumItemsCountChanged += OnAlbumItemsCountChanged;
            AlbumItemsChanged += AlbumCollectionViewModel_AlbumItemsChanged;
            ImagesCountChanged += OnImagesCountChanged;
            ImagesChanged += AlbumCollectionViewModel_ImagesChanged;
            UrlsCountChanged += OnUrlsCountChanged;
            UrlsChanged += AlbumCollectionViewModel_UrlsChanged;
        }

        private void DetachEvents()
        {
            PlaybackStateChanged -= OnPlaybackStateChanged;
            NameChanged -= OnNameChanged;
            DescriptionChanged -= OnDescriptionChanged;
            LastPlayedChanged -= OnLastPlayedChanged;
            Flow.Catch<NotSupportedException>(() => DownloadInfoChanged -= OnDownloadInfoChanged);

            IsPlayAlbumCollectionAsyncAvailableChanged -= OnIsPlayAlbumCollectionAsyncAvailableChanged;
            IsPauseAlbumCollectionAsyncAvailableChanged -= OnIsPauseAlbumCollectionAsyncAvailableChanged;
            IsChangeNameAsyncAvailableChanged -= OnIsChangeNameAsyncAvailableChanged;
            IsChangeDurationAsyncAvailableChanged -= OnIsChangeDurationAsyncAvailableChanged;
            IsChangeDescriptionAsyncAvailableChanged -= OnIsChangeDescriptionAsyncAvailableChanged;

            AlbumItemsCountChanged -= OnAlbumItemsCountChanged;
            AlbumItemsChanged -= AlbumCollectionViewModel_AlbumItemsChanged;
            ImagesCountChanged -= OnImagesCountChanged;
            ImagesChanged -= AlbumCollectionViewModel_ImagesChanged;
        }

        private void OnNameChanged(object sender, string e) => _syncContext.Post(_ => OnPropertyChanged(nameof(Name)), null);

        private void OnDescriptionChanged(object sender, string? e) => _syncContext.Post(_ => OnPropertyChanged(nameof(Description)), null);

        private void OnPlaybackStateChanged(object sender, PlaybackState e) => _syncContext.Post(_ => OnPropertyChanged(nameof(PlaybackState)), null);

        private void OnDownloadInfoChanged(object sender, DownloadInfo e) => _syncContext.Post(_ => OnPropertyChanged(nameof(DownloadInfo)), null);

        private void OnAlbumItemsCountChanged(object sender, int e) => _syncContext.Post(_ => OnPropertyChanged(nameof(TotalAlbumItemsCount)), null);

        private void OnImagesCountChanged(object sender, int e) => _syncContext.Post(_ => OnPropertyChanged(nameof(TotalImageCount)), null);

        private void OnUrlsCountChanged(object sender, int e) => _syncContext.Post(_ => OnPropertyChanged(nameof(TotalUrlCount)), null);

        private void OnLastPlayedChanged(object sender, DateTime? e) => _syncContext.Post(_ => OnPropertyChanged(nameof(LastPlayed)), null);

        private void OnIsChangeDescriptionAsyncAvailableChanged(object sender, bool e) => _syncContext.Post(_ => OnPropertyChanged(nameof(IsChangeDescriptionAsyncAvailable)), null);

        private void OnIsChangeDurationAsyncAvailableChanged(object sender, bool e) => _syncContext.Post(_ => OnPropertyChanged(nameof(IsChangeDurationAsyncAvailable)), null);

        private void OnIsChangeNameAsyncAvailableChanged(object sender, bool e) => _syncContext.Post(_ => OnPropertyChanged(nameof(IsChangeNameAsyncAvailable)), null);

        private void OnIsPauseAlbumCollectionAsyncAvailableChanged(object sender, bool e) => _syncContext.Post(_ => OnPropertyChanged(nameof(IsPauseAlbumCollectionAsyncAvailable)), null);

        private void OnIsPlayAlbumCollectionAsyncAvailableChanged(object sender, bool e) => _syncContext.Post(_ => OnPropertyChanged(nameof(IsPlayAlbumCollectionAsyncAvailable)), null);

        private void AlbumCollectionViewModel_ImagesChanged(object sender, IReadOnlyList<CollectionChangedItem<IImage>> addedItems, IReadOnlyList<CollectionChangedItem<IImage>> removedItems) => _syncContext.Post(_ =>
        {
            Images.ChangeCollection(addedItems, removedItems);
        }, null);

        private void AlbumCollectionViewModel_UrlsChanged(object sender, IReadOnlyList<CollectionChangedItem<IUrl>> addedItems, IReadOnlyList<CollectionChangedItem<IUrl>> removedItems) => _syncContext.Post(_ =>
        {
            Urls.ChangeCollection(addedItems, removedItems);
        }, null);

        private void AlbumCollectionViewModel_AlbumItemsChanged(object sender, IReadOnlyList<CollectionChangedItem<IAlbumCollectionItem>> addedItems, IReadOnlyList<CollectionChangedItem<IAlbumCollectionItem>> removedItems) => _syncContext.Post(_ =>
        {
            if (CurrentAlbumSortingType == AlbumSortingType.Unsorted)
            {
                Albums.ChangeCollection(addedItems, removedItems, item => item.Data switch
                {
                    IAlbum album => new AlbumViewModel(Root, album),
                    IAlbumCollection collection => new AlbumCollectionViewModel(Root, collection),
                    _ => ThrowHelper.ThrowNotSupportedException<IAlbumCollectionItem>(
                        $"{item.Data.GetType()} not supported for adding to {GetType()}")
                });
            }
            else
            {
                // Preventing index issues during albums emission from the core, also making sure that unordered albums updated. 
                UnsortedAlbums.ChangeCollection(addedItems, removedItems, item => item.Data switch
                {
                    IAlbum album => new AlbumViewModel(Root, album),
                    IAlbumCollection collection => new AlbumCollectionViewModel(Root, collection),
                    _ => ThrowHelper.ThrowNotSupportedException<IAlbumCollectionItem>(
                        $"{item.Data.GetType()} not supported for adding to {GetType()}")
                });
                
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
        }, null);

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
        public event EventHandler<bool>? IsPlayAlbumCollectionAsyncAvailableChanged
        {
            add => _collection.IsPlayAlbumCollectionAsyncAvailableChanged += value;
            remove => _collection.IsPlayAlbumCollectionAsyncAvailableChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<bool>? IsPauseAlbumCollectionAsyncAvailableChanged
        {
            add => _collection.IsPauseAlbumCollectionAsyncAvailableChanged += value;
            remove => _collection.IsPauseAlbumCollectionAsyncAvailableChanged -= value;
        }

        /// <inheritdoc/>
        public event EventHandler<int>? AlbumItemsCountChanged
        {
            add => _collection.AlbumItemsCountChanged += value;
            remove => _collection.AlbumItemsCountChanged -= value;
        }

        /// <inheritdoc />
        public event CollectionChangedEventHandler<IImage>? ImagesChanged
        {
            add => _collection.ImagesChanged += value;
            remove => _collection.ImagesChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<int>? ImagesCountChanged
        {
            add => _collection.ImagesCountChanged += value;
            remove => _collection.ImagesCountChanged -= value;
        }

        /// <inheritdoc />
        public event CollectionChangedEventHandler<IUrl>? UrlsChanged
        {
            add => _collection.UrlsChanged += value;
            remove => _collection.UrlsChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<int>? UrlsCountChanged
        {
            add => _collection.UrlsCountChanged += value;
            remove => _collection.UrlsCountChanged -= value;
        }

        /// <inheritdoc />
        public event CollectionChangedEventHandler<IAlbumCollectionItem>? AlbumItemsChanged
        {
            add => _collection.AlbumItemsChanged += value;
            remove => _collection.AlbumItemsChanged -= value;
        }

        /// <inheritdoc />
        public async Task PopulateMoreAlbumsAsync(int limit, CancellationToken cancellationToken = default)
        {
            using (await Flow.EasySemaphore(_populateAlbumsMutex))
            {
                using var releaseReg = cancellationToken.Register(() => _populateAlbumsMutex.Release());
                var items = await _collection.GetAlbumItemsAsync(limit, Albums.Count, cancellationToken);

                _syncContext.Post(_ =>
                {
                    foreach (var item in items)
                    {
                        switch (item)
                        {
                            case IAlbum album:
                                Albums.Add(new AlbumViewModel(Root, album));
                                break;
                            case IAlbumCollection collection:
                                Albums.Add(new AlbumCollectionViewModel(Root, collection));
                                break;
                        }
                    }
                }, null);
            }
        }

        /// <inheritdoc />
        public async Task PopulateMoreImagesAsync(int limit, CancellationToken cancellationToken = default)
        {
            using (await Flow.EasySemaphore(_populateImagesMutex))
            {
                using var releaseReg = cancellationToken.Register(() => _populateImagesMutex.Release());
                var items = await _collection.GetImagesAsync(limit, Images.Count, cancellationToken);

                _syncContext.Post(_ =>
                {
                    foreach (var item in items)
                        Images.Add(item);
                }, null);
            }
        }

        /// <inheritdoc />
        public async Task PopulateMoreUrlsAsync(int limit, CancellationToken cancellationToken = default)
        {
            using (await Flow.EasySemaphore(_populateUrlsMutex))
            {
                using var releaseReg = cancellationToken.Register(() => _populateUrlsMutex.Release());
                var items = await _collection.GetUrlsAsync(limit, Urls.Count, cancellationToken);

                _syncContext.Post(_ =>
                {
                    foreach (var item in items)
                    {
                        Urls.Add(item);
                    }
                }, null);
            }
        }

        /// <inheritdoc />
        public void SortAlbumCollection(AlbumSortingType albumSorting, SortDirection sortDirection)
        {
            CurrentAlbumSortingType = albumSorting;
            CurrentAlbumSortingDirection = sortDirection;

            CollectionSorting.SortAlbums(Albums, albumSorting, sortDirection, UnsortedAlbums);
        }

        /// <inheritdoc />
        public ObservableCollection<IAlbumCollectionItem> Albums { get; }

        ///<inheritdoc />
        public ObservableCollection<IAlbumCollectionItem> UnsortedAlbums { get; }

        /// <inheritdoc />
        public ObservableCollection<IImage> Images { get; }

        /// <inheritdoc />
        public ObservableCollection<IUrl> Urls { get; }

        /// <inheritdoc />
        public AlbumSortingType CurrentAlbumSortingType { get; private set; }

        /// <inheritdoc />
        public SortDirection CurrentAlbumSortingDirection { get; private set; }

        /// <inheritdoc />
        public string Id => _collection.Id;

        /// <inheritdoc />
        public string Name => _collection.Name;

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
        public int TotalAlbumItemsCount => _collection.TotalAlbumItemsCount;

        /// <inheritdoc />
        public bool IsPlayAlbumCollectionAsyncAvailable => _collection.IsPlayAlbumCollectionAsyncAvailable;

        /// <inheritdoc />
        public bool IsPauseAlbumCollectionAsyncAvailable => _collection.IsPauseAlbumCollectionAsyncAvailable;

        /// <inheritdoc />
        public int TotalImageCount => _collection.TotalImageCount;

        /// <inheritdoc />
        public int TotalUrlCount => _collection.TotalUrlCount;

        /// <inheritdoc />
        public bool IsChangeNameAsyncAvailable => _collection.IsChangeNameAsyncAvailable;

        /// <inheritdoc />
        public bool IsChangeDescriptionAsyncAvailable => _collection.IsChangeDescriptionAsyncAvailable;

        /// <inheritdoc />
        public bool IsChangeDurationAsyncAvailable => _collection.IsChangeDurationAsyncAvailable;

        /// <inheritdoc />
        public Task<bool> IsAddAlbumItemAvailableAsync(int index, CancellationToken cancellationToken = default) => _collection.IsAddAlbumItemAvailableAsync(index, cancellationToken);

        /// <inheritdoc />
        public Task<bool> IsRemoveAlbumItemAvailableAsync(int index, CancellationToken cancellationToken = default) => _collection.IsRemoveAlbumItemAvailableAsync(index, cancellationToken);

        /// <inheritdoc />
        public Task<bool> IsAddImageAvailableAsync(int index, CancellationToken cancellationToken = default) => _collection.IsAddImageAvailableAsync(index, cancellationToken);

        /// <inheritdoc />
        public Task<bool> IsRemoveImageAvailableAsync(int index, CancellationToken cancellationToken = default) => _collection.IsRemoveImageAvailableAsync(index, cancellationToken);

        /// <inheritdoc />
        public Task<bool> IsAddUrlAvailableAsync(int index, CancellationToken cancellationToken = default) => _collection.IsAddUrlAvailableAsync(index, cancellationToken);

        /// <inheritdoc />
        public Task<bool> IsRemoveUrlAvailableAsync(int index, CancellationToken cancellationToken = default) => _collection.IsRemoveUrlAvailableAsync(index, cancellationToken);

        /// <inheritdoc />
        public Task ChangeNameAsync(string name, CancellationToken cancellationToken = default) => ChangeNameInternalAsync(name, cancellationToken);

        /// <inheritdoc />
        public Task ChangeDescriptionAsync(string? description, CancellationToken cancellationToken = default) => _collection.ChangeDescriptionAsync(description, cancellationToken);

        /// <inheritdoc />
        public Task ChangeDurationAsync(TimeSpan duration, CancellationToken cancellationToken = default) => _collection.ChangeDurationAsync(duration, cancellationToken);

        /// <inheritdoc />
        public Task StartDownloadOperationAsync(DownloadOperation operation, CancellationToken cancellationToken = default) => _collection.StartDownloadOperationAsync(operation, cancellationToken);

        /// <inheritdoc />
        public Task<IReadOnlyList<IAlbumCollectionItem>> GetAlbumItemsAsync(int limit, int offset, CancellationToken cancellationToken = default) => _collection.GetAlbumItemsAsync(limit, offset, cancellationToken);

        /// <inheritdoc />
        public Task<IReadOnlyList<IImage>> GetImagesAsync(int limit, int offset, CancellationToken cancellationToken = default) => _collection.GetImagesAsync(limit, offset, cancellationToken);

        /// <inheritdoc />
        public Task<IReadOnlyList<IUrl>> GetUrlsAsync(int limit, int offset, CancellationToken cancellationToken = default) => _collection.GetUrlsAsync(limit, offset, cancellationToken);

        /// <summary>
        /// The sources for this item.
        /// </summary>
        public IReadOnlyList<ICoreAlbumCollection> Sources => _collection.GetSources<ICoreAlbumCollection>();

        /// <inheritdoc />
        IReadOnlyList<ICoreAlbumCollection> IMerged<ICoreAlbumCollection>.Sources => _collection.GetSources<ICoreAlbumCollection>();

        /// <inheritdoc />
        IReadOnlyList<ICoreAlbumCollectionItem> IMerged<ICoreAlbumCollectionItem>.Sources => _collection.GetSources<ICoreAlbumCollectionItem>();

        /// <inheritdoc />
        IReadOnlyList<ICoreImageCollection> IMerged<ICoreImageCollection>.Sources => _collection.GetSources<ICoreImageCollection>();

        /// <inheritdoc />
        IReadOnlyList<ICoreUrlCollection> IMerged<ICoreUrlCollection>.Sources => _collection.GetSources<ICoreUrlCollection>();

        /// <inheritdoc/>
        public MainViewModel Root { get; }

        /// <inheritdoc cref="IMerged{T}.SourceCores" />
        public IReadOnlyList<ICore> SourceCores { get; }

        /// <inheritdoc />
        public Task PlayAlbumCollectionAsync(CancellationToken cancellationToken = default) => _collection.PlayAlbumCollectionAsync(cancellationToken);

        /// <inheritdoc />
        public Task PlayAlbumCollectionAsync(IAlbumCollectionItem albumItem, CancellationToken cancellationToken = default) => _collection.PlayAlbumCollectionAsync(albumItem, cancellationToken);

        /// <inheritdoc />
        public Task PauseAlbumCollectionAsync(CancellationToken cancellationToken = default) => _collection.PauseAlbumCollectionAsync(cancellationToken);

        /// <inheritdoc />
        public Task AddAlbumItemAsync(IAlbumCollectionItem album, int index, CancellationToken cancellationToken = default) => _collection.AddAlbumItemAsync(album, index, cancellationToken);

        /// <inheritdoc />
        public Task RemoveAlbumItemAsync(int index, CancellationToken cancellationToken = default) => _collection.RemoveAlbumItemAsync(index, cancellationToken);

        /// <inheritdoc />
        public Task AddImageAsync(IImage image, int index, CancellationToken cancellationToken = default) => _collection.AddImageAsync(image, index, cancellationToken);

        /// <inheritdoc />
        public Task RemoveImageAsync(int index, CancellationToken cancellationToken = default) => _collection.RemoveImageAsync(index, cancellationToken);

        /// <inheritdoc />
        public Task AddUrlAsync(IUrl image, int index, CancellationToken cancellationToken = default) => _collection.AddUrlAsync(image, index, cancellationToken);

        /// <inheritdoc />
        public Task RemoveUrlAsync(int index, CancellationToken cancellationToken = default) => _collection.RemoveUrlAsync(index, cancellationToken);

        /// <inheritdoc />
        public Task InitAlbumCollectionAsync(CancellationToken cancellationToken = default) => CollectionInit.AlbumCollection(this, cancellationToken);

        /// <inheritdoc/>
        public Task InitImageCollectionAsync(CancellationToken cancellationToken = default) => CollectionInit.ImageCollection(this, cancellationToken);

        /// <inheritdoc />
        public IAsyncRelayCommand InitAlbumCollectionAsyncCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand<int> PopulateMoreAlbumsCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand<int> PopulateMoreImagesCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand<int> PopulateMoreUrlsCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand PlayAlbumCollectionAsyncCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand<IAlbumCollectionItem> PlayAlbumAsyncCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand PauseAlbumCollectionAsyncCommand { get; }

        ///<inheritdoc />
        public IRelayCommand<AlbumSortingType> ChangeAlbumCollectionSortingTypeCommand { get; }

        ///<inheritdoc />
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

        /// <inheritdoc/>
        public IAsyncRelayCommand InitImageCollectionAsyncCommand { get; }

        /// <inheritdoc />
        public bool Equals(ICoreAlbumCollectionItem other) => _collection.Equals(other);

        /// <inheritdoc />
        public bool Equals(ICoreAlbumCollection other) => _collection.Equals(other);

        /// <inheritdoc />
        public bool Equals(ICoreImageCollection other) => _collection.Equals(other);

        /// <inheritdoc />
        public bool Equals(ICoreUrlCollection other) => _collection.Equals(other);

        private Task ChangeNameInternalAsync(string? name, CancellationToken cancellationToken = default)
        {
            Guard.IsNotNull(name, nameof(name));
            return _collection.ChangeNameAsync(name, cancellationToken);
        }

        /// <inheritdoc />
        public bool IsInitialized { get; private set; }

        /// <inheritdoc />
        public Task InitAsync(CancellationToken cancellationToken = default)
        {
            if (IsInitialized)
                return Task.CompletedTask;

            IsInitialized = true;

            return Task.WhenAll(InitAlbumCollectionAsync(cancellationToken), InitImageCollectionAsync(cancellationToken));
        }

        /// <inheritdoc />
        public ValueTask DisposeAsync()
        {
            DetachEvents();
            return _collection.DisposeAsync();
        }
    }
}
