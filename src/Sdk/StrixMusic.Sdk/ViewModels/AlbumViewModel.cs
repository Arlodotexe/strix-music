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
using StrixMusic.Sdk.Extensions;
using StrixMusic.Sdk.MediaPlayback;
using StrixMusic.Sdk.Models;
using StrixMusic.Sdk.Models.Core;
using StrixMusic.Sdk.ViewModels.Helpers;

namespace StrixMusic.Sdk.ViewModels
{
    /// <summary>
    /// A ViewModel for <see cref="IAlbum"/>.
    /// </summary>
    public sealed class AlbumViewModel : ObservableObject, ISdkViewModel, IAlbum, IArtistCollectionViewModel, ITrackCollectionViewModel, IImageCollectionViewModel, IUrlCollectionViewModel, IGenreCollectionViewModel
    {
        private readonly IAlbum _album;

        private readonly SemaphoreSlim _populateTracksMutex = new(1, 1);
        private readonly SemaphoreSlim _populateArtistsMutex = new(1, 1);
        private readonly SemaphoreSlim _populateImagesMutex = new(1, 1);
        private readonly SemaphoreSlim _populateUrlsMutex = new(1, 1);
        private readonly SemaphoreSlim _populateGenresMutex = new(1, 1);
        private readonly SynchronizationContext _syncContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="AlbumViewModel"/> class.
        /// </summary>
        /// <param name="root">The <see cref="MainViewModel"/> that this or the object that created this originated from.</param>
        /// <param name="album"><inheritdoc cref="IAlbum"/></param>
        internal AlbumViewModel(MainViewModel root, IAlbum album)
        {
            _syncContext = SynchronizationContext.Current;

            Root = root;
            _album = root.Plugins.ModelPlugins.Album.Execute(album);

            SourceCores = _album.GetSourceCores<ICoreAlbum>().Select(root.GetLoadedCore).ToList();

            Tracks = new ObservableCollection<TrackViewModel>();
            Artists = new ObservableCollection<IArtistCollectionItem>();
            UnsortedTracks = new ObservableCollection<TrackViewModel>();
            UnsortedArtists = new ObservableCollection<IArtistCollectionItem>();

            Images = new ObservableCollection<IImage>();
            Genres = new ObservableCollection<IGenre>();
            Urls = new ObservableCollection<IUrl>();

            if (album.RelatedItems != null)
                RelatedItems = new PlayableCollectionGroupViewModel(root, album.RelatedItems);

            PauseTrackCollectionAsyncCommand = new AsyncRelayCommand(PauseTrackCollectionAsync);
            PlayTrackCollectionAsyncCommand = new AsyncRelayCommand(PlayTrackCollectionAsync);
            PauseArtistCollectionAsyncCommand = new AsyncRelayCommand(PauseArtistCollectionAsync);
            PlayArtistCollectionAsyncCommand = new AsyncRelayCommand(PlayArtistCollectionAsync);

            PlayTrackAsyncCommand = new AsyncRelayCommand<ITrack>((x, y) => _album.PlayTrackCollectionAsync(x ?? ThrowHelper.ThrowArgumentNullException<ITrack>(nameof(x)), y));
            PlayArtistAsyncCommand = new AsyncRelayCommand<IArtistCollectionItem>((x, y) => _album.PlayArtistCollectionAsync(x ?? ThrowHelper.ThrowArgumentNullException<IArtistCollectionItem>(nameof(x)), y));

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
            Flow.Catch<NotSupportedException>(() => DownloadInfoChanged += OnDownloadInfoChanged);

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
            UrlsCountChanged += OnUrlsCountChanged;
            UrlsChanged += OnUrlsChanged;
        }

        private void DetachEvents()
        {
            PlaybackStateChanged -= AlbumPlaybackStateChanged;
            DescriptionChanged -= AlbumDescriptionChanged;
            DatePublishedChanged -= AlbumDatePublishedChanged;
            NameChanged -= AlbumNameChanged;
            LastPlayedChanged -= OnLastPlayedChanged;
            Flow.Catch<NotSupportedException>(() => DownloadInfoChanged -= OnDownloadInfoChanged);

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

        private void AlbumNameChanged(object sender, string e) => _syncContext.Post(_ => OnPropertyChanged(nameof(Name)), null);

        private void AlbumDescriptionChanged(object sender, string? e) => _syncContext.Post(_ => OnPropertyChanged(nameof(Description)), null);

        private void AlbumPlaybackStateChanged(object sender, PlaybackState e) => _syncContext.Post(_ => OnPropertyChanged(nameof(PlaybackState)), null);

        private void OnDownloadInfoChanged(object sender, DownloadInfo e) => _syncContext.Post(_ => OnPropertyChanged(nameof(DownloadInfo)), null);

        private void AlbumDatePublishedChanged(object sender, DateTime? e) => _syncContext.Post(_ => OnPropertyChanged(nameof(DatePublished)), null);

        private void OnTrackItemsCountChanged(object sender, int e) => _syncContext.Post(_ => OnPropertyChanged(nameof(TotalTrackCount)), null);

        private void OnImagesCountChanged(object sender, int e) => _syncContext.Post(_ => OnPropertyChanged(nameof(TotalImageCount)), null);

        private void OnUrlsCountChanged(object sender, int e) => _syncContext.Post(_ => OnPropertyChanged(nameof(TotalUrlCount)), null);

        private void OnArtistItemsCountChanged(object sender, int e) => _syncContext.Post(_ => OnPropertyChanged(nameof(TotalArtistItemsCount)), null);

        private void OnGenresItemsCountChanged(object sender, int e) => _syncContext.Post(_ => OnPropertyChanged(nameof(TotalGenreCount)), null);

        private void OnLastPlayedChanged(object sender, DateTime? e) => _syncContext.Post(_ => OnPropertyChanged(nameof(LastPlayed)), null);

        private void OnIsChangeDescriptionAsyncAvailableChanged(object sender, bool e) => _syncContext.Post(_ => OnPropertyChanged(nameof(IsChangeDescriptionAsyncAvailable)), null);

        private void OnIsChangeDurationAsyncAvailableChanged(object sender, bool e) => _syncContext.Post(_ => OnPropertyChanged(nameof(IsChangeDurationAsyncAvailable)), null);

        private void OnIsChangeNameAsyncAvailableChanged(object sender, bool e) => _syncContext.Post(_ => OnPropertyChanged(nameof(IsChangeNameAsyncAvailable)), null);

        private void OnIsPauseTrackCollectionAsyncAvailableChanged(object sender, bool e) => _syncContext.Post(_ => OnPropertyChanged(nameof(IsPauseTrackCollectionAsyncAvailable)), null);

        private void OnIsPlayTrackCollectionAsyncAvailableChanged(object sender, bool e) => _syncContext.Post(_ => OnPropertyChanged(nameof(IsPlayTrackCollectionAsyncAvailable)), null);

        private void OnIsPauseArtistCollectionAsyncAvailableChanged(object sender, bool e) => _syncContext.Post(_ => OnPropertyChanged(nameof(IsPauseArtistCollectionAsyncAvailable)), null);

        private void OnIsPlayArtistCollectionAsyncAvailableChanged(object sender, bool e) => _syncContext.Post(_ => OnPropertyChanged(nameof(IsPlayArtistCollectionAsyncAvailable)), null);

        private void OnIsChangeDatePublishedAsyncAvailableChanged(object sender, bool e) => _syncContext.Post(_ => OnPropertyChanged(nameof(IsChangeDatePublishedAsyncAvailableChanged)), null);

        private void AlbumViewModel_TrackItemsChanged(object sender, IReadOnlyList<CollectionChangedItem<ITrack>> addedItems, IReadOnlyList<CollectionChangedItem<ITrack>> removedItems) => _syncContext.Post(_ =>
        {
            if (CurrentTracksSortingType == TrackSortingType.Unsorted)
            {
                Tracks.ChangeCollection(addedItems, removedItems, x => new TrackViewModel(Root, x.Data));
            }
            else
            {
                // Make sure both ordered and unordered tracks are updated. 
                UnsortedTracks.ChangeCollection(addedItems, removedItems, x => new TrackViewModel(Root, x.Data));

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

        private void OnArtistItemsChanged(object sender, IReadOnlyList<CollectionChangedItem<IArtistCollectionItem>> addedItems, IReadOnlyList<CollectionChangedItem<IArtistCollectionItem>> removedItems) => _syncContext.Post(_ =>
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
                // Make sure both ordered and unordered artists are updated. 
                UnsortedArtists.ChangeCollection(addedItems, removedItems, item => item.Data switch
                {
                    IArtist artist => new ArtistViewModel(Root, artist),
                    IArtistCollection collection => new ArtistCollectionViewModel(Root, collection),
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
        }, null);

        private void AlbumViewModel_ImagesChanged(object sender, IReadOnlyList<CollectionChangedItem<IImage>> addedItems, IReadOnlyList<CollectionChangedItem<IImage>> removedItems)
            => _syncContext.Post(_ => Images.ChangeCollection(addedItems, removedItems), null);

        private void OnGenresChanged(object sender, IReadOnlyList<CollectionChangedItem<IGenre>> addedItems, IReadOnlyList<CollectionChangedItem<IGenre>> removedItems)
            => _syncContext.Post(_ => Genres.ChangeCollection(addedItems, removedItems), null);

        private void OnUrlsChanged(object sender, IReadOnlyList<CollectionChangedItem<IUrl>> addedItems, IReadOnlyList<CollectionChangedItem<IUrl>> removedItems)
            => _syncContext.Post(_ => Urls.ChangeCollection(addedItems, removedItems), null);

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
        public ObservableCollection<TrackViewModel> Tracks { get; }

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
        public string Name => _album.Name;

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
        public Task PlayTrackCollectionAsync(CancellationToken cancellationToken = default) => _album.PlayTrackCollectionAsync(cancellationToken);

        /// <inheritdoc />
        public Task PauseTrackCollectionAsync(CancellationToken cancellationToken = default) => _album.PauseTrackCollectionAsync(cancellationToken);

        /// <inheritdoc />
        public Task PlayArtistCollectionAsync(CancellationToken cancellationToken = default) => _album.PlayArtistCollectionAsync(cancellationToken);

        /// <inheritdoc />
        public Task PauseArtistCollectionAsync(CancellationToken cancellationToken = default) => _album.PauseArtistCollectionAsync(cancellationToken);

        /// <inheritdoc />
        public Task PlayTrackCollectionAsync(ITrack track, CancellationToken cancellationToken = default) => _album.PlayTrackCollectionAsync(track, cancellationToken);

        /// <inheritdoc />
        public Task PlayArtistCollectionAsync(IArtistCollectionItem artistItem, CancellationToken cancellationToken = default) => _album.PlayArtistCollectionAsync(artistItem, cancellationToken);

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
        public Task ChangeNameAsync(string name, CancellationToken cancellationToken = default) => ChangeNameInternalAsync(name, cancellationToken);

        /// <inheritdoc />
        public Task ChangeDescriptionAsync(string? description, CancellationToken cancellationToken = default) => _album.ChangeDescriptionAsync(description, cancellationToken);

        /// <inheritdoc />
        public Task ChangeDurationAsync(TimeSpan duration, CancellationToken cancellationToken = default) => _album.ChangeDurationAsync(duration, cancellationToken);

        /// <inheritdoc />
        public Task<bool> IsAddImageAvailableAsync(int index, CancellationToken cancellationToken = default) => _album.IsAddImageAvailableAsync(index, cancellationToken);

        /// <inheritdoc />
        public Task<bool> IsAddTrackAvailableAsync(int index, CancellationToken cancellationToken = default) => _album.IsAddTrackAvailableAsync(index, cancellationToken);

        /// <inheritdoc />
        public Task<bool> IsAddArtistItemAvailableAsync(int index, CancellationToken cancellationToken = default) => _album.IsAddArtistItemAvailableAsync(index, cancellationToken);

        /// <inheritdoc />
        public Task<bool> IsAddGenreAvailableAsync(int index, CancellationToken cancellationToken = default) => _album.IsAddGenreAvailableAsync(index, cancellationToken);

        /// <inheritdoc />
        public Task<bool> IsAddUrlAvailableAsync(int index, CancellationToken cancellationToken = default) => _album.IsAddUrlAvailableAsync(index, cancellationToken);

        /// <inheritdoc />
        public Task<bool> IsRemoveArtistItemAvailableAsync(int index, CancellationToken cancellationToken = default) => _album.IsRemoveArtistItemAvailableAsync(index, cancellationToken);

        /// <inheritdoc />
        public Task<bool> IsRemoveImageAvailableAsync(int index, CancellationToken cancellationToken = default) => _album.IsRemoveImageAvailableAsync(index, cancellationToken);

        /// <inheritdoc />
        public Task<bool> IsRemoveGenreAvailableAsync(int index, CancellationToken cancellationToken = default) => _album.IsRemoveGenreAvailableAsync(index, cancellationToken);

        /// <inheritdoc />
        public Task<bool> IsRemoveTrackAvailableAsync(int index, CancellationToken cancellationToken = default) => _album.IsRemoveTrackAvailableAsync(index, cancellationToken);

        /// <inheritdoc />
        public Task<bool> IsRemoveUrlAvailableAsync(int index, CancellationToken cancellationToken = default) => _album.IsRemoveUrlAvailableAsync(index, cancellationToken);

        /// <inheritdoc />
        public Task ChangeDatePublishedAsync(DateTime datePublished, CancellationToken cancellationToken = default) => _album.ChangeDatePublishedAsync(datePublished, cancellationToken);

        /// <inheritdoc />
        public Task StartDownloadOperationAsync(DownloadOperation operation, CancellationToken cancellationToken = default) => _album.StartDownloadOperationAsync(operation, cancellationToken);

        /// <inheritdoc />
        public Task AddTrackAsync(ITrack track, int index, CancellationToken cancellationToken = default) => _album.AddTrackAsync(track, index, cancellationToken);

        /// <inheritdoc />
        public Task RemoveTrackAsync(int index, CancellationToken cancellationToken = default) => _album.RemoveTrackAsync(index, cancellationToken);

        /// <inheritdoc />
        public Task AddImageAsync(IImage image, int index, CancellationToken cancellationToken = default) => _album.AddImageAsync(image, index, cancellationToken);

        /// <inheritdoc />
        public Task RemoveImageAsync(int index, CancellationToken cancellationToken = default) => _album.RemoveImageAsync(index, cancellationToken);

        /// <inheritdoc />
        public Task AddArtistItemAsync(IArtistCollectionItem artist, int index, CancellationToken cancellationToken = default) => _album.AddArtistItemAsync(artist, index, cancellationToken);

        /// <inheritdoc />
        public Task RemoveArtistItemAsync(int index, CancellationToken cancellationToken = default) => _album.RemoveArtistItemAsync(index, cancellationToken);

        /// <inheritdoc/>
        public Task AddGenreAsync(IGenre genre, int index, CancellationToken cancellationToken = default) => _album.AddGenreAsync(genre, index, cancellationToken);

        /// <inheritdoc/>
        public Task RemoveGenreAsync(int index, CancellationToken cancellationToken = default) => _album.RemoveGenreAsync(index, cancellationToken);

        /// <inheritdoc/>
        public Task AddUrlAsync(IUrl url, int index, CancellationToken cancellationToken = default) => _album.AddUrlAsync(url, index, cancellationToken);

        /// <inheritdoc/>
        public Task RemoveUrlAsync(int index, CancellationToken cancellationToken = default) => _album.RemoveUrlAsync(index, cancellationToken);

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
        public Task<IReadOnlyList<ITrack>> GetTracksAsync(int limit, int offset, CancellationToken cancellationToken = default) => _album.GetTracksAsync(limit, offset, cancellationToken);

        /// <inheritdoc />
        public async Task PopulateMoreTracksAsync(int limit, CancellationToken cancellationToken = default)
        {
            using (await Flow.EasySemaphore(_populateTracksMutex))
            {
                using var releaseReg = cancellationToken.Register(() => _populateTracksMutex.Release());
                foreach (var item in await _album.GetTracksAsync(limit, Tracks.Count, cancellationToken))
                    Tracks.Add(new TrackViewModel(Root, item));
            }
        }

        /// <inheritdoc />
        public Task<IReadOnlyList<IImage>> GetImagesAsync(int limit, int offset, CancellationToken cancellationToken = default) => _album.GetImagesAsync(limit, offset, cancellationToken);

        /// <inheritdoc />
        public Task<IReadOnlyList<IArtistCollectionItem>> GetArtistItemsAsync(int limit, int offset, CancellationToken cancellationToken = default) => _album.GetArtistItemsAsync(limit, offset, cancellationToken);

        /// <inheritdoc />
        public Task<IReadOnlyList<IGenre>> GetGenresAsync(int limit, int offset, CancellationToken cancellationToken = default) => _album.GetGenresAsync(limit, offset, cancellationToken);

        /// <inheritdoc />
        public Task<IReadOnlyList<IUrl>> GetUrlsAsync(int limit, int offset, CancellationToken cancellationToken = default) => _album.GetUrlsAsync(limit, offset, cancellationToken);

        /// <inheritdoc />
        public async Task PopulateMoreArtistsAsync(int limit, CancellationToken cancellationToken = default)
        {
            using (await Flow.EasySemaphore(_populateArtistsMutex))
            {
                using var releaseReg = cancellationToken.Register(() => _populateArtistsMutex.Release());
                var items = await _album.GetArtistItemsAsync(limit, Artists.Count, cancellationToken);

                _syncContext.Post(_ =>
                {
                    foreach (var item in items)
                    {
                        if (item is IArtist artist)
                            Artists.Add(new ArtistViewModel(Root, artist));
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
                var items = await _album.GetImagesAsync(limit, Images.Count, cancellationToken);

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
                var items = await _album.GetUrlsAsync(limit, Urls.Count, cancellationToken);

                _syncContext.Post(_ =>
                {
                    foreach (var item in items)
                        Urls.Add(item);
                }, null);
            }
        }

        /// <inheritdoc />
        public async Task PopulateMoreGenresAsync(int limit, CancellationToken cancellationToken = default)
        {
            using (await Flow.EasySemaphore(_populateGenresMutex))
            {
                using var releaseReg = cancellationToken.Register(() => _populateGenresMutex.Release());
                var items = await _album.GetGenresAsync(limit, Genres.Count, cancellationToken);

                _syncContext.Post(_ =>
                {
                    foreach (var item in items)
                        Genres.Add(item);
                }, null);
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
        public Task InitArtistCollectionAsync(CancellationToken cancellationToken = default) => CollectionInit.ArtistCollection(this, cancellationToken);

        /// <inheritdoc />
        public Task InitImageCollectionAsync(CancellationToken cancellationToken = default) => CollectionInit.ImageCollection(this, cancellationToken);

        /// <inheritdoc />
        public Task InitTrackCollectionAsync(CancellationToken cancellationToken = default) => CollectionInit.TrackCollection(this, cancellationToken);

        /// <inheritdoc />
        public Task InitGenreCollectionAsync(CancellationToken cancellationToken = default) => CollectionInit.GenreCollection(this, cancellationToken);

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
        public Task InitAsync(CancellationToken cancellationToken = default)
        {
            if (IsInitialized)
                return Task.CompletedTask;

            IsInitialized = true;

            return Task.WhenAll(InitImageCollectionAsync(cancellationToken), InitTrackCollectionAsync(cancellationToken), InitGenreCollectionAsync(cancellationToken), InitArtistCollectionAsync(cancellationToken));
        }

        /// <inheritdoc />
        public bool IsInitialized { get; private set; }

        private Task ChangeNameInternalAsync(string? name, CancellationToken cancellationToken = default)
        {
            Guard.IsNotNull(name, nameof(name));
            return _album.ChangeNameAsync(name, cancellationToken);
        }

        /// <inheritdoc />
        public ValueTask DisposeAsync()
        {
            DetachEvents();
            return _album.DisposeAsync();
        }
    }
}
