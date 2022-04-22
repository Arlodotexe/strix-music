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
    /// A wrapper for <see cref="IArtistCollection"/> that contains props and methods for a ViewModel.
    /// </summary>
    public class ArtistCollectionViewModel : ObservableObject, ISdkViewModel, IArtistCollectionViewModel, IImageCollectionViewModel
    {
        private readonly IArtistCollection _collection;

        private readonly SemaphoreSlim _populateArtistsMutex = new(1, 1);
        private readonly SemaphoreSlim _populateImagesMutex = new(1, 1);
        private readonly SemaphoreSlim _populateUrlsMutex = new(1, 1);
        private readonly SynchronizationContext _syncContext;

        /// <summary>
        /// Creates a new instance of <see cref="ArtistCollectionViewModel"/>.
        /// </summary>
        /// <param name="collection">The <see cref="IArtistCollection"/> to wrap around.</param>
        internal ArtistCollectionViewModel(IArtistCollection collection)
        {
            _syncContext = SynchronizationContext.Current;

            _collection = collection;

            SourceCores = collection.GetSourceCores<ICoreArtistCollection>().Select(x => new CoreViewModel(x)).ToList();

            PopulateMoreArtistsCommand = new AsyncRelayCommand<int>(PopulateMoreArtistsAsync);
            PopulateMoreImagesCommand = new AsyncRelayCommand<int>(PopulateMoreImagesAsync);
            PopulateMoreUrlsCommand = new AsyncRelayCommand<int>(PopulateMoreUrlsAsync);

            PauseArtistCollectionAsyncCommand = new AsyncRelayCommand(PauseArtistCollectionAsync);
            PlayArtistCollectionAsyncCommand = new AsyncRelayCommand(PlayArtistCollectionAsync);

            ChangeDescriptionAsyncCommand = new AsyncRelayCommand<string?>(ChangeDescriptionAsync);
            ChangeNameAsyncCommand = new AsyncRelayCommand<string>(ChangeNameInternalAsync);
            ChangeDurationAsyncCommand = new AsyncRelayCommand<TimeSpan>(ChangeDurationAsync);

            ChangeArtistCollectionSortingTypeCommand = new RelayCommand<ArtistSortingType>(x => SortArtistCollection(x, CurrentArtistSortingDirection));
            ChangeArtistCollectionSortingDirectionCommand = new RelayCommand<SortDirection>(x => SortArtistCollection(CurrentArtistSortingType, x));

            InitArtistCollectionAsyncCommand = new AsyncRelayCommand(InitArtistCollectionAsync);
            InitImageCollectionAsyncCommand = new AsyncRelayCommand(InitImageCollectionAsync);

            PlayArtistAsyncCommand = new AsyncRelayCommand<IArtistCollectionItem>((x, y) => _collection.PlayArtistCollectionAsync(x ?? ThrowHelper.ThrowArgumentNullException<IArtistCollectionItem>(nameof(x)), y));

            Artists = new ObservableCollection<IArtistCollectionItem>();
            UnsortedArtists = new ObservableCollection<IArtistCollectionItem>();
            Images = new ObservableCollection<IImage>();
            Urls = new ObservableCollection<IUrl>();

            AttachEvents();
        }

        /// <inheritdoc />
        public Task InitAsync(CancellationToken cancellationToken = default)
        {
            if (IsInitialized)
                return Task.CompletedTask;

            IsInitialized = true;

            return Task.WhenAll(InitArtistCollectionAsync(cancellationToken), InitImageCollectionAsync(cancellationToken));
        }

        private void AttachEvents()
        {
            PlaybackStateChanged += OnPlaybackStateChanged;
            NameChanged += OnNameChanged;
            DescriptionChanged += OnDescriptionChanged;
            LastPlayedChanged += OnLastPlayedChanged;
            Flow.Catch<NotSupportedException>(() => DownloadInfoChanged += OnDownloadInfoChanged);

            IsPlayArtistCollectionAsyncAvailableChanged += OnIsPlayArtistCollectionAsyncAvailableChanged;
            IsPauseArtistCollectionAsyncAvailableChanged += OnIsPauseArtistCollectionAsyncAvailableChanged;
            IsChangeNameAsyncAvailableChanged += OnIsChangeNameAsyncAvailableChanged;
            IsChangeDurationAsyncAvailableChanged += OnIsChangeDurationAsyncAvailableChanged;
            IsChangeDescriptionAsyncAvailableChanged += OnIsChangeDescriptionAsyncAvailableChanged;

            ArtistItemsCountChanged += OnArtistItemsCountChanged;
            ArtistItemsChanged += ArtistCollectionViewModel_ArtistItemsChanged;
            ImagesCountChanged += ArtistCollectionViewModel_ImagesCountChanged;
            ImagesChanged += ArtistCollectionViewModel_ImagesChanged;
            UrlsCountChanged += ArtistCollectionViewModel_UrlsCountChanged;
            UrlsChanged += ArtistCollectionViewModel_UrlsChanged;
        }

        private void DetachEvents()
        {
            PlaybackStateChanged -= OnPlaybackStateChanged;
            NameChanged -= OnNameChanged;
            DescriptionChanged -= OnDescriptionChanged;
            LastPlayedChanged -= OnLastPlayedChanged;
            Flow.Catch<NotSupportedException>(() => DownloadInfoChanged -= OnDownloadInfoChanged);

            IsPlayArtistCollectionAsyncAvailableChanged -= OnIsPlayArtistCollectionAsyncAvailableChanged;
            IsPauseArtistCollectionAsyncAvailableChanged -= OnIsPauseArtistCollectionAsyncAvailableChanged;
            IsChangeNameAsyncAvailableChanged -= OnIsChangeNameAsyncAvailableChanged;
            IsChangeDurationAsyncAvailableChanged -= OnIsChangeDurationAsyncAvailableChanged;
            IsChangeDescriptionAsyncAvailableChanged -= OnIsChangeDescriptionAsyncAvailableChanged;

            ArtistItemsCountChanged -= OnArtistItemsCountChanged;
            ArtistItemsChanged -= ArtistCollectionViewModel_ArtistItemsChanged;
            ImagesCountChanged -= ArtistCollectionViewModel_ImagesCountChanged;
            ImagesChanged -= ArtistCollectionViewModel_ImagesChanged;
            UrlsCountChanged -= ArtistCollectionViewModel_UrlsCountChanged;
            UrlsChanged -= ArtistCollectionViewModel_UrlsChanged;
        }

        private void OnNameChanged(object sender, string e) => _syncContext.Post(_ => OnPropertyChanged(nameof(Name)), null);

        private void OnDescriptionChanged(object sender, string? e) => _syncContext.Post(_ => OnPropertyChanged(nameof(Description)), null);

        private void OnPlaybackStateChanged(object sender, PlaybackState e) => _syncContext.Post(_ => OnPropertyChanged(nameof(PlaybackState)), null);

        private void OnDownloadInfoChanged(object sender, DownloadInfo e) => _syncContext.Post(_ => OnPropertyChanged(nameof(DownloadInfo)), null);

        private void ArtistCollectionViewModel_ImagesCountChanged(object sender, int e) => _syncContext.Post(_ => OnPropertyChanged(nameof(TotalImageCount)), null);

        private void ArtistCollectionViewModel_UrlsCountChanged(object sender, int e) => _syncContext.Post(_ => OnPropertyChanged(nameof(TotalUrlCount)), null);

        private void OnArtistItemsCountChanged(object sender, int e) => _syncContext.Post(_ => OnPropertyChanged(nameof(TotalArtistItemsCount)), null);

        private void OnLastPlayedChanged(object sender, DateTime? e) => _syncContext.Post(_ => OnPropertyChanged(nameof(LastPlayed)), null);

        private void OnIsChangeDescriptionAsyncAvailableChanged(object sender, bool e) => _syncContext.Post(_ => OnPropertyChanged(nameof(IsChangeDescriptionAsyncAvailable)), null);

        private void OnIsChangeDurationAsyncAvailableChanged(object sender, bool e) => _syncContext.Post(_ => OnPropertyChanged(nameof(IsChangeDurationAsyncAvailable)), null);

        private void OnIsChangeNameAsyncAvailableChanged(object sender, bool e) => _syncContext.Post(_ => OnPropertyChanged(nameof(IsChangeNameAsyncAvailable)), null);

        private void OnIsPauseArtistCollectionAsyncAvailableChanged(object sender, bool e) => _syncContext.Post(_ => OnPropertyChanged(nameof(IsPauseArtistCollectionAsyncAvailable)), null);

        private void OnIsPlayArtistCollectionAsyncAvailableChanged(object sender, bool e) => _syncContext.Post(_ => OnPropertyChanged(nameof(IsPlayArtistCollectionAsyncAvailable)), null);

        private void ArtistCollectionViewModel_ImagesChanged(object sender, IReadOnlyList<CollectionChangedItem<IImage>> addedItems, IReadOnlyList<CollectionChangedItem<IImage>> removedItems) => _ = Threading.OnPrimaryThread(() =>
        {
            Images.ChangeCollection(addedItems, removedItems);
        });

        private void ArtistCollectionViewModel_UrlsChanged(object sender, IReadOnlyList<CollectionChangedItem<IUrl>> addedItems, IReadOnlyList<CollectionChangedItem<IUrl>> removedItems) => _ = Threading.OnPrimaryThread(() =>
        {
            Urls.ChangeCollection(addedItems, removedItems);
        });

        private void ArtistCollectionViewModel_ArtistItemsChanged(object sender, IReadOnlyList<CollectionChangedItem<IArtistCollectionItem>> addedItems, IReadOnlyList<CollectionChangedItem<IArtistCollectionItem>> removedItems) => _ = Threading.OnPrimaryThread(() =>
        {
            if (CurrentArtistSortingType == ArtistSortingType.Unsorted)
            {
                Artists.ChangeCollection(addedItems, removedItems, item => item.Data switch
                {
                    IArtist artist => new ArtistViewModel(artist),
                    IArtistCollection collection => new ArtistCollectionViewModel(collection),
                    _ => ThrowHelper.ThrowNotSupportedException<IArtistCollectionItem>(
                        $"{item.Data.GetType()} not supported for adding to {GetType()}")
                });
            }
            else
            {
                // Make sure both ordered and unordered artists are updated. 
                UnsortedArtists.ChangeCollection(addedItems, removedItems, item => item.Data switch
                {
                    IArtist artist => new ArtistViewModel(artist),
                    IArtistCollection collection => new ArtistCollectionViewModel(collection),
                    _ => ThrowHelper.ThrowNotSupportedException<IArtistCollectionItem>(
                        $"{item.Data.GetType()} not supported for adding to {GetType()}")
                });

                foreach (var item in UnsortedArtists)
                {
                    if (!Artists.Contains(item))
                        Artists.Add(item);
                }

                foreach (var item in Artists)
                {
                    if (!UnsortedArtists.Contains(item))
                        Artists.Remove(item);
                }

                SortArtistCollection(CurrentArtistSortingType, CurrentArtistSortingDirection);
            }
        });

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
        public event EventHandler<bool>? IsPauseArtistCollectionAsyncAvailableChanged
        {
            add => _collection.IsPauseArtistCollectionAsyncAvailableChanged += value;
            remove => _collection.IsPauseArtistCollectionAsyncAvailableChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<bool>? IsPlayArtistCollectionAsyncAvailableChanged
        {
            add => _collection.IsPlayArtistCollectionAsyncAvailableChanged += value;
            remove => _collection.IsPlayArtistCollectionAsyncAvailableChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<int>? ArtistItemsCountChanged
        {
            add => _collection.ArtistItemsCountChanged += value;
            remove => _collection.ArtistItemsCountChanged -= value;
        }

        /// <inheritdoc />
        public event CollectionChangedEventHandler<IArtistCollectionItem>? ArtistItemsChanged
        {
            add => _collection.ArtistItemsChanged += value;
            remove => _collection.ArtistItemsChanged -= value;
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
        public bool IsChangeNameAsyncAvailable => _collection.IsChangeNameAsyncAvailable;

        /// <inheritdoc />
        public bool IsChangeDescriptionAsyncAvailable => _collection.IsChangeDescriptionAsyncAvailable;

        /// <inheritdoc />
        public bool IsChangeDurationAsyncAvailable => _collection.IsChangeDurationAsyncAvailable;

        /// <inheritdoc />
        public string Name => _collection.Name;

        /// <inheritdoc />
        public string? Description => _collection.Description;

        /// <inheritdoc />
        public PlaybackState PlaybackState => _collection.PlaybackState;

        /// <inheritdoc />
        public DownloadInfo DownloadInfo => _collection.DownloadInfo;

        /// <inheritdoc />
        public ArtistSortingType CurrentArtistSortingType { get; private set; }

        /// <inheritdoc />
        public SortDirection CurrentArtistSortingDirection { get; private set; }

        /// <inheritdoc />
        public TimeSpan Duration => _collection.Duration;

        /// <inheritdoc />
        public DateTime? LastPlayed => _collection.LastPlayed;

        /// <inheritdoc />
        public DateTime? AddedAt => _collection.AddedAt;

        /// <inheritdoc />
        public ObservableCollection<IArtistCollectionItem> Artists { get; }

        /// <inheritdoc />
        public ObservableCollection<IArtistCollectionItem> UnsortedArtists { get; }

        /// <inheritdoc />
        public ObservableCollection<IImage> Images { get; }

        /// <inheritdoc />
        public ObservableCollection<IUrl> Urls { get; }

        /// <inheritdoc />
        public int TotalArtistItemsCount => _collection.TotalArtistItemsCount;

        /// <inheritdoc />
        public int TotalImageCount => _collection.TotalImageCount;

        /// <inheritdoc />
        public int TotalUrlCount => _collection.TotalUrlCount;

        /// <inheritdoc />
        public bool IsPlayArtistCollectionAsyncAvailable => _collection.IsPlayArtistCollectionAsyncAvailable;

        /// <inheritdoc />
        public bool IsPauseArtistCollectionAsyncAvailable => _collection.IsPauseArtistCollectionAsyncAvailable;

        /// <inheritdoc />
        public bool IsInitialized { get; private set; }

        /// <inheritdoc cref="IMerged{T}.SourceCores" />
        public IReadOnlyList<ICore> SourceCores { get; }

        /// <summary>
        /// The sources that were merged to form this member.
        /// </summary>
        public IReadOnlyList<ICoreArtistCollection> Sources => _collection.GetSources<ICoreArtistCollection>();

        /// <inheritdoc />
        IReadOnlyList<ICoreArtistCollection> IMerged<ICoreArtistCollection>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreArtistCollectionItem> IMerged<ICoreArtistCollectionItem>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreImageCollection> IMerged<ICoreImageCollection>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreUrlCollection> IMerged<ICoreUrlCollection>.Sources => Sources;

        ///<inheritdoc />
        public void SortArtistCollection(ArtistSortingType artistSorting, SortDirection sortDirection)
        {
            CurrentArtistSortingType = artistSorting;
            CurrentArtistSortingDirection = sortDirection;

            CollectionSorting.SortArtists(Artists, artistSorting, sortDirection, UnsortedArtists);
        }

        /// <inheritdoc />
        public Task<bool> IsAddArtistItemAvailableAsync(int index, CancellationToken cancellationToken = default) => _collection.IsAddArtistItemAvailableAsync(index, cancellationToken);

        /// <inheritdoc />
        public Task<bool> IsRemoveArtistItemAvailableAsync(int index, CancellationToken cancellationToken = default) => _collection.IsRemoveArtistItemAvailableAsync(index, cancellationToken);

        /// <inheritdoc />
        public Task<bool> IsAddImageAvailableAsync(int index, CancellationToken cancellationToken = default) => _collection.IsAddImageAvailableAsync(index, cancellationToken);

        /// <inheritdoc />
        public Task<bool> IsRemoveImageAvailableAsync(int index, CancellationToken cancellationToken = default) => _collection.IsRemoveImageAvailableAsync(index, cancellationToken);

        /// <inheritdoc />
        public Task<bool> IsAddUrlAvailableAsync(int index, CancellationToken cancellationToken = default) => _collection.IsAddUrlAvailableAsync(index, cancellationToken);

        /// <inheritdoc />
        public Task<bool> IsRemoveUrlAvailableAsync(int index, CancellationToken cancellationToken = default) => _collection.IsRemoveUrlAvailableAsync(index, cancellationToken);

        /// <inheritdoc />
        public Task StartDownloadOperationAsync(DownloadOperation operation, CancellationToken cancellationToken = default) => _collection.StartDownloadOperationAsync(operation, cancellationToken);

        /// <inheritdoc />
        public Task ChangeNameAsync(string name, CancellationToken cancellationToken = default) => ChangeNameInternalAsync(name, cancellationToken);

        /// <inheritdoc />
        public Task ChangeDescriptionAsync(string? description, CancellationToken cancellationToken = default) => _collection.ChangeDescriptionAsync(description, cancellationToken);

        /// <inheritdoc />
        public Task ChangeDurationAsync(TimeSpan duration, CancellationToken cancellationToken = default) => _collection.ChangeDurationAsync(duration, cancellationToken);

        /// <inheritdoc />
        public Task AddArtistItemAsync(IArtistCollectionItem artistItem, int index, CancellationToken cancellationToken = default) => _collection.AddArtistItemAsync(artistItem, index, cancellationToken);

        /// <inheritdoc />
        public Task RemoveArtistItemAsync(int index, CancellationToken cancellationToken = default) => _collection.RemoveArtistItemAsync(index, cancellationToken);

        /// <inheritdoc />
        public Task AddImageAsync(IImage image, int index, CancellationToken cancellationToken = default) => _collection.AddImageAsync(image, index, cancellationToken);

        /// <inheritdoc />
        public Task RemoveImageAsync(int index, CancellationToken cancellationToken = default) => _collection.RemoveImageAsync(index, cancellationToken);

        /// <inheritdoc />
        public Task AddUrlAsync(IUrl image, int index, CancellationToken cancellationToken = default) => _collection.AddUrlAsync(image, index, cancellationToken);

        /// <inheritdoc />
        public Task RemoveUrlAsync(int index, CancellationToken cancellationToken = default) => _collection.RemoveUrlAsync(index, cancellationToken);

        /// <inheritdoc />
        public Task PlayArtistCollectionAsync(CancellationToken cancellationToken = default) => _collection.PlayArtistCollectionAsync(cancellationToken);

        /// <inheritdoc />
        public Task PauseArtistCollectionAsync(CancellationToken cancellationToken = default) => _collection.PlayArtistCollectionAsync(cancellationToken);

        /// <inheritdoc />
        public Task PlayArtistCollectionAsync(IArtistCollectionItem artistItem, CancellationToken cancellationToken = default) => _collection.PlayArtistCollectionAsync(artistItem, cancellationToken);

        /// <inheritdoc />
        public Task<IReadOnlyList<IArtistCollectionItem>> GetArtistItemsAsync(int limit, int offset, CancellationToken cancellationToken = default) => _collection.GetArtistItemsAsync(limit, offset, cancellationToken);

        /// <inheritdoc />
        public Task<IReadOnlyList<IImage>> GetImagesAsync(int limit, int offset, CancellationToken cancellationToken = default) => _collection.GetImagesAsync(limit, offset, cancellationToken);

        /// <inheritdoc />
        public Task<IReadOnlyList<IUrl>> GetUrlsAsync(int limit, int offset, CancellationToken cancellationToken = default) => _collection.GetUrlsAsync(limit, offset, cancellationToken);

        /// <inheritdoc />
        public async Task PopulateMoreArtistsAsync(int limit, CancellationToken cancellationToken = default)
        {
            using (await Flow.EasySemaphore(_populateArtistsMutex))
            {
                using var releaseReg = cancellationToken.Register(() => _populateArtistsMutex.Release());
                var items = await _collection.GetArtistItemsAsync(limit, Artists.Count, cancellationToken);

                await Threading.OnPrimaryThread(() =>
                {
                    foreach (var item in items)
                    {
                        switch (item)
                        {
                            case IArtist artist:
                                Artists.Add(new ArtistViewModel(artist));
                                break;
                            case IArtistCollection collection:
                                Artists.Add(new ArtistCollectionViewModel(collection));
                                break;
                        }
                    }
                });
            }
        }

        /// <inheritdoc />
        public async Task PopulateMoreImagesAsync(int limit, CancellationToken cancellationToken = default)
        {
            using (await Flow.EasySemaphore(_populateImagesMutex))
            {
                using var releaseReg = cancellationToken.Register(() => _populateImagesMutex.Release());
                var items = await _collection.GetImagesAsync(limit, Images.Count, cancellationToken);

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
        public async Task PopulateMoreUrlsAsync(int limit, CancellationToken cancellationToken = default)
        {
            using (await Flow.EasySemaphore(_populateUrlsMutex))
            {
                using var releaseReg = cancellationToken.Register(() => _populateUrlsMutex.Release());
                var items = await _collection.GetUrlsAsync(limit, Urls.Count, cancellationToken);

                _ = Threading.OnPrimaryThread(() =>
                {
                    foreach (var item in items)
                    {
                        Urls.Add(item);
                    }
                });
            }
        }

        /// <inheritdoc />
        public Task InitImageCollectionAsync(CancellationToken cancellationToken = default) => CollectionInit.ImageCollection(this, cancellationToken);

        /// <inheritdoc />
        public Task InitArtistCollectionAsync(CancellationToken cancellationToken = default) => CollectionInit.ArtistCollection(this, cancellationToken);

        /// <inheritdoc />
        public IAsyncRelayCommand InitArtistCollectionAsyncCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand<int> PopulateMoreArtistsCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand<int> PopulateMoreImagesCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand<int> PopulateMoreUrlsCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand<IArtistCollectionItem> PlayArtistAsyncCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand PlayArtistCollectionAsyncCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand PauseArtistCollectionAsyncCommand { get; }

        /// <inheritdoc />
        public IRelayCommand<ArtistSortingType> ChangeArtistCollectionSortingTypeCommand { get; }

        /// <inheritdoc />
        public IRelayCommand<SortDirection> ChangeArtistCollectionSortingDirectionCommand { get; }

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
        public IAsyncRelayCommand InitImageCollectionAsyncCommand { get; }

        /// <inheritdoc />
        public bool Equals(ICoreArtistCollectionItem other) => _collection.Equals(other);

        /// <inheritdoc />
        public bool Equals(ICoreImageCollection other) => _collection.Equals(other);

        /// <inheritdoc />
        public bool Equals(ICoreUrlCollection other) => _collection.Equals(other);

        /// <inheritdoc />
        public bool Equals(ICoreArtistCollection other) => _collection.Equals(other);

        private Task ChangeNameInternalAsync(string? name, CancellationToken cancellationToken = default)
        {
            Guard.IsNotNull(name, nameof(name));
            return _collection.ChangeNameAsync(name, cancellationToken);
        }

        /// <inheritdoc />
        public ValueTask DisposeAsync()
        {
            DetachEvents();
            return _collection.DisposeAsync();
        }
    }
}
