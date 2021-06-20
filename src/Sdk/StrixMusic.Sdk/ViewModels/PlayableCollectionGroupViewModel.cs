using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Toolkit.Diagnostics;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Input;
using Nito.AsyncEx;
using OwlCore;
using OwlCore.Events;
using OwlCore.Extensions;
using StrixMusic.Sdk.Data;
using StrixMusic.Sdk.Data.Base;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.Data.Merged;
using StrixMusic.Sdk.Extensions;
using StrixMusic.Sdk.MediaPlayback;
using StrixMusic.Sdk.Services.MediaPlayback;
using StrixMusic.Sdk.ViewModels.Helpers;

namespace StrixMusic.Sdk.ViewModels
{
    /// <summary>
    /// An observable wrapper for a <see cref="IPlayableCollectionGroupBase"/>.
    /// </summary>
    public class PlayableCollectionGroupViewModel : ObservableObject, IPlayableCollectionGroup, IPlayableCollectionGroupChildrenViewModel, IAlbumCollectionViewModel, IArtistCollectionViewModel, ITrackCollectionViewModel, IPlaylistCollectionViewModel, IImageCollectionViewModel
    {
        private readonly IPlayableCollectionGroup _collectionGroup;
        private readonly IPlaybackHandlerService _playbackHandler;

        private readonly AsyncLock _populateTracksMutex = new AsyncLock();
        private readonly AsyncLock _populateAlbumsMutex = new AsyncLock();
        private readonly AsyncLock _populateArtistsMutex = new AsyncLock();
        private readonly AsyncLock _populatePlaylistsMutex = new AsyncLock();
        private readonly AsyncLock _populateChildrenMutex = new AsyncLock();
        private readonly AsyncLock _populateImagesMutex = new AsyncLock();

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayableCollectionGroupViewModel"/> class.
        /// </summary>
        /// <param name="collectionGroup">The base <see cref="IPlayableCollectionGroup"/> containing properties about this class.</param>
        public PlayableCollectionGroupViewModel(IPlayableCollectionGroup collectionGroup)
        {
            _collectionGroup = collectionGroup ?? throw new ArgumentNullException(nameof(collectionGroup));

            _playbackHandler = Ioc.Default.GetRequiredService<IPlaybackHandlerService>();

            SourceCores = _collectionGroup.GetSourceCores<ICorePlayableCollectionGroup>().Select(MainViewModel.GetLoadedCore).ToList();

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

            using (Threading.PrimaryContext)
            {
                Images = new ObservableCollection<IImage>();
                Tracks = new ObservableCollection<TrackViewModel>();
                Artists = new ObservableCollection<IArtistCollectionItem>();
                Children = new ObservableCollection<PlayableCollectionGroupViewModel>();
                Playlists = new ObservableCollection<IPlaylistCollectionItem>();
                Albums = new ObservableCollection<IAlbumCollectionItem>();
            }

            AttachPropertyEvents();
        }

        private void AttachPropertyEvents()
        {
            PlaybackStateChanged += CollectionGroupPlaybackStateChanged;
            DescriptionChanged += CollectionGroupDescriptionChanged;
            NameChanged += CollectionGroupNameChanged;
            UrlChanged += CollectionGroupUrlChanged;
            LastPlayedChanged += CollectionGroupLastPlayedChanged;

            AlbumItemsCountChanged += CollectionGroupOnAlbumItemsCountChanged;
            TrackItemsCountChanged += CollectionGroupOnTrackItemsCountChanged;
            ArtistItemsCountChanged += CollectionGroupOnArtistItemsCountChanged;
            PlaylistItemsCountChanged += CollectionGroupOnPlaylistItemsCountChanged;
            TotalChildrenCountChanged += CollectionGroupOnTotalChildrenCountChanged;
            ImagesCountChanged += PlayableCollectionGroupViewModel_ImagesCountChanged;

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
            TrackItemsChanged += PlayableCollectionGroupViewModel_TrackItemsChanged;
            ArtistItemsChanged += PlayableCollectionGroupViewModel_ArtistItemsChanged;
            PlaylistItemsChanged += PlayableCollectionGroupViewModel_PlaylistItemsChanged;
            ChildItemsChanged += PlayableCollectionGroupViewModel_ChildItemsChanged;
            ImagesChanged += PlayableCollectionGroupViewModel_ImagesChanged;
        }

        private void DetachPropertyEvents()
        {
            PlaybackStateChanged -= CollectionGroupPlaybackStateChanged;
            DescriptionChanged -= CollectionGroupDescriptionChanged;
            NameChanged -= CollectionGroupNameChanged;
            UrlChanged -= CollectionGroupUrlChanged;
            LastPlayedChanged -= CollectionGroupLastPlayedChanged;

            AlbumItemsCountChanged -= CollectionGroupOnAlbumItemsCountChanged;
            TrackItemsCountChanged -= CollectionGroupOnTrackItemsCountChanged;
            ArtistItemsCountChanged -= CollectionGroupOnArtistItemsCountChanged;
            PlaylistItemsCountChanged -= CollectionGroupOnPlaylistItemsCountChanged;
            TotalChildrenCountChanged -= CollectionGroupOnTotalChildrenCountChanged;
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
            TrackItemsChanged -= PlayableCollectionGroupViewModel_TrackItemsChanged;
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
        public event EventHandler<Uri?>? UrlChanged
        {
            add => _collectionGroup.UrlChanged += value;
            remove => _collectionGroup.UrlChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<PlaybackState>? PlaybackStateChanged
        {
            add => _collectionGroup.PlaybackStateChanged += value;
            remove => _collectionGroup.PlaybackStateChanged -= value;
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
        public event EventHandler<bool>? IsPauseTrackCollectionAsyncAvailableChanged
        {
            add => _collectionGroup.IsPauseTrackCollectionAsyncAvailableChanged += value;
            remove => _collectionGroup.IsPauseTrackCollectionAsyncAvailableChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<int>? TrackItemsCountChanged
        {
            add => _collectionGroup.TrackItemsCountChanged += value;
            remove => _collectionGroup.TrackItemsCountChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<bool>? IsPauseArtistCollectionAsyncAvailableChanged
        {
            add => _collectionGroup.IsPauseArtistCollectionAsyncAvailableChanged += value;
            remove => _collectionGroup.IsPauseArtistCollectionAsyncAvailableChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<int>? ArtistItemsCountChanged
        {
            add => _collectionGroup.ArtistItemsCountChanged += value;
            remove => _collectionGroup.ArtistItemsCountChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<bool>? IsPauseAlbumCollectionAsyncAvailableChanged
        {
            add => _collectionGroup.IsPauseAlbumCollectionAsyncAvailableChanged += value;
            remove => _collectionGroup.IsPauseAlbumCollectionAsyncAvailableChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<int>? AlbumItemsCountChanged
        {
            add => _collectionGroup.AlbumItemsCountChanged += value;
            remove => _collectionGroup.AlbumItemsCountChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<bool>? IsPausePlaylistCollectionAsyncAvailableChanged
        {
            add => _collectionGroup.IsPausePlaylistCollectionAsyncAvailableChanged += value;
            remove => _collectionGroup.IsPausePlaylistCollectionAsyncAvailableChanged -= value;
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
        public event CollectionChangedEventHandler<ITrack>? TrackItemsChanged
        {
            add => _collectionGroup.TrackItemsChanged += value;
            remove => _collectionGroup.TrackItemsChanged -= value;
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
        public event EventHandler<int>? TotalChildrenCountChanged
        {
            add => _collectionGroup.TotalChildrenCountChanged += value;
            remove => _collectionGroup.TotalChildrenCountChanged -= value;
        }

        private void CollectionGroupUrlChanged(object sender, Uri? e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(Url)));

        private void CollectionGroupNameChanged(object sender, string e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(Name)));

        private void CollectionGroupDescriptionChanged(object sender, string? e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(Description)));

        private void CollectionGroupPlaybackStateChanged(object sender, PlaybackState e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(PlaybackState)));

        private void CollectionGroupOnTotalChildrenCountChanged(object sender, int e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(TotalChildrenCount)));

        private void CollectionGroupOnPlaylistItemsCountChanged(object sender, int e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(TotalPlaylistItemsCount)));

        private void CollectionGroupOnArtistItemsCountChanged(object sender, int e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(TotalArtistItemsCount)));

        private void CollectionGroupOnTrackItemsCountChanged(object sender, int e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(TotalTracksCount)));

        private void CollectionGroupOnAlbumItemsCountChanged(object sender, int e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(TotalAlbumItemsCount)));

        private void PlayableCollectionGroupViewModel_ImagesCountChanged(object sender, int e) => _ = Threading.OnPrimaryThread(() => OnPropertyChanged(nameof(TotalImageCount)));

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

        private void PlayableCollectionGroupViewModel_ImagesChanged(object sender, IReadOnlyList<CollectionChangedItem<IImage>> addedItems, IReadOnlyList<CollectionChangedItem<IImage>> removedItems)
        {
            _ = Threading.OnPrimaryThread(() =>
            {
                Images.ChangeCollection(addedItems, removedItems);
            });
        }

        private void PlayableCollectionGroupViewModel_ChildItemsChanged(object sender, IReadOnlyList<CollectionChangedItem<IPlayableCollectionGroup>> addedItems, IReadOnlyList<CollectionChangedItem<IPlayableCollectionGroup>> removedItems)
        {
            _ = Threading.OnPrimaryThread(() =>
            {
                Children.ChangeCollection(addedItems, removedItems, item => new PlayableCollectionGroupViewModel(item.Data));
            });
        }

        private void PlayableCollectionGroupViewModel_PlaylistItemsChanged(object sender, IReadOnlyList<CollectionChangedItem<IPlaylistCollectionItem>> addedItems, IReadOnlyList<CollectionChangedItem<IPlaylistCollectionItem>> removedItems)
        {
            _ = Threading.OnPrimaryThread(() =>
            {
                Playlists.ChangeCollection(addedItems, removedItems, item => item.Data switch
                {
                    IPlaylist playlist => new PlaylistViewModel(playlist),
                    IPlaylistCollection collection => new PlaylistCollectionViewModel(collection),
                    _ => ThrowHelper.ThrowNotSupportedException<IPlaylistCollectionItem>($"{item.Data.GetType()} not supported for adding to {GetType()}")
                });
            });
        }

        private void PlayableCollectionGroupViewModel_ArtistItemsChanged(object sender, IReadOnlyList<CollectionChangedItem<IArtistCollectionItem>> addedItems, IReadOnlyList<CollectionChangedItem<IArtistCollectionItem>> removedItems)
        {
            _ = Threading.OnPrimaryThread(() =>
            {
                Artists.ChangeCollection(addedItems, removedItems, item => item.Data switch
                {
                    IArtist artist => new ArtistViewModel(artist),
                    IArtistCollection collection => new ArtistCollectionViewModel(collection),
                    _ => ThrowHelper.ThrowNotSupportedException<IArtistCollectionItem>($"{item.Data.GetType()} not supported for adding to {GetType()}")
                });
            });
        }

        private void PlayableCollectionGroupViewModel_TrackItemsChanged(object sender, IReadOnlyList<CollectionChangedItem<ITrack>> addedItems, IReadOnlyList<CollectionChangedItem<ITrack>> removedItems)
        {
            _ = Threading.OnPrimaryThread(() =>
            {
                Tracks.ChangeCollection(addedItems, removedItems, item => new TrackViewModel(item.Data));
            });
        }

        private void PlayableCollectionGroupViewModel_AlbumItemsChanged(object sender, IReadOnlyList<CollectionChangedItem<IAlbumCollectionItem>> addedItems, IReadOnlyList<CollectionChangedItem<IAlbumCollectionItem>> removedItems)
        {
            _ = Threading.OnPrimaryThread(() =>
            {
                Albums.ChangeCollection(addedItems, removedItems, item => item.Data switch
                {
                    IAlbum album => new AlbumViewModel(album),
                    IAlbumCollection collection => new AlbumCollectionViewModel(collection),
                    _ => ThrowHelper.ThrowNotSupportedException<IAlbumCollectionItem>($"{item.Data.GetType()} not supported for adding to {GetType()}")
                });
            });
        }

        /// <inheritdoc />
        public string Id => _collectionGroup.Id;

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
        public TimeSpan Duration => _collectionGroup.Duration;

        /// <inheritdoc />
        public DateTime? LastPlayed => _collectionGroup.LastPlayed;

        /// <inheritdoc />
        public DateTime? AddedAt => _collectionGroup.AddedAt;

        /// <inheritdoc />
        public ObservableCollection<IImage> Images { get; }

        /// <inheritdoc />
        public ObservableCollection<IPlaylistCollectionItem> Playlists { get; }

        /// <inheritdoc />
        public ObservableCollection<TrackViewModel> Tracks { get; }

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
        public string Name => _collectionGroup.Name;

        /// <inheritdoc />
        public int TotalPlaylistItemsCount => _collectionGroup.TotalPlaylistItemsCount;

        /// <inheritdoc />
        public int TotalArtistItemsCount => _collectionGroup.TotalArtistItemsCount;

        /// <inheritdoc />
        public int TotalAlbumItemsCount => _collectionGroup.TotalAlbumItemsCount;

        /// <inheritdoc />
        public int TotalTracksCount => _collectionGroup.TotalTracksCount;

        /// <inheritdoc />
        public int TotalChildrenCount => _collectionGroup.TotalChildrenCount;

        /// <inheritdoc />
        public int TotalImageCount => _collectionGroup.TotalImageCount;

        /// <inheritdoc />
        public Uri? Url => _collectionGroup.Url;

        /// <inheritdoc />
        public string? Description => _collectionGroup.Description;

        /// <inheritdoc />
        public PlaybackState PlaybackState => _collectionGroup.PlaybackState;

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
        public Task<bool> IsAddImageAvailable(int index) => _collectionGroup.IsAddImageAvailable(index);

        /// <inheritdoc />
        public Task<bool> IsAddPlaylistItemAvailable(int index) => _collectionGroup.IsAddPlaylistItemAvailable(index);

        /// <inheritdoc />
        public Task<bool> IsAddTrackAvailable(int index) => _collectionGroup.IsAddTrackAvailable(index);

        /// <inheritdoc />
        public Task<bool> IsAddAlbumItemAvailable(int index) => _collectionGroup.IsAddAlbumItemAvailable(index);

        /// <inheritdoc />
        public Task<bool> IsAddArtistItemAvailable(int index) => _collectionGroup.IsAddArtistItemAvailable(index);

        /// <inheritdoc />
        public Task<bool> IsAddChildAvailable(int index) => _collectionGroup.IsAddChildAvailable(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveImageAvailable(int index) => _collectionGroup.IsRemoveImageAvailable(index);

        /// <inheritdoc />
        public Task<bool> IsRemovePlaylistItemAvailable(int index) => _collectionGroup.IsRemovePlaylistItemAvailable(index);

        /// <inheritdoc />
        public event EventHandler<bool>? IsPlayPlaylistCollectionAsyncAvailableChanged
        {
            add => _collectionGroup.IsPlayPlaylistCollectionAsyncAvailableChanged += value;
            remove => _collectionGroup.IsPlayPlaylistCollectionAsyncAvailableChanged -= value;
        }

        /// <inheritdoc />
        public Task<bool> IsRemoveTrackAvailable(int index) => _collectionGroup.IsRemoveTrackAvailable(index);

        /// <inheritdoc />
        public event EventHandler<bool>? IsPlayTrackCollectionAsyncAvailableChanged
        {
            add => _collectionGroup.IsPlayTrackCollectionAsyncAvailableChanged += value;
            remove => _collectionGroup.IsPlayTrackCollectionAsyncAvailableChanged -= value;
        }

        /// <inheritdoc />
        public Task<bool> IsRemoveArtistItemAvailable(int index) => _collectionGroup.IsRemoveArtistItemAvailable(index);

        /// <inheritdoc />
        public event EventHandler<bool>? IsPlayArtistCollectionAsyncAvailableChanged
        {
            add => _collectionGroup.IsPlayArtistCollectionAsyncAvailableChanged += value;
            remove => _collectionGroup.IsPlayArtistCollectionAsyncAvailableChanged -= value;
        }

        /// <inheritdoc />
        public Task<bool> IsRemoveAlbumItemAvailable(int index) => _collectionGroup.IsRemoveAlbumItemAvailable(index);

        /// <inheritdoc />
        public event EventHandler<bool>? IsPlayAlbumCollectionAsyncAvailableChanged
        {
            add => _collectionGroup.IsPlayAlbumCollectionAsyncAvailableChanged += value;
            remove => _collectionGroup.IsPlayAlbumCollectionAsyncAvailableChanged -= value;
        }

        /// <inheritdoc />
        public Task<bool> IsRemoveChildAvailable(int index) => _collectionGroup.IsRemoveChildAvailable(index);

        /// <inheritdoc />
        public Task ChangeNameAsync(string name) => ChangeNameInternalAsync(name);

        /// <inheritdoc />
        public Task ChangeDescriptionAsync(string? description) => _collectionGroup.ChangeDescriptionAsync(description);

        /// <inheritdoc />
        public Task ChangeDurationAsync(TimeSpan duration) => _collectionGroup.ChangeDurationAsync(duration);

        /// <inheritdoc />
        public Task AddTrackAsync(ITrack track, int index) => _collectionGroup.AddTrackAsync(track, index);

        /// <inheritdoc />
        public Task AddArtistItemAsync(IArtistCollectionItem artist, int index) => _collectionGroup.AddArtistItemAsync(artist, index);

        /// <inheritdoc />
        public Task AddAlbumItemAsync(IAlbumCollectionItem album, int index) => _collectionGroup.AddAlbumItemAsync(album, index);

        /// <inheritdoc />
        public Task AddPlaylistItemAsync(IPlaylistCollectionItem playlist, int index) => _collectionGroup.AddPlaylistItemAsync(playlist, index);

        /// <inheritdoc />
        public Task AddChildAsync(IPlayableCollectionGroup child, int index) => _collectionGroup.AddChildAsync(child, index);

        /// <inheritdoc />
        public Task RemoveTrackAsync(int index) => _collectionGroup.RemoveTrackAsync(index);

        /// <inheritdoc />
        public Task RemoveArtistItemAsync(int index) => _collectionGroup.RemoveArtistItemAsync(index);

        /// <inheritdoc />
        public Task RemoveAlbumItemAsync(int index) => _collectionGroup.RemoveAlbumItemAsync(index);

        /// <inheritdoc />
        public Task RemovePlaylistItemAsync(int index) => _collectionGroup.RemovePlaylistItemAsync(index);

        /// <inheritdoc />
        public Task RemoveChildAsync(int index) => _collectionGroup.RemoveChildAsync(index);

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
        public Task RemoveImageAsync(int index) => _collectionGroup.RemoveImageAsync(index);

        /// <inheritdoc />
        public Task<IReadOnlyList<IImage>> GetImagesAsync(int limit, int offset) => _collectionGroup.GetImagesAsync(limit, offset);

        /// <inheritdoc />
        public Task AddImageAsync(IImage image, int index) => _collectionGroup.AddImageAsync(image, index);

        /// <inheritdoc />
        public async Task PopulateMorePlaylistsAsync(int limit)
        {
            using (await _populatePlaylistsMutex.LockAsync())
            {
                var items = await Task.Run(() => _collectionGroup.GetPlaylistItemsAsync(limit, Playlists.Count));

                _ = Threading.OnPrimaryThread(() =>
                {
                    foreach (var item in items)
                    {
                        switch (item)
                        {
                            case IPlaylist playlist:
                                Playlists.Add(new PlaylistViewModel(playlist));
                                break;
                            case IPlaylistCollection collection:
                                Playlists.Add(new PlaylistCollectionViewModel(collection));
                                break;
                        }
                    }
                });
            }
        }

        /// <inheritdoc />
        public async Task PopulateMoreTracksAsync(int limit)
        {
            using (await _populateTracksMutex.LockAsync())
            {
                var items = await Task.Run(() => _collectionGroup.GetTracksAsync(limit, Tracks.Count));

                _ = Threading.OnPrimaryThread(() =>
                {
                    foreach (var item in items)
                    {
                        Tracks.Add(new TrackViewModel(item));
                    }
                });
            }
        }

        /// <inheritdoc />
        public async Task PopulateMoreAlbumsAsync(int limit)
        {
            using (await _populateAlbumsMutex.LockAsync())
            {
                var items = await Task.Run(() => _collectionGroup.GetAlbumItemsAsync(limit, Albums.Count));

                _ = Threading.OnPrimaryThread(() =>
                {
                    foreach (var item in items)
                    {
                        switch (item)
                        {
                            case IAlbum album:
                                Albums.Add(new AlbumViewModel(album));
                                break;
                            case IAlbumCollection collection:
                                Albums.Add(new AlbumCollectionViewModel(collection));
                                break;
                        }
                    }
                });
            }
        }

        /// <inheritdoc />
        public async Task PopulateMoreArtistsAsync(int limit)
        {
            using (await _populateArtistsMutex.LockAsync())
            {
                var items = await Task.Run(() => _collectionGroup.GetArtistItemsAsync(limit, Artists.Count));

                _ = Threading.OnPrimaryThread(() =>
                {
                    foreach (var item in items)
                    {
                        if (item is IArtist artist)
                        {
                            Artists.Add(new ArtistViewModel(artist));
                        }

                        if (item is IArtistCollection collection)
                        {
                            Artists.Add(new ArtistCollectionViewModel(collection));
                        }
                    }
                });
            }
        }

        /// <inheritdoc />
        public async Task PopulateMoreChildrenAsync(int limit)
        {
            using (await _populateChildrenMutex.LockAsync())
            {
                var items = await Task.Run(() => _collectionGroup.GetChildrenAsync(limit, Albums.Count));

                _ = Threading.OnPrimaryThread(() =>
                {
                    foreach (var item in items)
                    {
                        Children.Add(new PlayableCollectionGroupViewModel(item));
                    }
                });
            }
        }

        /// <inheritdoc />
        public async Task PopulateMoreImagesAsync(int limit)
        {
            using (await _populateImagesMutex.LockAsync())
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
        public Task PlayPlayableCollectionGroupAsync()
        {
            return _collectionGroup.PlayPlayableCollectionGroupAsync();
        }

        /// <inheritdoc />
        public Task PlayPlayableCollectionGroupAsync(IPlayableCollectionGroup collectionGroup) => PlayPlayableCollectionGroupInternalAsync(collectionGroup);

        /// <inheritdoc />
        public Task PausePlayableCollectionGroupAsync()
        {
            return _collectionGroup.PausePlayableCollectionGroupAsync();
        }

        /// <inheritdoc />
        public Task PlayTrackCollectionAsync(ITrack track) => PlayTrackCollectionInternalAsync(track);

        /// <inheritdoc />
        public Task PlayAlbumCollectionAsync(IAlbumCollectionItem albumItem) => PlayAlbumCollectionInternalAsync(albumItem);

        /// <inheritdoc />
        public Task PlayArtistCollectionAsync(IArtistCollectionItem artistItem) => PlayArtistCollectionInternalAsync(artistItem);

        /// <inheritdoc />
        public Task PlayPlaylistCollectionAsync(IPlaylistCollectionItem playlistItem) => PlayPlaylistCollectionInternalAsync(playlistItem);

        /// <inheritdoc />
        public Task PlayPlaylistCollectionAsync() => _collectionGroup.PlayPlaylistCollectionAsync();

        /// <inheritdoc />
        public Task PausePlaylistCollectionAsync()
        {
            return _playbackHandler.PauseAsync();
        }

        /// <inheritdoc />
        public Task PlayTrackCollectionAsync()
        {
            return _playbackHandler.PlayAsync((ITrackCollectionViewModel)this, this);
        }

        /// <inheritdoc />
        public Task PauseTrackCollectionAsync()
        {
            return _playbackHandler.PauseAsync();
        }

        /// <inheritdoc />
        public Task PlayAlbumCollectionAsync()
        {
            return _playbackHandler.PlayAsync((IAlbumCollectionViewModel)this, _collectionGroup);
        }

        /// <inheritdoc />
        public Task PauseAlbumCollectionAsync()
        {
            return _playbackHandler.PauseAsync();
        }

        /// <inheritdoc />
        public Task PlayArtistCollectionAsync()
        {
            return _playbackHandler.PlayAsync((IArtistCollectionViewModel)this, _collectionGroup);
        }

        /// <inheritdoc />
        public Task PauseArtistCollectionAsync()
        {
            return _playbackHandler.PauseAsync();
        }

        /// <inheritdoc />
        public IAsyncRelayCommand<int> PopulateMoreTracksCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand PlayTrackCollectionAsyncCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand<ITrack> PlayTrackAsyncCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand PauseTrackCollectionAsyncCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand<int> PopulateMorePlaylistsCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand PlayPlaylistCollectionAsyncCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand<IPlaylistCollectionItem> PlayPlaylistAsyncCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand PausePlaylistCollectionAsyncCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand<int> PopulateMoreAlbumsCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand PlayAlbumCollectionAsyncCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand<IAlbumCollectionItem> PlayAlbumAsyncCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand PauseAlbumCollectionAsyncCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand<int> PopulateMoreArtistsCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand PlayArtistCollectionAsyncCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand<IArtistCollectionItem> PlayArtistAsyncCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand PauseArtistCollectionAsyncCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand<int> PopulateMoreChildrenCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand<int> PopulateMoreImagesCommand { get; }

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
        public bool Equals(ICoreImageCollection other) => _collectionGroup.Equals(other);

        /// <inheritdoc />
        public bool Equals(ICorePlaylistCollectionItem other) => _collectionGroup.Equals(other);

        /// <inheritdoc />
        public bool Equals(ICorePlaylistCollection other) => _collectionGroup.Equals(other);

        /// <inheritdoc />
        public bool Equals(ICoreTrackCollection other) => _collectionGroup.Equals(other);

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
        public async Task InitAsync()
        {
            if (IsInitialized)
                return;

            IsInitialized = true;

            await CollectionInit.TrackCollection(this);
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

            return _playbackHandler.PlayAsync((ITrack)playlistItem, this, this);
        }

        private Task PlayPlayableCollectionGroupInternalAsync(IPlayableCollectionGroup? collectionGroup)
        {
            Guard.IsNotNull(collectionGroup, nameof(collectionGroup));

            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public bool IsInitialized { get; private set; }

        /// <inheritdoc />
        public ValueTask DisposeAsync()
        {
            DetachPropertyEvents();
            return _collectionGroup.DisposeAsync();
        }
    }
}
