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
    /// A ViewModel for <see cref="IPlaylistCollection"/>.
    /// </summary>
    public sealed class PlaylistCollectionViewModel : ObservableObject, ISdkViewModel, IPlaylistCollectionViewModel, IImageCollectionViewModel, IUrlCollectionViewModel
    {
        private readonly IPlaylistCollection _collection;

        private readonly SemaphoreSlim _populatePlaylistsMutex = new(1, 1);
        private readonly SemaphoreSlim _populateImagesMutex = new(1, 1);
        private readonly SemaphoreSlim _populateUrlsMutex = new(1, 1);
        private readonly SynchronizationContext _syncContext;

        /// <summary>
        /// Creates a new instance of <see cref="PlaylistCollectionViewModel"/>.
        /// </summary>
        /// <param name="root">The <see cref="MainViewModel"/> that this or the object that created this originated from.</param>
        /// <param name="collection">The <see cref="IPlaylistCollection"/> to wrap around.</param>
        internal PlaylistCollectionViewModel(MainViewModel root, IPlaylistCollection collection)
        {
            _syncContext = SynchronizationContext.Current;

            _collection = root.Plugins.ModelPlugins.PlaylistCollection.Execute(collection);

            SourceCores = _collection.GetSourceCores<ICorePlaylistCollection>().Select(root.GetLoadedCore).ToList();
            Root = root;

            Playlists = new ObservableCollection<IPlaylistCollectionItem>();
            Images = new ObservableCollection<IImage>();
            Urls = new ObservableCollection<IUrl>();

            UnsortedPlaylists = new ObservableCollection<IPlaylistCollectionItem>();

            PlayPlaylistCollectionAsyncCommand = new AsyncRelayCommand(PlayPlaylistCollectionAsync);
            PausePlaylistCollectionAsyncCommand = new AsyncRelayCommand(PausePlaylistCollectionAsync);

            ChangePlaylistCollectionSortingTypeCommand = new RelayCommand<PlaylistSortingType>(x => SortPlaylistCollection(x, CurrentPlaylistSortingDirection));
            ChangePlaylistCollectionSortingDirectionCommand = new RelayCommand<SortDirection>(x => SortPlaylistCollection(CurrentPlaylistSortingType, x));

            PlayPlaylistAsyncCommand = new AsyncRelayCommand<IPlaylistCollectionItem>(x => _collection.PlayPlaylistCollectionAsync(x ?? ThrowHelper.ThrowArgumentNullException<IPlaylistCollectionItem>()));

            PopulateMorePlaylistsCommand = new AsyncRelayCommand<int>(PopulateMorePlaylistsAsync);
            PopulateMoreImagesCommand = new AsyncRelayCommand<int>(PopulateMoreImagesAsync);
            PopulateMoreUrlsCommand = new AsyncRelayCommand<int>(PopulateMoreUrlsAsync);

            ChangeNameAsyncCommand = new AsyncRelayCommand<string>(ChangeNameInternalAsync);
            ChangeDescriptionAsyncCommand = new AsyncRelayCommand<string?>(ChangeDescriptionAsync);
            ChangeDurationAsyncCommand = new AsyncRelayCommand<TimeSpan>(ChangeDurationAsync);

            InitImageCollectionAsyncCommand = new AsyncRelayCommand(InitImageCollectionAsync);
            InitPlaylistCollectionAsyncCommand = new AsyncRelayCommand(InitPlaylistCollectionAsync);

            AttachEvents();
        }

        private void AttachEvents()
        {
            PlaybackStateChanged += OnPlaybackStateChanged;
            NameChanged += OnNameChanged;
            DescriptionChanged += OnDescriptionChanged;
            LastPlayedChanged += OnLastPlayedChanged;
            Flow.Catch<NotSupportedException>(() => DownloadInfoChanged -= OnDownloadInfoChanged);

            IsPlayPlaylistCollectionAsyncAvailableChanged += OnIsPlayPlaylistCollectionAsyncAvailableChanged;
            IsPausePlaylistCollectionAsyncAvailableChanged += OnIsPausePlaylistCollectionAsyncAvailableChanged;

            IsChangeNameAsyncAvailableChanged += OnIsChangeNameAsyncAvailableChanged;
            IsChangeDurationAsyncAvailableChanged += OnIsChangeDurationAsyncAvailableChanged;
            IsChangeDescriptionAsyncAvailableChanged += OnIsChangeDescriptionAsyncAvailableChanged;

            PlaylistItemsCountChanged += OnPlaylistItemsCountChanged;
            ImagesCountChanged += PlaylistCollectionViewModel_ImagesCountChanged;
            PlaylistItemsChanged += PlaylistCollectionViewModel_PlaylistItemsChanged;
            ImagesChanged += PlaylistCollectionViewModel_ImagesChanged;
        }

        private void DetachEvents()
        {
            PlaybackStateChanged -= OnPlaybackStateChanged;
            NameChanged -= OnNameChanged;
            DescriptionChanged -= OnDescriptionChanged;
            LastPlayedChanged -= OnLastPlayedChanged;
            Flow.Catch<NotSupportedException>(() => DownloadInfoChanged -= OnDownloadInfoChanged);

            IsPlayPlaylistCollectionAsyncAvailableChanged -= OnIsPlayPlaylistCollectionAsyncAvailableChanged;
            IsPausePlaylistCollectionAsyncAvailableChanged -= OnIsPausePlaylistCollectionAsyncAvailableChanged;

            IsChangeNameAsyncAvailableChanged -= OnIsChangeNameAsyncAvailableChanged;
            IsChangeDurationAsyncAvailableChanged -= OnIsChangeDurationAsyncAvailableChanged;
            IsChangeDescriptionAsyncAvailableChanged -= OnIsChangeDescriptionAsyncAvailableChanged;

            PlaylistItemsCountChanged -= OnPlaylistItemsCountChanged;
            ImagesCountChanged -= PlaylistCollectionViewModel_ImagesCountChanged;
            PlaylistItemsChanged -= PlaylistCollectionViewModel_PlaylistItemsChanged;
            ImagesChanged -= PlaylistCollectionViewModel_ImagesChanged;
        }

        private void OnNameChanged(object sender, string e) => _syncContext.Post(_ => OnPropertyChanged(nameof(Name)), null);

        private void OnDescriptionChanged(object sender, string? e) => _syncContext.Post(_ => OnPropertyChanged(nameof(Description)), null);

        private void OnPlaybackStateChanged(object sender, PlaybackState e) => _syncContext.Post(_ => OnPropertyChanged(nameof(PlaybackState)), null);

        private void OnDownloadInfoChanged(object sender, DownloadInfo e) => _syncContext.Post(_ => OnPropertyChanged(nameof(DownloadInfo)), null);

        private void OnPlaylistItemsCountChanged(object sender, int e) => _syncContext.Post(_ => OnPropertyChanged(nameof(TotalPlaylistItemsCount)), null);

        private void PlaylistCollectionViewModel_ImagesCountChanged(object sender, int e) => _syncContext.Post(_ => OnPropertyChanged(nameof(TotalImageCount)), null);

        private void OnLastPlayedChanged(object sender, DateTime? e) => _syncContext.Post(_ => OnPropertyChanged(nameof(LastPlayed)), null);

        private void OnIsChangeDescriptionAsyncAvailableChanged(object sender, bool e) => _syncContext.Post(_ => OnPropertyChanged(nameof(IsChangeDescriptionAsyncAvailable)), null);

        private void OnIsChangeDurationAsyncAvailableChanged(object sender, bool e) => _syncContext.Post(_ => OnPropertyChanged(nameof(IsChangeDurationAsyncAvailable)), null);

        private void OnIsChangeNameAsyncAvailableChanged(object sender, bool e) => _syncContext.Post(_ => OnPropertyChanged(nameof(IsChangeNameAsyncAvailable)), null);

        private void OnIsPausePlaylistCollectionAsyncAvailableChanged(object sender, bool e) => _syncContext.Post(_ => OnPropertyChanged(nameof(IsPausePlaylistCollectionAsyncAvailable)), null);

        private void OnIsPlayPlaylistCollectionAsyncAvailableChanged(object sender, bool e) => _syncContext.Post(_ => OnPropertyChanged(nameof(IsPlayPlaylistCollectionAsyncAvailable)), null);

        private void PlaylistCollectionViewModel_ImagesChanged(object sender, IReadOnlyList<CollectionChangedItem<IImage>> addedItems, IReadOnlyList<CollectionChangedItem<IImage>> removedItems) => _syncContext.Post(_ =>
        {
            Images.ChangeCollection(addedItems, removedItems);
        }, null);

        private void PlaylistCollectionViewModel_PlaylistItemsChanged(object sender, IReadOnlyList<CollectionChangedItem<IPlaylistCollectionItem>> addedItems, IReadOnlyList<CollectionChangedItem<IPlaylistCollectionItem>> removedItems) => _syncContext.Post(_ =>
        {
            if (CurrentPlaylistSortingType == PlaylistSortingType.Unsorted)
            {
                Playlists.ChangeCollection(addedItems, removedItems, item => item.Data switch
                {
                    IPlaylist playlist => new PlaylistViewModel(Root, playlist),
                    IPlaylistCollection collection => new PlaylistCollectionViewModel(Root, collection),
                    _ => ThrowHelper.ThrowNotSupportedException<IPlaylistCollectionItem>(
                        $"{item.Data.GetType()} not supported for adding to {GetType()}")
                });
            }
            else
            {
                // Make sure both ordered and unordered playlists are updated.
                UnsortedPlaylists.ChangeCollection(addedItems, removedItems, item => item.Data switch
                {
                    IPlaylist playlist => new PlaylistViewModel(Root, playlist),
                    IPlaylistCollection collection => new PlaylistCollectionViewModel(Root, collection),
                    _ => ThrowHelper.ThrowNotSupportedException<IPlaylistCollection>(
                        $"{item.Data.GetType()} not supported for adding to {GetType()}")
                });

                foreach (var item in UnsortedPlaylists)
                {
                    if (!Playlists.Contains(item))
                        Playlists.Add(item);
                }

                foreach (var item in Playlists)
                {
                    if (!UnsortedPlaylists.Contains(item))
                        UnsortedPlaylists.Remove(item);
                }

                SortPlaylistCollection(CurrentPlaylistSortingType, CurrentPlaylistSortingDirection);
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
        public event EventHandler<bool>? IsPlayPlaylistCollectionAsyncAvailableChanged
        {
            add => _collection.IsPlayPlaylistCollectionAsyncAvailableChanged += value;
            remove => _collection.IsPlayPlaylistCollectionAsyncAvailableChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<bool>? IsPausePlaylistCollectionAsyncAvailableChanged
        {
            add => _collection.IsPausePlaylistCollectionAsyncAvailableChanged += value;
            remove => _collection.IsPausePlaylistCollectionAsyncAvailableChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<int>? PlaylistItemsCountChanged
        {
            add => _collection.PlaylistItemsCountChanged += value;
            remove => _collection.PlaylistItemsCountChanged -= value;
        }

        /// <inheritdoc />
        public event CollectionChangedEventHandler<IPlaylistCollectionItem>? PlaylistItemsChanged
        {
            add => _collection.PlaylistItemsChanged += value;
            remove => _collection.PlaylistItemsChanged -= value;
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
        public string Id => _collection.Id;

        /// <inheritdoc />
        public string Name => _collection.Name;

        /// <inheritdoc />
        public int TotalPlaylistItemsCount => _collection.TotalPlaylistItemsCount;

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
        public PlaylistSortingType CurrentPlaylistSortingType { get; private set; }

        /// <inheritdoc />
        public SortDirection CurrentPlaylistSortingDirection { get; private set; }

        ///<inheritdoc />
        public ObservableCollection<IPlaylistCollectionItem> UnsortedPlaylists { get; }

        /// <inheritdoc />
        public ObservableCollection<IPlaylistCollectionItem> Playlists { get; }

        /// <inheritdoc />
        public ObservableCollection<IImage> Images { get; }

        /// <inheritdoc />
        public ObservableCollection<IUrl> Urls { get; }

        /// <inheritdoc/>
        public MainViewModel Root { get; }

        /// <inheritdoc cref="IMerged{T}.SourceCores" />
        public IReadOnlyList<ICore> SourceCores { get; }

        /// <inheritdoc />
        IReadOnlyList<ICorePlaylistCollectionItem> IMerged<ICorePlaylistCollectionItem>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICorePlaylistCollection> IMerged<ICorePlaylistCollection>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreImageCollection> IMerged<ICoreImageCollection>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreUrlCollection> IMerged<ICoreUrlCollection>.Sources => Sources;

        /// <summary>
        /// The items that were merged to form this <see cref="PlaylistCollectionViewModel"/>.
        /// </summary>
        public IReadOnlyList<ICorePlaylistCollection> Sources => _collection.GetSources<ICorePlaylistCollection>();

        /// <inheritdoc />
        public bool IsPlayPlaylistCollectionAsyncAvailable => _collection.IsPlayPlaylistCollectionAsyncAvailable;

        /// <inheritdoc />
        public bool IsPausePlaylistCollectionAsyncAvailable => _collection.IsPausePlaylistCollectionAsyncAvailable;

        /// <inheritdoc />
        public bool IsChangeNameAsyncAvailable => _collection.IsChangeNameAsyncAvailable;

        /// <inheritdoc />
        public bool IsChangeDescriptionAsyncAvailable => _collection.IsChangeDescriptionAsyncAvailable;

        /// <inheritdoc />
        public bool IsChangeDurationAsyncAvailable => _collection.IsChangeDurationAsyncAvailable;

        /// <inheritdoc />
        public Task<bool> IsAddPlaylistItemAvailableAsync(int index) => _collection.IsAddPlaylistItemAvailableAsync(index);

        /// <inheritdoc />
        public Task<bool> IsAddImageAvailableAsync(int index) => _collection.IsAddImageAvailableAsync(index);

        /// <inheritdoc />
        public Task<bool> IsAddUrlAvailableAsync(int index) => _collection.IsAddUrlAvailableAsync(index);

        /// <inheritdoc />
        public Task<bool> IsRemovePlaylistItemAvailableAsync(int index) => _collection.IsRemovePlaylistItemAvailableAsync(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveImageAvailableAsync(int index) => _collection.IsRemoveImageAvailableAsync(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveUrlAvailableAsync(int index) => _collection.IsRemoveUrlAvailableAsync(index);

        /// <inheritdoc />
        public Task ChangeDescriptionAsync(string? description) => _collection.ChangeDescriptionAsync(description);

        /// <inheritdoc />
        public Task ChangeDurationAsync(TimeSpan duration) => _collection.ChangeDurationAsync(duration);

        /// <inheritdoc />
        public Task ChangeNameAsync(string name) => ChangeNameInternalAsync(name);

        /// <inheritdoc />
        public Task StartDownloadOperationAsync(DownloadOperation operation) => _collection.StartDownloadOperationAsync(operation);

        /// <inheritdoc />
        public Task AddPlaylistItemAsync(IPlaylistCollectionItem playlist, int index) => _collection.AddPlaylistItemAsync(playlist, index);

        /// <inheritdoc />
        public Task AddImageAsync(IImage image, int index) => _collection.AddImageAsync(image, index);

        /// <inheritdoc />
        public Task AddUrlAsync(IUrl image, int index) => _collection.AddUrlAsync(image, index);

        /// <inheritdoc />
        public Task RemovePlaylistItemAsync(int index) => _collection.RemovePlaylistItemAsync(index);

        /// <inheritdoc />
        public Task RemoveImageAsync(int index) => _collection.RemoveImageAsync(index);

        /// <inheritdoc />
        public Task RemoveUrlAsync(int index) => _collection.RemoveUrlAsync(index);

        /// <inheritdoc />
        public Task PlayPlaylistCollectionAsync() => _collection.PlayPlaylistCollectionAsync();

        /// <inheritdoc />
        public Task PlayPlaylistCollectionAsync(IPlaylistCollectionItem playlistItem) => _collection.PlayPlaylistCollectionAsync(playlistItem);

        /// <inheritdoc />
        public Task PausePlaylistCollectionAsync() => _collection.PausePlaylistCollectionAsync();

        /// <inheritdoc />
        public Task<IReadOnlyList<IPlaylistCollectionItem>> GetPlaylistItemsAsync(int limit, int offset) => _collection.GetPlaylistItemsAsync(limit, offset);

        /// <inheritdoc />
        public Task<IReadOnlyList<IImage>> GetImagesAsync(int limit, int offset) => _collection.GetImagesAsync(limit, offset);

        /// <inheritdoc />
        public Task<IReadOnlyList<IUrl>> GetUrlsAsync(int limit, int offset) => _collection.GetUrlsAsync(limit, offset);

        /// <inheritdoc />
        public async Task PopulateMorePlaylistsAsync(int limit, CancellationToken cancellationToken = default)
        {
            using (await Flow.EasySemaphore(_populatePlaylistsMutex))
            {
                var items = await _collection.GetPlaylistItemsAsync(limit, Playlists.Count);

                _syncContext.Post(_ =>
                {
                    foreach (var item in items)
                    {
                        switch (item)
                        {
                            case IPlaylist playlist:
                                Playlists.Add(new PlaylistViewModel(Root, playlist));
                                break;
                            case IPlaylistCollection collection:
                                Playlists.Add(new PlaylistCollectionViewModel(Root, collection));
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
                var items = await _collection.GetImagesAsync(limit, Images.Count);

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
                var items = await _collection.GetUrlsAsync(limit, Urls.Count);

                _syncContext.Post(_ =>
                {
                    foreach (var item in items)
                        Urls.Add(item);
                }, null);
            }
        }

        /// <inheritdoc />
        public Task InitPlaylistCollectionAsync(CancellationToken cancellationToken = default) => CollectionInit.PlaylistCollection(this, cancellationToken);

        /// <inheritdoc />
        public Task InitImageCollectionAsync(CancellationToken cancellationToken = default) => CollectionInit.ImageCollection(this, cancellationToken);

        ///<inheritdoc />
        public void SortPlaylistCollection(PlaylistSortingType playlistSorting, SortDirection sortDirection)
        {
            CurrentPlaylistSortingType = playlistSorting;
            CurrentPlaylistSortingDirection = sortDirection;

            CollectionSorting.SortPlaylists(Playlists, playlistSorting, sortDirection, UnsortedPlaylists);
        }

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
        public IRelayCommand<PlaylistSortingType> ChangePlaylistCollectionSortingTypeCommand { get; }

        /// <inheritdoc />
        public IRelayCommand<SortDirection> ChangePlaylistCollectionSortingDirectionCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand<IPlaylistCollectionItem> PlayPlaylistAsyncCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand PlayPlaylistCollectionAsyncCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand PausePlaylistCollectionAsyncCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand<int> PopulateMorePlaylistsCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand<int> PopulateMoreImagesCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand<int> PopulateMoreUrlsCommand { get; }

        /// <inheritdoc />
        public Task InitAsync(CancellationToken cancellationToken = default)
        {
            if (IsInitialized)
                return Task.CompletedTask;

            IsInitialized = true;

            return Task.WhenAll(InitImageCollectionAsync(cancellationToken), InitPlaylistCollectionAsync(cancellationToken));
        }

        /// <inheritdoc />
        public bool IsInitialized { get; private set; }

        /// <inheritdoc />
        public IAsyncRelayCommand InitPlaylistCollectionAsyncCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand InitImageCollectionAsyncCommand { get; }

        /// <inheritdoc />
        public bool Equals(ICorePlaylistCollectionItem other) => _collection.Equals(other);

        /// <inheritdoc />
        public bool Equals(ICorePlaylistCollection other) => _collection.Equals(other);

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
