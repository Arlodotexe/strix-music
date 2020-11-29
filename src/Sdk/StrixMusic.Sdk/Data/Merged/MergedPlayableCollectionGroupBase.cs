using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Toolkit.Diagnostics;
using OwlCore.Events;
using OwlCore.Extensions;
using StrixMusic.Sdk.Data.Base;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.MediaPlayback;

namespace StrixMusic.Sdk.Data.Merged
{
    /// <summary>
    /// A base that merges multiple <see cref="IPlayableCollectionGroupBase"/>s.
    /// </summary>
    public abstract class MergedPlayableCollectionGroupBase<TCoreBase> : IPlayableCollectionGroup
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
            TotalArtistItemsCount = 0;
            AlbumItemsCountChanged?.Invoke(this, e);
        }

        private void AlbumCollectionMap_ItemsCountChanged(object sender, int e)
        {
            TotalAlbumItemsCount = 0;
            AlbumItemsCountChanged?.Invoke(this, e);
        }

        private void ImagesCollectionMap_ItemsChanged(object sender, IReadOnlyList<CollectionChangedEventItem<IImage>> addedItems, IReadOnlyList<CollectionChangedEventItem<IImage>> removedItems)
        {
            ImagesChanged?.Invoke(this, addedItems, removedItems);
        }

        private void TrackCollectionMap_ItemsChanged(object sender, IReadOnlyList<CollectionChangedEventItem<ITrack>> addedItems, IReadOnlyList<CollectionChangedEventItem<ITrack>> removedItems)
        {
            TrackItemsChanged?.Invoke(this, addedItems, removedItems);
        }

        private void PlayableCollectionGroupMap_ItemsChanged(object sender, IReadOnlyList<CollectionChangedEventItem<IPlayableCollectionGroup>> addedItems, IReadOnlyList<CollectionChangedEventItem<IPlayableCollectionGroup>> removedItems)
        {
            ChildItemsChanged?.Invoke(this, addedItems, removedItems);
        }

        private void PlaylistCollectionMap_ItemsChanged(object sender, IReadOnlyList<CollectionChangedEventItem<IPlaylistCollectionItem>> addedItems, IReadOnlyList<CollectionChangedEventItem<IPlaylistCollectionItem>> removedItems)
        {
            PlaylistItemsChanged?.Invoke(this, addedItems, removedItems);
        }

        private void AlbumCollectionMap_ItemsChanged(object sender, IReadOnlyList<CollectionChangedEventItem<IAlbumCollectionItem>> addedItems, IReadOnlyList<CollectionChangedEventItem<IAlbumCollectionItem>> removedItems)
        {
            AlbumItemsChanged?.Invoke(this, addedItems, removedItems);
        }

        private void ArtistCollectionMap_ItemsChanged(object sender, IReadOnlyList<CollectionChangedEventItem<IArtistCollectionItem>> addedItems, IReadOnlyList<CollectionChangedEventItem<IArtistCollectionItem>> removedItems)
        {
            ArtistItemsChanged?.Invoke(this, addedItems, removedItems);
        }

        /// <inheritdoc cref="ISdkMember{T}.SourceCores"/>
        public IReadOnlyList<ICore> SourceCores => _sourceCores;

        /// <inheritdoc />
        IReadOnlyList<ICorePlayableCollectionGroupChildren> ISdkMember<ICorePlayableCollectionGroupChildren>.Sources => StoredSources;

        /// <inheritdoc />
        IReadOnlyList<ICoreArtistCollection> ISdkMember<ICoreArtistCollection>.Sources => StoredSources;

        /// <inheritdoc />
        IReadOnlyList<ICoreArtistCollectionItem> ISdkMember<ICoreArtistCollectionItem>.Sources => StoredSources;

        /// <inheritdoc />
        IReadOnlyList<ICoreAlbumCollection> ISdkMember<ICoreAlbumCollection>.Sources => StoredSources;

        /// <inheritdoc />
        IReadOnlyList<ICoreTrackCollection> ISdkMember<ICoreTrackCollection>.Sources => StoredSources;

        /// <inheritdoc />
        IReadOnlyList<ICorePlaylistCollection> ISdkMember<ICorePlaylistCollection>.Sources => StoredSources;

        /// <inheritdoc />
        IReadOnlyList<ICorePlaylistCollectionItem> ISdkMember<ICorePlaylistCollectionItem>.Sources => StoredSources;

        /// <inheritdoc />
        IReadOnlyList<ICoreImageCollection> ISdkMember<ICoreImageCollection>.Sources => StoredSources;

        /// <inheritdoc />
        IReadOnlyList<ICorePlayableCollectionGroup> ISdkMember<ICorePlayableCollectionGroup>.Sources => StoredSources;

        /// <inheritdoc />
        IReadOnlyList<ICoreAlbumCollectionItem> ISdkMember<ICoreAlbumCollectionItem>.Sources => StoredSources;

        /// <inheritdoc cref="ISdkMember{T}.Sources"/>
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
        public virtual bool IsPlayAsyncSupported => PreferredSource.IsPlayAsyncSupported;

        /// <inheritdoc/>
        public virtual bool IsPauseAsyncSupported => PreferredSource.IsPauseAsyncSupported;

        /// <inheritdoc/>
        public virtual bool IsChangeNameAsyncSupported => PreferredSource.IsChangeNameAsyncSupported;

        /// <inheritdoc/>
        public virtual bool IsChangeDescriptionAsyncSupported => PreferredSource.IsChangeDescriptionAsyncSupported;

        /// <inheritdoc/>
        public virtual bool IsChangeDurationAsyncSupported => PreferredSource.IsChangeDurationAsyncSupported;

        /// <inheritdoc />
        public Task<bool> IsAddTrackSupported(int index) => _trackCollectionMap.IsAddItemSupported(index);

        /// <inheritdoc />
        public Task<bool> IsAddAlbumItemSupported(int index) => _albumCollectionMap.IsAddItemSupported(index);

        /// <inheritdoc />
        public Task<bool> IsAddArtistSupported(int index) => _artistCollectionMap.IsAddItemSupported(index);

        /// <inheritdoc />
        public Task<bool> IsAddPlaylistItemSupported(int index) => _playlistCollectionMap.IsAddItemSupported(index);

        /// <inheritdoc />
        public Task<bool> IsAddChildSupported(int index) => _playableCollectionGroupMap.IsAddItemSupported(index);

        /// <inheritdoc />
        public Task<bool> IsAddImageSupported(int index) => _imagesCollectionMap.IsAddItemSupported(index);

        /// <inheritdoc/>
        public Task<bool> IsRemoveImageSupported(int index) => _imagesCollectionMap.IsRemoveItemSupport(index);

        /// <inheritdoc/>
        public Task<bool> IsRemoveTrackSupported(int index) => _trackCollectionMap.IsRemoveItemSupport(index);

        /// <inheritdoc/>
        public Task<bool> IsRemoveArtistSupported(int index) => _artistCollectionMap.IsRemoveItemSupport(index);

        /// <inheritdoc/>
        public Task<bool> IsRemoveAlbumItemSupported(int index) => _albumCollectionMap.IsRemoveItemSupport(index);

        /// <inheritdoc/>
        public Task<bool> IsRemovePlaylistItemSupported(int index) => _playableCollectionGroupMap.IsRemoveItemSupport(index);

        /// <inheritdoc/>
        public Task<bool> IsRemoveChildSupported(int index) => _playableCollectionGroupMap.IsRemoveItemSupport(index);

        /// <inheritdoc/>
        public Task PauseAsync() => PreferredSource.PauseAsync();

        /// <inheritdoc/>
        public Task PlayAsync() => PreferredSource.PlayAsync();

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
            return Sources.InParallel(source => source.IsChangeNameAsyncSupported ? source.ChangeNameAsync(name) : Task.CompletedTask);
        }

        /// <inheritdoc/>
        public Task ChangeDescriptionAsync(string? description)
        {
            return Sources.InParallel(source => source.IsChangeDescriptionAsyncSupported ? source.ChangeDescriptionAsync(description) : Task.CompletedTask);
        }

        /// <inheritdoc/>
        public Task ChangeDurationAsync(TimeSpan duration)
        {
            return Sources.InParallel(source => source.IsChangeDurationAsyncSupported ? source.ChangeDurationAsync(duration) : Task.CompletedTask);
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
        public Task RemoveArtistAsync(int index)
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

        /// <inheritdoc cref="IMerged{TCoreBase}.AddSource" />
        public void AddSource(TCoreBase itemToAdd)
        {
            Guard.IsNotNull(itemToAdd, nameof(itemToAdd));

            if (!Equals(itemToAdd))
                ThrowHelper.ThrowArgumentException<TCoreBase>("Tried to merge an artist that doesn't match. Verify that the item matches before merging the source.");

            StoredSources.Add(itemToAdd);
            _sourceCores.Add(itemToAdd.SourceCore);

            _albumCollectionMap.AddSource(itemToAdd);
            _artistCollectionMap.AddSource(itemToAdd);
            _playableCollectionGroupMap.AddSource(itemToAdd);
            _playlistCollectionMap.AddSource(itemToAdd);
            _trackCollectionMap.AddSource(itemToAdd);
        }

        /// <inheritdoc cref="IMerged{TCoreBase}.RemoveSource(TCoreBase)" />
        public void RemoveSource(TCoreBase itemToRemove)
        {
            Guard.IsNotNull(itemToRemove, nameof(itemToRemove));

            StoredSources.Remove(itemToRemove);
            _sourceCores.Remove(itemToRemove.SourceCore);

            _imagesCollectionMap.RemoveSource(itemToRemove);
            _albumCollectionMap.RemoveSource(itemToRemove);
            _artistCollectionMap.RemoveSource(itemToRemove);
            _trackCollectionMap.RemoveSource(itemToRemove);
            _playableCollectionGroupMap.RemoveSource(itemToRemove);
            _playlistCollectionMap.RemoveSource(itemToRemove);
        }
    }
}
