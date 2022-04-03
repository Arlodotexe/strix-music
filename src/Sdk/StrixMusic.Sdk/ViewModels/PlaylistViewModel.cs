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
    /// A ViewModel for <see cref="IPlaylist"/>.
    /// </summary>
    public sealed class PlaylistViewModel : ObservableObject, ISdkViewModel, IPlaylist, ITrackCollectionViewModel, IImageCollectionViewModel
    {
        private readonly IPlaylist _playlist;
        private readonly IUserProfile? _owner;

        private readonly SemaphoreSlim _populateTracksMutex = new(1, 1);
        private readonly SemaphoreSlim _populateImagesMutex = new(1, 1);
        private readonly SemaphoreSlim _populateUrlsMutex = new(1, 1);
        private readonly SynchronizationContext _syncContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlaylistViewModel"/> class.
        /// </summary>
        /// <param name="root">The <see cref="MainViewModel"/> that this or the object that created this originated from.</param>
        /// <param name="playlist">The <see cref="IPlaylist"/> to wrap.</param>
        internal PlaylistViewModel(MainViewModel root, IPlaylist playlist)
        {
            _syncContext = SynchronizationContext.Current;

            _playlist = root.Plugins.ModelPlugins.Playlist.Execute(playlist);
            Root = root;

            SourceCores = playlist.GetSourceCores<ICorePlaylist>().Select(root.GetLoadedCore).ToList();

            PauseTrackCollectionAsyncCommand = new AsyncRelayCommand(PauseTrackCollectionAsync);
            PlayTrackCollectionAsyncCommand = new AsyncRelayCommand(PlayTrackCollectionAsync);

            PlayTrackAsyncCommand = new AsyncRelayCommand<ITrack>(x => _playlist.PlayTrackCollectionAsync(x ?? ThrowHelper.ThrowArgumentNullException<ITrack>()));

            ChangeNameAsyncCommand = new AsyncRelayCommand<string>(ChangeNameInternalAsync);
            ChangeDescriptionAsyncCommand = new AsyncRelayCommand<string?>(ChangeDescriptionAsync);
            ChangeDurationAsyncCommand = new AsyncRelayCommand<TimeSpan>(ChangeDurationAsync);

            PopulateMoreTracksCommand = new AsyncRelayCommand<int>(PopulateMoreTracksAsync);
            PopulateMoreImagesCommand = new AsyncRelayCommand<int>(PopulateMoreImagesAsync);
            PopulateMoreUrlsCommand = new AsyncRelayCommand<int>(PopulateMoreUrlsAsync);

            InitTrackCollectionAsyncCommand = new AsyncRelayCommand(InitTrackCollectionAsync);
            InitImageCollectionAsyncCommand = new AsyncRelayCommand(InitImageCollectionAsync);

            ChangeTrackCollectionSortingTypeCommand = new RelayCommand<TrackSortingType>(x => SortTrackCollection(x, CurrentTracksSortingDirection));
            ChangeTrackCollectionSortingDirectionCommand = new RelayCommand<SortDirection>(x => SortTrackCollection(CurrentTracksSortingType, x));

            if (_playlist.Owner != null)
                _owner = new UserProfileViewModel(root, _playlist.Owner);

            if (_playlist.RelatedItems != null)
                RelatedItems = new PlayableCollectionGroupViewModel(root, _playlist.RelatedItems);

            Tracks = new ObservableCollection<TrackViewModel>();
            Images = new ObservableCollection<IImage>();
            Urls = new ObservableCollection<IUrl>();

            UnsortedTracks = new ObservableCollection<TrackViewModel>();

            AttachEvents();
        }

        /// <inheritdoc />
        public Task InitAsync(CancellationToken cancellationToken = default)
        {
            if (IsInitialized)
                return Task.CompletedTask;

            IsInitialized = true;

            return Task.WhenAll(InitImageCollectionAsync(cancellationToken), InitTrackCollectionAsync(cancellationToken));
        }

        private void AttachEvents()
        {
            DescriptionChanged += CorePlaylistDescriptionChanged;
            NameChanged += CorePlaylistNameChanged;
            PlaybackStateChanged += CorePlaylistPlaybackStateChanged;
            LastPlayedChanged += CorePlaylistLastPlayedChanged;
            Flow.Catch<NotSupportedException>(() => DownloadInfoChanged += OnDownloadInfoChanged);

            IsPlayTrackCollectionAsyncAvailableChanged += OnIsPlayTrackCollectionAsyncAvailableChanged;
            IsPauseTrackCollectionAsyncAvailableChanged += OnIsPauseTrackCollectionAsyncAvailableChanged;
            IsChangeNameAsyncAvailableChanged += OnIsChangeNameAsyncAvailableChanged;
            IsChangeDurationAsyncAvailableChanged += OnIsChangeDurationAsyncAvailableChanged;
            IsChangeDescriptionAsyncAvailableChanged += OnIsChangeDescriptionAsyncAvailableChanged;

            TracksCountChanged += PlaylistOnTrackItemsCountChanged;
            TracksChanged += PlaylistViewModel_TrackItemsChanged;
            ImagesCountChanged += PlaylistViewModel_ImagesCountChanged;
            ImagesChanged += PlaylistViewModel_ImagesChanged;
            UrlsCountChanged += PlaylistViewModel_UrlsCountChanged;
            UrlsChanged += PlaylistViewModel_UrlsChanged;
        }

        private void DetachEvents()
        {
            DescriptionChanged -= CorePlaylistDescriptionChanged;
            NameChanged -= CorePlaylistNameChanged;
            PlaybackStateChanged -= CorePlaylistPlaybackStateChanged;
            LastPlayedChanged += CorePlaylistLastPlayedChanged;
            Flow.Catch<NotSupportedException>(() => DownloadInfoChanged -= OnDownloadInfoChanged);

            IsPlayTrackCollectionAsyncAvailableChanged -= OnIsPlayTrackCollectionAsyncAvailableChanged;
            IsPauseTrackCollectionAsyncAvailableChanged -= OnIsPauseTrackCollectionAsyncAvailableChanged;
            IsChangeNameAsyncAvailableChanged -= OnIsChangeNameAsyncAvailableChanged;
            IsChangeDurationAsyncAvailableChanged -= OnIsChangeDurationAsyncAvailableChanged;
            IsChangeDescriptionAsyncAvailableChanged -= OnIsChangeDescriptionAsyncAvailableChanged;

            TracksCountChanged -= PlaylistOnTrackItemsCountChanged;
            TracksChanged -= PlaylistViewModel_TrackItemsChanged;
            ImagesCountChanged -= PlaylistViewModel_ImagesCountChanged;
            ImagesChanged -= PlaylistViewModel_ImagesChanged;
            UrlsCountChanged -= PlaylistViewModel_UrlsCountChanged;
            UrlsChanged -= PlaylistViewModel_UrlsChanged;
        }

        /// <inheritdoc />
        public event EventHandler<PlaybackState>? PlaybackStateChanged
        {
            add => _playlist.PlaybackStateChanged += value;
            remove => _playlist.PlaybackStateChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<DownloadInfo>? DownloadInfoChanged
        {
            add => _playlist.DownloadInfoChanged += value;
            remove => _playlist.DownloadInfoChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<string>? NameChanged
        {
            add => _playlist.NameChanged += value;
            remove => _playlist.NameChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<string?>? DescriptionChanged
        {
            add => _playlist.DescriptionChanged += value;
            remove => _playlist.DescriptionChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<TimeSpan>? DurationChanged
        {
            add => _playlist.DurationChanged += value;
            remove => _playlist.DurationChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<DateTime?>? LastPlayedChanged
        {
            add => _playlist.LastPlayedChanged += value;
            remove => _playlist.LastPlayedChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<bool>? IsChangeNameAsyncAvailableChanged
        {
            add => _playlist.IsChangeNameAsyncAvailableChanged += value;
            remove => _playlist.IsChangeNameAsyncAvailableChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<bool>? IsChangeDescriptionAsyncAvailableChanged
        {
            add => _playlist.IsChangeDescriptionAsyncAvailableChanged += value;
            remove => _playlist.IsChangeDescriptionAsyncAvailableChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<bool>? IsChangeDurationAsyncAvailableChanged
        {
            add => _playlist.IsChangeDurationAsyncAvailableChanged += value;
            remove => _playlist.IsChangeDurationAsyncAvailableChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<bool>? IsPlayTrackCollectionAsyncAvailableChanged
        {
            add => _playlist.IsPlayTrackCollectionAsyncAvailableChanged += value;
            remove => _playlist.IsPlayTrackCollectionAsyncAvailableChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<bool>? IsPauseTrackCollectionAsyncAvailableChanged
        {
            add => _playlist.IsPauseTrackCollectionAsyncAvailableChanged += value;
            remove => _playlist.IsPauseTrackCollectionAsyncAvailableChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<int>? TracksCountChanged
        {
            add => _playlist.TracksCountChanged += value;
            remove => _playlist.TracksCountChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<int>? ImagesCountChanged
        {
            add => _playlist.ImagesCountChanged += value;
            remove => _playlist.ImagesCountChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<int>? UrlsCountChanged
        {
            add => _playlist.UrlsCountChanged += value;
            remove => _playlist.UrlsCountChanged -= value;
        }

        /// <inheritdoc />
        public event CollectionChangedEventHandler<ITrack>? TracksChanged
        {
            add => _playlist.TracksChanged += value;
            remove => _playlist.TracksChanged -= value;
        }

        /// <inheritdoc />
        public event CollectionChangedEventHandler<IImage>? ImagesChanged
        {
            add => _playlist.ImagesChanged += value;
            remove => _playlist.ImagesChanged -= value;
        }

        /// <inheritdoc />
        public event CollectionChangedEventHandler<IUrl>? UrlsChanged
        {
            add => _playlist.UrlsChanged += value;
            remove => _playlist.UrlsChanged -= value;
        }

        private void CorePlaylistPlaybackStateChanged(object sender, PlaybackState e) => _syncContext.Post(_ => OnPropertyChanged(nameof(PlaybackState)), null);

        private void OnDownloadInfoChanged(object sender, DownloadInfo e) => _syncContext.Post(_ => OnPropertyChanged(nameof(DownloadInfo)), null);

        private void CorePlaylistNameChanged(object sender, string e) => _syncContext.Post(_ => OnPropertyChanged(nameof(Name)), null);

        private void CorePlaylistDescriptionChanged(object sender, string? e) => _syncContext.Post(_ => OnPropertyChanged(nameof(Description)), null);

        private void PlaylistOnTrackItemsCountChanged(object sender, int e) => _syncContext.Post(_ => OnPropertyChanged(nameof(TotalTrackCount)), null);

        private void PlaylistViewModel_ImagesCountChanged(object sender, int e) => _syncContext.Post(_ => OnPropertyChanged(nameof(TotalImageCount)), null);

        private void PlaylistViewModel_UrlsCountChanged(object sender, int e) => _syncContext.Post(_ => OnPropertyChanged(nameof(TotalUrlCount)), null);

        private void CorePlaylistLastPlayedChanged(object sender, DateTime? e) => _syncContext.Post(_ => OnPropertyChanged(nameof(LastPlayed)), null);

        private void OnIsChangeDescriptionAsyncAvailableChanged(object sender, bool e) => _syncContext.Post(_ => OnPropertyChanged(nameof(IsChangeDescriptionAsyncAvailable)), null);

        private void OnIsChangeDurationAsyncAvailableChanged(object sender, bool e) => _syncContext.Post(_ => OnPropertyChanged(nameof(IsChangeDurationAsyncAvailable)), null);

        private void OnIsChangeNameAsyncAvailableChanged(object sender, bool e) => _syncContext.Post(_ => OnPropertyChanged(nameof(IsChangeNameAsyncAvailable)), null);

        private void OnIsPauseTrackCollectionAsyncAvailableChanged(object sender, bool e) => _syncContext.Post(_ => OnPropertyChanged(nameof(IsPauseTrackCollectionAsyncAvailable)), null);

        private void OnIsPlayTrackCollectionAsyncAvailableChanged(object sender, bool e) => _syncContext.Post(_ => OnPropertyChanged(nameof(IsPlayTrackCollectionAsyncAvailable)), null);

        private void PlaylistViewModel_TrackItemsChanged(object sender, IReadOnlyList<CollectionChangedItem<ITrack>> addedItems, IReadOnlyList<CollectionChangedItem<ITrack>> removedItems) => _syncContext.Post(_ =>
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
        }, null);

        private void PlaylistViewModel_ImagesChanged(object sender, IReadOnlyList<CollectionChangedItem<IImage>> addedItems, IReadOnlyList<CollectionChangedItem<IImage>> removedItems) => _syncContext.Post(_ =>
        {
            Images.ChangeCollection(addedItems, removedItems);
        }, null);

        private void PlaylistViewModel_UrlsChanged(object sender, IReadOnlyList<CollectionChangedItem<IUrl>> addedItems, IReadOnlyList<CollectionChangedItem<IUrl>> removedItems) => _syncContext.Post(_ =>
        {
            Urls.ChangeCollection(addedItems, removedItems);
        }, null);

        /// <inheritdoc />
        public bool IsInitialized { get; private set; }

        /// <inheritdoc/>
        public MainViewModel Root { get; }

        /// <inheritdoc cref="IMerged{T}.SourceCores" />
        public IReadOnlyList<ICore> SourceCores { get; }

        /// <inheritdoc />
        public TrackSortingType CurrentTracksSortingType { get; private set; }

        /// <inheritdoc />
        public SortDirection CurrentTracksSortingDirection { get; private set; }

        /// <summary>
        /// The merged sources that form this member.
        /// </summary>
        public IReadOnlyList<ICorePlaylist> Sources => _playlist.GetSources<ICorePlaylist>();

        /// <inheritdoc />
        IReadOnlyList<ICorePlaylistCollectionItem> IMerged<ICorePlaylistCollectionItem>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreTrackCollection> IMerged<ICoreTrackCollection>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICorePlaylist> IMerged<ICorePlaylist>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreImageCollection> IMerged<ICoreImageCollection>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreUrlCollection> IMerged<ICoreUrlCollection>.Sources => Sources;

        /// <inheritdoc />
        public string Id => _playlist.Id;

        /// <inheritdoc />
        public TimeSpan Duration => _playlist.Duration;

        /// <inheritdoc />
        public DateTime? LastPlayed { get; }

        /// <inheritdoc />
        public DateTime? AddedAt { get; }

        /// <inheritdoc />
        public ObservableCollection<TrackViewModel> UnsortedTracks { get; }

        /// <inheritdoc />
        public ObservableCollection<TrackViewModel> Tracks { get; set; }

        /// <inheritdoc />
        public ObservableCollection<IImage> Images { get; }

        /// <inheritdoc />
        public ObservableCollection<IUrl> Urls { get; }

        /// <inheritdoc />
        public IPlayableCollectionGroup? RelatedItems { get; }

        /// <inheritdoc />
        public IUserProfile? Owner => _owner;

        /// <inheritdoc />
        public string Name => _playlist.Name;

        /// <inheritdoc />
        public string? Description => _playlist.Description;

        /// <inheritdoc />
        public PlaybackState PlaybackState => _playlist.PlaybackState;

        /// <inheritdoc />
        public DownloadInfo DownloadInfo => _playlist.DownloadInfo;

        /// <inheritdoc />
        public int TotalTrackCount => _playlist.TotalTrackCount;

        /// <inheritdoc />
        public int TotalImageCount => _playlist.TotalImageCount;

        /// <inheritdoc />
        public int TotalUrlCount => _playlist.TotalUrlCount;

        /// <inheritdoc />
        public bool IsPlayTrackCollectionAsyncAvailable => _playlist.IsPlayTrackCollectionAsyncAvailable;

        /// <inheritdoc />
        public bool IsPauseTrackCollectionAsyncAvailable => _playlist.IsPauseTrackCollectionAsyncAvailable;

        /// <inheritdoc />
        public bool IsChangeNameAsyncAvailable => _playlist.IsChangeNameAsyncAvailable;

        /// <inheritdoc />
        public bool IsChangeDescriptionAsyncAvailable => _playlist.IsChangeDescriptionAsyncAvailable;

        /// <inheritdoc />
        public bool IsChangeDurationAsyncAvailable => _playlist.IsChangeDurationAsyncAvailable;

        /// <inheritdoc />
        public Task<bool> IsAddTrackAvailableAsync(int index) => _playlist.IsAddTrackAvailableAsync(index);

        /// <inheritdoc />
        public Task<bool> IsAddImageAvailableAsync(int index) => _playlist.IsAddImageAvailableAsync(index);

        /// <inheritdoc />
        public Task<bool> IsAddUrlAvailableAsync(int index) => _playlist.IsAddUrlAvailableAsync(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveTrackAvailableAsync(int index) => _playlist.IsRemoveTrackAvailableAsync(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveImageAvailableAsync(int index) => _playlist.IsRemoveImageAvailableAsync(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveUrlAvailableAsync(int index) => _playlist.IsRemoveUrlAvailableAsync(index);

        ///<inheritdoc />
        public void SortTrackCollection(TrackSortingType trackSorting, SortDirection sortDirection)
        {
            CurrentTracksSortingType = trackSorting;
            CurrentTracksSortingDirection = sortDirection;

            CollectionSorting.SortTracks(Tracks, trackSorting, sortDirection, UnsortedTracks);
        }

        /// <inheritdoc />
        public Task PlayTrackCollectionAsync(ITrack track) => _playlist.PlayTrackCollectionAsync(track);

        /// <inheritdoc />
        public Task PlayTrackCollectionAsync() => _playlist.PlayTrackCollectionAsync();

        /// <inheritdoc />
        public Task PauseTrackCollectionAsync() => _playlist.PauseTrackCollectionAsync();

        /// <inheritdoc />
        public Task StartDownloadOperationAsync(DownloadOperation operation) => _playlist.StartDownloadOperationAsync(operation);

        /// <inheritdoc />
        public Task ChangeNameAsync(string name) => ChangeNameInternalAsync(name);

        /// <inheritdoc />
        public Task ChangeDescriptionAsync(string? description) => _playlist.ChangeDescriptionAsync(description);

        /// <inheritdoc />
        public Task ChangeDurationAsync(TimeSpan duration) => _playlist.ChangeDurationAsync(duration);

        /// <inheritdoc />
        public Task AddTrackAsync(ITrack track, int index) => _playlist.AddTrackAsync(track, index);

        /// <inheritdoc />
        public Task AddImageAsync(IImage image, int index) => _playlist.AddImageAsync(image, index);

        /// <inheritdoc />
        public Task AddUrlAsync(IUrl image, int index) => _playlist.AddUrlAsync(image, index);

        /// <inheritdoc />
        public Task RemoveTrackAsync(int index) => _playlist.RemoveTrackAsync(index);

        /// <inheritdoc />
        public Task RemoveImageAsync(int index) => _playlist.RemoveImageAsync(index);

        /// <inheritdoc />
        public Task RemoveUrlAsync(int index) => _playlist.RemoveUrlAsync(index);

        /// <inheritdoc />
        public Task<IReadOnlyList<ITrack>> GetTracksAsync(int limit, int offset = 0) => _playlist.GetTracksAsync(limit, offset);

        /// <inheritdoc />
        public Task<IReadOnlyList<IImage>> GetImagesAsync(int limit, int offset) => _playlist.GetImagesAsync(limit, offset);

        /// <inheritdoc />
        public Task<IReadOnlyList<IUrl>> GetUrlsAsync(int limit, int offset) => _playlist.GetUrlsAsync(limit, offset);

        /// <inheritdoc />
        public async Task PopulateMoreTracksAsync(int limit, CancellationToken cancellationToken = default)
        {
            using (await Flow.EasySemaphore(_populateTracksMutex))
            {
                var items = await _playlist.GetTracksAsync(limit, Tracks.Count);

                _syncContext.Post(_ =>
                {
                    foreach (var item in items)
                        Tracks.Add(new TrackViewModel(Root, item));

                    _syncContext.Post(_ => OnPropertyChanged(nameof(TotalTrackCount)), null);
                }, null);
            }
        }

        /// <inheritdoc />
        public async Task PopulateMoreImagesAsync(int limit, CancellationToken cancellationToken = default)
        {
            using (await Flow.EasySemaphore(_populateImagesMutex))
            {
                var items = await _playlist.GetImagesAsync(limit, Images.Count);

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
                var items = await _playlist.GetUrlsAsync(limit, Urls.Count);

                _syncContext.Post(_ =>
                {
                    foreach (var item in items)
                        Urls.Add(item);
                }, null);
            }
        }

        /// <inheritdoc />
        public Task InitTrackCollectionAsync(CancellationToken cancellationToken = default) => CollectionInit.TrackCollection(this, cancellationToken);

        /// <inheritdoc />
        public Task InitImageCollectionAsync(CancellationToken cancellationToken = default) => CollectionInit.ImageCollection(this, cancellationToken);

        /// <inheritdoc />
        public IAsyncRelayCommand<int> PopulateMoreTracksCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand<int> PopulateMoreImagesCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand<int> PopulateMoreUrlsCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand<ITrack> PlayTrackAsyncCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand PlayTrackCollectionAsyncCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand PauseTrackCollectionAsyncCommand { get; }

        /// <summary>
        /// Command to change the name. It triggers <see cref="ChangeNameAsync"/>.
        /// </summary>
        public IAsyncRelayCommand ChangeNameAsyncCommand { get; }

        /// <summary>
        /// Command to change the description. It triggers <see cref="ChangeDescriptionAsync"/>.
        /// </summary>
        public IAsyncRelayCommand<string?> ChangeDescriptionAsyncCommand { get; }

        /// <summary>
        /// Command to change the duration. It triggers <see cref="ChangeDurationAsync"/>.
        /// </summary>
        public IAsyncRelayCommand<TimeSpan> ChangeDurationAsyncCommand { get; }

        /// <inheritdoc />
        public IRelayCommand<TrackSortingType> ChangeTrackCollectionSortingTypeCommand { get; }

        /// <inheritdoc />
        public IRelayCommand<SortDirection> ChangeTrackCollectionSortingDirectionCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand InitTrackCollectionAsyncCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand InitImageCollectionAsyncCommand { get; }

        /// <inheritdoc />
        public bool Equals(ICorePlaylistCollectionItem other) => _playlist.Equals(other);

        /// <inheritdoc />
        public bool Equals(ICorePlaylist other) => _playlist.Equals(other);

        /// <inheritdoc />
        public bool Equals(ICoreTrackCollection other) => _playlist.Equals(other);

        /// <inheritdoc />
        public bool Equals(ICoreImageCollection other) => _playlist.Equals(other);

        /// <inheritdoc />
        public bool Equals(ICoreUrlCollection other) => _playlist.Equals(other);

        private Task ChangeNameInternalAsync(string? name)
        {
            Guard.IsNotNull(name, nameof(name));
            return _playlist.ChangeNameAsync(name);
        }

        /// <inheritdoc />
        public ValueTask DisposeAsync()
        {
            DetachEvents();
            return _playlist.DisposeAsync();
        }
    }
}
