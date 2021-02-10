using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Toolkit.Diagnostics;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using OwlCore;
using OwlCore.Events;
using OwlCore.Extensions;
using StrixMusic.Sdk.Data;
using StrixMusic.Sdk.Data.Base;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.Data.Merged;
using StrixMusic.Sdk.Extensions;
using StrixMusic.Sdk.MediaPlayback;

namespace StrixMusic.Sdk.ViewModels
{
    /// <summary>
    /// An observable wrapper for a <see cref="IPlayableCollectionGroupBase"/>.
    /// </summary>
    public class PlayableCollectionGroupViewModel : ObservableObject, IPlayableCollectionGroup, IPlayableCollectionGroupChildrenViewModel, IAlbumCollectionViewModel, IArtistCollectionViewModel, ITrackCollectionViewModel, IPlaylistCollectionViewModel, IImageCollectionViewModel
    {
        private readonly IPlayableCollectionGroup _collectionGroup;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayableCollectionGroupViewModel"/> class.
        /// </summary>
        /// <param name="collectionGroup">The base <see cref="IPlayableCollectionGroup"/> containing properties about this class.</param>
        public PlayableCollectionGroupViewModel(IPlayableCollectionGroup collectionGroup)
        {
            _collectionGroup = collectionGroup ?? throw new ArgumentNullException(nameof(collectionGroup));

            SourceCores = _collectionGroup.GetSourceCores<ICorePlayableCollectionGroup>().Select(MainViewModel.GetLoadedCore).ToList();

            PauseAsyncCommand = new AsyncRelayCommand(PauseAsync);
            PlayAsyncCommand = new AsyncRelayCommand(PlayAsync);
            ChangeNameAsyncCommand = new AsyncRelayCommand<string>(ChangeNameAsync);
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

            IsPlayAsyncAvailableChanged += OnIsPlayAsyncAvailableChanged;
            IsPauseAsyncAvailableChanged += OnIsPauseAsyncAvailableChanged;
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

            IsPlayAsyncAvailableChanged -= OnIsPlayAsyncAvailableChanged;
            IsPauseAsyncAvailableChanged -= OnIsPauseAsyncAvailableChanged;
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
        public event EventHandler<bool>? IsPlayAsyncAvailableChanged
        {
            add => _collectionGroup.IsPlayAsyncAvailableChanged += value;
            remove => _collectionGroup.IsPlayAsyncAvailableChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<bool>? IsPauseAsyncAvailableChanged
        {
            add => _collectionGroup.IsPauseAsyncAvailableChanged += value;
            remove => _collectionGroup.IsPauseAsyncAvailableChanged -= value;
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
        public event EventHandler<int>? TrackItemsCountChanged
        {
            add => _collectionGroup.TrackItemsCountChanged += value;
            remove => _collectionGroup.TrackItemsCountChanged -= value;
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

        private void CollectionGroupUrlChanged(object sender, Uri? e) => OnPropertyChanged(nameof(Url));

        private void CollectionGroupNameChanged(object sender, string e) => OnPropertyChanged(nameof(Name));

        private void CollectionGroupDescriptionChanged(object sender, string? e) => OnPropertyChanged(nameof(Description));

        private void CollectionGroupPlaybackStateChanged(object sender, PlaybackState e) => OnPropertyChanged(nameof(PlaybackState));

        private void CollectionGroupOnTotalChildrenCountChanged(object sender, int e) => OnPropertyChanged(nameof(TotalChildrenCount));

        private void CollectionGroupOnPlaylistItemsCountChanged(object sender, int e) => OnPropertyChanged(nameof(TotalPlaylistItemsCount));

        private void CollectionGroupOnArtistItemsCountChanged(object sender, int e) => OnPropertyChanged(nameof(TotalArtistItemsCount));

        private void CollectionGroupOnTrackItemsCountChanged(object sender, int e) => OnPropertyChanged(nameof(TotalTracksCount));

        private void CollectionGroupOnAlbumItemsCountChanged(object sender, int e) => OnPropertyChanged(nameof(TotalAlbumItemsCount));

        private void PlayableCollectionGroupViewModel_ImagesCountChanged(object sender, int e) => OnPropertyChanged(nameof(TotalImageCount));

        private void CollectionGroupLastPlayedChanged(object sender, DateTime? e) => OnPropertyChanged(nameof(LastPlayed));

        private void OnIsChangeDescriptionAsyncAvailableChanged(object sender, bool e) => OnPropertyChanged(nameof(IsChangeDescriptionAsyncAvailable));

        private void OnIsChangeDurationAsyncAvailableChanged(object sender, bool e) => OnPropertyChanged(nameof(IsChangeDurationAsyncAvailable));

        private void OnIsChangeNameAsyncAvailableChanged(object sender, bool e) => OnPropertyChanged(nameof(IsChangeNameAsyncAvailable));

        private void OnIsPauseAsyncAvailableChanged(object sender, bool e) => OnPropertyChanged(nameof(IsPauseAsyncAvailable));

        private void OnIsPlayAsyncAvailableChanged(object sender, bool e) => OnPropertyChanged(nameof(IsPlayAsyncAvailable));

        private void PlayableCollectionGroupViewModel_ImagesChanged(object sender, IReadOnlyList<CollectionChangedEventItem<IImage>> addedItems, IReadOnlyList<CollectionChangedEventItem<IImage>> removedItems)
        {
            foreach (var item in addedItems)
            {
                Images.InsertOrAdd(item.Index, item.Data);
            }

            foreach (var item in removedItems)
            {
                Guard.IsInRangeFor(item.Index, (IReadOnlyList<IImage>)Images, nameof(Images));
                Images.RemoveAt(item.Index);
            }
        }

        private void PlayableCollectionGroupViewModel_ChildItemsChanged(object sender, IReadOnlyList<CollectionChangedEventItem<IPlayableCollectionGroup>> addedItems, IReadOnlyList<CollectionChangedEventItem<IPlayableCollectionGroup>> removedItems)
        {
            foreach (var item in addedItems)
            {
                Children.InsertOrAdd(item.Index, new PlayableCollectionGroupViewModel(item.Data));
            }

            foreach (var item in removedItems)
            {
                Guard.IsInRangeFor(item.Index, (IReadOnlyList<IPlayableCollectionGroup>)Children, nameof(Children));
                Tracks.RemoveAt(item.Index);
            }
        }

        private void PlayableCollectionGroupViewModel_PlaylistItemsChanged(object sender, IReadOnlyList<CollectionChangedEventItem<IPlaylistCollectionItem>> addedItems, IReadOnlyList<CollectionChangedEventItem<IPlaylistCollectionItem>> removedItems)
        {
            foreach (var item in addedItems)
            {
                switch (item.Data)
                {
                    case IPlaylist playlist:
                        Playlists.InsertOrAdd(item.Index, new PlaylistViewModel(playlist));
                        break;
                    case IPlaylistCollection collection:
                        Playlists.InsertOrAdd(item.Index, new PlaylistCollectionViewModel(collection));
                        break;
                    default:
                        ThrowHelper.ThrowNotSupportedException($"{item.Data.GetType()} not supported for adding to {GetType()}");
                        break;
                }
            }

            foreach (var item in removedItems)
            {
                Guard.IsInRangeFor(item.Index, (IReadOnlyList<IPlaylistCollectionItem>)Playlists, nameof(Playlists));
                Playlists.RemoveAt(item.Index);
            }
        }

        private void PlayableCollectionGroupViewModel_ArtistItemsChanged(object sender, IReadOnlyList<CollectionChangedEventItem<IArtistCollectionItem>> addedItems, IReadOnlyList<CollectionChangedEventItem<IArtistCollectionItem>> removedItems)
        {
            foreach (var item in addedItems)
            {
                switch (item.Data)
                {
                    case IArtist artist:
                        Artists.InsertOrAdd(item.Index, new ArtistViewModel(artist));
                        break;
                    case IArtistCollection collection:
                        Artists.InsertOrAdd(item.Index, new ArtistCollectionViewModel(collection));
                        break;
                    default:
                        ThrowHelper.ThrowNotSupportedException($"{item.Data.GetType()} not supported for adding to {GetType()}");
                        break;
                }
            }

            foreach (var item in removedItems)
            {
                Guard.IsInRangeFor(item.Index, (IReadOnlyList<IArtistCollectionItem>)Artists, nameof(Artists));
                Artists.RemoveAt(item.Index);
            }
        }

        private void PlayableCollectionGroupViewModel_TrackItemsChanged(object sender, IReadOnlyList<CollectionChangedEventItem<ITrack>> addedItems, IReadOnlyList<CollectionChangedEventItem<ITrack>> removedItems)
        {
            foreach (var item in addedItems)
            {
                Tracks.InsertOrAdd(item.Index, new TrackViewModel(item.Data));
            }

            foreach (var item in removedItems)
            {
                Guard.IsInRangeFor(item.Index, (IReadOnlyList<ITrack>)Tracks, nameof(Tracks));
                Tracks.RemoveAt(item.Index);
            }
        }

        private void PlayableCollectionGroupViewModel_AlbumItemsChanged(object sender, IReadOnlyList<CollectionChangedEventItem<IAlbumCollectionItem>> addedItems, IReadOnlyList<CollectionChangedEventItem<IAlbumCollectionItem>> removedItems)
        {
            foreach (var item in addedItems)
            {
                switch (item.Data)
                {
                    case IAlbum album:
                        Albums.InsertOrAdd(item.Index, new AlbumViewModel(album));
                        break;
                    case IAlbumCollection collection:
                        Albums.InsertOrAdd(item.Index, new AlbumCollectionViewModel(collection));
                        break;
                    default:
                        ThrowHelper.ThrowNotSupportedException($"{item.Data.GetType()} not supported for adding to {GetType()}");
                        break;
                }
            }

            foreach (var item in removedItems)
            {
                Guard.IsInRangeFor(item.Index, (IReadOnlyList<IAlbumCollectionItem>)Albums, nameof(Albums));
                Albums.RemoveAt(item.Index);
            }
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
        public int TotalTracksCount => _collectionGroup.TotalTracksCount;

        /// <inheritdoc />
        public int TotalAlbumItemsCount => _collectionGroup.TotalAlbumItemsCount;

        /// <inheritdoc />
        public int TotalArtistItemsCount => _collectionGroup.TotalArtistItemsCount;

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
        public bool IsPlayAsyncAvailable => _collectionGroup.IsPlayAsyncAvailable;

        /// <inheritdoc />
        public bool IsPauseAsyncAvailable => _collectionGroup.IsPauseAsyncAvailable;

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
        public Task<bool> IsRemoveTrackAvailable(int index) => _collectionGroup.IsRemoveTrackAvailable(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveArtistItemAvailable(int index) => _collectionGroup.IsRemoveArtistItemAvailable(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveAlbumItemAvailable(int index) => _collectionGroup.IsRemoveAlbumItemAvailable(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveChildAvailable(int index) => _collectionGroup.IsRemoveChildAvailable(index);

        /// <inheritdoc />
        public Task PauseAsync() => _collectionGroup.PauseAsync();

        /// <inheritdoc />
        public Task PlayAsync() => _collectionGroup.PlayAsync();

        /// <inheritdoc />
        public Task ChangeNameAsync(string name) => _collectionGroup.ChangeNameAsync(name);

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
            foreach (var item in await Task.Run(() => _collectionGroup.GetPlaylistItemsAsync(limit, Playlists.Count)))
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
        }

        /// <inheritdoc />
        public async Task PopulateMoreTracksAsync(int limit)
        {
            foreach (var item in await Task.Run(() => _collectionGroup.GetTracksAsync(limit, Tracks.Count)))
            {
                Tracks.Add(new TrackViewModel(item));
            }
        }

        /// <inheritdoc />
        public async Task PopulateMoreAlbumsAsync(int limit)
        {
            foreach (var item in await Task.Run(() => _collectionGroup.GetAlbumItemsAsync(limit, Albums.Count)))
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
        }

        /// <inheritdoc />
        public async Task PopulateMoreArtistsAsync(int limit)
        {
            foreach (var item in await Task.Run(() => _collectionGroup.GetArtistItemsAsync(limit, Artists.Count)))
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
        }

        /// <inheritdoc />
        public async Task PopulateMoreChildrenAsync(int limit)
        {
            foreach (var item in await Task.Run(() => _collectionGroup.GetChildrenAsync(limit, Albums.Count)))
            {
                Children.Add(new PlayableCollectionGroupViewModel(item));
            }
        }

        /// <inheritdoc />
        public async Task PopulateMoreImagesAsync(int limit)
        {
            foreach (var item in await Task.Run(() => _collectionGroup.GetImagesAsync(limit, Images.Count)))
            {
                Images.Add(item);
            }
        }

        /// <summary>
        /// Attempts to play the album.
        /// </summary>
        public IAsyncRelayCommand PlayAsyncCommand { get; }

        /// <summary>
        /// Attempts to pause the album, if playing.
        /// </summary>
        public IAsyncRelayCommand PauseAsyncCommand { get; }

        /// <summary>
        /// Attempts to change the name of the album, if supported.
        /// </summary>
        public IAsyncRelayCommand ChangeNameAsyncCommand { get; }

        /// <summary>
        /// Attempts to change the description of the album, if supported.
        /// </summary>
        public IAsyncRelayCommand ChangeDescriptionAsyncCommand { get; }

        /// <summary>
        /// Attempts to change the duration of the album, if supported.
        /// </summary>
        public IAsyncRelayCommand ChangeDurationAsyncCommand { get; }

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
    }
}
