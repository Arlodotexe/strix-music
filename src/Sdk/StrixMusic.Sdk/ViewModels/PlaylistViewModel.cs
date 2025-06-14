﻿// Copyright (c) Arlo Godfrey. All Rights Reserved.
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
using OwlCore.ComponentModel;
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
    /// A ViewModel for <see cref="IPlaylist"/>.
    /// </summary>
    public sealed class PlaylistViewModel : ObservableObject, ISdkViewModel, IPlaylist, ITrackCollectionViewModel, IImageCollectionViewModel, IDelegable<IPlaylist>
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
        /// <param name="playlist">The <see cref="IPlaylist"/> to wrap.</param>
        public PlaylistViewModel(IPlaylist playlist)
        {
            _syncContext = SynchronizationContext.Current ?? new SynchronizationContext();

            _playlist = playlist;
            
            PauseTrackCollectionAsyncCommand = new AsyncRelayCommand(PauseTrackCollectionAsync);
            PlayTrackCollectionAsyncCommand = new AsyncRelayCommand(PlayTrackCollectionAsync);

            PlayTrackAsyncCommand = new AsyncRelayCommand<ITrack>((x, y) => _playlist.PlayTrackCollectionAsync(x ?? ThrowHelper.ThrowArgumentNullException<ITrack>(), y));

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
                _owner = new UserProfileViewModel(_playlist.Owner);

            if (_playlist.RelatedItems != null)
                RelatedItems = new PlayableCollectionGroupViewModel(_playlist.RelatedItems);

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
            DownloadInfoChanged += OnDownloadInfoChanged;

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
            DownloadInfoChanged -= OnDownloadInfoChanged;

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

        /// <inheritdoc/>
        IPlaylist IDelegable<IPlaylist>.Inner => _playlist;

        /// <inheritdoc/>
        public event EventHandler? SourcesChanged
        {
            add => _playlist.SourcesChanged += value;
            remove => _playlist.SourcesChanged -= value;
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

        private void CorePlaylistPlaybackStateChanged(object? sender, PlaybackState e) => _syncContext.Post(_ => OnPropertyChanged(nameof(PlaybackState)), null);

        private void OnDownloadInfoChanged(object? sender, DownloadInfo e) => _syncContext.Post(_ => OnPropertyChanged(nameof(DownloadInfo)), null);

        private void CorePlaylistNameChanged(object? sender, string e) => _syncContext.Post(_ => OnPropertyChanged(nameof(Name)), null);

        private void CorePlaylistDescriptionChanged(object? sender, string? e) => _syncContext.Post(_ => OnPropertyChanged(nameof(Description)), null);

        private void PlaylistOnTrackItemsCountChanged(object? sender, int e) => _syncContext.Post(_ => OnPropertyChanged(nameof(TotalTrackCount)), null);

        private void PlaylistViewModel_ImagesCountChanged(object? sender, int e) => _syncContext.Post(_ => OnPropertyChanged(nameof(TotalImageCount)), null);

        private void PlaylistViewModel_UrlsCountChanged(object? sender, int e) => _syncContext.Post(_ => OnPropertyChanged(nameof(TotalUrlCount)), null);

        private void CorePlaylistLastPlayedChanged(object? sender, DateTime? e) => _syncContext.Post(_ => OnPropertyChanged(nameof(LastPlayed)), null);

        private void OnIsChangeDescriptionAsyncAvailableChanged(object? sender, bool e) => _syncContext.Post(_ => OnPropertyChanged(nameof(IsChangeDescriptionAsyncAvailable)), null);

        private void OnIsChangeDurationAsyncAvailableChanged(object? sender, bool e) => _syncContext.Post(_ => OnPropertyChanged(nameof(IsChangeDurationAsyncAvailable)), null);

        private void OnIsChangeNameAsyncAvailableChanged(object? sender, bool e) => _syncContext.Post(_ => OnPropertyChanged(nameof(IsChangeNameAsyncAvailable)), null);

        private void OnIsPauseTrackCollectionAsyncAvailableChanged(object? sender, bool e) => _syncContext.Post(_ => OnPropertyChanged(nameof(IsPauseTrackCollectionAsyncAvailable)), null);

        private void OnIsPlayTrackCollectionAsyncAvailableChanged(object? sender, bool e) => _syncContext.Post(_ => OnPropertyChanged(nameof(IsPlayTrackCollectionAsyncAvailable)), null);

        private void PlaylistViewModel_TrackItemsChanged(object? sender, IReadOnlyList<CollectionChangedItem<ITrack>> addedItems, IReadOnlyList<CollectionChangedItem<ITrack>> removedItems) => _syncContext.Post(_ =>
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

                foreach (var item in Tracks.ToArray())
                {
                    if (!UnsortedTracks.Contains(item))
                        Tracks.Remove(item);
                }

                SortTrackCollection(CurrentTracksSortingType, CurrentTracksSortingDirection);
            }
        }, null);

        private void PlaylistViewModel_ImagesChanged(object? sender, IReadOnlyList<CollectionChangedItem<IImage>> addedItems, IReadOnlyList<CollectionChangedItem<IImage>> removedItems) => _syncContext.Post(_ =>
        {
            Images.ChangeCollection(addedItems, removedItems);
        }, null);

        private void PlaylistViewModel_UrlsChanged(object? sender, IReadOnlyList<CollectionChangedItem<IUrl>> addedItems, IReadOnlyList<CollectionChangedItem<IUrl>> removedItems) => _syncContext.Post(_ =>
        {
            Urls.ChangeCollection(addedItems, removedItems);
        }, null);

        /// <inheritdoc />
        public bool IsInitialized { get; private set; }

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
        public Task<bool> IsAddTrackAvailableAsync(int index, CancellationToken cancellationToken = default) => _playlist.IsAddTrackAvailableAsync(index, cancellationToken);

        /// <inheritdoc />
        public Task<bool> IsAddImageAvailableAsync(int index, CancellationToken cancellationToken = default) => _playlist.IsAddImageAvailableAsync(index, cancellationToken);

        /// <inheritdoc />
        public Task<bool> IsAddUrlAvailableAsync(int index, CancellationToken cancellationToken = default) => _playlist.IsAddUrlAvailableAsync(index, cancellationToken);

        /// <inheritdoc />
        public Task<bool> IsRemoveTrackAvailableAsync(int index, CancellationToken cancellationToken = default) => _playlist.IsRemoveTrackAvailableAsync(index, cancellationToken);

        /// <inheritdoc />
        public Task<bool> IsRemoveImageAvailableAsync(int index, CancellationToken cancellationToken = default) => _playlist.IsRemoveImageAvailableAsync(index, cancellationToken);

        /// <inheritdoc />
        public Task<bool> IsRemoveUrlAvailableAsync(int index, CancellationToken cancellationToken = default) => _playlist.IsRemoveUrlAvailableAsync(index, cancellationToken);

        ///<inheritdoc />
        public void SortTrackCollection(TrackSortingType trackSorting, SortDirection sortDirection)
        {
            CurrentTracksSortingType = trackSorting;
            CurrentTracksSortingDirection = sortDirection;

            CollectionSorting.SortTracks(Tracks, trackSorting, sortDirection, UnsortedTracks);
        }

        /// <inheritdoc />
        public Task PlayTrackCollectionAsync(ITrack track, CancellationToken cancellationToken = default) => _playlist.PlayTrackCollectionAsync(track, cancellationToken);

        /// <inheritdoc />
        public Task PlayTrackCollectionAsync(CancellationToken cancellationToken = default) => _playlist.PlayTrackCollectionAsync(cancellationToken);

        /// <inheritdoc />
        public Task PauseTrackCollectionAsync(CancellationToken cancellationToken = default) => _playlist.PauseTrackCollectionAsync(cancellationToken);

        /// <inheritdoc />
        public Task StartDownloadOperationAsync(DownloadOperation operation, CancellationToken cancellationToken = default) => _playlist.StartDownloadOperationAsync(operation, cancellationToken);

        /// <inheritdoc />
        public Task ChangeNameAsync(string name, CancellationToken cancellationToken = default) => ChangeNameInternalAsync(name, cancellationToken);

        /// <inheritdoc />
        public Task ChangeDescriptionAsync(string? description, CancellationToken cancellationToken = default) => _playlist.ChangeDescriptionAsync(description, cancellationToken);

        /// <inheritdoc />
        public Task ChangeDurationAsync(TimeSpan duration, CancellationToken cancellationToken = default) => _playlist.ChangeDurationAsync(duration, cancellationToken);

        /// <inheritdoc />
        public Task AddTrackAsync(ITrack track, int index, CancellationToken cancellationToken = default) => _playlist.AddTrackAsync(track, index, cancellationToken);

        /// <inheritdoc />
        public Task AddImageAsync(IImage image, int index, CancellationToken cancellationToken = default) => _playlist.AddImageAsync(image, index, cancellationToken);

        /// <inheritdoc />
        public Task AddUrlAsync(IUrl image, int index, CancellationToken cancellationToken = default) => _playlist.AddUrlAsync(image, index, cancellationToken);

        /// <inheritdoc />
        public Task RemoveTrackAsync(int index, CancellationToken cancellationToken = default) => _playlist.RemoveTrackAsync(index, cancellationToken);

        /// <inheritdoc />
        public Task RemoveImageAsync(int index, CancellationToken cancellationToken = default) => _playlist.RemoveImageAsync(index, cancellationToken);

        /// <inheritdoc />
        public Task RemoveUrlAsync(int index, CancellationToken cancellationToken = default) => _playlist.RemoveUrlAsync(index, cancellationToken);

        /// <inheritdoc />
        public IAsyncEnumerable<ITrack> GetTracksAsync(int limit, int offset, CancellationToken cancellationToken = default) => _playlist.GetTracksAsync(limit, offset, cancellationToken);

        /// <inheritdoc />
        public IAsyncEnumerable<IImage> GetImagesAsync(int limit, int offset, CancellationToken cancellationToken = default) => _playlist.GetImagesAsync(limit, offset, cancellationToken);

        /// <inheritdoc />
        public IAsyncEnumerable<IUrl> GetUrlsAsync(int limit, int offset, CancellationToken cancellationToken = default) => _playlist.GetUrlsAsync(limit, offset, cancellationToken);

        /// <inheritdoc />
        public async Task PopulateMoreTracksAsync(int limit, CancellationToken cancellationToken = default)
        {
            using (await _populateTracksMutex.DisposableWaitAsync(cancellationToken))
            {
                using var releaseReg = cancellationToken.Register(() => _populateTracksMutex.Release());

                await _syncContext.PostAsync(async () =>
                {
                    await foreach (var item in _playlist.GetTracksAsync(limit, Tracks.Count, cancellationToken))
                    {
                        var tvm = new TrackViewModel(item);
                        Tracks.Add(tvm);
                        UnsortedTracks.Add(tvm);
                    }
                });
            }
        }

        /// <inheritdoc />
        public async Task PopulateMoreImagesAsync(int limit, CancellationToken cancellationToken = default)
        {
            using (await _populateImagesMutex.DisposableWaitAsync(cancellationToken))
            {
                using var releaseReg = cancellationToken.Register(() => _populateImagesMutex.Release());

                await _syncContext.PostAsync(async () =>
                {
                    await foreach (var item in _playlist.GetImagesAsync(limit, Images.Count, cancellationToken))
                        Images.Add(item);
                });
            }
        }

        /// <inheritdoc />
        public async Task PopulateMoreUrlsAsync(int limit, CancellationToken cancellationToken = default)
        {
            using (await _populateUrlsMutex.DisposableWaitAsync(cancellationToken))
            {
                using var releaseReg = cancellationToken.Register(() => _populateUrlsMutex.Release());

                await _syncContext.PostAsync(async () =>
                {
                    await foreach (var item in _playlist.GetUrlsAsync(limit, Urls.Count, cancellationToken))
                        Urls.Add(item);
                });
            }
        }

        /// <inheritdoc />
        public Task InitTrackCollectionAsync(CancellationToken cancellationToken = default) => CollectionInit.TrackCollectionAsync(this, cancellationToken);

        /// <inheritdoc />
        public Task InitImageCollectionAsync(CancellationToken cancellationToken = default) => CollectionInit.ImageCollectionAsync(this, cancellationToken);

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
        public bool Equals(ICorePlaylistCollectionItem? other) => _playlist.Equals(other!);

        /// <inheritdoc />
        public bool Equals(ICorePlaylist? other) => _playlist.Equals(other!);

        /// <inheritdoc />
        public bool Equals(ICoreTrackCollection? other) => _playlist.Equals(other!);

        /// <inheritdoc />
        public bool Equals(ICoreImageCollection? other) => _playlist.Equals(other!);

        /// <inheritdoc />
        public bool Equals(ICoreUrlCollection? other) => _playlist.Equals(other!);

        private Task ChangeNameInternalAsync(string? name, CancellationToken cancellationToken = default)
        {
            Guard.IsNotNull(name, nameof(name));
            return _playlist.ChangeNameAsync(name, cancellationToken);
        }
    }
}
