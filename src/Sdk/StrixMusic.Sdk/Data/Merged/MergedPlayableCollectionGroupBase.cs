using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Toolkit.Diagnostics;
using OwlCore.Events;
using OwlCore.Extensions;
using StrixMusic.Sdk.Data.Base;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.Extensions;
using StrixMusic.Sdk.MediaPlayback;

namespace StrixMusic.Sdk.Data.Merged
{
    /// <summary>
    /// A base that merges multiple <see cref="IPlayableCollectionGroupBase"/>s.
    /// </summary>
    public abstract class MergedPlayableCollectionGroupBase<TCoreBase> : IPlayableCollectionGroup, IMergedMutable<TCoreBase>
        where TCoreBase : class, ICorePlayableCollectionGroup
    {
        private readonly MergedCollectionMap<IAlbumCollection, ICoreAlbumCollection, IAlbumCollectionItem, ICoreAlbumCollectionItem> _albumCollectionMap;
        private readonly MergedCollectionMap<IArtistCollection, ICoreArtistCollection, IArtistCollectionItem, ICoreArtistCollectionItem> _artistCollectionMap;
        private readonly MergedCollectionMap<IPlaylistCollection, ICorePlaylistCollection, IPlaylistCollectionItem, ICorePlaylistCollectionItem> _playlistCollectionMap;
        private readonly MergedCollectionMap<ITrackCollection, ICoreTrackCollection, ITrack, ICoreTrack> _trackCollectionMap;
        private readonly MergedCollectionMap<IPlayableCollectionGroup, ICorePlayableCollectionGroup, IPlayableCollectionGroup, ICorePlayableCollectionGroup> _playableCollectionGroupMap;
        private readonly MergedCollectionMap<IImageCollection, ICoreImageCollection, IImage, ICoreImage> _imagesCollectionMap;
        private readonly List<ICore> _sourceCores;

        /// <summary>
        /// Initializes a new instance of the <see cref="MergedPlayableCollectionGroupBase{T}"/> class.
        /// </summary>
        /// <param name="sources">The search results to merge.</param>
        protected MergedPlayableCollectionGroupBase(IEnumerable<TCoreBase> sources)
        {
            // TODO: Use top Preferred core.
            if (sources is null)
                throw new ArgumentNullException(nameof(sources));

            StoredSources = sources.ToList();
            Guard.HasSizeGreaterThan(StoredSources, 0, nameof(StoredSources));

            PreferredSource = StoredSources[0];
            _sourceCores = StoredSources.Select(x => x.SourceCore).ToList();

            _albumCollectionMap = new MergedCollectionMap<IAlbumCollection, ICoreAlbumCollection, IAlbumCollectionItem, ICoreAlbumCollectionItem>(this);
            _artistCollectionMap = new MergedCollectionMap<IArtistCollection, ICoreArtistCollection, IArtistCollectionItem, ICoreArtistCollectionItem>(this);
            _playlistCollectionMap = new MergedCollectionMap<IPlaylistCollection, ICorePlaylistCollection, IPlaylistCollectionItem, ICorePlaylistCollectionItem>(this);
            _trackCollectionMap = new MergedCollectionMap<ITrackCollection, ICoreTrackCollection, ITrack, ICoreTrack>(this);
            _playableCollectionGroupMap = new MergedCollectionMap<IPlayableCollectionGroup, ICorePlayableCollectionGroup, IPlayableCollectionGroup, ICorePlayableCollectionGroup>(this);
            _imagesCollectionMap = new MergedCollectionMap<IImageCollection, ICoreImageCollection, IImage, ICoreImage>(this);

            AttachPropertyChangedEvents(PreferredSource);
            AttachCollectionChangedEvents();

            foreach (var item in StoredSources)
            {
                TotalChildrenCount += item.TotalChildrenCount;
                TotalPlaylistItemsCount += item.TotalPlaylistItemsCount;
                TotalTracksCount += item.TotalTracksCount;
                TotalAlbumItemsCount += item.TotalAlbumItemsCount;
                TotalArtistItemsCount += item.TotalArtistItemsCount;
                TotalImageCount += item.TotalImageCount;
                Duration += item.Duration;
            }

            Name = PreferredSource.Name;
            Url = PreferredSource.Url;
            Description = PreferredSource.Description;
            PlaybackState = PreferredSource.PlaybackState;
            LastPlayed = PreferredSource.LastPlayed;
            AddedAt = PreferredSource.AddedAt;
        }

        /// <summary>
        /// The top preferred source for this item, used for property getters.
        /// </summary>
        protected ICorePlayableCollectionGroup PreferredSource { get; }

        /// <summary>
        /// The source items that were merged to create this <see cref="MergedPlayableCollectionGroupBase{T}"/>
        /// </summary>
        protected List<TCoreBase> StoredSources { get; }

        private void AttachPropertyChangedEvents(ICorePlayableCollectionGroup source)
        {
            source.PlaybackStateChanged += PlaybackStateChanged;
            source.NameChanged += NameChanged;
            source.DescriptionChanged += DescriptionChanged;
            source.UrlChanged += UrlChanged;
            source.DurationChanged += DurationChanged;
            source.LastPlayedChanged += LastPlayedChanged;
            source.IsPlayTrackCollectionAsyncAvailableChanged += IsPlayTrackCollectionAsyncAvailableChanged;
            source.IsPauseTrackCollectionAsyncAvailableChanged += IsPauseTrackCollectionAsyncAvailableChanged;
            source.IsPlayArtistCollectionAsyncAvailableChanged += IsPlayArtistCollectionAsyncAvailableChanged;
            source.IsPauseArtistCollectionAsyncAvailableChanged += IsPauseArtistCollectionAsyncAvailableChanged;
            source.IsPlayAlbumCollectionAsyncAvailableChanged += IsPlayAlbumCollectionAsyncAvailableChanged;
            source.IsPauseAlbumCollectionAsyncAvailableChanged += IsPauseAlbumCollectionAsyncAvailableChanged;
            source.IsChangeNameAsyncAvailableChanged += IsChangeNameAsyncAvailableChanged;
            source.IsChangeDurationAsyncAvailableChanged += IsChangeDurationAsyncAvailableChanged;
            source.IsChangeDescriptionAsyncAvailableChanged += IsChangeDescriptionAsyncAvailableChanged;

            source.AlbumItemsCountChanged += AlbumItemsCountChanged;
            source.ArtistItemsCountChanged += ArtistItemsCountChanged;
            source.PlaylistItemsCountChanged += PlaylistItemsCountChanged;
            source.TrackItemsCountChanged += TrackItemsCountChanged;
            source.TotalChildrenCountChanged += TotalChildrenCountChanged;
        }

        private void DetachPropertyChangedEvents(ICorePlayableCollectionGroup source)
        {
            source.PlaybackStateChanged -= PlaybackStateChanged;
            source.NameChanged -= NameChanged;
            source.DescriptionChanged -= DescriptionChanged;
            source.UrlChanged -= UrlChanged;
            source.DurationChanged -= DurationChanged;
            source.LastPlayedChanged -= LastPlayedChanged;
            source.IsPlayTrackCollectionAsyncAvailableChanged -= IsPlayTrackCollectionAsyncAvailableChanged;
            source.IsPauseTrackCollectionAsyncAvailableChanged -= IsPauseTrackCollectionAsyncAvailableChanged;
            source.IsPlayArtistCollectionAsyncAvailableChanged -= IsPlayArtistCollectionAsyncAvailableChanged;
            source.IsPauseArtistCollectionAsyncAvailableChanged -= IsPauseArtistCollectionAsyncAvailableChanged;
            source.IsPlayAlbumCollectionAsyncAvailableChanged -= IsPlayAlbumCollectionAsyncAvailableChanged;
            source.IsPauseAlbumCollectionAsyncAvailableChanged -= IsPauseAlbumCollectionAsyncAvailableChanged;
            source.IsChangeNameAsyncAvailableChanged -= IsChangeNameAsyncAvailableChanged;
            source.IsChangeDurationAsyncAvailableChanged -= IsChangeDurationAsyncAvailableChanged;
            source.IsChangeDescriptionAsyncAvailableChanged -= IsChangeDescriptionAsyncAvailableChanged;

            source.AlbumItemsCountChanged -= AlbumItemsCountChanged;
            source.ArtistItemsCountChanged -= ArtistItemsCountChanged;
            source.PlaylistItemsCountChanged -= PlaylistItemsCountChanged;
            source.TrackItemsCountChanged -= TrackItemsCountChanged;
            source.TotalChildrenCountChanged -= TotalChildrenCountChanged;
        }

        private void AttachCollectionChangedEvents()
        {
            _albumCollectionMap.ItemsChanged += AlbumCollectionMap_ItemsChanged;
            _artistCollectionMap.ItemsChanged += ArtistCollectionMap_ItemsChanged;
            _playlistCollectionMap.ItemsChanged += PlaylistCollectionMap_ItemsChanged;
            _playableCollectionGroupMap.ItemsChanged += PlayableCollectionGroupMap_ItemsChanged;
            _trackCollectionMap.ItemsChanged += TrackCollectionMap_ItemsChanged;
            _imagesCollectionMap.ItemsChanged += ImagesCollectionMap_ItemsChanged;

            _albumCollectionMap.ItemsCountChanged += AlbumCollectionMap_ItemsCountChanged;
            _artistCollectionMap.ItemsCountChanged += ArtistCollectionMap_ItemsCountChanged;
            _trackCollectionMap.ItemsCountChanged += TrackCollectionMap_ItemsCountChanged;
            _playlistCollectionMap.ItemsCountChanged += PlaylistCollectionMap_ItemsCountChanged;
            _playableCollectionGroupMap.ItemsCountChanged += PlayableCollectionGroupMap_ItemsCountChanged;
            _imagesCollectionMap.ItemsCountChanged += ImagesCollectionMap_ItemsCountChanged;
        }

        private void DetachCollectionChangedEvents()
        {
            _albumCollectionMap.ItemsChanged -= AlbumCollectionMap_ItemsChanged;
            _artistCollectionMap.ItemsChanged -= ArtistCollectionMap_ItemsChanged;
            _playlistCollectionMap.ItemsChanged -= PlaylistCollectionMap_ItemsChanged;
            _playableCollectionGroupMap.ItemsChanged -= PlayableCollectionGroupMap_ItemsChanged;
            _trackCollectionMap.ItemsChanged -= TrackCollectionMap_ItemsChanged;
            _imagesCollectionMap.ItemsChanged -= ImagesCollectionMap_ItemsChanged;

            _albumCollectionMap.ItemsCountChanged -= AlbumCollectionMap_ItemsCountChanged;
            _artistCollectionMap.ItemsCountChanged -= ArtistCollectionMap_ItemsCountChanged;
            _playlistCollectionMap.ItemsCountChanged -= PlaylistCollectionMap_ItemsCountChanged;
            _playableCollectionGroupMap.ItemsCountChanged -= PlayableCollectionGroupMap_ItemsCountChanged;
            _imagesCollectionMap.ItemsCountChanged -= ImagesCollectionMap_ItemsCountChanged;
            _trackCollectionMap.ItemsCountChanged -= TrackCollectionMap_ItemsCountChanged;
        }

        /// <inheritdoc/>
        public event EventHandler<PlaybackState>? PlaybackStateChanged;

        /// <inheritdoc/>
        public event EventHandler<string>? NameChanged;

        /// <inheritdoc/>
        public event EventHandler<string?>? DescriptionChanged;

        /// <inheritdoc/>
        public event EventHandler<Uri?>? UrlChanged;

        /// <inheritdoc/>
        public event EventHandler<TimeSpan>? DurationChanged;

        /// <inheritdoc />
        public event EventHandler<DateTime?>? LastPlayedChanged;

        /// <inheritdoc />
        public event EventHandler<bool>? IsPlayAlbumCollectionAsyncAvailableChanged;

        /// <inheritdoc />
        public event EventHandler<bool>? IsPauseAlbumCollectionAsyncAvailableChanged;

        /// <inheritdoc />
        public event EventHandler<bool>? IsPlayArtistCollectionAsyncAvailableChanged;

        /// <inheritdoc />
        public event EventHandler<bool>? IsPauseArtistCollectionAsyncAvailableChanged;

        /// <inheritdoc />
        public event EventHandler<bool>? IsPlayPlaylistCollectionAsyncAvailableChanged;

        /// <inheritdoc />
        public event EventHandler<bool>? IsPausePlaylistCollectionAsyncAvailableChanged;

        /// <inheritdoc />
        public event EventHandler<bool>? IsPlayTrackCollectionAsyncAvailableChanged;

        /// <inheritdoc />
        public event EventHandler<bool>? IsPauseTrackCollectionAsyncAvailableChanged;

        /// <inheritdoc />
        public event EventHandler<bool>? IsChangeNameAsyncAvailableChanged;

        /// <inheritdoc />
        public event EventHandler<bool>? IsChangeDescriptionAsyncAvailableChanged;

        /// <inheritdoc />
        public event EventHandler<bool>? IsChangeDurationAsyncAvailableChanged;

        /// <inheritdoc />
        public event EventHandler<int>? TrackItemsCountChanged;

        /// <inheritdoc />
        public event EventHandler<int>? ArtistItemsCountChanged;

        /// <inheritdoc />
        public event EventHandler<int>? AlbumItemsCountChanged;

        /// <inheritdoc />
        public event EventHandler<int>? PlaylistItemsCountChanged;

        /// <inheritdoc />
        public event EventHandler<int>? TotalChildrenCountChanged;

        /// <inheritdoc />
        public event EventHandler<int>? ImagesCountChanged;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<ITrack>? TrackItemsChanged;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<IArtistCollectionItem>? ArtistItemsChanged;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<IAlbumCollectionItem>? AlbumItemsChanged;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<IPlaylistCollectionItem>? PlaylistItemsChanged;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<IPlayableCollectionGroup>? ChildItemsChanged;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<IImage>? ImagesChanged;

        private void ImagesCollectionMap_ItemsCountChanged(object sender, int e)
        {
            TotalImageCount = e;
            ImagesCountChanged?.Invoke(this, e);
        }

        private void PlayableCollectionGroupMap_ItemsCountChanged(object sender, int e)
        {
            TotalChildrenCount = e;
            TotalChildrenCountChanged?.Invoke(this, e);
        }

        private void PlaylistCollectionMap_ItemsCountChanged(object sender, int e)
        {
            TotalPlaylistItemsCount = e;
            PlaylistItemsCountChanged?.Invoke(this, e);
        }

        private void ArtistCollectionMap_ItemsCountChanged(object sender, int e)
        {
            TotalArtistItemsCount = e;
            ArtistItemsCountChanged?.Invoke(this, e);
        }

        private void AlbumCollectionMap_ItemsCountChanged(object sender, int e)
        {
            TotalAlbumItemsCount = e;
            AlbumItemsCountChanged?.Invoke(this, e);
        }

        private void TrackCollectionMap_ItemsCountChanged(object sender, int e)
        {
            TotalTracksCount = e;
            TrackItemsCountChanged?.Invoke(this, e);
        }

        private void ImagesCollectionMap_ItemsChanged(object sender, IReadOnlyList<CollectionChangedItem<IImage>> addedItems, IReadOnlyList<CollectionChangedItem<IImage>> removedItems)
        {
            ImagesChanged?.Invoke(this, addedItems, removedItems);
        }

        private void TrackCollectionMap_ItemsChanged(object sender, IReadOnlyList<CollectionChangedItem<ITrack>> addedItems, IReadOnlyList<CollectionChangedItem<ITrack>> removedItems)
        {
            TrackItemsChanged?.Invoke(this, addedItems, removedItems);
        }

        private void PlayableCollectionGroupMap_ItemsChanged(object sender, IReadOnlyList<CollectionChangedItem<IPlayableCollectionGroup>> addedItems, IReadOnlyList<CollectionChangedItem<IPlayableCollectionGroup>> removedItems)
        {
            ChildItemsChanged?.Invoke(this, addedItems, removedItems);
        }

        private void PlaylistCollectionMap_ItemsChanged(object sender, IReadOnlyList<CollectionChangedItem<IPlaylistCollectionItem>> addedItems, IReadOnlyList<CollectionChangedItem<IPlaylistCollectionItem>> removedItems)
        {
            PlaylistItemsChanged?.Invoke(this, addedItems, removedItems);
        }

        private void AlbumCollectionMap_ItemsChanged(object sender, IReadOnlyList<CollectionChangedItem<IAlbumCollectionItem>> addedItems, IReadOnlyList<CollectionChangedItem<IAlbumCollectionItem>> removedItems)
        {
            AlbumItemsChanged?.Invoke(this, addedItems, removedItems);
        }

        private void ArtistCollectionMap_ItemsChanged(object sender, IReadOnlyList<CollectionChangedItem<IArtistCollectionItem>> addedItems, IReadOnlyList<CollectionChangedItem<IArtistCollectionItem>> removedItems)
        {
            ArtistItemsChanged?.Invoke(this, addedItems, removedItems);
        }

        /// <inheritdoc cref="IMerged{T}.SourceCores"/>
        public IReadOnlyList<ICore> SourceCores => _sourceCores;

        /// <inheritdoc />
        IReadOnlyList<ICorePlayableCollectionGroupChildren> IMerged<ICorePlayableCollectionGroupChildren>.Sources => StoredSources;

        /// <inheritdoc />
        IReadOnlyList<ICoreArtistCollection> IMerged<ICoreArtistCollection>.Sources => StoredSources;

        /// <inheritdoc />
        IReadOnlyList<ICoreArtistCollectionItem> IMerged<ICoreArtistCollectionItem>.Sources => StoredSources;

        /// <inheritdoc />
        IReadOnlyList<ICoreAlbumCollection> IMerged<ICoreAlbumCollection>.Sources => StoredSources;

        /// <inheritdoc />
        IReadOnlyList<ICoreTrackCollection> IMerged<ICoreTrackCollection>.Sources => StoredSources;

        /// <inheritdoc />
        IReadOnlyList<ICorePlaylistCollection> IMerged<ICorePlaylistCollection>.Sources => StoredSources;

        /// <inheritdoc />
        IReadOnlyList<ICorePlaylistCollectionItem> IMerged<ICorePlaylistCollectionItem>.Sources => StoredSources;

        /// <inheritdoc />
        IReadOnlyList<ICoreImageCollection> IMerged<ICoreImageCollection>.Sources => StoredSources;

        /// <inheritdoc />
        IReadOnlyList<ICorePlayableCollectionGroup> IMerged<ICorePlayableCollectionGroup>.Sources => StoredSources;

        /// <inheritdoc />
        IReadOnlyList<ICoreAlbumCollectionItem> IMerged<ICoreAlbumCollectionItem>.Sources => StoredSources;

        /// <inheritdoc cref="IMerged{T}.Sources"/>
        public virtual IReadOnlyList<TCoreBase> Sources => StoredSources;

        /// <inheritdoc/>
        public string Id => PreferredSource.Id;

        /// <inheritdoc/>
        public Uri? Url { get; internal set; }

        /// <inheritdoc/>
        public string Name { get; internal set; }

        /// <inheritdoc/>
        public string? Description { get; internal set; }

        /// <inheritdoc/>
        public PlaybackState PlaybackState { get; internal set; }

        /// <inheritdoc/>
        public TimeSpan Duration { get; internal set; } = new TimeSpan(0);

        /// <inheritdoc />
        public DateTime? LastPlayed { get; internal set; }

        /// <inheritdoc />
        public DateTime? AddedAt { get; internal set; }

        /// <inheritdoc/>
        public int TotalChildrenCount { get; internal set; }

        /// <inheritdoc/>
        public int TotalPlaylistItemsCount { get; internal set; }

        /// <inheritdoc/>
        public int TotalTracksCount { get; internal set; }

        /// <inheritdoc/>
        public int TotalAlbumItemsCount { get; internal set; }

        /// <inheritdoc/>
        public int TotalArtistItemsCount { get; internal set; }

        /// <inheritdoc />
        public int TotalImageCount { get; internal set; }

        /// <inheritdoc/>
        public virtual bool IsPlayAlbumCollectionAsyncAvailable => PreferredSource.IsPlayAlbumCollectionAsyncAvailable;

        /// <inheritdoc/>
        public virtual bool IsPauseAlbumCollectionAsyncAvailable => PreferredSource.IsPauseAlbumCollectionAsyncAvailable;

        /// <inheritdoc/>
        public virtual bool IsPlayArtistCollectionAsyncAvailable => PreferredSource.IsPlayArtistCollectionAsyncAvailable;

        /// <inheritdoc/>
        public virtual bool IsPauseArtistCollectionAsyncAvailable => PreferredSource.IsPauseArtistCollectionAsyncAvailable;

        /// <inheritdoc/>
        public virtual bool IsPlayPlaylistCollectionAsyncAvailable => PreferredSource.IsPlayPlaylistCollectionAsyncAvailable;

        /// <inheritdoc/>
        public virtual bool IsPausePlaylistCollectionAsyncAvailable => PreferredSource.IsPausePlaylistCollectionAsyncAvailable;

        /// <inheritdoc/>
        public virtual bool IsPlayTrackCollectionAsyncAvailable => PreferredSource.IsPlayTrackCollectionAsyncAvailable;

        /// <inheritdoc/>
        public virtual bool IsPauseTrackCollectionAsyncAvailable => PreferredSource.IsPauseTrackCollectionAsyncAvailable;

        /// <inheritdoc/>
        public virtual bool IsChangeNameAsyncAvailable => PreferredSource.IsChangeNameAsyncAvailable;

        /// <inheritdoc/>
        public virtual bool IsChangeDescriptionAsyncAvailable => PreferredSource.IsChangeDescriptionAsyncAvailable;

        /// <inheritdoc/>
        public virtual bool IsChangeDurationAsyncAvailable => PreferredSource.IsChangeDurationAsyncAvailable;

        /// <inheritdoc />
        public Task<bool> IsAddTrackAvailable(int index) => _trackCollectionMap.IsAddItemAvailable(index);

        /// <inheritdoc />
        public Task<bool> IsAddAlbumItemAvailable(int index) => _albumCollectionMap.IsAddItemAvailable(index);

        /// <inheritdoc />
        public Task<bool> IsAddArtistItemAvailable(int index) => _artistCollectionMap.IsAddItemAvailable(index);

        /// <inheritdoc />
        public Task<bool> IsAddPlaylistItemAvailable(int index) => _playlistCollectionMap.IsAddItemAvailable(index);

        /// <inheritdoc />
        public Task<bool> IsAddChildAvailable(int index) => _playableCollectionGroupMap.IsAddItemAvailable(index);

        /// <inheritdoc />
        public Task<bool> IsAddImageAvailable(int index) => _imagesCollectionMap.IsAddItemAvailable(index);

        /// <inheritdoc/>
        public Task<bool> IsRemoveImageAvailable(int index) => _imagesCollectionMap.IsRemoveItemSupport(index);

        /// <inheritdoc/>
        public Task<bool> IsRemoveTrackAvailable(int index) => _trackCollectionMap.IsRemoveItemSupport(index);

        /// <inheritdoc/>
        public Task<bool> IsRemoveArtistItemAvailable(int index) => _artistCollectionMap.IsRemoveItemSupport(index);

        /// <inheritdoc/>
        public Task<bool> IsRemoveAlbumItemAvailable(int index) => _albumCollectionMap.IsRemoveItemSupport(index);

        /// <inheritdoc/>
        public Task<bool> IsRemovePlaylistItemAvailable(int index) => _playableCollectionGroupMap.IsRemoveItemSupport(index);

        /// <inheritdoc/>
        public Task<bool> IsRemoveChildAvailable(int index) => _playableCollectionGroupMap.IsRemoveItemSupport(index);

        /// <inheritdoc/>
        public Task PauseAlbumCollectionAsync() => PreferredSource.PauseAlbumCollectionAsync();

        /// <inheritdoc/>
        public Task PlayAlbumCollectionAsync() => PreferredSource.PlayAlbumCollectionAsync();

        /// <inheritdoc/>
        public Task PauseArtistCollectionAsync() => PreferredSource.PauseArtistCollectionAsync();

        /// <inheritdoc/>
        public Task PlayArtistCollectionAsync() => PreferredSource.PlayArtistCollectionAsync();

        /// <inheritdoc/>
        public Task PausePlaylistCollectionAsync() => PreferredSource.PausePlaylistCollectionAsync();

        /// <inheritdoc/>
        public Task PlayPlaylistCollectionAsync() => PreferredSource.PlayPlaylistCollectionAsync();

        /// <inheritdoc/>
        public Task PauseTrackCollectionAsync() => PreferredSource.PauseTrackCollectionAsync();

        /// <inheritdoc/>
        public Task PlayTrackCollectionAsync() => PreferredSource.PlayTrackCollectionAsync();

        /// <inheritdoc />
        public Task PlayPlayableCollectionGroupAsync() => PreferredSource.PlayPlayableCollectionGroupAsync();

        /// <inheritdoc />
        public Task PausePlayableCollectionGroupAsync() => PreferredSource.PausePlayableCollectionGroupAsync();

        /// <inheritdoc />
        public Task PlayTrackCollectionAsync(ITrack track)
        {
            var targetCore = PreferredSource.SourceCore;
            var source = track.GetSources<ICoreTrack>().FirstOrDefault(x => x.SourceCore.InstanceId == targetCore.InstanceId);

            Guard.IsNotNull(source, nameof(source));

            return PreferredSource.PlayTrackCollectionAsync(source);
        }

        /// <inheritdoc />
        public Task PlayArtistCollectionAsync(IArtist artist)
        {
            var targetCore = PreferredSource.SourceCore;
            var source = artist.GetSources<ICoreArtist>().FirstOrDefault(x => x.SourceCore.InstanceId == targetCore.InstanceId);

            Guard.IsNotNull(source, nameof(source));

            return PreferredSource.PlayArtistCollectionAsync(source);
        }

        /// <inheritdoc />
        public Task PlayAlbumCollectionAsync(IAlbum album)
        {
            var targetCore = PreferredSource.SourceCore;
            var source = album.GetSources<ICoreAlbum>().FirstOrDefault(x => x.SourceCore.InstanceId == targetCore.InstanceId);

            Guard.IsNotNull(source, nameof(source));

            return PreferredSource.PlayAlbumCollectionAsync(source);
        }

        /// <inheritdoc />
        public Task PlayPlayableCollectionGroupAsync(IPlayableCollectionGroup collectionGroup)
        {
            var targetCore = PreferredSource.SourceCore;
            var source = collectionGroup.GetSources<ICorePlayableCollectionGroup>().FirstOrDefault(x => x.SourceCore.InstanceId == targetCore.InstanceId);

            Guard.IsNotNull(source, nameof(source));

            return PreferredSource.PlayPlayableCollectionGroupAsync(source);
        }

        /// <inheritdoc />
        public Task PlayPlaylistCollectionAsync(IPlaylist playlist)
        {
            var targetCore = PreferredSource.SourceCore;
            var source = playlist.GetSources<ICorePlaylist>().FirstOrDefault(x => x.SourceCore.InstanceId == targetCore.InstanceId);

            Guard.IsNotNull(source, nameof(source));

            return PreferredSource.PlayPlaylistCollectionAsync(source);
        }

        /// <inheritdoc/>
        public Task<IReadOnlyList<IAlbumCollectionItem>> GetAlbumItemsAsync(int limit, int offset)
        {
            return _albumCollectionMap.GetItems(limit, offset);
        }

        /// <inheritdoc/>
        public Task<IReadOnlyList<IArtistCollectionItem>> GetArtistItemsAsync(int limit, int offset)
        {
            return _artistCollectionMap.GetItems(limit, offset);
        }

        /// <inheritdoc/>
        public Task<IReadOnlyList<IPlayableCollectionGroup>> GetChildrenAsync(int limit, int offset = 0)
        {
            return _playableCollectionGroupMap.GetItems(limit, offset);
        }

        /// <inheritdoc/>
        public Task<IReadOnlyList<IPlaylistCollectionItem>> GetPlaylistItemsAsync(int limit, int offset)
        {
            return _playlistCollectionMap.GetItems(limit, offset);
        }

        /// <inheritdoc/>
        public Task<IReadOnlyList<ITrack>> GetTracksAsync(int limit, int offset = 0)
        {
            return _trackCollectionMap.GetItems(limit, offset);
        }

        /// <inheritdoc />
        public Task<IReadOnlyList<IImage>> GetImagesAsync(int limit, int offset)
        {
            return _imagesCollectionMap.GetItems(limit, offset);
        }

        /// <inheritdoc/>
        public Task ChangeNameAsync(string name)
        {
            return Sources.InParallel(source => source.IsChangeNameAsyncAvailable ? source.ChangeNameAsync(name) : Task.CompletedTask);
        }

        /// <inheritdoc/>
        public Task ChangeDescriptionAsync(string? description)
        {
            return Sources.InParallel(source => source.IsChangeDescriptionAsyncAvailable ? source.ChangeDescriptionAsync(description) : Task.CompletedTask);
        }

        /// <inheritdoc/>
        public Task ChangeDurationAsync(TimeSpan duration)
        {
            return Sources.InParallel(source => source.IsChangeDurationAsyncAvailable ? source.ChangeDurationAsync(duration) : Task.CompletedTask);
        }

        /// <inheritdoc/>
        public Task AddTrackAsync(ITrack track, int index)
        {
            return _trackCollectionMap.InsertItem(track, index);
        }

        /// <inheritdoc/>
        public Task AddArtistItemAsync(IArtistCollectionItem artist, int index)
        {
            return _artistCollectionMap.InsertItem(artist, index);
        }

        /// <inheritdoc/>
        public Task AddAlbumItemAsync(IAlbumCollectionItem album, int index)
        {
            return _albumCollectionMap.InsertItem(album, index);
        }

        /// <inheritdoc/>
        public Task AddPlaylistItemAsync(IPlaylistCollectionItem playlist, int index)
        {
            return _playlistCollectionMap.InsertItem(playlist, index);
        }

        /// <inheritdoc/>
        public Task AddChildAsync(IPlayableCollectionGroup child, int index)
        {
            return _playableCollectionGroupMap.InsertItem(child, index);
        }

        /// <inheritdoc />
        public Task AddImageAsync(IImage image, int index)
        {
            return _imagesCollectionMap.InsertItem(image, index);
        }

        /// <inheritdoc/>
        public Task RemoveTrackAsync(int index)
        {
            return _trackCollectionMap.RemoveAt(index);
        }

        /// <inheritdoc/>
        public Task RemoveArtistItemAsync(int index)
        {
            return _artistCollectionMap.RemoveAt(index);
        }

        /// <inheritdoc/>
        public Task RemoveAlbumItemAsync(int index)
        {
            return _albumCollectionMap.RemoveAt(index);
        }

        /// <inheritdoc/>
        public Task RemovePlaylistItemAsync(int index)
        {
            return _playlistCollectionMap.RemoveAt(index);
        }

        /// <inheritdoc />
        public Task RemoveImageAsync(int index)
        {
            return _imagesCollectionMap.RemoveAt(index);
        }

        /// <inheritdoc/>
        public Task RemoveChildAsync(int index)
        {
            return _playableCollectionGroupMap.RemoveAt(index);
        }

        /// <inheritdoc />
        void IMergedMutable<TCoreBase>.AddSource(TCoreBase itemToAdd)
        {
            Guard.IsNotNull(itemToAdd, nameof(itemToAdd));

            if (!Equals(itemToAdd))
                ThrowHelper.ThrowArgumentException<TCoreBase>("Tried to merge an artist that doesn't match. Verify that the item matches before merging the source.");

            StoredSources.Add(itemToAdd);
            _sourceCores.Add(itemToAdd.SourceCore);

            _imagesCollectionMap.Cast<IMergedMutable<ICoreImageCollection>>().AddSource(itemToAdd);
            _albumCollectionMap.Cast<IMergedMutable<ICoreAlbumCollection>>().AddSource(itemToAdd);
            _artistCollectionMap.Cast<IMergedMutable<ICoreArtistCollection>>().AddSource(itemToAdd);
            _playableCollectionGroupMap.Cast<IMergedMutable<ICorePlayableCollectionGroup>>().AddSource(itemToAdd);
            _playlistCollectionMap.Cast<IMergedMutable<ICorePlaylistCollection>>().AddSource(itemToAdd);
            _trackCollectionMap.Cast<IMergedMutable<ICoreTrackCollection>>().AddSource(itemToAdd);
        }

        /// <inheritdoc />
        void IMergedMutable<TCoreBase>.RemoveSource(TCoreBase itemToRemove)
        {
            Guard.IsNotNull(itemToRemove, nameof(itemToRemove));

            StoredSources.Remove(itemToRemove);
            _sourceCores.Remove(itemToRemove.SourceCore);

            _imagesCollectionMap.Cast<IMergedMutable<ICoreImageCollection>>().RemoveSource(itemToRemove);
            _albumCollectionMap.Cast<IMergedMutable<ICoreAlbumCollection>>().RemoveSource(itemToRemove);
            _artistCollectionMap.Cast<IMergedMutable<ICoreArtistCollection>>().RemoveSource(itemToRemove);
            _trackCollectionMap.Cast<IMergedMutable<ICoreTrackCollection>>().RemoveSource(itemToRemove);
            _playableCollectionGroupMap.Cast<IMergedMutable<ICorePlayableCollectionGroup>>().RemoveSource(itemToRemove);
            _playlistCollectionMap.Cast<IMergedMutable<ICorePlaylistCollection>>().RemoveSource(itemToRemove);
        }

        /// <inheritdoc />
        public bool Equals(ICoreImageCollection other) => Equals(other as ICorePlayableCollectionGroup);

        /// <inheritdoc />
        public bool Equals(ICorePlaylistCollectionItem other) => Equals(other as ICorePlayableCollectionGroup);

        /// <inheritdoc />
        public bool Equals(ICorePlaylistCollection other) => Equals(other as ICorePlayableCollectionGroup);

        /// <inheritdoc />
        public bool Equals(ICoreTrackCollection other) => Equals(other as ICorePlayableCollectionGroup);

        /// <inheritdoc />
        public bool Equals(ICoreAlbumCollectionItem other) => Equals(other as ICorePlayableCollectionGroup);

        /// <inheritdoc />
        public bool Equals(ICoreAlbumCollection other) => Equals(other as ICorePlayableCollectionGroup);

        /// <inheritdoc />
        public bool Equals(ICoreArtistCollectionItem other) => Equals(other as ICorePlayableCollectionGroup);

        /// <inheritdoc />
        public bool Equals(ICoreArtistCollection other) => Equals(other as ICorePlayableCollectionGroup);

        /// <inheritdoc />
        public bool Equals(ICorePlayableCollectionGroupChildren other) => Equals(other as ICorePlayableCollectionGroup);

        /// <inheritdoc />
        public bool Equals(ICorePlayableCollectionGroup? other)
        {
            return Equals(other as TCoreBase);
        }

        /// <summary>
        /// Overrides the default equality comparer.
        /// </summary>
        /// <param name="other">The object to compare.</param>
        /// <returns>True if this is a match, otherwise false.</returns>
        public virtual bool Equals(TCoreBase? other)
        {
            return other != null && other.Name.Equals(Name, StringComparison.InvariantCulture);
        }
    }
}
