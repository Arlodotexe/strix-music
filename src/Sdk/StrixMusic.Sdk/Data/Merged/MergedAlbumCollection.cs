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
    /// Merged multiple <see cref="ICoreAlbumCollection"/> into a single <see cref="IAlbumCollection"/>
    /// </summary>
    public class MergedAlbumCollection : IAlbumCollection, IMergedMutable<ICoreAlbumCollection>, IMergedMutable<ICoreAlbumCollectionItem>
    {
        private readonly List<ICoreAlbumCollection> _sources;
        private readonly List<ICore> _sourceCores;
        private readonly ICoreAlbumCollection _preferredSource;
        private readonly MergedCollectionMap<IAlbumCollection, ICoreAlbumCollection, IAlbumCollectionItem, ICoreAlbumCollectionItem> _albumMap;
        private readonly MergedCollectionMap<IImageCollection, ICoreImageCollection, IImage, ICoreImage> _imageMap;
        private readonly MergedCollectionMap<IUrlCollection, ICoreUrlCollection, IUrl, ICoreUrl> _urlMap;

        /// <summary>
        /// Creates a new instance of <see cref="MergedAlbumCollection"/>.
        /// </summary>
        /// <param name="sources">The initial sources to merge together.</param>
        public MergedAlbumCollection(IEnumerable<ICoreAlbumCollection> sources)
        {
            _sources = sources.ToList();
            _sourceCores = _sources.Select(x => x.SourceCore).ToList();

            _preferredSource = _sources[0];

            foreach (var source in _sources)
            {
                TotalAlbumItemsCount += source.TotalAlbumItemsCount;
                TotalImageCount += source.TotalImageCount;
                TotalUrlCount += source.TotalUrlCount;
            }

            Duration = _preferredSource.Duration;
            PlaybackState = _preferredSource.PlaybackState;
            Description = _preferredSource.Description;
            Name = _preferredSource.Name;
            LastPlayed = _preferredSource.LastPlayed;
            AddedAt = _preferredSource.AddedAt;

            _imageMap = new MergedCollectionMap<IImageCollection, ICoreImageCollection, IImage, ICoreImage>(this);
            _urlMap = new MergedCollectionMap<IUrlCollection, ICoreUrlCollection, IUrl, ICoreUrl>(this);
            _albumMap = new MergedCollectionMap<IAlbumCollection, ICoreAlbumCollection, IAlbumCollectionItem, ICoreAlbumCollectionItem>(this);

            AttachEvents(_preferredSource);
        }

        private void AttachEvents(ICoreAlbumCollection source)
        {
            AttachPlayableEvents(source);

            source.IsPlayAlbumCollectionAsyncAvailableChanged += IsPlayAlbumCollectionAsyncAvailableChanged;

            _albumMap.ItemsChanged += AlbumMap_ItemsChanged;
            _albumMap.ItemsCountChanged += AlbumMap_ItemsCountChanged;
            _imageMap.ItemsChanged += ImageMap_ItemsChanged;
            _imageMap.ItemsCountChanged += ImageMap_ItemsCountChanged;
            _urlMap.ItemsChanged += UrlMap_ItemsChanged;
            _urlMap.ItemsCountChanged += UrlMap_ItemsCountChanged;
        }

        private void DetachEvents(ICoreAlbumCollection source)
        {
            DetachPlayableEvents(source);

            source.IsPlayAlbumCollectionAsyncAvailableChanged -= IsPlayAlbumCollectionAsyncAvailableChanged;
            
            _albumMap.ItemsChanged -= AlbumMap_ItemsChanged;
            _albumMap.ItemsCountChanged -= AlbumMap_ItemsCountChanged;
            _imageMap.ItemsChanged -= ImageMap_ItemsChanged;
            _imageMap.ItemsCountChanged -= ImageMap_ItemsCountChanged;
            _urlMap.ItemsChanged -= UrlMap_ItemsChanged;
            _urlMap.ItemsCountChanged -= UrlMap_ItemsCountChanged;
        }

        private void AttachPlayableEvents(IPlayableBase source)
        {
            source.PlaybackStateChanged += PlaybackStateChanged;
            source.NameChanged += NameChanged;
            source.DescriptionChanged += DescriptionChanged;
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
            source.DurationChanged -= DurationChanged;
            source.LastPlayedChanged -= LastPlayedChanged;
            source.IsChangeNameAsyncAvailableChanged -= IsChangeNameAsyncAvailableChanged;
            source.IsChangeDurationAsyncAvailableChanged -= IsChangeDurationAsyncAvailableChanged;
            source.IsChangeDescriptionAsyncAvailableChanged -= IsChangeDescriptionAsyncAvailableChanged;
        }

        /// <inheritdoc />
        public event EventHandler<PlaybackState>? PlaybackStateChanged;

        /// <inheritdoc />
        public event EventHandler<string>? NameChanged;

        /// <inheritdoc />
        public event EventHandler<string?>? DescriptionChanged;

        /// <inheritdoc />
        public event EventHandler<TimeSpan>? DurationChanged;

        /// <inheritdoc />
        public event EventHandler<DateTime?>? LastPlayedChanged;

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
        public event CollectionChangedEventHandler<IAlbumCollectionItem>? AlbumItemsChanged;

        /// <inheritdoc />
        public event EventHandler<int>? AlbumItemsCountChanged;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<IImage>? ImagesChanged;

        /// <inheritdoc />
        public event EventHandler<int>? ImagesCountChanged;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<IUrl>? UrlsChanged;

        /// <inheritdoc />
        public event EventHandler<int>? UrlsCountChanged;

        private void AlbumMap_ItemsChanged(object sender, IReadOnlyList<CollectionChangedItem<IAlbumCollectionItem>> addedItems, IReadOnlyList<CollectionChangedItem<IAlbumCollectionItem>> removedItems)
        {
            AlbumItemsChanged?.Invoke(this, addedItems, removedItems);
        }

        private void AlbumMap_ItemsCountChanged(object sender, int e)
        {
            TotalAlbumItemsCount = e;
            AlbumItemsCountChanged?.Invoke(this, e);
        }

        private void ImageMap_ItemsCountChanged(object sender, int e)
        {
            TotalImageCount = e;
            ImagesCountChanged?.Invoke(this, e);
        }

        private void ImageMap_ItemsChanged(object sender, IReadOnlyList<CollectionChangedItem<IImage>> addedItems, IReadOnlyList<CollectionChangedItem<IImage>> removedItems)
        {
            ImagesChanged?.Invoke(this, addedItems, removedItems);
        }

        private void UrlMap_ItemsCountChanged(object sender, int e)
        {
            TotalUrlCount = e;
            UrlsCountChanged?.Invoke(this, e);
        }

        private void UrlMap_ItemsChanged(object sender, IReadOnlyList<CollectionChangedItem<IUrl>> addedItems, IReadOnlyList<CollectionChangedItem<IUrl>> removedItems)
        {
            UrlsChanged?.Invoke(this, addedItems, removedItems);
        }

        /// <inheritdoc cref="IMerged{T}.SourceCores" />
        public IReadOnlyList<ICore> SourceCores => _sourceCores;

        /// <inheritdoc cref="IMerged{T}.Sources"/>
        public IReadOnlyList<ICoreAlbumCollection> Sources => _sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreAlbumCollectionItem> IMerged<ICoreAlbumCollectionItem>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreAlbumCollection> IMerged<ICoreAlbumCollection>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreImageCollection> IMerged<ICoreImageCollection>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreUrlCollection> IMerged<ICoreUrlCollection>.Sources => Sources;

        /// <inheritdoc />
        public string Id => _preferredSource.Id;

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
        public int TotalAlbumItemsCount { get; internal set; }

        /// <inheritdoc />
        public int TotalImageCount { get; internal set; }

        /// <inheritdoc />
        public int TotalUrlCount { get; internal set; }

        /// <inheritdoc />
        public Task<bool> IsAddAlbumItemAvailableAsync(int index) => _albumMap.IsAddItemAvailableAsync(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveAlbumItemAvailableAsync(int index) => _albumMap.IsRemoveItemAvailableAsync(index);

        /// <inheritdoc />
        public Task<bool> IsAddImageAvailableAsync(int index) => _imageMap.IsAddItemAvailableAsync(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveImageAvailableAsync(int index) => _imageMap.IsRemoveItemAvailableAsync(index);

        /// <inheritdoc />
        public Task<bool> IsAddUrlAvailableAsync(int index) => _urlMap.IsAddItemAvailableAsync(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveUrlAvailableAsync(int index) => _urlMap.IsRemoveItemAvailableAsync(index);

        /// <inheritdoc />
        public Task PlayAlbumCollectionAsync() => _preferredSource.PlayAlbumCollectionAsync();

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
        public Task PauseAlbumCollectionAsync() => _preferredSource.PauseAlbumCollectionAsync();

        /// <inheritdoc />
        public Task ChangeNameAsync(string name) => _preferredSource.ChangeNameAsync(name);

        /// <inheritdoc />
        public Task ChangeDescriptionAsync(string? description) => _preferredSource.ChangeDescriptionAsync(description);

        /// <inheritdoc />
        public Task ChangeDurationAsync(TimeSpan duration) => _preferredSource.ChangeDurationAsync(duration);

        /// <inheritdoc />
        public Task AddAlbumItemAsync(IAlbumCollectionItem album, int index) => _albumMap.InsertItem(album, index);

        /// <inheritdoc />
        public Task RemoveAlbumItemAsync(int index) => _albumMap.RemoveAt(index);

        /// <inheritdoc />
        public Task AddImageAsync(IImage image, int index) => _imageMap.InsertItem(image, index);

        /// <inheritdoc />
        public Task RemoveImageAsync(int index) => _imageMap.RemoveAt(index);

        /// <inheritdoc />
        public Task AddUrlAsync(IUrl url, int index) => _urlMap.InsertItem(url, index);

        /// <inheritdoc />
        public Task RemoveUrlAsync(int index) => _urlMap.RemoveAt(index);

        /// <inheritdoc />
        public Task<IReadOnlyList<IAlbumCollectionItem>> GetAlbumItemsAsync(int limit, int offset) => _albumMap.GetItemsAsync(limit, offset);

        /// <inheritdoc />
        public Task<IReadOnlyList<IImage>> GetImagesAsync(int limit, int offset) => _imageMap.GetItemsAsync(limit, offset);

        /// <inheritdoc />
        public Task<IReadOnlyList<IUrl>> GetUrlsAsync(int limit, int offset) => _urlMap.GetItemsAsync(limit, offset);

        /// <inheritdoc />
        void IMergedMutable<ICoreAlbumCollection>.AddSource(ICoreAlbumCollection itemToMerge)
        {
            Guard.IsNotNull(itemToMerge, nameof(itemToMerge));

            _sources.Add(itemToMerge);
            _sourceCores.Remove(itemToMerge.SourceCore);
            _albumMap.Cast<IMergedMutable<ICoreAlbumCollection>>().AddSource(itemToMerge);
            _imageMap.Cast<IMergedMutable<ICoreImageCollection>>().AddSource(itemToMerge);
            _imageMap.Cast<IMergedMutable<ICoreUrlCollection>>().AddSource(itemToMerge);
        }

        /// <inheritdoc />
        void IMergedMutable<ICoreAlbumCollection>.RemoveSource(ICoreAlbumCollection itemToRemove)
        {
            Guard.IsNotNull(itemToRemove, nameof(itemToRemove));

            _sources.Remove(itemToRemove);
            _sourceCores.Remove(itemToRemove.SourceCore);
            _imageMap.Cast<IMergedMutable<ICoreImageCollection>>().RemoveSource(itemToRemove);
            _albumMap.Cast<IMergedMutable<ICoreAlbumCollection>>().RemoveSource(itemToRemove);
            _albumMap.Cast<IMergedMutable<ICoreUrlCollection>>().RemoveSource(itemToRemove);
        }

        /// <inheritdoc />
        void IMergedMutable<ICoreAlbumCollectionItem>.AddSource(ICoreAlbumCollectionItem itemToMerge)
        {
            if (itemToMerge is ICoreAlbumCollection albumCol)
                ((IMergedMutable<ICoreAlbumCollection>)this).AddSource(albumCol);
        }

        /// <inheritdoc />
        void IMergedMutable<ICoreAlbumCollectionItem>.RemoveSource(ICoreAlbumCollectionItem itemToMerge)
        {
            if (itemToMerge is ICoreAlbumCollection albumCol)
                ((IMergedMutable<ICoreAlbumCollection>)this).AddSource(albumCol);
        }

        /// <inheritdoc />
        public bool Equals(ICoreAlbumCollection? other)
        {
            return other?.Name.Equals(Name, StringComparison.InvariantCulture) ?? false;
        }

        /// <inheritdoc />
        public bool Equals(ICoreAlbumCollectionItem other) => Equals(other as ICoreAlbumCollection);

        /// <inheritdoc />
        public bool Equals(ICoreImageCollection other) => Equals(other as ICoreAlbumCollection);

        /// <inheritdoc />
        public bool Equals(ICoreUrlCollection other) => Equals(other as ICoreAlbumCollection);

        /// <inheritdoc />
        public async ValueTask DisposeAsync()
        {
            DetachEvents(_preferredSource);

            await _albumMap.DisposeAsync();
            await _imageMap.DisposeAsync();
            await _urlMap.DisposeAsync();

            await Sources.InParallel(x => x.DisposeAsync().AsTask());
        }
    }
}