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
using StrixMusic.Sdk.Models.Base;
using StrixMusic.Sdk.Models.Core;
using StrixMusic.Sdk.Models.Merged;
using StrixMusic.Sdk.ViewModels.Helpers;

namespace StrixMusic.Sdk.ViewModels
{
    /// <summary>
    /// An observable wrapper for a <see cref="IPlayableCollectionGroupBase"/>.
    /// </summary>
    public class PlayableCollectionGroupViewModel : ObservableObject, ISdkViewModel, IPlayableCollectionGroup, IPlayableCollectionGroupChildrenViewModel, IAlbumCollectionViewModel, IArtistCollectionViewModel, ITrackCollectionViewModel, IPlaylistCollectionViewModel, IImageCollectionViewModel, IUrlCollectionViewModel
    {
        private readonly IPlayableCollectionGroup _collectionGroup;

        private readonly IPlaybackHandlerService _playbackHandler;

        private readonly SemaphoreSlim _populateTracksMutex = new SemaphoreSlim(1,1);
        private readonly SemaphoreSlim _populateAlbumsMutex = new SemaphoreSlim(1, 1);
        private readonly SemaphoreSlim _populateArtistsMutex = new SemaphoreSlim(1, 1);
        private readonly SemaphoreSlim _populatePlaylistsMutex = new SemaphoreSlim(1, 1);
        private readonly SemaphoreSlim _populateChildrenMutex = new SemaphoreSlim(1, 1);
        private readonly SemaphoreSlim _populateImagesMutex = new SemaphoreSlim(1, 1);
        private readonly SemaphoreSlim _populateUrlsMutex = new SemaphoreSlim(1, 1);

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayableCollectionGroupViewModel"/> class.
        /// </summary>
        /// <param name="root">The <see cref="MainViewModel"/> that this or the object that created this originated from.</param>
        /// <param name="collectionGroup">The base <see cref="IPlayableCollectionGroup"/> containing properties about this class.</param>
        internal PlayableCollectionGroupViewModel(MainViewModel root, IPlayableCollectionGroup collectionGroup)
        {
            _collectionGroup = root.Plugins.ModelPlugins.PlayableCollectionGroup.Execute(collectionGroup);
            Root = root;

            _playbackHandler = Ioc.Default.GetRequiredService<IPlaybackHandlerService>();

            SourceCores = _collectionGroup.GetSourceCores<ICorePlayableCollectionGroup>().Select(root.GetLoadedCore).ToList();

            PauseAlbumCollectionAsyncCommand = new AsyncRelayCommand(PauseAlbumCollectionAsync);
            PlayAlbumCollectionAsyncCommand = new AsyncRelayCommand(PlayAlbumCollectionAsync);
            PauseArtistCollectionAsyncCommand = new AsyncRelayCommand(PauseArtistCollectionAsync);
            PlayArtistCollectionAsyncCommand = new AsyncRelayCommand(PlayArtistCollectionAsync);
            PausePlaylistCollectionAsyncCommand = new AsyncRelayCommand(PausePlaylistCollectionAsync);
            PlayPlaylistCollectionAsyncCommand = new AsyncRelayCommand(PlayPlaylistCollectionAsync);
            PauseTrackCollectionAsyncCommand = new AsyncRelayCommand(PauseTrackCollectionAsync);
            PlayTrackCollectionAsyncCommand = new AsyncRelayCommand(PlayTrackCollectionAsync);

            PlayTrackAsyncCommand = new AsyncRelayCommand<ITrack>(PlayTrackCollectionInternalAsync);
            PlayAlbumAsyncCommand = new AsyncRelayCommand<IAlbumCollectionItem>(PlayAlbumCollectionInternalAsync);
            PlayPlaylistAsyncCommand = new AsyncRelayCommand<IPlaylistCollectionItem>(PlayPlaylistCollectionInternalAsync);
            PlayArtistAsyncCommand = new AsyncRelayCommand<IArtistCollectionItem>(PlayArtistCollectionInternalAsync);

            ChangeNameAsyncCommand = new AsyncRelayCommand<string>(ChangeNameInternalAsync);
            ChangeDescriptionAsyncCommand = new AsyncRelayCommand<string?>(ChangeDescriptionAsync);
            ChangeDurationAsyncCommand = new AsyncRelayCommand<TimeSpan>(ChangeDurationAsync);

            PopulateMoreTracksCommand = new AsyncRelayCommand<int>(PopulateMoreTracksAsync);
            PopulateMorePlaylistsCommand = new AsyncRelayCommand<int>(PopulateMorePlaylistsAsync);
            PopulateMoreAlbumsCommand = new AsyncRelayCommand<int>(PopulateMoreAlbumsAsync);
            PopulateMoreArtistsCommand = new AsyncRelayCommand<int>(PopulateMoreArtistsAsync);
            PopulateMoreChildrenCommand = new AsyncRelayCommand<int>(PopulateMoreChildrenAsync);
            PopulateMoreImagesCommand = new AsyncRelayCommand<int>(PopulateMoreImagesAsync);
            PopulateMoreUrlsCommand = new AsyncRelayCommand<int>(PopulateMoreUrlsAsync);

            InitImageCollectionAsyncCommand = new AsyncRelayCommand(InitImageCollectionAsync);
            InitTrackCollectionAsyncCommand = new AsyncRelayCommand(InitTrackCollectionAsync);
            InitArtistCollectionAsyncCommand = new AsyncRelayCommand(InitArtistCollectionAsync);
            InitAlbumCollectionAsyncCommand = new AsyncRelayCommand(InitAlbumCollectionAsync);
            InitPlaylistCollectionAsyncCommand = new AsyncRelayCommand(InitPlaylistCollectionAsync);

            ChangeTrackCollectionSortingTypeCommand = new RelayCommand<TrackSortingType>(x => SortTrackCollection(x, CurrentTracksSortingDirection));
            ChangeTrackCollectionSortingDirectionCommand = new RelayCommand<SortDirection>(x => SortTrackCollection(CurrentTracksSortingType, x));
            ChangeArtistCollectionSortingTypeCommand = new RelayCommand<ArtistSortingType>(x => SortArtistCollection(x, CurrentArtistSortingDirection));
            ChangeArtistCollectionSortingDirectionCommand = new RelayCommand<SortDirection>(x => SortArtistCollection(CurrentArtistSortingType, x));
            ChangeAlbumCollectionSortingTypeCommand = new RelayCommand<AlbumSortingType>(x => SortAlbumCollection(x, CurrentAlbumSortingDirection));
            ChangeAlbumCollectionSortingDirectionCommand = new RelayCommand<SortDirection>(x => SortAlbumCollection(CurrentAlbumSortingType, x));
            ChangePlaylistCollectionSortingTypeCommand = new RelayCommand<PlaylistSortingType>(x => SortPlaylistCollection(x, CurrentPlaylistSortingDirection));
            ChangePlaylistCollectionSortingDirectionCommand = new RelayCommand<SortDirection>(x => SortPlaylistCollection(CurrentPlaylistSortingType, x));

            using (Threading.PrimaryContext)
            {
                Albums = new ObservableCollection<IAlbumCollectionItem>();
                Artists = new ObservableCollection<IArtistCollectionItem>();
                Children = new ObservableCollection<PlayableCollectionGroupViewModel>();
                Playlists = new ObservableCollection<IPlaylistCollectionItem>();
                Tracks = new ObservableCollection<TrackViewModel>();
                Images = new ObservableCollection<IImage>();
                Urls = new ObservableCollection<IUrl>();

                UnsortedAlbums = new ObservableCollection<IAlbumCollectionItem>();
                UnsortedArtists = new ObservableCollection<IArtistCollectionItem>();
                UnsortedPlaylists = new ObservableCollection<IPlaylistCollectionItem>();
                UnsortedTracks = new ObservableCollection<TrackViewModel>();
            }

            AttachPropertyEvents();
        }

        private void AttachPropertyEvents()
        {
            PlaybackStateChanged += CollectionGroupPlaybackStateChanged;
            DescriptionChanged += CollectionGroupDescriptionChanged;
            NameChanged += CollectionGroupNameChanged;
            LastPlayedChanged += CollectionGroupLastPlayedChanged;
            DownloadInfoChanged += OnDownloadInfoChanged;

            AlbumItemsCountChanged += CollectionGroupOnAlbumItemsCountChanged;
            TracksCountChanged += CollectionGroupOnTrackItemsCountChanged;
            ArtistItemsCountChanged += CollectionGroupOnArtistItemsCountChanged;
            PlaylistItemsCountChanged += CollectionGroupOnPlaylistItemsCountChanged;
            ChildrenCountChanged += CollectionGroupOnTotalChildrenCountChanged;
            ImagesCountChanged += PlayableCollectionGroupViewModel_ImagesCountChanged;
            UrlsCountChanged += PlayableCollectionGroupViewModel_UrlsCountChanged;

            IsPlayAlbumCollectionAsyncAvailableChanged += OnIsPlayAlbumCollectionAsyncAvailableChanged;
            IsPauseAlbumCollectionAsyncAvailableChanged += OnIsPauseAlbumCollectionAsyncAvailableChanged;
            IsPlayArtistCollectionAsyncAvailableChanged += OnIsPlayArtistCollectionAsyncAvailableChanged;
            IsPauseArtistCollectionAsyncAvailableChanged += OnIsPauseArtistCollectionAsyncAvailableChanged;
            IsPlayPlaylistCollectionAsyncAvailableChanged += OnIsPlayPlaylistCollectionAsyncAvailableChanged;
            IsPausePlaylistCollectionAsyncAvailableChanged += OnIsPausePlaylistCollectionAsyncAvailableChanged;
            IsPlayTrackCollectionAsyncAvailableChanged += OnIsPlayTrackCollectionAsyncAvailableChanged;
            IsPauseTrackCollectionAsyncAvailableChanged += OnIsPauseTrackCollectionAsyncAvailableChanged;

            IsChangeNameAsyncAvailableChanged += OnIsChangeNameAsyncAvailableChanged;
            IsChangeDurationAsyncAvailableChanged += OnIsChangeDurationAsyncAvailableChanged;
            IsChangeDescriptionAsyncAvailableChanged += OnIsChangeDescriptionAsyncAvailableChanged;

            AlbumItemsChanged += PlayableCollectionGroupViewModel_AlbumItemsChanged;
            TracksChanged += PlayableCollectionGroupViewModel_TrackItemsChanged;
            ArtistItemsChanged += PlayableCollectionGroupViewModel_ArtistItemsChanged;
            PlaylistItemsChanged += PlayableCollectionGroupViewModel_PlaylistItemsChanged;
            ChildItemsChanged += PlayableCollectionGroupViewModel_ChildItemsChanged;
            ImagesChanged += PlayableCollectionGroupViewModel_ImagesChanged;
            UrlsChanged += PlayableCollectionGroupViewModel_UrlsChanged;
        }

        private void DetachPropertyEvents()
        {
            PlaybackStateChanged -= CollectionGroupPlaybackStateChanged;
            DescriptionChanged -= CollectionGroupDescriptionChanged;
            NameChanged -= CollectionGroupNameChanged;
            LastPlayedChanged -= CollectionGroupLastPlayedChanged;
            DownloadInfoChanged -= OnDownloadInfoChanged;

            AlbumItemsCountChanged -= CollectionGroupOnAlbumItemsCountChanged;
            TracksCountChanged -= CollectionGroupOnTrackItemsCountChanged;
            ArtistItemsCountChanged -= CollectionGroupOnArtistItemsCountChanged;
            PlaylistItemsCountChanged -= CollectionGroupOnPlaylistItemsCountChanged;
            ChildrenCountChanged -= CollectionGroupOnTotalChildrenCountChanged;
            ImagesCountChanged += PlayableCollectionGroupViewModel_ImagesCountChanged;

            IsPlayAlbumCollectionAsyncAvailableChanged -= OnIsPlayAlbumCollectionAsyncAvailableChanged;
            IsPauseAlbumCollectionAsyncAvailableChanged -= OnIsPauseAlbumCollectionAsyncAvailableChanged;
            IsPlayArtistCollectionAsyncAvailableChanged -= OnIsPlayArtistCollectionAsyncAvailableChanged;
            IsPauseArtistCollectionAsyncAvailableChanged -= OnIsPauseArtistCollectionAsyncAvailableChanged;
            IsPlayPlaylistCollectionAsyncAvailableChanged -= OnIsPlayPlaylistCollectionAsyncAvailableChanged;
            IsPausePlaylistCollectionAsyncAvailableChanged -= OnIsPausePlaylistCollectionAsyncAvailableChanged;
            IsPlayTrackCollectionAsyncAvailableChanged -= OnIsPlayTrackCollectionAsyncAvailableChanged;
            IsPauseTrackCollectionAsyncAvailableChanged -= OnIsPauseTrackCollectionAsyncAvailableChanged;

            IsChangeNameAsyncAvailableChanged -= OnIsChangeNameAsyncAvailableChanged;
            IsChangeDurationAsyncAvailableChanged -= OnIsChangeDurationAsyncAvailableChanged;
            IsChangeDescriptionAsyncAvailableChanged -= OnIsChangeDescriptionAsyncAvailableChanged;

            AlbumItemsChanged -= PlayableCollectionGroupViewModel_AlbumItemsChanged;
            TracksChanged -= PlayableCollectionGroupViewModel_TrackItemsChanged;
            ArtistItemsChanged -= PlayableCollectionGroupViewModel_ArtistItemsChanged;
            PlaylistItemsChanged -= PlayableCollectionGroupViewModel_PlaylistItemsChanged;
            ChildItemsChanged -= PlayableCollectionGroupViewModel_ChildItemsChanged;
            ImagesChanged -= PlayableCollectionGroupViewModel_ImagesChanged;
        }

        /// <inheritdoc />
        public event EventHandler<string>? NameChanged
        {
            add => _collectionGroup.NameChanged += value;
            remove => _collectionGroup.NameChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<string?>? DescriptionChanged
        {
            add => _collectionGroup.DescriptionChanged += value;
            remove => _collectionGroup.DescriptionChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<PlaybackState>? PlaybackStateChanged
        {
            add => _collectionGroup.PlaybackStateChanged += value;
            remove => _collectionGroup.PlaybackStateChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<DownloadInfo>? DownloadInfoChanged
        {
            add => _collectionGroup.DownloadInfoChanged += value;
            remove => _collectionGroup.DownloadInfoChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<TimeSpan>? DurationChanged
        {
            add => _collectionGroup.DurationChanged += value;
            remove => _collectionGroup.DurationChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<DateTime?>? LastPlayedChanged
        {
            add => _collectionGroup.LastPlayedChanged += value;
            remove => _collectionGroup.LastPlayedChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<bool>? IsChangeNameAsyncAvailableChanged
        {
            add => _collectionGroup.IsChangeNameAsyncAvailableChanged += value;
            remove => _collectionGroup.IsChangeNameAsyncAvailableChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<bool>? IsChangeDescriptionAsyncAvailableChanged
        {
            add => _collectionGroup.IsChangeDescriptionAsyncAvailableChanged += value;
            remove => _collectionGroup.IsChangeDescriptionAsyncAvailableChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<bool>? IsChangeDurationAsyncAvailableChanged
        {
            add => _collectionGroup.IsChangeDurationAsyncAvailableChanged += value;
            remove => _collectionGroup.IsChangeDurationAsyncAvailableChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<bool>? IsPlayAlbumCollectionAsyncAvailableChanged
        {
            add => _collectionGroup.IsPlayAlbumCollectionAsyncAvailableChanged += value;
            remove => _collectionGroup.IsPlayAlbumCollectionAsyncAvailableChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<bool>? IsPlayArtistCollectionAsyncAvailableChanged
        {
            add => _collectionGroup.IsPlayArtistCollectionAsyncAvailableChanged += value;
            remove => _collectionGroup.IsPlayArtistCollectionAsyncAvailableChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<bool>? IsPlayPlaylistCollectionAsyncAvailableChanged
        {
            add => _collectionGroup.IsPlayPlaylistCollectionAsyncAvailableChanged += value;
            remove => _collectionGroup.IsPlayPlaylistCollectionAsyncAvailableChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<bool>? IsPlayTrackCollectionAsyncAvailableChanged
        {
            add => _collectionGroup.IsPlayTrackCollectionAsyncAvailableChanged += value;
            remove => _collectionGroup.IsPlayTrackCollectionAsyncAvailableChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<bool>? IsPauseArtistCollectionAsyncAvailableChanged
        {
            add => _collectionGroup.IsPauseArtistCollectionAsyncAvailableChanged += value;
            remove => _collectionGroup.IsPauseArtistCollectionAsyncAvailableChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<bool>? IsPauseAlbumCollectionAsyncAvailableChanged
        {
            add => _collectionGroup.IsPauseAlbumCollectionAsyncAvailableChanged += value;
            remove => _collectionGroup.IsPauseAlbumCollectionAsyncAvailableChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<bool>? IsPausePlaylistCollectionAsyncAvailableChanged
        {
            add => _collectionGroup.IsPausePlaylistCollectionAsyncAvailableChanged += value;
            remove => _collectionGroup.IsPausePlaylistCollectionAsyncAvailableChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<bool>? IsPauseTrackCollectionAsyncAvailableChanged
        {
            add => _collectionGroup.IsPauseTrackCollectionAsyncAvailableChanged += value;
            remove => _collectionGroup.IsPauseTrackCollectionAsyncAvailableChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<int>? TracksCountChanged
        {
            add => _collectionGroup.TracksCountChanged += value;
            remove => _collectionGroup.TracksCountChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<int>? ArtistItemsCountChanged
        {
            add => _collectionGroup.ArtistItemsCountChanged += value;
            remove => _collectionGroup.ArtistItemsCountChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<int>? AlbumItemsCountChanged
        {
            add => _collectionGroup.AlbumItemsCountChanged += value;
            remove => _collectionGroup.AlbumItemsCountChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<int>? PlaylistItemsCountChanged
        {
            add => _collectionGroup.PlaylistItemsCountChanged += value;
            remove => _collectionGroup.PlaylistItemsCountChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<int>? ImagesCountChanged
        {
            add => _collectionGroup.ImagesCountChanged += value;
            remove => _collectionGroup.ImagesCountChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<int>? UrlsCountChanged
        {
            add => _collectionGroup.UrlsCountChanged += value;
            remove => _collectionGroup.UrlsCountChanged -= value;
        }

        /// <inheritdoc />
        public event CollectionChangedEventHandler<IImage>? ImagesChanged
        {
            add => _collectionGroup.ImagesChanged += value;
            remove => _collectionGroup.ImagesChanged -= value;
        }

        /// <inheritdoc />
        public event CollectionChangedEventHandler<IPlaylistCollectionItem>? PlaylistItemsChanged
        {
            add => _collectionGroup.PlaylistItemsChanged += value;
            remove => _collectionGroup.PlaylistItemsChanged -= value;
        }

        /// <inheritdoc />
        public event CollectionChangedEventHandler<ITrack>? TracksChanged
        {
            add => _collectionGroup.TracksChanged += value;
            remove => _collectionGroup.TracksChanged -= value;
        }

        /// <inheritdoc />
        public event CollectionChangedEventHandler<IAlbumCollectionItem>? AlbumItemsChanged
        {
            add => _collectionGroup.AlbumItemsChanged += value;
            remove => _collectionGroup.AlbumItemsChanged -= value;
        }

        /// <inheritdoc />
        public event CollectionChangedEventHandler<IArtistCollectionItem>? ArtistItemsChanged
        {
            add => _collectionGroup.ArtistItemsChanged += value;
            remove => _collectionGroup.ArtistItemsChanged -= value;
        }

        /// <inheritdoc />
        public event CollectionChangedEventHandler<IPlayableCollectionGroup>? ChildItemsChanged
        {
            add => _collectionGroup.ChildItemsChanged += value;
            remove => _collectionGroup.ChildItemsChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<int>? ChildrenCountChanged
        {
            add => _collectionGroup.ChildrenCountChanged += value;
            remove => _collectionGroup.ChildrenCountChanged -= value;
        }

        /// <inheritdoc />
        public event CollectionChangedEventHandler<IUrl>? UrlsChanged
        {
            add => _collectionGroup.UrlsChanged += value;
            remove => _collectionGroup.UrlsChanged -= value;
        }

        private void CollectionGroupNameChanged(object sender, string e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(Name)));

        private void CollectionGroupDescriptionChanged(object sender, string? e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(Description)));

        private void CollectionGroupPlaybackStateChanged(object sender, PlaybackState e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(PlaybackState)));

        private void OnDownloadInfoChanged(object sender, DownloadInfo e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(DownloadInfo)));

        private void CollectionGroupOnTotalChildrenCountChanged(object sender, int e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(TotalChildrenCount)));

        private void CollectionGroupOnPlaylistItemsCountChanged(object sender, int e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(TotalPlaylistItemsCount)));

        private void CollectionGroupOnArtistItemsCountChanged(object sender, int e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(TotalArtistItemsCount)));

        private void CollectionGroupOnTrackItemsCountChanged(object sender, int e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(TotalTrackCount)));

        private void CollectionGroupOnAlbumItemsCountChanged(object sender, int e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(TotalAlbumItemsCount)));

        private void PlayableCollectionGroupViewModel_ImagesCountChanged(object sender, int e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(TotalImageCount)));

        private void PlayableCollectionGroupViewModel_UrlsCountChanged(object sender, int e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(TotalUrlCount)));

        private void CollectionGroupLastPlayedChanged(object sender, DateTime? e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(LastPlayed)));

        private void OnIsChangeDescriptionAsyncAvailableChanged(object sender, bool e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(IsChangeDescriptionAsyncAvailable)));

        private void OnIsChangeDurationAsyncAvailableChanged(object sender, bool e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(IsChangeDurationAsyncAvailable)));

        private void OnIsChangeNameAsyncAvailableChanged(object sender, bool e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(IsChangeNameAsyncAvailable)));

        private void OnIsPauseAlbumCollectionAsyncAvailableChanged(object sender, bool e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(IsPauseAlbumCollectionAsyncAvailable)));

        private void OnIsPlayAlbumCollectionAsyncAvailableChanged(object sender, bool e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(IsPlayAlbumCollectionAsyncAvailable)));

        private void OnIsPauseArtistCollectionAsyncAvailableChanged(object sender, bool e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(IsPauseArtistCollectionAsyncAvailable)));

        private void OnIsPlayArtistCollectionAsyncAvailableChanged(object sender, bool e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(IsPlayArtistCollectionAsyncAvailable)));

        private void OnIsPausePlaylistCollectionAsyncAvailableChanged(object sender, bool e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(IsPausePlaylistCollectionAsyncAvailable)));

        private void OnIsPlayPlaylistCollectionAsyncAvailableChanged(object sender, bool e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(IsPlayPlaylistCollectionAsyncAvailable)));

        private void OnIsPauseTrackCollectionAsyncAvailableChanged(object sender, bool e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(IsPauseTrackCollectionAsyncAvailable)));

        private void OnIsPlayTrackCollectionAsyncAvailableChanged(object sender, bool e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(IsPlayTrackCollectionAsyncAvailable)));

        private void PlayableCollectionGroupViewModel_AlbumItemsChanged(object sender, IReadOnlyList<CollectionChangedItem<IAlbumCollectionItem>> addedItems, IReadOnlyList<CollectionChangedItem<IAlbumCollectionItem>> removedItems)
        {
            _ = Threading.OnPrimaryThread(() =>
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

        private void PlayableCollectionGroupViewModel_ArtistItemsChanged(object sender, IReadOnlyList<CollectionChangedItem<IArtistCollectionItem>> addedItems, IReadOnlyList<CollectionChangedItem<IArtistCollectionItem>> removedItems)
        {
            _ = Threading.OnPrimaryThread(() =>
            {
                Artists.ChangeCollection(addedItems, removedItems, item => item.Data switch
                {
                    IArtist artist => new ArtistViewModel(Root, artist),
                    IArtistCollection collection => new ArtistCollectionViewModel(Root, collection),
                    _ => ThrowHelper.ThrowNotSupportedException<IArtistCollectionItem>($"{item.Data.GetType()} not supported for adding to {GetType()}")
                });
            });
        }

        private void PlayableCollectionGroupViewModel_ChildItemsChanged(object sender, IReadOnlyList<CollectionChangedItem<IPlayableCollectionGroup>> addedItems, IReadOnlyList<CollectionChangedItem<IPlayableCollectionGroup>> removedItems)
        {
            _ = Threading.OnPrimaryThread(() =>
            {
                Children.ChangeCollection(addedItems, removedItems, item => new PlayableCollectionGroupViewModel(Root, item.Data));
            });
        }

        private void PlayableCollectionGroupViewModel_PlaylistItemsChanged(object sender, IReadOnlyList<CollectionChangedItem<IPlaylistCollectionItem>> addedItems, IReadOnlyList<CollectionChangedItem<IPlaylistCollectionItem>> removedItems)
        {
            _ = Threading.OnPrimaryThread(() =>
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
                    // Preventing index issues during playlists emission from the core, also making sure that unordered artists updated. 
                    UnsortedPlaylists.ChangeCollection(addedItems, removedItems, item => item.Data switch
                    {
                        IPlaylist playlist => new PlaylistViewModel(Root, playlist),
                        IPlaylistCollection collection => new PlaylistCollectionViewModel(Root, collection),
                        _ => ThrowHelper.ThrowNotSupportedException<IPlaylistCollection>(
                            $"{item.Data.GetType()} not supported for adding to {GetType()}")
                    });

                    // Avoiding direct assignment to prevent effect on UI.
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
            });
        }

        private void PlayableCollectionGroupViewModel_TrackItemsChanged(object sender, IReadOnlyList<CollectionChangedItem<ITrack>> addedItems, IReadOnlyList<CollectionChangedItem<ITrack>> removedItems)
        {
            _ = Threading.OnPrimaryThread((Action)(() =>
            {
                if (this.CurrentTracksSortingType == TrackSortingType.Unsorted)
                {
                    Tracks.ChangeCollection<ITrack, TrackViewModel>(addedItems, removedItems, x => new TrackViewModel(Root, x.Data));
                }
                else
                {
                    // Preventing index issues during tracks emission from the core, also making sure that unordered tracks updated. 
                    UnsortedTracks.ChangeCollection<ITrack, TrackViewModel>(addedItems, removedItems, x => new TrackViewModel(Root, x.Data));

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
            }));
        }

        private void PlayableCollectionGroupViewModel_ImagesChanged(object sender, IReadOnlyList<CollectionChangedItem<IImage>> addedItems, IReadOnlyList<CollectionChangedItem<IImage>> removedItems)
        {
            _ = Threading.OnPrimaryThread(() =>
            {
                Images.ChangeCollection(addedItems, removedItems);
            });
        }

        private void PlayableCollectionGroupViewModel_UrlsChanged(object sender, IReadOnlyList<CollectionChangedItem<IUrl>> addedItems, IReadOnlyList<CollectionChangedItem<IUrl>> removedItems)
        {
            _ = Threading.OnPrimaryThread(() =>
            {
                Urls.ChangeCollection(addedItems, removedItems);
            });
        }

        /// <inheritdoc />
        public string Id => _collectionGroup.Id;

        /// <inheritdoc/>
        public MainViewModel Root { get; }

        /// <inheritdoc cref="IMerged{T}.SourceCores" />
        public IReadOnlyList<ICore> SourceCores { get; }

        /// <summary>
        /// The merged sources for this item.
        /// </summary>
        public IReadOnlyList<ICorePlayableCollectionGroup> Sources => _collectionGroup.GetSources<ICorePlayableCollectionGroup>();

        /// <inheritdoc />
        IReadOnlyList<ICorePlayableCollectionGroup> IMerged<ICorePlayableCollectionGroup>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICorePlayableCollectionGroupChildren> IMerged<ICorePlayableCollectionGroupChildren>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreArtistCollection> IMerged<ICoreArtistCollection>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreArtistCollectionItem> IMerged<ICoreArtistCollectionItem>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreAlbumCollection> IMerged<ICoreAlbumCollection>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreAlbumCollectionItem> IMerged<ICoreAlbumCollectionItem>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreTrackCollection> IMerged<ICoreTrackCollection>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICorePlaylistCollection> IMerged<ICorePlaylistCollection>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICorePlaylistCollectionItem> IMerged<ICorePlaylistCollectionItem>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreImageCollection> IMerged<ICoreImageCollection>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreUrlCollection> IMerged<ICoreUrlCollection>.Sources => Sources;

        /// <inheritdoc />
        public TimeSpan Duration => _collectionGroup.Duration;

        /// <inheritdoc />
        public DateTime? LastPlayed => _collectionGroup.LastPlayed;

        /// <inheritdoc />
        public DateTime? AddedAt => _collectionGroup.AddedAt;

        /// <inheritdoc />
        public ObservableCollection<TrackViewModel> UnsortedTracks { get; }

        /// <inheritdoc />
        public ObservableCollection<IArtistCollectionItem> UnsortedArtists { get; }

        ///<inheritdoc />
        public ObservableCollection<IAlbumCollectionItem> UnsortedAlbums { get; }

        ///<inheritdoc />
        public ObservableCollection<IPlaylistCollectionItem> UnsortedPlaylists { get; }

        /// <summary>
        /// The albums in this collection.
        /// </summary>
        public ObservableCollection<IAlbumCollectionItem> Albums { get; }

        /// <inheritdoc />
        public ObservableCollection<IArtistCollectionItem> Artists { get; }

        /// <summary>
        /// The nested <see cref="IPlayableCollectionGroupBase"/> items in this collection.
        /// </summary>
        public ObservableCollection<PlayableCollectionGroupViewModel> Children { get; }

        /// <inheritdoc />
        public ObservableCollection<IPlaylistCollectionItem> Playlists { get; }

        /// <inheritdoc />
        public ObservableCollection<TrackViewModel> Tracks { get; set; }

        /// <inheritdoc />
        public ObservableCollection<IImage> Images { get; }

        /// <inheritdoc />
        public ObservableCollection<IUrl> Urls { get; }

        /// <inheritdoc />
        public TrackSortingType CurrentTracksSortingType { get; private set; }

        /// <inheritdoc />
        public SortDirection CurrentTracksSortingDirection { get; private set; }

        ///<inheritdoc />
        public PlaylistSortingType CurrentPlaylistSortingType { get; private set; }

        ///<inheritdoc />
        public SortDirection CurrentPlaylistSortingDirection { get; private set; }

        /// <inheritdoc />
        public ArtistSortingType CurrentArtistSortingType { get; private set; }

        /// <inheritdoc />
        public SortDirection CurrentArtistSortingDirection { get; private set; }

        /// <inheritdoc />
        public AlbumSortingType CurrentAlbumSortingType { get; private set; }

        /// <inheritdoc />
        public SortDirection CurrentAlbumSortingDirection { get; private set; }

        /// <inheritdoc />
        public string Name => _collectionGroup.Name;

        /// <inheritdoc />
        public int TotalTrackCount => _collectionGroup.TotalTrackCount;

        /// <inheritdoc />
        public int TotalAlbumItemsCount => _collectionGroup.TotalAlbumItemsCount;

        /// <inheritdoc />
        public int TotalArtistItemsCount => _collectionGroup.TotalArtistItemsCount;

        /// <inheritdoc />
        public int TotalChildrenCount => _collectionGroup.TotalChildrenCount;

        /// <inheritdoc />
        public int TotalPlaylistItemsCount => _collectionGroup.TotalPlaylistItemsCount;

        /// <inheritdoc />
        public int TotalImageCount => _collectionGroup.TotalImageCount;

        /// <inheritdoc />
        public int TotalUrlCount => _collectionGroup.TotalUrlCount;

        /// <inheritdoc />
        public string? Description => _collectionGroup.Description;

        /// <inheritdoc />
        public PlaybackState PlaybackState => _collectionGroup.PlaybackState;

        /// <inheritdoc />
        public DownloadInfo DownloadInfo => _collectionGroup.DownloadInfo;

        /// <inheritdoc />
        public bool IsPlayPlaylistCollectionAsyncAvailable => _collectionGroup.IsPlayPlaylistCollectionAsyncAvailable;

        /// <inheritdoc />
        public bool IsPausePlaylistCollectionAsyncAvailable => _collectionGroup.IsPausePlaylistCollectionAsyncAvailable;

        /// <inheritdoc />
        public bool IsPlayTrackCollectionAsyncAvailable => _collectionGroup.IsPlayTrackCollectionAsyncAvailable;

        /// <inheritdoc />
        public bool IsPauseTrackCollectionAsyncAvailable => _collectionGroup.IsPauseTrackCollectionAsyncAvailable;

        /// <inheritdoc />
        public bool IsPlayAlbumCollectionAsyncAvailable => _collectionGroup.IsPlayAlbumCollectionAsyncAvailable;

        /// <inheritdoc />
        public bool IsPauseAlbumCollectionAsyncAvailable => _collectionGroup.IsPauseAlbumCollectionAsyncAvailable;

        /// <inheritdoc />
        public bool IsPlayArtistCollectionAsyncAvailable => _collectionGroup.IsPlayArtistCollectionAsyncAvailable;

        /// <inheritdoc />
        public bool IsPauseArtistCollectionAsyncAvailable => _collectionGroup.IsPauseArtistCollectionAsyncAvailable;

        /// <inheritdoc />
        public bool IsChangeNameAsyncAvailable => _collectionGroup.IsChangeNameAsyncAvailable;

        /// <inheritdoc />
        public bool IsChangeDescriptionAsyncAvailable => _collectionGroup.IsChangeDescriptionAsyncAvailable;

        /// <inheritdoc />
        public bool IsChangeDurationAsyncAvailable => _collectionGroup.IsChangeDurationAsyncAvailable;

        /// <inheritdoc />
        public Task<bool> IsAddAlbumItemAvailableAsync(int index) => _collectionGroup.IsAddAlbumItemAvailableAsync(index);

        /// <inheritdoc />
        public Task<bool> IsAddArtistItemAvailableAsync(int index) => _collectionGroup.IsAddArtistItemAvailableAsync(index);

        /// <inheritdoc />
        public Task<bool> IsAddChildAvailableAsync(int index) => _collectionGroup.IsAddChildAvailableAsync(index);

        /// <inheritdoc />
        public Task<bool> IsAddPlaylistItemAvailableAsync(int index) => _collectionGroup.IsAddPlaylistItemAvailableAsync(index);

        /// <inheritdoc />
        public Task<bool> IsAddTrackAvailableAsync(int index) => _collectionGroup.IsAddTrackAvailableAsync(index);

        /// <inheritdoc />
        public Task<bool> IsAddImageAvailableAsync(int index) => _collectionGroup.IsAddImageAvailableAsync(index);

        /// <inheritdoc />
        public Task<bool> IsAddUrlAvailableAsync(int index) => _collectionGroup.IsAddUrlAvailableAsync(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveAlbumItemAvailableAsync(int index) => _collectionGroup.IsRemoveAlbumItemAvailableAsync(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveArtistItemAvailableAsync(int index) => _collectionGroup.IsRemoveArtistItemAvailableAsync(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveChildAvailableAsync(int index) => _collectionGroup.IsRemoveChildAvailableAsync(index);

        /// <inheritdoc />
        public Task<bool> IsRemovePlaylistItemAvailableAsync(int index) => _collectionGroup.IsRemovePlaylistItemAvailableAsync(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveTrackAvailableAsync(int index) => _collectionGroup.IsRemoveTrackAvailableAsync(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveImageAvailableAsync(int index) => _collectionGroup.IsRemoveImageAvailableAsync(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveUrlAvailableAsync(int index) => _collectionGroup.IsRemoveUrlAvailableAsync(index);

        /// <inheritdoc />
        public Task StartDownloadOperationAsync(DownloadOperation operation) => _collectionGroup.StartDownloadOperationAsync(operation);

        /// <inheritdoc />
        public Task ChangeNameAsync(string name) => ChangeNameInternalAsync(name);

        /// <inheritdoc />
        public Task ChangeDescriptionAsync(string? description) => _collectionGroup.ChangeDescriptionAsync(description);

        /// <inheritdoc />
        public Task ChangeDurationAsync(TimeSpan duration) => _collectionGroup.ChangeDurationAsync(duration);

        /// <inheritdoc />
        public Task AddAlbumItemAsync(IAlbumCollectionItem album, int index) => _collectionGroup.AddAlbumItemAsync(album, index);

        /// <inheritdoc />
        public Task AddArtistItemAsync(IArtistCollectionItem artist, int index) => _collectionGroup.AddArtistItemAsync(artist, index);

        /// <inheritdoc />
        public Task AddChildAsync(IPlayableCollectionGroup child, int index) => _collectionGroup.AddChildAsync(child, index);

        /// <inheritdoc />
        public Task AddPlaylistItemAsync(IPlaylistCollectionItem playlist, int index) => _collectionGroup.AddPlaylistItemAsync(playlist, index);

        /// <inheritdoc />
        public Task AddTrackAsync(ITrack track, int index) => _collectionGroup.AddTrackAsync(track, index);

        /// <inheritdoc />
        public Task AddUrlAsync(IUrl url, int index) => _collectionGroup.AddUrlAsync(url, index);

        /// <inheritdoc />
        public Task RemoveAlbumItemAsync(int index) => _collectionGroup.RemoveAlbumItemAsync(index);

        /// <inheritdoc />
        public Task RemoveArtistItemAsync(int index) => _collectionGroup.RemoveArtistItemAsync(index);

        /// <inheritdoc />
        public Task RemoveChildAsync(int index) => _collectionGroup.RemoveChildAsync(index);

        /// <inheritdoc />
        public Task RemovePlaylistItemAsync(int index) => _collectionGroup.RemovePlaylistItemAsync(index);

        /// <inheritdoc />
        public Task RemoveTrackAsync(int index) => _collectionGroup.RemoveTrackAsync(index);

        /// <inheritdoc />
        public Task RemoveImageAsync(int index) => _collectionGroup.RemoveImageAsync(index);

        /// <inheritdoc />
        public Task RemoveUrlAsync(int index) => _collectionGroup.RemoveUrlAsync(index);

        /// <inheritdoc />
        public Task<IReadOnlyList<IPlayableCollectionGroup>> GetChildrenAsync(int limit, int offset) => _collectionGroup.GetChildrenAsync(limit, offset);

        /// <inheritdoc />
        public Task<IReadOnlyList<IPlaylistCollectionItem>> GetPlaylistItemsAsync(int limit, int offset) => _collectionGroup.GetPlaylistItemsAsync(limit, offset);

        /// <inheritdoc />
        public Task<IReadOnlyList<ITrack>> GetTracksAsync(int limit, int offset = 0) => _collectionGroup.GetTracksAsync(limit, offset);

        /// <inheritdoc />
        public Task<IReadOnlyList<IAlbumCollectionItem>> GetAlbumItemsAsync(int limit, int offset) => _collectionGroup.GetAlbumItemsAsync(limit, offset);

        /// <inheritdoc />
        public Task<IReadOnlyList<IArtistCollectionItem>> GetArtistItemsAsync(int limit, int offset) => _collectionGroup.GetArtistItemsAsync(limit, offset);

        /// <inheritdoc />
        public Task AddImageAsync(IImage image, int index) => _collectionGroup.AddImageAsync(image, index);

        /// <inheritdoc />
        public Task<IReadOnlyList<IImage>> GetImagesAsync(int limit, int offset) => _collectionGroup.GetImagesAsync(limit, offset);

        /// <inheritdoc />
        public Task<IReadOnlyList<IUrl>> GetUrlsAsync(int limit, int offset) => _collectionGroup.GetUrlsAsync(limit, offset);

        /// <inheritdoc />
        public async Task PopulateMorePlaylistsAsync(int limit)
        {
            using (await Flow.EasySemaphore(_populatePlaylistsMutex))
            {
                var items = await Task.Run(() => _collectionGroup.GetPlaylistItemsAsync(limit, Playlists.Count));

                await Threading.OnPrimaryThread(() =>
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
                });
            }
        }

        /// <inheritdoc />
        public async Task PopulateMoreTracksAsync(int limit)
        {
            using (await Flow.EasySemaphore(_populateTracksMutex))
            {
                var items = await Task.Run(() => _collectionGroup.GetTracksAsync(limit, Tracks.Count));

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
        public async Task PopulateMoreAlbumsAsync(int limit)
        {
            using (await Flow.EasySemaphore(_populateAlbumsMutex))
            {
                var items = await Task.Run(() => _collectionGroup.GetAlbumItemsAsync(limit, Albums.Count));

                await Threading.OnPrimaryThread(() =>
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
                });
            }
        }

        /// <inheritdoc />
        public async Task PopulateMoreArtistsAsync(int limit)
        {
            using (await Flow.EasySemaphore(_populateArtistsMutex))
            {
                var items = await Task.Run(() => _collectionGroup.GetArtistItemsAsync(limit, Artists.Count));

                await Threading.OnPrimaryThread(() =>
                {
                    foreach (var item in items)
                    {
                        if (item is IArtist artist)
                        {
                            Artists.Add(new ArtistViewModel(Root, artist));
                        }

                        if (item is IArtistCollection collection)
                        {
                            Artists.Add(new ArtistCollectionViewModel(Root, collection));
                        }
                    }
                });
            }
        }

        /// <inheritdoc />
        public async Task PopulateMoreChildrenAsync(int limit)
        {
            using (await Flow.EasySemaphore(_populateChildrenMutex))
            {
                var items = await Task.Run(() => _collectionGroup.GetChildrenAsync(limit, Albums.Count));

                await Threading.OnPrimaryThread(() =>
                {
                    foreach (var item in items)
                    {
                        Children.Add(new PlayableCollectionGroupViewModel(Root, item));
                    }
                });
            }
        }

        /// <inheritdoc />
        public async Task PopulateMoreImagesAsync(int limit)
        {
            using (await Flow.EasySemaphore(_populateImagesMutex))
            {
                var items = await Task.Run(() => _collectionGroup.GetImagesAsync(limit, Images.Count));

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
        public async Task PopulateMoreUrlsAsync(int limit)
        {
            using (await Flow.EasySemaphore(_populateUrlsMutex))
            {
                var items = await Task.Run(() => _collectionGroup.GetUrlsAsync(limit, Urls.Count));

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
        public Task PlayAlbumCollectionAsync(IAlbumCollectionItem albumItem) => PlayAlbumCollectionInternalAsync(albumItem);

        /// <inheritdoc />
        public Task PlayAlbumCollectionAsync() => _playbackHandler.PlayAsync((IAlbumCollectionViewModel)this, _collectionGroup);

        /// <inheritdoc />
        public Task PauseAlbumCollectionAsync() => _playbackHandler.PauseAsync();

        /// <inheritdoc />
        public Task PlayArtistCollectionAsync(IArtistCollectionItem artistItem) => PlayArtistCollectionInternalAsync(artistItem);

        /// <inheritdoc />
        public Task PlayArtistCollectionAsync() => _playbackHandler.PlayAsync((IArtistCollectionViewModel)this, _collectionGroup);

        /// <inheritdoc />
        public Task PauseArtistCollectionAsync() => _playbackHandler.PauseAsync();

        /// <inheritdoc />
        public Task PlayPlayableCollectionGroupAsync(IPlayableCollectionGroup collectionGroup) => PlayPlayableCollectionGroupInternalAsync(collectionGroup);

        /// <inheritdoc />
        public Task PlayPlayableCollectionGroupAsync() => _collectionGroup.PlayPlayableCollectionGroupAsync();

        /// <inheritdoc />
        public Task PausePlayableCollectionGroupAsync() => _collectionGroup.PausePlayableCollectionGroupAsync();

        /// <inheritdoc />
        public Task PlayPlaylistCollectionAsync(IPlaylistCollectionItem playlistItem) => PlayPlaylistCollectionInternalAsync(playlistItem);

        /// <inheritdoc />
        public Task PlayPlaylistCollectionAsync() => _playbackHandler.PlayAsync((IPlaylistCollectionViewModel)this, _collectionGroup);

        /// <inheritdoc />
        public Task PausePlaylistCollectionAsync() => _playbackHandler.PauseAsync();

        /// <inheritdoc />
        public Task PlayTrackCollectionAsync(ITrack track) => PlayTrackCollectionInternalAsync(track);

        /// <inheritdoc />
        public Task PlayTrackCollectionAsync() => _playbackHandler.PlayAsync((ITrackCollectionViewModel)this, this);

        /// <inheritdoc />
        public Task PauseTrackCollectionAsync() => _playbackHandler.PauseAsync();

        /// <inheritdoc />
        public void SortAlbumCollection(AlbumSortingType albumSorting, SortDirection sortDirection)
        {
            CurrentAlbumSortingType = albumSorting;
            CurrentAlbumSortingDirection = sortDirection;

            CollectionSorting.SortAlbums(Albums, albumSorting, sortDirection, UnsortedAlbums);
        }

        ///<inheritdoc />
        public void SortArtistCollection(ArtistSortingType artistSorting, SortDirection sortDirection)
        {
            CurrentArtistSortingType = artistSorting;
            CurrentArtistSortingDirection = sortDirection;

            CollectionSorting.SortArtists(Artists, artistSorting, sortDirection, UnsortedArtists);
        }

        ///<inheritdoc />
        public void SortPlaylistCollection(PlaylistSortingType playlistSorting, SortDirection sortDirection)
        {
            CurrentPlaylistSortingType = playlistSorting;
            CurrentPlaylistSortingDirection = sortDirection;

            CollectionSorting.SortPlaylists(Playlists, playlistSorting, sortDirection, UnsortedPlaylists);
        }

        ///<inheritdoc />
        public void SortTrackCollection(TrackSortingType trackSorting, SortDirection sortDirection)
        {
            CurrentTracksSortingType = trackSorting;
            CurrentTracksSortingDirection = sortDirection;

            CollectionSorting.SortTracks(Tracks, trackSorting, sortDirection, UnsortedTracks);
        }

        /// <inheritdoc />
        public Task InitAlbumCollectionAsync() => CollectionInit.AlbumCollection(this);

        /// <inheritdoc />
        public Task InitImageCollectionAsync() => CollectionInit.ImageCollection(this);

        /// <inheritdoc />
        public Task InitArtistCollectionAsync() => CollectionInit.ArtistCollection(this);

        /// <inheritdoc />
        public Task InitTrackCollectionAsync() => CollectionInit.TrackCollection(this);

        /// <inheritdoc />
        public Task InitPlaylistCollectionAsync() => CollectionInit.PlaylistCollection(this);

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
        public IAsyncRelayCommand<int> PopulateMorePlaylistsCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand<int> PopulateMoreAlbumsCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand<int> PopulateMoreArtistsCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand<int> PopulateMoreChildrenCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand<int> PopulateMoreImagesCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand<int> PopulateMoreUrlsCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand<IAlbumCollectionItem> PlayAlbumAsyncCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand PlayAlbumCollectionAsyncCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand PauseAlbumCollectionAsyncCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand<IArtistCollectionItem> PlayArtistAsyncCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand PlayArtistCollectionAsyncCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand PauseArtistCollectionAsyncCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand<IPlaylistCollectionItem> PlayPlaylistAsyncCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand PlayPlaylistCollectionAsyncCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand PausePlaylistCollectionAsyncCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand<ITrack> PlayTrackAsyncCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand PlayTrackCollectionAsyncCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand PauseTrackCollectionAsyncCommand { get; }

        /// <inheritdoc />
        public IRelayCommand<AlbumSortingType> ChangeAlbumCollectionSortingTypeCommand { get; }

        /// <inheritdoc />
        public IRelayCommand<SortDirection> ChangeAlbumCollectionSortingDirectionCommand { get; }

        /// <inheritdoc />
        public IRelayCommand<ArtistSortingType> ChangeArtistCollectionSortingTypeCommand { get; }

        /// <inheritdoc />
        public IRelayCommand<SortDirection> ChangeArtistCollectionSortingDirectionCommand { get; }

        /// <inheritdoc />
        public IRelayCommand<PlaylistSortingType> ChangePlaylistCollectionSortingTypeCommand { get; }

        /// <inheritdoc />
        public IRelayCommand<SortDirection> ChangePlaylistCollectionSortingDirectionCommand { get; }

        /// <inheritdoc />
        public IRelayCommand<TrackSortingType> ChangeTrackCollectionSortingTypeCommand { get; }

        /// <inheritdoc />
        public IRelayCommand<SortDirection> ChangeTrackCollectionSortingDirectionCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand InitAlbumCollectionAsyncCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand InitImageCollectionAsyncCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand InitArtistCollectionAsyncCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand InitTrackCollectionAsyncCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand InitPlaylistCollectionAsyncCommand { get; }

        /// <inheritdoc />
        public bool Equals(ICoreAlbumCollectionItem other) => _collectionGroup.Equals(other);

        /// <inheritdoc />
        public bool Equals(ICoreAlbumCollection other) => _collectionGroup.Equals(other);

        /// <inheritdoc />
        public bool Equals(ICoreArtistCollectionItem other) => _collectionGroup.Equals(other);

        /// <inheritdoc />
        public bool Equals(ICoreArtistCollection other) => _collectionGroup.Equals(other);

        /// <inheritdoc />
        public bool Equals(ICorePlayableCollectionGroupChildren other) => _collectionGroup.Equals(other);

        /// <inheritdoc />
        public bool Equals(ICorePlayableCollectionGroup other) => _collectionGroup.Equals(other);

        /// <inheritdoc />
        public bool Equals(ICorePlaylistCollectionItem other) => _collectionGroup.Equals(other);

        /// <inheritdoc />
        public bool Equals(ICorePlaylistCollection other) => _collectionGroup.Equals(other);

        /// <inheritdoc />
        public bool Equals(ICoreTrackCollection other) => _collectionGroup.Equals(other);

        /// <inheritdoc />
        public bool Equals(ICoreImageCollection other) => _collectionGroup.Equals(other);

        /// <inheritdoc />
        public bool Equals(ICoreUrlCollection other) => _collectionGroup.Equals(other);

        /// <inheritdoc />
        public virtual Task InitAsync()
        {
            if (IsInitialized)
                return Task.CompletedTask;

            IsInitialized = true;

            return Task.WhenAll(InitImageCollectionAsync(), InitPlaylistCollectionAsync(), InitTrackCollectionAsync(), InitAlbumCollectionAsync(), InitArtistCollectionAsync());
        }

        private Task ChangeNameInternalAsync(string? name)
        {
            Guard.IsNotNull(name, nameof(name));
            return _collectionGroup.ChangeNameAsync(name);
        }

        private Task PlayTrackCollectionInternalAsync(ITrack? track)
        {
            Guard.IsNotNull(track, nameof(track));
            return _playbackHandler.PlayAsync(track, this, _collectionGroup);
        }

        private Task PlayAlbumCollectionInternalAsync(IAlbumCollectionItem? albumItem)
        {
            Guard.IsNotNull(albumItem, nameof(albumItem));
            return _playbackHandler.PlayAsync(albumItem, this, _collectionGroup);
        }

        private Task PlayArtistCollectionInternalAsync(IArtistCollectionItem? artistItem)
        {
            Guard.IsNotNull(artistItem, nameof(artistItem));
            return _playbackHandler.PlayAsync(artistItem, this, _collectionGroup);
        }

        private Task PlayPlaylistCollectionInternalAsync(IPlaylistCollectionItem? playlistItem)
        {
            Guard.IsNotNull(playlistItem, nameof(playlistItem));

            return _playbackHandler.PlayAsync(playlistItem, this, _collectionGroup);
        }

        private Task PlayPlayableCollectionGroupInternalAsync(IPlayableCollectionGroup? collectionGroup)
        {
            Guard.IsNotNull(collectionGroup, nameof(collectionGroup));

            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public bool IsInitialized { get; protected set; }

        /// <inheritdoc />
        public ValueTask DisposeAsync()
        {
            DetachPropertyEvents();
            return _collectionGroup.DisposeAsync();
        }
    }
}
