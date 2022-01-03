using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Toolkit.Diagnostics;
using OwlCore.Events;
using OwlCore.Extensions;
using StrixMusic.Sdk.Models.Base;
using StrixMusic.Sdk.Models.Core;
using StrixMusic.Sdk.Extensions;
using StrixMusic.Sdk.MediaPlayback;

namespace StrixMusic.Sdk.Models.Merged
{
    /// <summary>
    /// A concrete class that merged multiple <see cref="ICoreArtistCollection"/>s.
    /// </summary>
    public class MergedTrackCollection : ITrackCollection, IMergedMutable<ICoreTrackCollection>
    {
        private readonly List<ICoreTrackCollection> _sources;
        private readonly ICoreTrackCollection _preferredSource;
        private readonly MergedCollectionMap<ITrackCollection, ICoreTrackCollection, ITrack, ICoreTrack> _trackMap;
        private readonly MergedCollectionMap<IImageCollection, ICoreImageCollection, IImage, ICoreImage> _imageMap;
        private readonly MergedCollectionMap<IUrlCollection, ICoreUrlCollection, IUrl, ICoreUrl> _urlMap;
        private readonly List<ICore> _sourceCores;

        /// <summary>
        /// Creates a new instance of <see cref="MergedTrackCollection"/>.
        /// </summary>
        /// <param name="collections">The collections to merge in.</param>
        public MergedTrackCollection(IEnumerable<ICoreTrackCollection> collections)
        {
            _sources = collections.ToList();
            _sourceCores = _sources.Select(x => x.SourceCore).ToList();

            _preferredSource = _sources[0];

            _imageMap = new MergedCollectionMap<IImageCollection, ICoreImageCollection, IImage, ICoreImage>(this);
            _trackMap = new MergedCollectionMap<ITrackCollection, ICoreTrackCollection, ITrack, ICoreTrack>(this);
            _urlMap = new MergedCollectionMap<IUrlCollection, ICoreUrlCollection, IUrl, ICoreUrl>(this);

            foreach (var item in _sources)
            {
                TotalTrackCount += item.TotalTrackCount;
                TotalImageCount += item.TotalImageCount;
                TotalUrlCount += item.TotalUrlCount;
            }

            AttachEvents(_preferredSource);
        }

        private void AttachEvents(ICoreTrackCollection source)
        {
            AttachPlayableEvents(source);

            source.IsPlayTrackCollectionAsyncAvailableChanged += IsPlayTrackCollectionAsyncAvailableChanged;
            source.IsPauseTrackCollectionAsyncAvailableChanged += IsPauseTrackCollectionAsyncAvailableChanged;

            _trackMap.ItemsChanged += TrackMap_ItemsChanged;
            _trackMap.ItemsCountChanged += TrackMap_ItemsCountChanged;
            _imageMap.ItemsChanged += ImageCollectionMap_ItemsChanged;
            _imageMap.ItemsCountChanged += ImageCollectionMap_ItemsCountChanged;
            _urlMap.ItemsChanged += UrlCollectionMap_ItemsChanged;
            _urlMap.ItemsCountChanged += UrlCollectionMap_ItemsCountChanged;
        }

        private void DetachEvents(ICoreTrackCollection source)
        {
            DetachPlayableEvents(source);

            source.IsPlayTrackCollectionAsyncAvailableChanged -= IsPlayTrackCollectionAsyncAvailableChanged;
            source.IsPauseTrackCollectionAsyncAvailableChanged -= IsPauseTrackCollectionAsyncAvailableChanged;

            _trackMap.ItemsChanged -= TrackMap_ItemsChanged;
            _trackMap.ItemsCountChanged -= TrackMap_ItemsCountChanged;
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

        private void TrackMap_ItemsCountChanged(object sender, int e)
        {
            TotalTrackCount = e;
            TracksCountChanged?.Invoke(this, e);
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

        private void TrackMap_ItemsChanged(object sender, IReadOnlyList<CollectionChangedItem<ITrack>> addedItems, IReadOnlyList<CollectionChangedItem<ITrack>> removedItems)
        {
            TracksChanged?.Invoke(this, addedItems, removedItems);
        }

        private void ImageCollectionMap_ItemsChanged(object sender, IReadOnlyList<CollectionChangedItem<IImage>> addedItems, IReadOnlyList<CollectionChangedItem<IImage>> removedItems)
        {
            ImagesChanged?.Invoke(this, addedItems, removedItems);
        }

        private void UrlCollectionMap_ItemsChanged(object sender, IReadOnlyList<CollectionChangedItem<IUrl>> addedItems, IReadOnlyList<CollectionChangedItem<IUrl>> removedItems)
        {
            UrlsChanged?.Invoke(this, addedItems, removedItems);
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
        public event EventHandler<int>? TracksCountChanged;

        /// <inheritdoc />
        public event EventHandler<int>? ImagesCountChanged;

        /// <inheritdoc />
        public event EventHandler<int>? UrlsCountChanged;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<ITrack>? TracksChanged;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<IImage>? ImagesChanged;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<IUrl>? UrlsChanged;

        /// <inheritdoc />
        public string Id => _preferredSource.Id;

        /// <inheritdoc />
        public string Name => _preferredSource.Name;

        /// <inheritdoc />
        public string? Description => _preferredSource.Description;

        /// <inheritdoc />
        public PlaybackState PlaybackState => _preferredSource.PlaybackState;

        /// <inheritdoc />
        public TimeSpan Duration => _preferredSource.Duration;

        /// <inheritdoc />
        public DateTime? LastPlayed => _preferredSource.LastPlayed;

        /// <inheritdoc />
        public DateTime? AddedAt => _preferredSource.AddedAt;

        /// <inheritdoc />
        public int TotalTrackCount { get; private set; }

        /// <inheritdoc />
        public int TotalImageCount { get; private set; }

        /// <inheritdoc />
        public int TotalUrlCount { get; private set; }

        /// <inheritdoc />
        public bool IsPlayTrackCollectionAsyncAvailable => _preferredSource.IsPlayTrackCollectionAsyncAvailable;

        /// <inheritdoc />
        public bool IsPauseTrackCollectionAsyncAvailable => _preferredSource.IsPauseTrackCollectionAsyncAvailable;

        /// <inheritdoc />
        public bool IsChangeNameAsyncAvailable => _preferredSource.IsChangeNameAsyncAvailable;

        /// <inheritdoc />
        public bool IsChangeDescriptionAsyncAvailable => _preferredSource.IsChangeDescriptionAsyncAvailable;

        /// <inheritdoc />
        public bool IsChangeDurationAsyncAvailable => _preferredSource.IsChangeDurationAsyncAvailable;

        /// <inheritdoc />
        public Task PlayTrackCollectionAsync() => _preferredSource.PlayTrackCollectionAsync();

        /// <inheritdoc />
        public Task PauseTrackCollectionAsync() => _preferredSource.PauseTrackCollectionAsync();

        /// <inheritdoc />
        public Task PlayTrackCollectionAsync(ITrack track)
        {
            var targetCore = _preferredSource.SourceCore;
            var source = track.GetSources<ICoreTrack>().FirstOrDefault(x => x.SourceCore.InstanceId == targetCore.InstanceId);

            Guard.IsNotNull(source, nameof(source));

            return _preferredSource.PlayTrackCollectionAsync(source);
        }

        /// <inheritdoc />
        public Task ChangeNameAsync(string name) => _preferredSource.ChangeNameAsync(name);

        /// <inheritdoc />
        public Task ChangeDescriptionAsync(string? description) => _preferredSource.ChangeDescriptionAsync(description);

        /// <inheritdoc />
        public Task ChangeDurationAsync(TimeSpan duration) => _preferredSource.ChangeDurationAsync(duration);

        /// <inheritdoc />
        public Task<bool> IsAddTrackAvailableAsync(int index) => _trackMap.IsAddItemAvailableAsync(index);

        /// <inheritdoc />
        public Task<bool> IsAddImageAvailableAsync(int index) => _imageMap.IsAddItemAvailableAsync(index);

        /// <inheritdoc />
        public Task<bool> IsAddUrlAvailableAsync(int index) => _urlMap.IsAddItemAvailableAsync(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveTrackAvailableAsync(int index) => _trackMap.IsRemoveItemAvailableAsync(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveImageAvailableAsync(int index) => _imageMap.IsRemoveItemAvailableAsync(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveUrlAvailableAsync(int index) => _urlMap.IsRemoveItemAvailableAsync(index);

        /// <inheritdoc cref="IMerged{T}.Sources" />
        public IReadOnlyList<ICore> SourceCores => _sourceCores;

        /// <inheritdoc />
        IReadOnlyList<ICoreTrackCollection> IMerged<ICoreTrackCollection>.Sources => _sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreImageCollection> IMerged<ICoreImageCollection>.Sources => _sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreUrlCollection> IMerged<ICoreUrlCollection>.Sources => _sources;

        /// <inheritdoc />
        public Task<IReadOnlyList<ITrack>> GetTracksAsync(int limit, int offset) => _trackMap.GetItemsAsync(limit, offset);

        /// <inheritdoc />
        public Task<IReadOnlyList<IImage>> GetImagesAsync(int limit, int offset) => _imageMap.GetItemsAsync(limit, offset);

        /// <inheritdoc />
        public Task<IReadOnlyList<IUrl>> GetUrlsAsync(int limit, int offset) => _urlMap.GetItemsAsync(limit, offset);

        /// <inheritdoc />
        public Task AddTrackAsync(ITrack track, int index) => _trackMap.InsertItem(track, index);

        /// <inheritdoc />
        public Task AddImageAsync(IImage image, int index) => _imageMap.InsertItem(image, index);

        /// <inheritdoc />
        public Task AddUrlAsync(IUrl url, int index) => _urlMap.InsertItem(url, index);

        /// <inheritdoc />
        public Task RemoveTrackAsync(int index) => _trackMap.RemoveAt(index);

        /// <inheritdoc />
        public Task RemoveImageAsync(int index) => _imageMap.RemoveAt(index);

        /// <inheritdoc />
        public Task RemoveUrlAsync(int index) => _urlMap.RemoveAt(index);

        /// <inheritdoc />
        void IMergedMutable<ICoreTrackCollection>.AddSource(ICoreTrackCollection itemToMerge)
        {
            Guard.IsNotNull(itemToMerge, nameof(itemToMerge));

            _sources.Add(itemToMerge);
            _sourceCores.Add(itemToMerge.SourceCore);

            _trackMap.Cast<IMergedMutable<ICoreTrackCollection>>().AddSource(itemToMerge);
            _imageMap.Cast<IMergedMutable<ICoreImageCollection>>().AddSource(itemToMerge);
            _urlMap.Cast<IMergedMutable<ICoreUrlCollection>>().AddSource(itemToMerge);
        }

        /// <inheritdoc />
        void IMergedMutable<ICoreTrackCollection>.RemoveSource(ICoreTrackCollection itemToRemove)
        {
            Guard.IsNotNull(itemToRemove, nameof(itemToRemove));

            _sources.Remove(itemToRemove);
            _sourceCores.Remove(itemToRemove.SourceCore);

            _trackMap.Cast<IMergedMutable<ICoreTrackCollection>>().RemoveSource(itemToRemove);
            _imageMap.Cast<IMergedMutable<ICoreImageCollection>>().RemoveSource(itemToRemove);
            _urlMap.Cast<IMergedMutable<ICoreUrlCollection>>().RemoveSource(itemToRemove);
        }

        /// <inheritdoc />
        public bool Equals(ICoreTrackCollection? other)
        {
            return other?.Name == Name;
        }

        /// <inheritdoc />
        public bool Equals(ICoreImageCollection other) => Equals(other as ICoreTrackCollection);

        /// <inheritdoc />
        public bool Equals(ICoreUrlCollection other) => Equals(other as ICoreTrackCollection);

        /// <inheritdoc />
        public async ValueTask DisposeAsync()
        {
            DetachEvents(_preferredSource);

            await _sources.InParallel(x => x.DisposeAsync().AsTask());

            await _trackMap.DisposeAsync();
            await _urlMap.DisposeAsync();
            await _imageMap.DisposeAsync();
        }
    }
}