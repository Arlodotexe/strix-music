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
using StrixMusic.Sdk.Services;
using StrixMusic.Sdk.ViewModels.Helpers;

namespace StrixMusic.Sdk.ViewModels
{
    /// <summary>
    /// Contains bindable information about an <see cref="IAlbum"/>.
    /// </summary>
    public sealed class AlbumViewModel : ObservableObject, ISdkViewModel, IAlbum, IArtistCollectionViewModel, ITrackCollectionViewModel, IImageCollectionViewModel, IUrlCollectionViewModel, IGenreCollectionViewModel
    {
        private readonly IAlbum _album;

        private readonly IPlaybackHandlerService _playbackHandler;
        private readonly ILocalizationService _localizationService;

        private readonly SemaphoreSlim _populateTracksMutex = new SemaphoreSlim(1, 1);
        private readonly SemaphoreSlim _populateArtistsMutex = new SemaphoreSlim(1, 1);
        private readonly SemaphoreSlim _populateImagesMutex = new SemaphoreSlim(1, 1);
        private readonly SemaphoreSlim _populateUrlsMutex = new SemaphoreSlim(1, 1);
        private readonly SemaphoreSlim _populateGenresMutex = new SemaphoreSlim(1, 1);

        /// <summary>
        /// Initializes a new instance of the <see cref="AlbumViewModel"/> class.
        /// </summary>
        /// <param name="root">The <see cref="MainViewModel"/> that this or the object that created this originated from.</param>
        /// <param name="album"><inheritdoc cref="IAlbum"/></param>
        internal AlbumViewModel(MainViewModel root, IAlbum album)
        {
            Root = root;
            _album = root.Plugins.ModelPlugins.Album.Execute(album);

            SourceCores = _album.GetSourceCores<ICoreAlbum>().Select(root.GetLoadedCore).ToList();

            _playbackHandler = Ioc.Default.GetRequiredService<IPlaybackHandlerService>();
            _localizationService = Ioc.Default.GetRequiredService<ILocalizationService>();

            using (Threading.PrimaryContext)
            {
                Tracks = new ObservableCollection<TrackViewModel>();
                Artists = new ObservableCollection<IArtistCollectionItem>();
                UnsortedTracks = new ObservableCollection<TrackViewModel>();
                UnsortedArtists = new ObservableCollection<IArtistCollectionItem>();

                Images = new ObservableCollection<IImage>();
                Genres = new ObservableCollection<IGenre>();
                Urls = new ObservableCollection<IUrl>();
            }

            if (album.RelatedItems != null)
                RelatedItems = new PlayableCollectionGroupViewModel(root, album.RelatedItems);

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
            PopulateMoreArtistsCommand = new AsyncRelayCommand<int>(PopulateMoreArtistsAsync);
            PopulateMoreGenresCommand = new AsyncRelayCommand<int>(PopulateMoreGenresAsync);
            PopulateMoreUrlsCommand = new AsyncRelayCommand<int>(PopulateMoreUrlsAsync);

            InitImageCollectionAsyncCommand = new AsyncRelayCommand(InitImageCollectionAsync);
            InitTrackCollectionAsyncCommand = new AsyncRelayCommand(InitTrackCollectionAsync);
            InitGenreCollectionAsyncCommand = new AsyncRelayCommand(InitGenreCollectionAsync);
            InitArtistCollectionAsyncCommand = new AsyncRelayCommand(InitArtistCollectionAsync);

            ChangeTrackCollectionSortingTypeCommand = new RelayCommand<TrackSortingType>(x => SortTrackCollection(x, CurrentTracksSortingDirection));
            ChangeTrackCollectionSortingDirectionCommand = new RelayCommand<SortDirection>(x => SortTrackCollection(CurrentTracksSortingType, x));
            ChangeArtistCollectionSortingTypeCommand = new RelayCommand<ArtistSortingType>(x => SortArtistCollection(x, CurrentArtistSortingDirection));
            ChangeArtistCollectionSortingDirectionCommand = new RelayCommand<SortDirection>(x => SortArtistCollection(CurrentArtistSortingType, x));

            AttachEvents();
        }

        private void AttachEvents()
        {
            PlaybackStateChanged += AlbumPlaybackStateChanged;
            DescriptionChanged += AlbumDescriptionChanged;
            DatePublishedChanged += AlbumDatePublishedChanged;
            NameChanged += AlbumNameChanged;
            LastPlayedChanged += OnLastPlayedChanged;
            DownloadInfoChanged += OnDownloadInfoChanged;

            IsPlayTrackCollectionAsyncAvailableChanged += OnIsPlayTrackCollectionAsyncAvailableChanged;
            IsPauseTrackCollectionAsyncAvailableChanged += OnIsPauseTrackCollectionAsyncAvailableChanged;
            IsPlayArtistCollectionAsyncAvailableChanged += OnIsPlayArtistCollectionAsyncAvailableChanged;
            IsPauseArtistCollectionAsyncAvailableChanged += OnIsPauseArtistCollectionAsyncAvailableChanged;
            IsChangeNameAsyncAvailableChanged += OnIsChangeNameAsyncAvailableChanged;
            IsChangeDurationAsyncAvailableChanged += OnIsChangeDurationAsyncAvailableChanged;
            IsChangeDescriptionAsyncAvailableChanged += OnIsChangeDescriptionAsyncAvailableChanged;
            IsChangeDatePublishedAsyncAvailableChanged += OnIsChangeDatePublishedAsyncAvailableChanged;

            TracksCountChanged += OnTrackItemsCountChanged;
            TracksChanged += AlbumViewModel_TrackItemsChanged;
            ArtistItemsCountChanged += OnArtistItemsCountChanged;
            ArtistItemsChanged += OnArtistItemsChanged;
            ImagesCountChanged += OnImagesCountChanged;
            ImagesChanged += AlbumViewModel_ImagesChanged;
            GenresCountChanged += OnGenresItemsCountChanged;
            GenresChanged += OnGenresChanged;
        }

        private void DetachEvents()
        {
            PlaybackStateChanged -= AlbumPlaybackStateChanged;
            DescriptionChanged -= AlbumDescriptionChanged;
            DatePublishedChanged -= AlbumDatePublishedChanged;
            NameChanged -= AlbumNameChanged;
            LastPlayedChanged -= OnLastPlayedChanged;
            DownloadInfoChanged -= OnDownloadInfoChanged;

            IsPlayTrackCollectionAsyncAvailableChanged -= OnIsPlayTrackCollectionAsyncAvailableChanged;
            IsPauseTrackCollectionAsyncAvailableChanged -= OnIsPauseTrackCollectionAsyncAvailableChanged;
            IsPlayArtistCollectionAsyncAvailableChanged -= OnIsPlayArtistCollectionAsyncAvailableChanged;
            IsPauseArtistCollectionAsyncAvailableChanged -= OnIsPauseArtistCollectionAsyncAvailableChanged;
            IsChangeNameAsyncAvailableChanged -= OnIsChangeNameAsyncAvailableChanged;
            IsChangeDurationAsyncAvailableChanged -= OnIsChangeDurationAsyncAvailableChanged;
            IsChangeDescriptionAsyncAvailableChanged -= OnIsChangeDescriptionAsyncAvailableChanged;
            IsChangeDatePublishedAsyncAvailableChanged -= OnIsChangeDatePublishedAsyncAvailableChanged;

            TracksCountChanged += OnTrackItemsCountChanged;
            TracksChanged -= AlbumViewModel_TrackItemsChanged;
            ArtistItemsCountChanged -= OnArtistItemsCountChanged;
            ArtistItemsChanged -= OnArtistItemsChanged;
            ImagesCountChanged -= OnImagesCountChanged;
            ImagesChanged -= AlbumViewModel_ImagesChanged;
            GenresCountChanged -= OnGenresItemsCountChanged;
            GenresChanged -= OnGenresChanged;
        }

        /// <inheritdoc />
        public event EventHandler<PlaybackState>? PlaybackStateChanged
        {
            add => _album.PlaybackStateChanged += value;
            remove => _album.PlaybackStateChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<DownloadInfo>? DownloadInfoChanged
        {
            add => _album.DownloadInfoChanged += value;
            remove => _album.DownloadInfoChanged -= value;
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
        public event EventHandler<bool>? IsChangeDatePublishedAsyncAvailableChanged
        {
            add => _album.IsChangeDatePublishedAsyncAvailableChanged += value;
            remove => _album.IsChangeDatePublishedAsyncAvailableChanged -= value;
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
        public event EventHandler<bool>? IsPauseArtistCollectionAsyncAvailableChanged
        {
            add => _album.IsPauseArtistCollectionAsyncAvailableChanged += value;
            remove => _album.IsPauseArtistCollectionAsyncAvailableChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<int>? TracksCountChanged
        {
            add => _album.TracksCountChanged += value;
            remove => _album.TracksCountChanged -= value;
        }

        /// <inheritdoc />
        public event CollectionChangedEventHandler<ITrack>? TracksChanged
        {
            add => _album.TracksChanged += value;
            remove => _album.TracksChanged -= value;
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
        public event CollectionChangedEventHandler<IUrl>? UrlsChanged
        {
            add => _album.UrlsChanged += value;
            remove => _album.UrlsChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<int>? UrlsCountChanged
        {
            add => _album.UrlsCountChanged += value;
            remove => _album.UrlsCountChanged -= value;
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

        /// <inheritdoc />
        public event CollectionChangedEventHandler<IGenre>? GenresChanged
        {
            add => _album.GenresChanged += value;
            remove => _album.GenresChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<int>? GenresCountChanged
        {
            add => _album.GenresCountChanged += value;
            remove => _album.GenresCountChanged -= value;
        }

        private void AlbumNameChanged(object sender, string e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(Name)));

        private void AlbumDescriptionChanged(object sender, string? e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(Description)));

        private void AlbumPlaybackStateChanged(object sender, PlaybackState e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(PlaybackState)));

        private void OnDownloadInfoChanged(object sender, DownloadInfo e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(DownloadInfo)));

        private void AlbumDatePublishedChanged(object sender, DateTime? e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(DatePublished)));

        private void OnTrackItemsCountChanged(object sender, int e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(TotalTrackCount)));

        private void OnImagesCountChanged(object sender, int e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(TotalImageCount)));

        private void OnUrlsCountChanged(object sender, int e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(TotalUrlCount)));

        private void OnArtistItemsCountChanged(object sender, int e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(TotalArtistItemsCount)));

        private void OnGenresItemsCountChanged(object sender, int e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(TotalGenreCount)));

        private void OnLastPlayedChanged(object sender, DateTime? e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(LastPlayed)));

        private void OnIsChangeDescriptionAsyncAvailableChanged(object sender, bool e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(IsChangeDescriptionAsyncAvailable)));

        private void OnIsChangeDurationAsyncAvailableChanged(object sender, bool e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(IsChangeDurationAsyncAvailable)));

        private void OnIsChangeNameAsyncAvailableChanged(object sender, bool e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(IsChangeNameAsyncAvailable)));

        private void OnIsPauseTrackCollectionAsyncAvailableChanged(object sender, bool e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(IsPauseTrackCollectionAsyncAvailable)));

        private void OnIsPlayTrackCollectionAsyncAvailableChanged(object sender, bool e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(IsPlayTrackCollectionAsyncAvailable)));

        private void OnIsPauseArtistCollectionAsyncAvailableChanged(object sender, bool e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(IsPauseArtistCollectionAsyncAvailable)));

        private void OnIsPlayArtistCollectionAsyncAvailableChanged(object sender, bool e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(IsPlayArtistCollectionAsyncAvailable)));

        private void OnIsChangeDatePublishedAsyncAvailableChanged(object sender, bool e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(IsChangeDatePublishedAsyncAvailableChanged)));

        private void AlbumViewModel_TrackItemsChanged(object sender, IReadOnlyList<CollectionChangedItem<ITrack>> addedItems, IReadOnlyList<CollectionChangedItem<ITrack>> removedItems)
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

        private void OnArtistItemsChanged(object sender, IReadOnlyList<CollectionChangedItem<IArtistCollectionItem>> addedItems, IReadOnlyList<CollectionChangedItem<IArtistCollectionItem>> removedItems)
        {
            _ = Threading.OnPrimaryThread(() =>
            {
                if (CurrentArtistSortingType == ArtistSortingType.Unsorted)
                {
                    Artists.ChangeCollection(addedItems, removedItems, item => item.Data switch
                    {
                        IArtist artist => new ArtistViewModel(Root, artist),
                        IArtistCollection collection => new ArtistCollectionViewModel(Root, collection),
                        _ => ThrowHelper.ThrowNotSupportedException<IArtistCollectionItem>(
                            $"{item.Data.GetType()} not supported for adding to {GetType()}")
                    });
                }
                else
                {
                    // Preventing index issues during artists emission from the core, also making sure that unordered artists updated. 
                    UnsortedArtists.ChangeCollection(addedItems, removedItems, item => item.Data switch
                    {
                        IArtist artist => new ArtistViewModel(Root, artist),
                        IArtistCollection collection => new ArtistCollectionViewModel(Root, collection),
                        _ => ThrowHelper.ThrowNotSupportedException<IArtistCollectionItem>(
                            $"{item.Data.GetType()} not supported for adding to {GetType()}")
                    });

                    // Avoiding direct assignment to prevent effect on UI.
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
        }

        private void AlbumViewModel_ImagesChanged(object sender, IReadOnlyList<CollectionChangedItem<IImage>> addedItems, IReadOnlyList<CollectionChangedItem<IImage>> removedItems)
        {
            _ = Threading.OnPrimaryThread(() =>
            {
                Images.ChangeCollection(addedItems, removedItems);
            });
        }

        private void OnGenresChanged(object sender, IReadOnlyList<CollectionChangedItem<IGenre>> addedItems, IReadOnlyList<CollectionChangedItem<IGenre>> removedItems)
        {
            _ = Threading.OnPrimaryThread(() =>
            {
                Genres.ChangeCollection(addedItems, removedItems);
            });
        }

        /// <inheritdoc />
        public string Id => _album.Id;

        /// <inheritdoc/>
        public MainViewModel Root { get; }

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
        IReadOnlyList<ICoreGenreCollection> IMerged<ICoreGenreCollection>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreUrlCollection> IMerged<ICoreUrlCollection>.Sources => Sources;

        /// <inheritdoc />
        public TimeSpan Duration => _album.Duration;

        /// <inheritdoc />
        public DateTime? LastPlayed => _album.LastPlayed;

        /// <inheritdoc />
        public DateTime? AddedAt => _album.AddedAt;

        /// <inheritdoc />
        public IPlayableCollectionGroup? RelatedItems { get; }

        /// <summary>
        /// The tracks for this album.
        /// </summary>
        public ObservableCollection<TrackViewModel> Tracks { get; set; }

        /// <inheritdoc />
        public ObservableCollection<TrackViewModel> UnsortedTracks { get; }

        /// <inheritdoc />
        public ObservableCollection<IArtistCollectionItem> UnsortedArtists { get; }

        /// <inheritdoc />
        public ObservableCollection<IArtistCollectionItem> Artists { get; }

        /// <inheritdoc />
        public ObservableCollection<IGenre> Genres { get; }

        /// <inheritdoc />
        public ObservableCollection<IImage> Images { get; }

        /// <inheritdoc />
        public ObservableCollection<IUrl> Urls { get; }

        /// <inheritdoc />
        public string Name => _localizationService.LocalizeIfNullOrEmpty(_album.Name, this);

        /// <inheritdoc />
        public int TotalTrackCount => _album.TotalTrackCount;

        /// <inheritdoc />
        public int TotalArtistItemsCount => _album.TotalArtistItemsCount;

        /// <inheritdoc />
        public int TotalImageCount => _album.TotalImageCount;

        /// <inheritdoc />
        public int TotalGenreCount => _album.TotalGenreCount;

        /// <inheritdoc />
        public int TotalUrlCount => _album.TotalUrlCount;

        /// <inheritdoc />
        public bool IsPlayTrackCollectionAsyncAvailable => _album.IsPlayTrackCollectionAsyncAvailable;

        /// <inheritdoc />
        public bool IsPauseTrackCollectionAsyncAvailable => _album.IsPauseTrackCollectionAsyncAvailable;

        /// <inheritdoc />
        public bool IsPlayArtistCollectionAsyncAvailable => _album.IsPlayArtistCollectionAsyncAvailable;

        /// <inheritdoc />
        public bool IsPauseArtistCollectionAsyncAvailable => _album.IsPauseArtistCollectionAsyncAvailable;

        /// <inheritdoc />
        public Task PlayTrackCollectionAsync() => _playbackHandler.PlayAsync((ITrackCollectionViewModel)this, _album);

        /// <inheritdoc />
        public Task PauseTrackCollectionAsync() => _playbackHandler.PauseAsync();

        /// <inheritdoc />
        public Task PlayArtistCollectionAsync() => _playbackHandler.PlayAsync((IArtistCollectionViewModel)this, _album);

        /// <inheritdoc />
        public Task PauseArtistCollectionAsync() => _playbackHandler.PauseAsync();

        /// <inheritdoc />
        public Task PlayTrackCollectionAsync(ITrack track) => PlayTrackCollectionInternalAsync(track);

        /// <inheritdoc />
        public Task PlayArtistCollectionAsync(IArtistCollectionItem artistItem) => PlayArtistCollectionInternalAsync(artistItem);

        /// <inheritdoc />
        public DateTime? DatePublished => _album.DatePublished;

        /// <inheritdoc />
        public string? Description => _album.Description;

        /// <inheritdoc />
        public PlaybackState PlaybackState => _album.PlaybackState;

        /// <inheritdoc />
        public DownloadInfo DownloadInfo => _album.DownloadInfo;

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
        public Task<bool> IsAddImageAvailableAsync(int index) => _album.IsAddImageAvailableAsync(index);

        /// <inheritdoc />
        public Task<bool> IsAddTrackAvailableAsync(int index) => _album.IsAddTrackAvailableAsync(index);

        /// <inheritdoc />
        public Task<bool> IsAddArtistItemAvailableAsync(int index) => _album.IsAddArtistItemAvailableAsync(index);

        /// <inheritdoc />
        public Task<bool> IsAddGenreAvailableAsync(int index) => _album.IsAddGenreAvailableAsync(index);

        /// <inheritdoc />
        public Task<bool> IsAddUrlAvailableAsync(int index) => _album.IsAddUrlAvailableAsync(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveArtistItemAvailableAsync(int index) => _album.IsRemoveArtistItemAvailableAsync(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveImageAvailableAsync(int index) => _album.IsRemoveImageAvailableAsync(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveGenreAvailableAsync(int index) => _album.IsRemoveGenreAvailableAsync(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveTrackAvailableAsync(int index) => _album.IsRemoveTrackAvailableAsync(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveUrlAvailableAsync(int index) => _album.IsRemoveUrlAvailableAsync(index);

        /// <inheritdoc />
        public Task ChangeDatePublishedAsync(DateTime datePublished) => _album.ChangeDatePublishedAsync(datePublished);

        /// <inheritdoc />
        public Task StartDownloadOperationAsync(DownloadOperation operation) => _album.StartDownloadOperationAsync(operation);

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

        /// <inheritdoc/>
        public Task AddGenreAsync(IGenre genre, int index) => _album.AddGenreAsync(genre, index);

        /// <inheritdoc/>
        public Task RemoveGenreAsync(int index) => _album.RemoveGenreAsync(index);

        /// <inheritdoc/>
        public Task AddUrlAsync(IUrl url, int index) => _album.AddUrlAsync(url, index);

        /// <inheritdoc/>
        public Task RemoveUrlAsync(int index) => _album.RemoveUrlAsync(index);

        ///<inheritdoc />
        public void SortTrackCollection(TrackSortingType trackSorting, SortDirection sortDirection)
        {
            CurrentTracksSortingType = trackSorting;
            CurrentTracksSortingDirection = sortDirection;

            CollectionSorting.SortTracks(Tracks, trackSorting, sortDirection, UnsortedTracks);
        }

        ///<inheritdoc />
        public void SortArtistCollection(ArtistSortingType artistSorting, SortDirection sortDirection)
        {
            CurrentArtistSortingType = artistSorting;
            CurrentArtistSortingDirection = sortDirection;

            CollectionSorting.SortArtists(Artists, artistSorting, sortDirection, UnsortedArtists);
        }

        /// <inheritdoc />
        public Task<IReadOnlyList<ITrack>> GetTracksAsync(int limit, int offset) => _album.GetTracksAsync(limit, offset);

        /// <inheritdoc />
        public async Task PopulateMoreTracksAsync(int limit)
        {
            using (await Flow.EasySemaphore(_populateTracksMutex))
            {
                foreach (var item in await _album.GetTracksAsync(limit, Tracks.Count))
                    Tracks.Add(new TrackViewModel(Root, item));
            }
        }

        /// <inheritdoc />
        public Task<IReadOnlyList<IImage>> GetImagesAsync(int limit, int offset) => _album.GetImagesAsync(limit, offset);

        /// <inheritdoc />
        public Task<IReadOnlyList<IArtistCollectionItem>> GetArtistItemsAsync(int limit, int offset) => _album.GetArtistItemsAsync(limit, offset);

        /// <inheritdoc />
        public Task<IReadOnlyList<IGenre>> GetGenresAsync(int limit, int offset) => _album.GetGenresAsync(limit, offset);

        /// <inheritdoc />
        public Task<IReadOnlyList<IUrl>> GetUrlsAsync(int limit, int offset) => _album.GetUrlsAsync(limit, offset);

        /// <inheritdoc />
        public async Task PopulateMoreArtistsAsync(int limit)
        {
            using (await Flow.EasySemaphore(_populateArtistsMutex))
            {
                var items = await _album.GetArtistItemsAsync(limit, Artists.Count);

                await Threading.OnPrimaryThread(() =>
                {
                    foreach (var item in items)
                    {
                        if (item is IArtist artist)
                        {
                            Artists.Add(new ArtistViewModel(Root, artist));
                        }
                    }
                });
            }
        }

        /// <inheritdoc />
        public async Task PopulateMoreImagesAsync(int limit)
        {
            using (await Flow.EasySemaphore(_populateImagesMutex))
            {
                var items = await _album.GetImagesAsync(limit, Images.Count);

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
                var items = await _album.GetUrlsAsync(limit, Urls.Count);

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
        public async Task PopulateMoreGenresAsync(int limit)
        {
            using (await Flow.EasySemaphore(_populateGenresMutex))
            {
                var items = await _album.GetGenresAsync(limit, Genres.Count);

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
        public TrackSortingType CurrentTracksSortingType { get; private set; }

        /// <inheritdoc />
        public SortDirection CurrentTracksSortingDirection { get; private set; }

        /// <inheritdoc />
        public ArtistSortingType CurrentArtistSortingType { get; private set; }

        /// <inheritdoc />
        public SortDirection CurrentArtistSortingDirection { get; private set; }

        /// <inheritdoc />
        public IRelayCommand<TrackSortingType> ChangeTrackCollectionSortingTypeCommand { get; }

        /// <inheritdoc />
        public IRelayCommand<SortDirection> ChangeTrackCollectionSortingDirectionCommand { get; }

        /// <inheritdoc />
        public IRelayCommand<ArtistSortingType> ChangeArtistCollectionSortingTypeCommand { get; }

        /// <inheritdoc />
        public IRelayCommand<SortDirection> ChangeArtistCollectionSortingDirectionCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand<int> PopulateMoreTracksCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand<int> PopulateMoreImagesCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand<int> PopulateMoreGenresCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand<int> PopulateMoreUrlsCommand { get; }

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
        public IAsyncRelayCommand InitArtistCollectionAsyncCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand InitImageCollectionAsyncCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand InitTrackCollectionAsyncCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand InitGenreCollectionAsyncCommand { get; }

        /// <inheritdoc />
        public Task InitArtistCollectionAsync() => CollectionInit.ArtistCollection(this);

        /// <inheritdoc />
        public Task InitImageCollectionAsync() => CollectionInit.ImageCollection(this);

        /// <inheritdoc />
        public Task InitTrackCollectionAsync() => CollectionInit.TrackCollection(this);

        /// <inheritdoc />
        public Task InitGenreCollectionAsync() => CollectionInit.GenreCollection(this);

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
        public bool Equals(ICoreGenreCollection other) => _album.Equals(other);

        /// <inheritdoc/>
        public bool Equals(ICoreUrlCollection other) => _album.Equals(other);

        /// <inheritdoc />
        public bool Equals(ICoreAlbum other) => _album.Equals(other);

        /// <inheritdoc />
        public Task InitAsync()
        {
            if (IsInitialized)
                return Task.CompletedTask;

            IsInitialized = true;

            return Task.WhenAll(InitImageCollectionAsync(), InitTrackCollectionAsync(), InitGenreCollectionAsync(), InitArtistCollectionAsync());
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
