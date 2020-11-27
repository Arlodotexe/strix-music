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
using StrixMusic.Sdk.MediaPlayback;

namespace StrixMusic.Sdk.Data.Merged
{
    /// <summary>
    /// Merged multiple <see cref="ICoreArtist"/> into a single <see cref="IArtist"/>
    /// </summary>
    public class MergedArtist : IArtist, IMerged<ICoreArtist>, IMerged<ICoreArtistCollectionItem>
    {
        private readonly List<ICoreArtist> _sources;
        private readonly List<ICore> _sourceCores;
        private readonly MergedCollectionMap<ITrackCollection, ICoreTrackCollection, ITrack, ICoreTrack> _trackCollectionMap;
        private readonly MergedCollectionMap<IImageCollection, ICoreImageCollection, IImage, ICoreImage> _imageCollectionMap;
        private readonly MergedCollectionMap<IAlbumCollection, ICoreAlbumCollection, IAlbumCollectionItem, ICoreAlbumCollectionItem> _albumCollectionItemMap;

        /// <summary>
        /// Creates a new instance of <see cref="MergedArtist"/>.
        /// </summary>
        /// <param name="sources">The sources used</param>
        public MergedArtist(IEnumerable<ICoreArtist> sources)
        {
            _sources = sources?.ToList() ?? throw new ArgumentNullException();
            _sourceCores = _sources.Select(x => x.SourceCore).ToList();

            foreach (var source in _sources)
            {
                TotalAlbumItemsCount += source.TotalAlbumItemsCount;
                TotalImageCount += source.TotalImageCount;
                TotalTracksCount += source.TotalTracksCount;
            }

            _trackCollectionMap = new MergedCollectionMap<ITrackCollection, ICoreTrackCollection, ITrack, ICoreTrack>(this);
            _imageCollectionMap = new MergedCollectionMap<IImageCollection, ICoreImageCollection, IImage, ICoreImage>(this);
            _albumCollectionItemMap = new MergedCollectionMap<IAlbumCollection, ICoreAlbumCollection, IAlbumCollectionItem, ICoreAlbumCollectionItem>(this);

            var relatedItems = _sources.Select(x => x.RelatedItems).PruneNull().ToList();

            if (relatedItems.Count > 0)
                RelatedItems = new MergedPlayableCollectionGroup(relatedItems);

            // TODO: Get the actual preferred source.
            PreferredSource = _sources[0];

            AttachEvents(PreferredSource);
        }

        private void AttachEvents(ICoreArtist preferredSource)
        {
            AttachPlayableEvents(preferredSource);

            _trackCollectionMap.ItemsChanged += TrackCollectionMap_ItemsChanged;
            _trackCollectionMap.ItemsCountChanged += TrackCollectionMap_ItemsCountChanged;
            _imageCollectionMap.ItemsChanged += ImageCollectionMap_ItemsChanged;
            _imageCollectionMap.ItemsCountChanged += ImageCollectionMap_ItemsCountChanged;
            _albumCollectionItemMap.ItemsChanged += AlbumCollectionItemsChanged;
            _albumCollectionItemMap.ItemsCountChanged += AlbumCollectionMap_ItemsCountChanged;
        }

        private void DetachEvents(ICoreAlbum source)
        {
            DetachPlayableEvents(source);

            _trackCollectionMap.ItemsChanged -= TrackCollectionMap_ItemsChanged;
            _trackCollectionMap.ItemsCountChanged -= TrackCollectionMap_ItemsCountChanged;
            _imageCollectionMap.ItemsChanged -= ImageCollectionMap_ItemsChanged;
            _imageCollectionMap.ItemsCountChanged -= ImageCollectionMap_ItemsCountChanged;
            _albumCollectionItemMap.ItemsChanged -= AlbumCollectionItemsChanged;
        }

        private void AttachPlayableEvents(IPlayable source)
        {
            source.PlaybackStateChanged += PlaybackStateChanged;
            source.NameChanged += NameChanged;
            source.DescriptionChanged += DescriptionChanged;
            source.UrlChanged += UrlChanged;
            source.DurationChanged += DurationChanged;
        }

        private void DetachPlayableEvents(IPlayable source)
        {
            source.PlaybackStateChanged -= PlaybackStateChanged;
            source.NameChanged -= NameChanged;
            source.DescriptionChanged -= DescriptionChanged;
            source.UrlChanged -= UrlChanged;
            source.DurationChanged -= DurationChanged;
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

        private void AlbumCollectionItemsChanged(object sender, IReadOnlyList<CollectionChangedEventItem<IAlbumCollectionItem>> addedItems, IReadOnlyList<CollectionChangedEventItem<IAlbumCollectionItem>> removedItems)
        {
            AlbumItemsChanged?.Invoke(this, addedItems, removedItems);
        }

        private void ImageCollectionMap_ItemsChanged(object sender, IReadOnlyList<CollectionChangedEventItem<IImage>> addedItems, IReadOnlyList<CollectionChangedEventItem<IImage>> removedItems)
        {
            ImagesChanged?.Invoke(this, addedItems, removedItems);
        }

        private void TrackCollectionMap_ItemsChanged(object sender, IReadOnlyList<CollectionChangedEventItem<ITrack>> addedItems, IReadOnlyList<CollectionChangedEventItem<ITrack>> removedItems)
        {
            TrackItemsChanged?.Invoke(this, addedItems, removedItems);
        }

        /// <summary>
        /// The preferred source for this artist.
        /// </summary>
        internal ICoreArtist PreferredSource { get; }

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
        public event EventHandler<string>? NameChanged;

        /// <inheritdoc />
        public event EventHandler<string?>? DescriptionChanged;

        /// <inheritdoc />
        public event EventHandler<PlaybackState>? PlaybackStateChanged;

        /// <inheritdoc />
        public event EventHandler<Uri?>? UrlChanged;

        /// <inheritdoc cref="ISdkMember{T}.SourceCores" />
        public IReadOnlyList<ICore> SourceCores => _sourceCores;

        /// <inheritdoc />
        public void AddSource(ICoreArtistCollectionItem itemToMerge)
        {
            
        }

        /// <inheritdoc />
        public void RemoveSource(ICoreArtistCollectionItem itemToRemove)
        {

        }

        /// <inheritdoc />
        IReadOnlyList<ICoreArtistCollectionItem> IMerged<ICoreArtistCollectionItem>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreGenreCollection> ISdkMember<ICoreGenreCollection>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreTrackCollection> ISdkMember<ICoreTrackCollection>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreAlbumCollection> ISdkMember<ICoreAlbumCollection>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreImageCollection> ISdkMember<ICoreImageCollection>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreArtistCollectionItem> ISdkMember<ICoreArtistCollectionItem>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreAlbumCollectionItem> ISdkMember<ICoreAlbumCollectionItem>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreArtist> ISdkMember<ICoreArtist>.Sources => Sources;

        /// <summary>
        /// The merged sources for this <see cref="IArtist"/>
        /// </summary>
        public IReadOnlyList<ICoreArtist> Sources => _sources;

        /// <inheritdoc />
        public string Id => PreferredSource.Id;

        /// <inheritdoc />
        public Uri? Url => PreferredSource.Url;

        /// <inheritdoc />
        public string Name => PreferredSource.Name;

        /// <inheritdoc />
        public string? Description => PreferredSource.Description;

        /// <inheritdoc />
        public PlaybackState PlaybackState => PreferredSource.PlaybackState;

        /// <inheritdoc />
        public TimeSpan Duration => PreferredSource.Duration;

        /// <inheritdoc />
        public SynchronizedObservableCollection<string>? Genres => PreferredSource.Genres;

        /// <inheritdoc />
        public IPlayableCollectionGroup? RelatedItems { get; }

        /// <inheritdoc />
        public int TotalTracksCount { get; private set; }

        /// <inheritdoc />
        public int TotalAlbumItemsCount { get; private set; }

        /// <inheritdoc />
        public int TotalImageCount { get; private set; }

        /// <inheritdoc />
        public bool IsPlayAsyncSupported => PreferredSource.IsPlayAsyncSupported;

        /// <inheritdoc />
        public bool IsPauseAsyncSupported => PreferredSource.IsPauseAsyncSupported;

        /// <inheritdoc />
        public bool IsChangeNameAsyncSupported => PreferredSource.IsChangeNameAsyncSupported;

        /// <inheritdoc />
        public bool IsChangeDescriptionAsyncSupported => PreferredSource.IsChangeDescriptionAsyncSupported;

        /// <inheritdoc />
        public bool IsChangeDurationAsyncSupported => PreferredSource.IsChangeDurationAsyncSupported;

        /// <inheritdoc />
        public Task<bool> IsAddTrackSupported(int index) => _trackCollectionMap.IsAddItemSupported(index);

        /// <inheritdoc />
        public Task<bool> IsAddAlbumItemSupported(int index) => _albumCollectionItemMap.IsAddItemSupported(index);

        /// <inheritdoc />
        public Task<bool> IsAddImageSupported(int index) => _imageCollectionMap.IsAddItemSupported(index);

        /// <inheritdoc />
        public Task<bool> IsAddGenreSupported(int index) => PreferredSource.IsAddGenreSupported(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveTrackSupported(int index) => _trackCollectionMap.IsRemoveItemSupport(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveAlbumItemSupported(int index) => _albumCollectionItemMap.IsRemoveItemSupport(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveImageSupported(int index) => _imageCollectionMap.IsRemoveItemSupport(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveGenreSupported(int index) => PreferredSource.IsRemoveGenreSupported(index);

        /// <inheritdoc />
        public Task<IReadOnlyList<IAlbumCollectionItem>> GetAlbumItemsAsync(int limit, int offset) => _albumCollectionItemMap.GetItems(limit, offset);

        /// <inheritdoc />
        public Task AddAlbumItemAsync(IAlbumCollectionItem album, int index) => _albumCollectionItemMap.InsertItem(album, index);

        /// <inheritdoc />
        public Task<IReadOnlyList<ITrack>> GetTracksAsync(int limit, int offset) => _trackCollectionMap.GetItems(limit, offset);

        /// <inheritdoc />
        public Task AddTrackAsync(ITrack track, int index) => _trackCollectionMap.InsertItem(track, index);

        /// <inheritdoc />
        public Task PlayAsync() => PreferredSource.PlayAsync();

        /// <inheritdoc />
        public Task PauseAsync() => PreferredSource.PauseAsync();

        /// <inheritdoc />
        public Task ChangeNameAsync(string name) => PreferredSource.ChangeNameAsync(name);

        /// <inheritdoc />
        public Task<IReadOnlyList<IImage>> GetImagesAsync(int limit, int offset) => _imageCollectionMap.GetItems(limit, offset);

        /// <inheritdoc />
        public Task AddImageAsync(IImage image, int index) => _imageCollectionMap.InsertItem(image, index);

        /// <inheritdoc />
        public Task ChangeDescriptionAsync(string? description) => PreferredSource.ChangeDescriptionAsync(description);

        /// <inheritdoc />
        public Task ChangeDurationAsync(TimeSpan duration) => PreferredSource.ChangeDurationAsync(duration);

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
        public void AddSource(ICoreArtist itemToMerge)
        {
            Guard.IsNotNull(itemToMerge, nameof(itemToMerge));

            if (!Equals(itemToMerge))
                ThrowHelper.ThrowArgumentException(nameof(itemToMerge), "Tried to merge an artist that doesn't match. Verify that the item matches before merging the source.");

            _sources.Add(itemToMerge);
            _sourceCores.Add(itemToMerge.SourceCore);
            _albumCollectionItemMap.AddSource(itemToMerge);
            _trackCollectionMap.AddSource(itemToMerge);
            _imageCollectionMap.AddSource(itemToMerge);
        }

        /// <inheritdoc />
        public void RemoveSource(ICoreArtist itemToRemove)
        {
            Guard.IsNotNull(itemToRemove, nameof(itemToRemove));

            _sources.Remove(itemToRemove);
            _sourceCores.Remove(itemToRemove.SourceCore);
            _imageCollectionMap.RemoveSource(itemToRemove);
            _albumCollectionItemMap.RemoveSource(itemToRemove);
            _trackCollectionMap.RemoveSource(itemToRemove);
        }

        /// <inheritdoc cref="Equals(object?)" />
        public bool Equals(ICoreArtist? other)
        {
            return other?.Name == Name;
        }

        /// <inheritdoc />
        public bool Equals(ICoreArtistCollectionItem other)
        {
            return false;
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj) || (obj is ICoreArtist other && Equals(other));
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return PreferredSource.GetHashCode();
        }
    }
}
