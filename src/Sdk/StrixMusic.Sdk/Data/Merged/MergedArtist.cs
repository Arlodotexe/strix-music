using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Toolkit.Diagnostics;
using OwlCore.Collections;
using OwlCore.Events;
using StrixMusic.Sdk.Data.Base;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.MediaPlayback;

namespace StrixMusic.Sdk.Data.Merged
{
    /// <summary>
    /// Merged multiple <see cref="ICoreArtist"/> into a single <see cref="IArtist"/>
    /// </summary>
    public class MergedArtist : IArtist, IMerged<ICoreArtist>
    {
        private readonly List<ICoreArtist> _sources;
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

            Images = new SynchronizedObservableCollection<IImage>();

            _trackCollectionMap = new MergedCollectionMap<ITrackCollection, ICoreTrackCollection, ITrack, ICoreTrack>(this);
            _imageCollectionMap = new MergedCollectionMap<IImageCollection, ICoreImageCollection, IImage, ICoreImage>(this);
            _albumCollectionItemMap = new MergedCollectionMap<IAlbumCollection, ICoreAlbumCollection, IAlbumCollectionItem, ICoreAlbumCollectionItem>(this);

            // TODO: Get the actual preferred source.
            PreferredSource = _sources[0];

            AttachEvents(PreferredSource);
        }

        private void AttachEvents(ICoreArtist preferredSource)
        {
            AttachPropertyEvents(preferredSource);
            _trackCollectionMap.ItemsChanged += TrackCollectionMap_ItemsChanged;
            _trackCollectionMap.ItemsCountChanged += TrackCollectionMap_ItemsCountChanged;
            _imageCollectionMap.ItemsChanged += ImageCollectionMap_ItemsChanged;
            _imageCollectionMap.ItemsCountChanged += ImageCollectionMap_ItemsCountChanged;
            _albumCollectionItemMap.ItemsChanged += AlbumCollectionItemsChanged;
            _albumCollectionItemMap.ItemsCountChanged += AlbumCollectionMap_ItemsCountChanged;
        }

        private void AttachPropertyEvents(ICoreArtist source)
        {
            source.PlaybackStateChanged += PlaybackStateChanged;
            source.NameChanged += NameChanged;
            source.DescriptionChanged += DescriptionChanged;
            source.UrlChanged += UrlChanged;
            source.DurationChanged += DurationChanged;
        }

        private void DetachEvents(ICoreAlbum source)
        {
            DetachPropertyEvents(source);
            _trackCollectionMap.ItemsChanged -= TrackCollectionMap_ItemsChanged;
            _trackCollectionMap.ItemsCountChanged -= TrackCollectionMap_ItemsCountChanged;
            _imageCollectionMap.ItemsChanged -= ImageCollectionMap_ItemsChanged;
            _imageCollectionMap.ItemsCountChanged -= ImageCollectionMap_ItemsCountChanged;
            _albumCollectionItemMap.ItemsChanged -= AlbumCollectionItemsChanged;
        }

        private void DetachPropertyEvents(ICoreAlbum source)
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

        private void AlbumCollectionItemsChanged(object sender, IReadOnlyList<CollectionChangedEventItem<IAlbumCollectionItem>> addedItems, IReadOnlyList<CollectionChangedEventItem<IAlbumCollectionItem>> removedItems)
        {
            AlbumItemsChanged?.Invoke(this, addedItems, removedItems);
        }

        private void ImageCollectionMap_ItemsCountChanged(object sender, int e)
        {
            ImagesCountChanged?.Invoke(this, e);
        }

        private void ImageCollectionMap_ItemsChanged(object sender, IReadOnlyList<CollectionChangedEventItem<IImage>> addedItems, IReadOnlyList<CollectionChangedEventItem<IImage>> removedItems)
        {
            ImagesChanged?.Invoke(this, addedItems, removedItems);
        }

        private void TrackCollectionMap_ItemsCountChanged(object sender, int e)
        {
            TotalTracksCount = e;
            TrackItemsCountChanged?.Invoke(this, e);
        }

        private void TrackCollectionMap_ItemsChanged(object sender, IReadOnlyList<CollectionChangedEventItem<ITrack>> addedItems, IReadOnlyList<CollectionChangedEventItem<ITrack>> removedItems)
        {
            TrackItemsChanged?.Invoke(this, addedItems, removedItems);
        }

        /// <inheritdoc />
        protected ICoreArtist PreferredSource { get; }

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

        /// <inheritdoc cref="ISdkMember{T}.SourceCores" />
        public IReadOnlyList<ICore> SourceCores => Sources.Select(x => x.SourceCore).ToList();

        /// <summary>
        /// Adds a new source to this merged item.
        /// </summary>
        /// <param name="itemToMerge">The item to merge into this Artist</param>
        public void AddSource(ICoreArtist itemToMerge)
        {
            if (!Equals(itemToMerge))
                ThrowHelper.ThrowArgumentException(nameof(itemToMerge), "Tried to merge an artist that doesn't match. Verify that the item matches before merging the source.");

            _albumCollectionItemMap.AddSource(itemToMerge);
            _trackCollectionMap.AddSource(itemToMerge);
            _imageCollectionMap.AddSource(itemToMerge);
            _sources.Add(itemToMerge);
        }

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
        public SynchronizedObservableCollection<IImage> Images { get; }

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
        public async Task<IReadOnlyList<IAlbumCollectionItem>> GetAlbumItemsAsync(int limit, int offset)
        {
            var albums = new List<ICoreAlbum>();
            var albumCollections = new List<ICoreAlbumCollection>();

            var items = await _albumCollectionItemMap.GetItems(limit, offset);

            // Using one source for now.
            foreach (var item in items)
            {
                if (item is ICoreAlbum album)
                    albums.Add(album);

                if (item is ICoreAlbumCollection collection)
                    albumCollections.Add(collection);
            }

            // Turn each item into an Sdk Member
            var mergedAlbums = albums.Select(x => new MergedAlbum(new List<ICoreAlbum> { x }));
            var mergedCollections = albumCollections.Select(x => new MergedAlbumCollection(new List<ICoreAlbumCollection> { x }));

            // Handle possible multiple enumeration.
            var albumCollectionItems = mergedAlbums as MergedAlbum[] ?? mergedAlbums.ToArray();

            return mergedCollections.Union<IAlbumCollectionItem>(albumCollectionItems).ToList();
        }

        /// <inheritdoc />
        public Task AddAlbumItemAsync(IAlbumCollectionItem album, int index)
        {
            return _albumCollectionItemMap.InsertItem(album, index);
        }

        /// <inheritdoc />
        public Task<IReadOnlyList<ITrack>> GetTracksAsync(int limit, int offset) => _trackCollectionMap.GetItems(limit, offset);

        /// <inheritdoc />
        public Task AddTrackAsync(ITrack track, int index) => _trackCollectionMap.InsertItem(track, index);

        /// <inheritdoc />
        public Task PlayAsync()
        {
            return PreferredSource.PlayAsync();
        }

        /// <inheritdoc />
        public Task PauseAsync()
        {
            return PreferredSource.PauseAsync();
        }

        /// <inheritdoc />
        public Task ChangeNameAsync(string name)
        {
            return PreferredSource.ChangeNameAsync(name);
        }

        /// <inheritdoc />
        public Task<IReadOnlyList<IImage>> GetImagesAsync(int limit, int offset)
        {
            return _imageCollectionMap.GetItems(limit, offset);
        }

        /// <inheritdoc />
        public Task AddImageAsync(IImage image, int index)
        {
            return _imageCollectionMap.InsertItem(image, index);
        }

        /// <inheritdoc />
        public async Task RemoveImageAsync(int index)
        {
            await _albumCollectionItemMap.RemoveAt(index);
        }

        /// <inheritdoc />
        public Task ChangeDescriptionAsync(string? description)
        {
            return PreferredSource.ChangeDescriptionAsync(description);
        }

        /// <inheritdoc />
        public Task ChangeDurationAsync(TimeSpan duration)
        {
            return PreferredSource.ChangeDurationAsync(duration);
        }

        /// <inheritdoc />
        public Task RemoveTrackAsync(int index)
        {
            return _trackCollectionMap.RemoveAt(index);
        }

        /// <inheritdoc />
        public Task RemoveAlbumItemAsync(int index)
        {
            return _albumCollectionItemMap.RemoveAt(index);
        }

        /// <inheritdoc />
        public event EventHandler<string>? NameChanged;

        /// <inheritdoc />
        public event EventHandler<string?>? DescriptionChanged;

        /// <inheritdoc />
        public event EventHandler<PlaybackState>? PlaybackStateChanged;

        /// <inheritdoc />
        public event EventHandler<Uri?>? UrlChanged;

        /// <inheritdoc cref="Equals(object?)" />
        public bool Equals(ICoreArtist? other)
        {
            return other?.Name == Name;
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
