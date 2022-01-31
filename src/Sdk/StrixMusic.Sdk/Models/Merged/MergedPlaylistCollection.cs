using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Toolkit.Diagnostics;
using OwlCore.Events;
using OwlCore.Extensions;
using StrixMusic.Sdk.Extensions;
using StrixMusic.Sdk.MediaPlayback;
using StrixMusic.Sdk.Models.Base;
using StrixMusic.Sdk.Models.Core;
using StrixMusic.Sdk.Services;

namespace StrixMusic.Sdk.Models.Merged
{
    /// <summary>
    /// A concrete class that merged multiple <see cref="ICorePlaylistCollection"/>s.
    /// </summary>
    public class MergedPlaylistCollection : IPlaylistCollection, IMergedMutable<ICorePlaylistCollection>
    {
        private readonly List<ICorePlaylistCollection> _sources;
        private readonly List<ICore> _sourceCores;
        private readonly ICorePlaylistCollection _preferredSource;
        private readonly MergedCollectionMap<IPlaylistCollection, ICorePlaylistCollection, IPlaylistCollectionItem, ICorePlaylistCollectionItem> _playlistMap;
        private readonly MergedCollectionMap<IImageCollection, ICoreImageCollection, IImage, ICoreImage> _imageMap;
        private readonly MergedCollectionMap<IUrlCollection, ICoreUrlCollection, IUrl, ICoreUrl> _urlMap;

        /// <summary>
        /// Creates a new instance of <see cref="MergedPlaylistCollection"/>.
        /// </summary>
        public MergedPlaylistCollection(IEnumerable<ICorePlaylistCollection> collections, ISettingsService settingsService)
        {
            _sources = collections.ToList();
            _sourceCores = _sources.Select(x => x.SourceCore).ToList();

            _preferredSource = _sources[0];

            _imageMap = new MergedCollectionMap<IImageCollection, ICoreImageCollection, IImage, ICoreImage>(this, settingsService);
            _urlMap = new MergedCollectionMap<IUrlCollection, ICoreUrlCollection, IUrl, ICoreUrl>(this, settingsService);
            _playlistMap = new MergedCollectionMap<IPlaylistCollection, ICorePlaylistCollection, IPlaylistCollectionItem, ICorePlaylistCollectionItem>(this, settingsService);

            foreach (var item in _sources)
            {
                TotalPlaylistItemsCount += item.TotalPlaylistItemsCount;
                TotalImageCount += item.TotalImageCount;
                TotalUrlCount += item.TotalUrlCount;
            }

            Name = _preferredSource.Name;
            Description = _preferredSource.Description;
            PlaybackState = _preferredSource.PlaybackState;
            Duration = _preferredSource.Duration;
            LastPlayed = _preferredSource.LastPlayed;
            AddedAt = _preferredSource.AddedAt;

            AttachEvents(_preferredSource);
        }

        private void AttachEvents(ICorePlaylistCollection source)
        {
            AttachPlayableEvents(source);

            source.IsPlayPlaylistCollectionAsyncAvailableChanged += IsPlayPlaylistCollectionAsyncAvailableChanged;
            source.IsPausePlaylistCollectionAsyncAvailableChanged += IsPausePlaylistCollectionAsyncAvailableChanged;

            _playlistMap.ItemsChanged += PlaylistMap_ItemsChanged;
            _playlistMap.ItemsCountChanged += PlaylistMap_ItemsCountChanged;
            _imageMap.ItemsChanged += ImageCollectionMap_ItemsChanged;
            _imageMap.ItemsCountChanged += ImageCollectionMap_ItemsCountChanged;
            _urlMap.ItemsChanged += UrlCollectionMap_ItemsChanged;
            _urlMap.ItemsCountChanged += UrlCollectionMap_ItemsCountChanged;
        }

        private void DetachEvents(ICorePlaylistCollection source)
        {
            DetachPlayableEvents(source);

            source.IsPlayPlaylistCollectionAsyncAvailableChanged -= IsPlayPlaylistCollectionAsyncAvailableChanged;
            source.IsPausePlaylistCollectionAsyncAvailableChanged -= IsPausePlaylistCollectionAsyncAvailableChanged;

            _playlistMap.ItemsChanged -= PlaylistMap_ItemsChanged;
            _playlistMap.ItemsCountChanged -= PlaylistMap_ItemsCountChanged;
            _imageMap.ItemsChanged -= ImageCollectionMap_ItemsChanged;
            _imageMap.ItemsCountChanged -= ImageCollectionMap_ItemsCountChanged;
            _urlMap.ItemsChanged -= UrlCollectionMap_ItemsChanged;
            _urlMap.ItemsCountChanged -= UrlCollectionMap_ItemsCountChanged;
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

        private void PlaylistMap_ItemsCountChanged(object sender, int e)
        {
            TotalPlaylistItemsCount = e;
            PlaylistItemsCountChanged?.Invoke(this, e);
        }

        private void ImageCollectionMap_ItemsCountChanged(object sender, int e)
        {
            TotalImageCount = e;
            ImagesCountChanged?.Invoke(this, e);
        }

        private void UrlCollectionMap_ItemsCountChanged(object sender, int e)
        {
            TotalUrlCount = e;
            UrlsCountChanged?.Invoke(this, e);
        }

        private void ImageCollectionMap_ItemsChanged(object sender, IReadOnlyList<CollectionChangedItem<IImage>> addedItems, IReadOnlyList<CollectionChangedItem<IImage>> removedItems)
        {
            ImagesChanged?.Invoke(this, addedItems, removedItems);
        }

        private void UrlCollectionMap_ItemsChanged(object sender, IReadOnlyList<CollectionChangedItem<IUrl>> addedItems, IReadOnlyList<CollectionChangedItem<IUrl>> removedItems)
        {
            UrlsChanged?.Invoke(this, addedItems, removedItems);
        }

        private void PlaylistMap_ItemsChanged(object sender, IReadOnlyList<CollectionChangedItem<IPlaylistCollectionItem>> addedItems, IReadOnlyList<CollectionChangedItem<IPlaylistCollectionItem>> removedItems)
        {
            PlaylistItemsChanged?.Invoke(this, addedItems, removedItems);
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
        public event EventHandler<bool>? IsPlayPlaylistCollectionAsyncAvailableChanged;

        /// <inheritdoc />
        public event EventHandler<bool>? IsPausePlaylistCollectionAsyncAvailableChanged;

        /// <inheritdoc />
        public event EventHandler<bool>? IsChangeNameAsyncAvailableChanged;

        /// <inheritdoc />
        public event EventHandler<bool>? IsChangeDescriptionAsyncAvailableChanged;

        /// <inheritdoc />
        public event EventHandler<bool>? IsChangeDurationAsyncAvailableChanged;

        /// <inheritdoc />
        public event EventHandler<int>? ImagesCountChanged;

        /// <inheritdoc />
        public event EventHandler<int>? UrlsCountChanged;

        /// <inheritdoc />
        public event EventHandler<int>? PlaylistItemsCountChanged;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<IImage>? ImagesChanged;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<IUrl>? UrlsChanged;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<IPlaylistCollectionItem>? PlaylistItemsChanged;

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
        public int TotalPlaylistItemsCount { get; internal set; }

        /// <inheritdoc />
        public int TotalImageCount { get; internal set; }

        /// <inheritdoc />
        public int TotalUrlCount { get; internal set; }

        /// <inheritdoc />
        public bool IsPlayPlaylistCollectionAsyncAvailable => _preferredSource.IsPlayPlaylistCollectionAsyncAvailable;

        /// <inheritdoc />
        public bool IsPausePlaylistCollectionAsyncAvailable => _preferredSource.IsPausePlaylistCollectionAsyncAvailable;

        /// <inheritdoc />
        public bool IsChangeNameAsyncAvailable => _preferredSource.IsChangeNameAsyncAvailable;

        /// <inheritdoc />
        public bool IsChangeDescriptionAsyncAvailable => _preferredSource.IsChangeDescriptionAsyncAvailable;

        /// <inheritdoc />
        public bool IsChangeDurationAsyncAvailable => _preferredSource.IsChangeDurationAsyncAvailable;

        /// <inheritdoc />
        public Task PlayPlaylistCollectionAsync() => _preferredSource.PlayPlaylistCollectionAsync();

        /// <inheritdoc />
        public Task PausePlaylistCollectionAsync() => _preferredSource.PausePlaylistCollectionAsync();

        /// <inheritdoc />
        public Task PlayPlaylistCollectionAsync(IPlaylistCollectionItem playlistItem)
        {
            var targetCore = _preferredSource.SourceCore;

            ICorePlaylistCollectionItem? source = null;

            if (playlistItem is IPlaylist playlist)
                source = playlist.GetSources<ICorePlaylist>().FirstOrDefault(x => x.SourceCore.InstanceId == targetCore.InstanceId);

            if (playlistItem is IPlaylistCollection collection)
                source = collection.GetSources<ICorePlaylistCollection>().FirstOrDefault(x => x.SourceCore.InstanceId == targetCore.InstanceId);

            Guard.IsNotNull(source, nameof(source));

            return _preferredSource.PlayPlaylistCollectionAsync(source);
        }

        /// <inheritdoc />
        public Task ChangeNameAsync(string name)
        {
            return _preferredSource.ChangeNameAsync(name);
        }

        /// <inheritdoc />
        public Task ChangeDescriptionAsync(string? description)
        {
            return _preferredSource.ChangeDescriptionAsync(description);
        }

        /// <inheritdoc />
        public Task ChangeDurationAsync(TimeSpan duration)
        {
            return _preferredSource.ChangeDurationAsync(duration);
        }

        /// <inheritdoc />
        public Task<bool> IsAddPlaylistItemAvailableAsync(int index) => _playlistMap.IsAddItemAvailableAsync(index);

        /// <inheritdoc />
        public Task<bool> IsRemovePlaylistItemAvailableAsync(int index) => _playlistMap.IsRemoveItemAvailableAsync(index);

        /// <inheritdoc />
        public Task<bool> IsAddImageAvailableAsync(int index) => _imageMap.IsAddItemAvailableAsync(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveImageAvailableAsync(int index) => _imageMap.IsRemoveItemAvailableAsync(index);

        /// <inheritdoc />
        public Task<bool> IsAddUrlAvailableAsync(int index) => _urlMap.IsAddItemAvailableAsync(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveUrlAvailableAsync(int index) => _urlMap.IsRemoveItemAvailableAsync(index);

        /// <inheritdoc cref="IMerged{T}.SourceCores" />
        public IReadOnlyList<ICore> SourceCores => _sourceCores;

        /// <inheritdoc cref="IMerged{T}.Sources" />
        public IReadOnlyList<ICorePlaylistCollection> Sources => _sources;

        /// <inheritdoc />
        IReadOnlyList<ICorePlaylistCollectionItem> IMerged<ICorePlaylistCollectionItem>.Sources => _sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreImageCollection> IMerged<ICoreImageCollection>.Sources => _sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreUrlCollection> IMerged<ICoreUrlCollection>.Sources => _sources;

        /// <inheritdoc />
        IReadOnlyList<ICorePlaylistCollection> IMerged<ICorePlaylistCollection>.Sources => _sources;

        /// <inheritdoc />
        public Task<IReadOnlyList<IPlaylistCollectionItem>> GetPlaylistItemsAsync(int limit, int offset) => _playlistMap.GetItemsAsync(limit, offset);

        /// <inheritdoc />
        public Task<IReadOnlyList<IImage>> GetImagesAsync(int limit, int offset) => _imageMap.GetItemsAsync(limit, offset);

        /// <inheritdoc />
        public Task<IReadOnlyList<IUrl>> GetUrlsAsync(int limit, int offset) => _urlMap.GetItemsAsync(limit, offset);

        /// <inheritdoc />
        public Task AddPlaylistItemAsync(IPlaylistCollectionItem playlist, int index) => _playlistMap.InsertItem(playlist, index);

        /// <inheritdoc />
        public Task AddImageAsync(IImage image, int index) => _imageMap.InsertItem(image, index);

        /// <inheritdoc />
        public Task AddUrlAsync(IUrl url, int index) => _urlMap.InsertItem(url, index);

        /// <inheritdoc />
        public Task RemovePlaylistItemAsync(int index) => _playlistMap.RemoveAt(index);

        /// <inheritdoc />
        public Task RemoveImageAsync(int index) => _imageMap.RemoveAt(index);

        /// <inheritdoc />
        public Task RemoveUrlAsync(int index) => _urlMap.RemoveAt(index);

        /// <inheritdoc />
        void IMergedMutable<ICorePlaylistCollection>.AddSource(ICorePlaylistCollection itemToMerge)
        {
            Guard.IsNotNull(itemToMerge, nameof(itemToMerge));

            _sources.Add(itemToMerge);
            _sourceCores.Remove(itemToMerge.SourceCore);

            _playlistMap.Cast<IMergedMutable<ICorePlaylistCollection>>().AddSource(itemToMerge);
            _imageMap.Cast<IMergedMutable<ICorePlaylistCollection>>().AddSource(itemToMerge);
            _urlMap.Cast<IMergedMutable<ICorePlaylistCollection>>().AddSource(itemToMerge);
        }

        /// <inheritdoc />
        void IMergedMutable<ICorePlaylistCollection>.RemoveSource(ICorePlaylistCollection itemToRemove)
        {
            Guard.IsNotNull(itemToRemove, nameof(itemToRemove));

            _sources.Remove(itemToRemove);
            _sourceCores.Remove(itemToRemove.SourceCore);

            _playlistMap.Cast<IMergedMutable<ICorePlaylistCollection>>().RemoveSource(itemToRemove);
            _imageMap.Cast<IMergedMutable<ICorePlaylistCollection>>().RemoveSource(itemToRemove);
            _urlMap.Cast<IMergedMutable<ICorePlaylistCollection>>().RemoveSource(itemToRemove);
        }

        /// <inheritdoc />
        public bool Equals(ICorePlaylistCollection? other)
        {
            return other?.Name == Name;
        }

        /// <inheritdoc />
        public bool Equals(ICorePlaylistCollectionItem other) => Equals(other as ICorePlaylistCollection);

        /// <inheritdoc />
        public bool Equals(ICoreImageCollection other) => Equals(other as ICorePlaylistCollection);

        /// <inheritdoc />
        public bool Equals(ICoreUrlCollection other) => Equals(other as ICorePlaylistCollection);

        /// <inheritdoc />
        public async ValueTask DisposeAsync()
        {
            DetachEvents(_preferredSource);

            await _sources.InParallel(x => x.DisposeAsync().AsTask());

            await _playlistMap.DisposeAsync();
            await _imageMap.DisposeAsync();
            await _urlMap.DisposeAsync();
        }
    }
}