using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Toolkit.Diagnostics;
using OwlCore.Collections;
using OwlCore.Events;
using OwlCore.Extensions;
using StrixMusic.Sdk.Data.Base;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.Extensions;
using StrixMusic.Sdk.MediaPlayback;

namespace StrixMusic.Sdk.Data.Merged
{
    /// <summary>
    /// Merged multiple <see cref="ICoreArtist"/> into a single <see cref="IArtist"/>
    /// </summary>
    public class MergedArtist : IArtist, IMergedMutable<ICoreArtist>, IMergedMutable<ICoreArtistCollectionItem>
    {
        private readonly List<ICoreArtist> _sources;
        private readonly List<ICore> _sourceCores;
        private readonly MergedCollectionMap<ITrackCollection, ICoreTrackCollection, ITrack, ICoreTrack> _trackCollectionMap;
        private readonly MergedCollectionMap<IImageCollection, ICoreImageCollection, IImage, ICoreImage> _imageCollectionMap;
        private readonly MergedCollectionMap<IAlbumCollection, ICoreAlbumCollection, IAlbumCollectionItem, ICoreAlbumCollectionItem> _albumCollectionItemMap;
        private readonly ICoreArtist _preferredSource;

        /// <summary>
        /// Creates a new instance of <see cref="MergedArtist"/>.
        /// </summary>
        /// <param name="sources">The sources used</param>
        public MergedArtist(IEnumerable<ICoreArtist> sources)
        {
            _sources = sources.ToList();
            _sourceCores = _sources.Select(x => x.SourceCore).ToList();

            // TODO: Get the actual preferred source.
            _preferredSource = _sources[0];

            foreach (var source in _sources)
            {
                TotalAlbumItemsCount += source.TotalAlbumItemsCount;
                TotalImageCount += source.TotalImageCount;
                TotalTracksCount += source.TotalTracksCount;
            }

            Url = _preferredSource.Url;
            Name = _preferredSource.Name;
            Description = _preferredSource.Description;
            PlaybackState = _preferredSource.PlaybackState;
            Duration = _preferredSource.Duration;
            LastPlayed = _preferredSource.LastPlayed;
            AddedAt = _preferredSource.AddedAt;

            _trackCollectionMap = new MergedCollectionMap<ITrackCollection, ICoreTrackCollection, ITrack, ICoreTrack>(this);
            _imageCollectionMap = new MergedCollectionMap<IImageCollection, ICoreImageCollection, IImage, ICoreImage>(this);
            _albumCollectionItemMap = new MergedCollectionMap<IAlbumCollection, ICoreAlbumCollection, IAlbumCollectionItem, ICoreAlbumCollectionItem>(this);

            var relatedItems = _sources.Select(x => x.RelatedItems).PruneNull().ToList();

            if (relatedItems.Count > 0)
                RelatedItems = new MergedPlayableCollectionGroup(relatedItems);

            AttachEvents(_preferredSource);
        }

        private void AttachEvents(ICoreArtist source)
        {
            AttachPlayableEvents(source);

            source.IsPlayTrackCollectionAsyncAvailableChanged += IsPlayTrackCollectionAsyncAvailableChanged;
            source.IsPauseTrackCollectionAsyncAvailableChanged += IsPauseTrackCollectionAsyncAvailableChanged;
            source.IsPlayAlbumCollectionAsyncAvailableChanged += IsPlayAlbumCollectionAsyncAvailableChanged;
            source.IsPauseAlbumCollectionAsyncAvailableChanged += IsPauseAlbumCollectionAsyncAvailableChanged;

            _trackCollectionMap.ItemsChanged += TrackCollectionMap_ItemsChanged;
            _trackCollectionMap.ItemsCountChanged += TrackCollectionMap_ItemsCountChanged;
            _imageCollectionMap.ItemsChanged += ImageCollectionMap_ItemsChanged;
            _imageCollectionMap.ItemsCountChanged += ImageCollectionMap_ItemsCountChanged;
            _albumCollectionItemMap.ItemsChanged += AlbumCollectionItemsChanged;
            _albumCollectionItemMap.ItemsCountChanged += AlbumCollectionMap_ItemsCountChanged;
        }

        private void DetachEvents(ICoreArtist source)
        {
            DetachPlayableEvents(source);

            source.IsPlayTrackCollectionAsyncAvailableChanged -= IsPlayTrackCollectionAsyncAvailableChanged;
            source.IsPauseTrackCollectionAsyncAvailableChanged -= IsPauseTrackCollectionAsyncAvailableChanged;
            source.IsPlayAlbumCollectionAsyncAvailableChanged += IsPlayAlbumCollectionAsyncAvailableChanged;
            source.IsPauseAlbumCollectionAsyncAvailableChanged += IsPauseAlbumCollectionAsyncAvailableChanged;

            _trackCollectionMap.ItemsChanged -= TrackCollectionMap_ItemsChanged;
            _trackCollectionMap.ItemsCountChanged -= TrackCollectionMap_ItemsCountChanged;
            _imageCollectionMap.ItemsChanged -= ImageCollectionMap_ItemsChanged;
            _imageCollectionMap.ItemsCountChanged -= ImageCollectionMap_ItemsCountChanged;
            _albumCollectionItemMap.ItemsChanged -= AlbumCollectionItemsChanged;
        }

        private void AttachPlayableEvents(IPlayableBase source)
        {
            source.PlaybackStateChanged += PlaybackStateChanged;
            source.NameChanged += NameChanged;
            source.DescriptionChanged += DescriptionChanged;
            source.UrlChanged += UrlChanged;
            source.DurationChanged += DurationChanged;
            source.LastPlayedChanged += LastPlayedChanged;
            source.IsChangeNameAsyncAvailableChanged += IsChangeNameAsyncAvailableChanged;
            source.IsChangeDurationAsyncAvailableChanged += IsChangeDurationAsyncAvailableChanged;
            source.IsChangeDescriptionAsyncAvailableChanged += IsChangeDescriptionAsyncAvailableChanged;
        }

        private void DetachPlayableEvents(IPlayableBase source)
        {
            source.PlaybackStateChanged -= PlaybackStateChanged;
            source.NameChanged -= NameChanged;
            source.DescriptionChanged -= DescriptionChanged;
            source.UrlChanged -= UrlChanged;
            source.DurationChanged -= DurationChanged;
            source.LastPlayedChanged -= LastPlayedChanged;
            source.IsChangeNameAsyncAvailableChanged -= IsChangeNameAsyncAvailableChanged;
            source.IsChangeDurationAsyncAvailableChanged -= IsChangeDurationAsyncAvailableChanged;
            source.IsChangeDescriptionAsyncAvailableChanged -= IsChangeDescriptionAsyncAvailableChanged;
        }

        private void AlbumCollectionMap_ItemsCountChanged(object sender, int e)
        {
            TotalAlbumItemsCount = e;
            AlbumItemsCountChanged?.Invoke(this, e);
        }

        private void ImageCollectionMap_ItemsCountChanged(object sender, int e)
        {
            TotalImageCount = e;
            ImagesCountChanged?.Invoke(this, e);
        }

        private void TrackCollectionMap_ItemsCountChanged(object sender, int e)
        {
            TotalTracksCount = e;
            TrackItemsCountChanged?.Invoke(this, e);
        }

        private void AlbumCollectionItemsChanged(object sender, IReadOnlyList<CollectionChangedItem<IAlbumCollectionItem>> addedItems, IReadOnlyList<CollectionChangedItem<IAlbumCollectionItem>> removedItems)
        {
            AlbumItemsChanged?.Invoke(this, addedItems, removedItems);
        }

        private void ImageCollectionMap_ItemsChanged(object sender, IReadOnlyList<CollectionChangedItem<IImage>> addedItems, IReadOnlyList<CollectionChangedItem<IImage>> removedItems)
        {
            ImagesChanged?.Invoke(this, addedItems, removedItems);
        }

        private void TrackCollectionMap_ItemsChanged(object sender, IReadOnlyList<CollectionChangedItem<ITrack>> addedItems, IReadOnlyList<CollectionChangedItem<ITrack>> removedItems)
        {
            TrackItemsChanged?.Invoke(this, addedItems, removedItems);
        }

        /// <inheritdoc />
        public event EventHandler<int>? AlbumItemsCountChanged;

        /// <inheritdoc />
        public event EventHandler<int>? TrackItemsCountChanged;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<IAlbumCollectionItem>? AlbumItemsChanged;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<ITrack>? TrackItemsChanged;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<IImage>? ImagesChanged;

        /// <inheritdoc />
        public event EventHandler<int>? ImagesCountChanged;

        /// <inheritdoc />
        public event EventHandler<TimeSpan>? DurationChanged;

        /// <inheritdoc />
        public event EventHandler<DateTime?>? LastPlayedChanged;

        /// <inheritdoc />
        public event EventHandler<bool>? IsPlayTrackCollectionAsyncAvailableChanged;

        /// <inheritdoc />
        public event EventHandler<bool>? IsPauseTrackCollectionAsyncAvailableChanged;

        /// <inheritdoc />
        public event EventHandler<bool>? IsPlayAlbumCollectionAsyncAvailableChanged;

        /// <inheritdoc />
        public event EventHandler<bool>? IsPauseAlbumCollectionAsyncAvailableChanged;

        /// <inheritdoc />
        public event EventHandler<bool>? IsChangeNameAsyncAvailableChanged;

        /// <inheritdoc />
        public event EventHandler<bool>? IsChangeDescriptionAsyncAvailableChanged;

        /// <inheritdoc />
        public event EventHandler<bool>? IsChangeDurationAsyncAvailableChanged;

        /// <inheritdoc />
        public event EventHandler<string>? NameChanged;

        /// <inheritdoc />
        public event EventHandler<string?>? DescriptionChanged;

        /// <inheritdoc />
        public event EventHandler<PlaybackState>? PlaybackStateChanged;

        /// <inheritdoc />
        public event EventHandler<Uri?>? UrlChanged;

        /// <inheritdoc cref="IMerged{T}.SourceCores" />
        public IReadOnlyList<ICore> SourceCores => _sourceCores;

        /// <inheritdoc />
        IReadOnlyList<ICoreArtistCollectionItem> IMerged<ICoreArtistCollectionItem>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreGenreCollection> IMerged<ICoreGenreCollection>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreTrackCollection> IMerged<ICoreTrackCollection>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreAlbumCollection> IMerged<ICoreAlbumCollection>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreImageCollection> IMerged<ICoreImageCollection>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreAlbumCollectionItem> IMerged<ICoreAlbumCollectionItem>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreArtist> IMerged<ICoreArtist>.Sources => Sources;

        /// <summary>
        /// The merged sources for this <see cref="IArtist"/>
        /// </summary>
        public IReadOnlyList<ICoreArtist> Sources => _sources;

        /// <inheritdoc />
        public string Id => _preferredSource.Id;

        /// <inheritdoc />
        public Uri? Url { get; internal set; }

        /// <inheritdoc />
        public string Name { get; internal set; }

        /// <inheritdoc />
        public string? Description { get; internal set; }

        /// <inheritdoc />
        public PlaybackState PlaybackState { get; internal set; }

        /// <inheritdoc />
        public TimeSpan Duration { get; internal set; }

        /// <inheritdoc />
        public DateTime? LastPlayed { get; internal set; }

        /// <inheritdoc />
        public DateTime? AddedAt { get; internal set; }

        /// <inheritdoc />
        public int TotalTracksCount { get; internal set; }

        /// <inheritdoc />
        public int TotalAlbumItemsCount { get; internal set; }

        /// <inheritdoc />
        public int TotalImageCount { get; internal set; }

        /// <inheritdoc />
        public SynchronizedObservableCollection<string>? Genres => _preferredSource.Genres;

        /// <inheritdoc />
        public IPlayableCollectionGroup? RelatedItems { get; }

        /// <inheritdoc />
        public bool IsPlayTrackCollectionAsyncAvailable => _preferredSource.IsPlayTrackCollectionAsyncAvailable;

        /// <inheritdoc />
        public bool IsPauseTrackCollectionAsyncAvailable => _preferredSource.IsPauseTrackCollectionAsyncAvailable;

        /// <inheritdoc />
        public bool IsPlayAlbumCollectionAsyncAvailable => _preferredSource.IsPlayAlbumCollectionAsyncAvailable;

        /// <inheritdoc />
        public bool IsPauseAlbumCollectionAsyncAvailable => _preferredSource.IsPauseAlbumCollectionAsyncAvailable;

        /// <inheritdoc />
        public bool IsChangeNameAsyncAvailable => _preferredSource.IsChangeNameAsyncAvailable;

        /// <inheritdoc />
        public bool IsChangeDescriptionAsyncAvailable => _preferredSource.IsChangeDescriptionAsyncAvailable;

        /// <inheritdoc />
        public bool IsChangeDurationAsyncAvailable => _preferredSource.IsChangeDurationAsyncAvailable;

        /// <inheritdoc />
        public Task<bool> IsAddTrackAvailable(int index) => _trackCollectionMap.IsAddItemAvailable(index);

        /// <inheritdoc />
        public Task<bool> IsAddAlbumItemAvailable(int index) => _albumCollectionItemMap.IsAddItemAvailable(index);

        /// <inheritdoc />
        public Task<bool> IsAddImageAvailable(int index) => _imageCollectionMap.IsAddItemAvailable(index);

        /// <inheritdoc />
        public Task<bool> IsAddGenreAvailable(int index) => _preferredSource.IsAddGenreAvailable(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveTrackAvailable(int index) => _trackCollectionMap.IsRemoveItemSupport(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveAlbumItemAvailable(int index) => _albumCollectionItemMap.IsRemoveItemSupport(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveImageAvailable(int index) => _imageCollectionMap.IsRemoveItemSupport(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveGenreAvailable(int index) => _preferredSource.IsRemoveGenreAvailable(index);

        /// <inheritdoc />
        public Task<IReadOnlyList<IAlbumCollectionItem>> GetAlbumItemsAsync(int limit, int offset) => _albumCollectionItemMap.GetItems(limit, offset);

        /// <inheritdoc />
        public Task AddAlbumItemAsync(IAlbumCollectionItem album, int index) => _albumCollectionItemMap.InsertItem(album, index);

        /// <inheritdoc />
        public Task<IReadOnlyList<ITrack>> GetTracksAsync(int limit, int offset) => _trackCollectionMap.GetItems(limit, offset);

        /// <inheritdoc />
        public Task AddTrackAsync(ITrack track, int index) => _trackCollectionMap.InsertItem(track, index);

        /// <inheritdoc />
        public Task PlayTrackCollectionAsync() => _preferredSource.PlayTrackCollectionAsync();

        /// <inheritdoc />
        public Task PauseTrackCollectionAsync() => _preferredSource.PauseTrackCollectionAsync();

        /// <inheritdoc />
        public Task PlayAlbumCollectionAsync() => _preferredSource.PlayAlbumCollectionAsync();

        /// <inheritdoc />
        public Task PauseAlbumCollectionAsync() => _preferredSource.PauseAlbumCollectionAsync();

        /// <inheritdoc />
        public Task PlayTrackCollectionAsync(ITrack track)
        {
            var targetCore = _preferredSource.SourceCore;
            var source = track.GetSources<ICoreTrack>()
                .FirstOrDefault(x => x.SourceCore.InstanceId == targetCore.InstanceId);

            Guard.IsNotNull(source, nameof(source));

            return _preferredSource.PlayTrackCollectionAsync(source);
        }

        /// <inheritdoc />
        public Task PlayAlbumCollectionAsync(IAlbumCollectionItem albumItem)
        {
            var targetCore = _preferredSource.SourceCore;

            ICoreAlbumCollectionItem? source = null;

            if (albumItem is IAlbum album)
                source = album.GetSources<ICoreAlbum>().FirstOrDefault(x => x.SourceCore.InstanceId == targetCore.InstanceId);

            if (albumItem is IAlbumCollection collection)
                source = collection.GetSources<ICoreAlbumCollection>().FirstOrDefault(x => x.SourceCore.InstanceId == targetCore.InstanceId);

            Guard.IsNotNull(source, nameof(source));

            return _preferredSource.PlayAlbumCollectionAsync(source);
        }

        /// <inheritdoc />
        public Task ChangeNameAsync(string name) => _preferredSource.ChangeNameAsync(name);

        /// <inheritdoc />
        public Task<IReadOnlyList<IImage>> GetImagesAsync(int limit, int offset) => _imageCollectionMap.GetItems(limit, offset);

        /// <inheritdoc />
        public Task AddImageAsync(IImage image, int index) => _imageCollectionMap.InsertItem(image, index);

        /// <inheritdoc />
        public Task ChangeDescriptionAsync(string? description) => _preferredSource.ChangeDescriptionAsync(description);

        /// <inheritdoc />
        public Task ChangeDurationAsync(TimeSpan duration) => _preferredSource.ChangeDurationAsync(duration);

        /// <inheritdoc />
        public Task RemoveTrackAsync(int index) => _trackCollectionMap.RemoveAt(index);

        /// <inheritdoc />
        public Task RemoveImageAsync(int index) => _trackCollectionMap.RemoveAt(index);

        /// <inheritdoc />
        public Task RemoveAlbumItemAsync(int index) => _albumCollectionItemMap.RemoveAt(index);

        /// <summary>
        /// Adds a new source to this merged item.
        /// </summary>
        /// <param name="itemToMerge">The item to merge into this Artist</param>
        void IMergedMutable<ICoreArtist>.AddSource(ICoreArtist itemToMerge)
        {
            Guard.IsNotNull(itemToMerge, nameof(itemToMerge));

            if (!Equals(itemToMerge))
                ThrowHelper.ThrowArgumentException(nameof(itemToMerge), "Tried to merge an artist that doesn't match. Verify that the item matches before merging the source.");

            _sources.Add(itemToMerge);
            _sourceCores.Add(itemToMerge.SourceCore);
            _albumCollectionItemMap.Cast<IMergedMutable<ICoreAlbumCollection>>().AddSource(itemToMerge);
            _trackCollectionMap.Cast<IMergedMutable<ICoreTrackCollection>>().AddSource(itemToMerge);
            _imageCollectionMap.Cast<IMergedMutable<ICoreImageCollection>>().AddSource(itemToMerge);
        }

        /// <inheritdoc />
        void IMergedMutable<ICoreArtist>.RemoveSource(ICoreArtist itemToRemove)
        {
            Guard.IsNotNull(itemToRemove, nameof(itemToRemove));

            _sources.Remove(itemToRemove);
            _sourceCores.Remove(itemToRemove.SourceCore);
            _imageCollectionMap.Cast<IMergedMutable<ICoreImageCollection>>().RemoveSource(itemToRemove);
            _albumCollectionItemMap.Cast<IMergedMutable<ICoreAlbumCollection>>().RemoveSource(itemToRemove);
            _trackCollectionMap.Cast<IMergedMutable<ICoreTrackCollection>>().RemoveSource(itemToRemove);
        }

        /// <inheritdoc />
        void IMergedMutable<ICoreArtistCollectionItem>.AddSource(ICoreArtistCollectionItem itemToMerge)
        {
            if (itemToMerge is ICoreArtist col)
                ((IMergedMutable<ICoreArtist>)this).AddSource(col);
        }

        /// <inheritdoc />
        void IMergedMutable<ICoreArtistCollectionItem>.RemoveSource(ICoreArtistCollectionItem itemToRemove)
        {
            if (itemToRemove is ICoreArtist col)
                ((IMergedMutable<ICoreArtist>)this).RemoveSource(col);
        }

        /// <inheritdoc cref="Equals(object?)" />
        public bool Equals(ICoreArtist? other)
        {
            return other?.Name == Name;
        }

        /// <inheritdoc />
        public bool Equals(ICoreArtistCollectionItem other)
        {
            return Equals(other as ICoreArtist);
        }

        /// <inheritdoc />
        public bool Equals(ICoreAlbumCollectionItem other)
        {
            return Equals(other as ICoreArtist);
        }

        /// <inheritdoc />
        public bool Equals(ICoreImageCollection other)
        {
            return Equals(other as ICoreArtist);
        }

        /// <inheritdoc />
        public bool Equals(ICoreAlbumCollection other)
        {
            return Equals(other as ICoreArtist);
        }

        /// <inheritdoc />
        public bool Equals(ICoreTrackCollection other)
        {
            return Equals(other as ICoreArtist);
        }

        /// <inheritdoc />
        public bool Equals(ICoreGenreCollection other)
        {
            return Equals(other as ICoreArtist);
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj) || (obj is ICoreArtist other && Equals(other));
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return _preferredSource.GetHashCode();
        }

        /// <inheritdoc />
        public async ValueTask DisposeAsync()
        {
            DetachEvents(_preferredSource);

            await _albumCollectionItemMap.DisposeAsync();
            await _trackCollectionMap.DisposeAsync();
            await _imageCollectionMap.DisposeAsync();

            await Sources.InParallel(x => x.DisposeAsync().AsTask());
        }
    }
}
