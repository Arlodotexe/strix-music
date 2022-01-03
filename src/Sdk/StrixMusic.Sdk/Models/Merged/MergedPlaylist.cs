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
    /// Merged multiple <see cref="ICoreAlbum"/> into a single <see cref="IAlbum"/>
    /// </summary>
    public class MergedPlaylist : IPlaylist, IMergedMutable<ICorePlaylist>, IMergedMutable<ICorePlaylistCollectionItem>
    {
        private readonly List<ICorePlaylist> _sources;
        private readonly ICorePlaylist _preferredSource;
        private readonly List<ICore> _sourceCores;

        private readonly MergedCollectionMap<ITrackCollection, ICoreTrackCollection, ITrack, ICoreTrack> _trackCollectionMap;
        private readonly MergedCollectionMap<IImageCollection, ICoreImageCollection, IImage, ICoreImage> _imageCollectionMap;
        private readonly MergedCollectionMap<IUrlCollection, ICoreUrlCollection, IUrl, ICoreUrl> _urlCollectionMap;

        /// <summary>
        /// Creates a new instance of <see cref="MergedPlaylist"/>.
        /// </summary>
        /// <param name="sources"></param>
        public MergedPlaylist(IEnumerable<ICorePlaylist> sources)
        {
            _sources = sources.ToList();
            _sourceCores = _sources.Select(x => x.SourceCore).ToList();

            _trackCollectionMap = new MergedCollectionMap<ITrackCollection, ICoreTrackCollection, ITrack, ICoreTrack>(this);
            _imageCollectionMap = new MergedCollectionMap<IImageCollection, ICoreImageCollection, IImage, ICoreImage>(this);
            _urlCollectionMap = new MergedCollectionMap<IUrlCollection, ICoreUrlCollection, IUrl, ICoreUrl>(this);

            // TODO: Get the actual preferred source.
            _preferredSource = _sources[0];

            Name = _preferredSource.Name;
            Description = _preferredSource.Description;
            PlaybackState = _preferredSource.PlaybackState;
            LastPlayed = _preferredSource.LastPlayed;
            AddedAt = _preferredSource.AddedAt;

            foreach (var source in _sources)
            {
                TotalImageCount += source.TotalImageCount;
                TotalTrackCount += source.TotalTrackCount;
                TotalUrlCount += source.TotalUrlCount;
            }

            if (_preferredSource.RelatedItems != null)
            {
                RelatedItems = new MergedPlayableCollectionGroup(_preferredSource.RelatedItems.IntoList());
            }

            if (_preferredSource.Owner != null)
            {
                Owner = new CoreUserProfileProxy(_preferredSource.Owner);
            }

            AttachEvents(_preferredSource);
        }

        /// <inheritdoc/>
        public event CollectionChangedEventHandler<ITrack>? TracksChanged;

        /// <inheritdoc/>
        public event EventHandler<int>? TracksCountChanged;

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
        public event EventHandler<bool>? IsPlayTrackCollectionAsyncAvailableChanged;

        /// <inheritdoc />
        public event EventHandler<bool>? IsPauseTrackCollectionAsyncAvailableChanged;

        /// <inheritdoc />
        public event EventHandler<bool>? IsChangeNameAsyncAvailableChanged;

        /// <inheritdoc />
        public event EventHandler<bool>? IsChangeDescriptionAsyncAvailableChanged;

        /// <inheritdoc />
        public event EventHandler<bool>? IsChangeDurationAsyncAvailableChanged;

        /// <inheritdoc/>
        public event CollectionChangedEventHandler<IImage>? ImagesChanged;

        /// <inheritdoc/>
        public event EventHandler<int>? ImagesCountChanged;

        /// <inheritdoc/>
        public event CollectionChangedEventHandler<IUrl>? UrlsChanged;

        /// <inheritdoc/>
        public event EventHandler<int>? UrlsCountChanged;

        private void AttachEvents(ICorePlaylist source)
        {
            AttachPlayableEvents(source);

            source.IsPlayTrackCollectionAsyncAvailableChanged += IsPlayTrackCollectionAsyncAvailableChanged;
            source.IsPauseTrackCollectionAsyncAvailableChanged += IsPauseTrackCollectionAsyncAvailableChanged;

            _trackCollectionMap.ItemsCountChanged += TrackCollectionMap_ItemsCountChanged;
            _trackCollectionMap.ItemsChanged += TrackCollectionMap_ItemsChanged;
            _imageCollectionMap.ItemsChanged += ImageCollectionMap_ItemsChanged;
            _imageCollectionMap.ItemsCountChanged += ImageCollectionMap_ItemsCountChanged;
            _urlCollectionMap.ItemsChanged += UrlCollectionMap_ItemsChanged;
            _urlCollectionMap.ItemsCountChanged += UrlCollectionMap_ItemsCountChanged;
        }

        private void DetachEvents(ICorePlaylist source)
        {
            DetachPlayableEvents(source);

            source.IsPlayTrackCollectionAsyncAvailableChanged += IsPlayTrackCollectionAsyncAvailableChanged;
            source.IsPauseTrackCollectionAsyncAvailableChanged += IsPauseTrackCollectionAsyncAvailableChanged;

            _trackCollectionMap.ItemsChanged -= TrackCollectionMap_ItemsChanged;
            _trackCollectionMap.ItemsCountChanged -= TrackCollectionMap_ItemsCountChanged;
            _imageCollectionMap.ItemsChanged -= ImageCollectionMap_ItemsChanged;
            _imageCollectionMap.ItemsCountChanged -= ImageCollectionMap_ItemsCountChanged;
            _urlCollectionMap.ItemsChanged -= UrlCollectionMap_ItemsChanged;
            _urlCollectionMap.ItemsCountChanged -= UrlCollectionMap_ItemsCountChanged;
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

        private void TrackCollectionMap_ItemsChanged(object sender, IReadOnlyList<CollectionChangedItem<ITrack>> addedItems, IReadOnlyList<CollectionChangedItem<ITrack>> removedItems)
        {
            TracksChanged?.Invoke(this, addedItems, removedItems);
        }

        private void TrackCollectionMap_ItemsCountChanged(object sender, int e)
        {
            TotalTrackCount = e;
            TracksCountChanged?.Invoke(this, e);
        }

        private void ImageCollectionMap_ItemsCountChanged(object sender, int e)
        {
            TotalImageCount = e;
            ImagesCountChanged?.Invoke(this, e);
        }

        private void ImageCollectionMap_ItemsChanged(object sender, IReadOnlyList<CollectionChangedItem<IImage>> addedItems, IReadOnlyList<CollectionChangedItem<IImage>> removedItems)
        {
            ImagesChanged?.Invoke(this, addedItems, removedItems);
        }

        private void UrlCollectionMap_ItemsCountChanged(object sender, int e)
        {
            TotalUrlCount = e;
            UrlsCountChanged?.Invoke(this, e);
        }

        private void UrlCollectionMap_ItemsChanged(object sender, IReadOnlyList<CollectionChangedItem<IUrl>> addedItems, IReadOnlyList<CollectionChangedItem<IUrl>> removedItems)
        {
            UrlsChanged?.Invoke(this, addedItems, removedItems);
        }

        /// <inheritdoc/>
        public Task AddImageAsync(IImage image, int index) => _imageCollectionMap.InsertItem(image, index);

        /// <inheritdoc/>
        public Task AddUrlAsync(IUrl url, int index) => _urlCollectionMap.InsertItem(url, index);

        /// <inheritdoc/>
        void IMergedMutable<ICorePlaylist>.AddSource(ICorePlaylist itemToMerge)
        {
            Guard.IsNotNull(itemToMerge, nameof(itemToMerge));

            if (!Equals(itemToMerge))
                ThrowHelper.ThrowArgumentException(nameof(itemToMerge), "Tried to merge an artist that doesn't match. Verify that the item matches before merging the source.");

            _sources.Add(itemToMerge);
            _sourceCores.Add(itemToMerge.SourceCore);

            _trackCollectionMap.Cast<IMergedMutable<ICoreTrackCollection>>().AddSource(itemToMerge);
            _imageCollectionMap.Cast<IMergedMutable<ICoreImageCollection>>().AddSource(itemToMerge);
        }

        /// <inheritdoc />
        void IMergedMutable<ICorePlaylist>.RemoveSource(ICorePlaylist itemToRemove)
        {
            Guard.IsNotNull(itemToRemove, nameof(itemToRemove));

            _sources.Remove(itemToRemove);
            _sourceCores.Remove(itemToRemove.SourceCore);

            _trackCollectionMap.Cast<IMergedMutable<ICoreTrackCollection>>().RemoveSource(itemToRemove);
            _imageCollectionMap.Cast<IMergedMutable<ICoreImageCollection>>().RemoveSource(itemToRemove);
        }

        /// <inheritdoc />
        void IMergedMutable<ICorePlaylistCollectionItem>.AddSource(ICorePlaylistCollectionItem itemToMerge)
        {
            if (itemToMerge is ICorePlaylist col)
                ((IMergedMutable<ICorePlaylist>)this).AddSource(col);
        }

        /// <inheritdoc />
        void IMergedMutable<ICorePlaylistCollectionItem>.RemoveSource(ICorePlaylistCollectionItem itemToRemove)
        {
            if (itemToRemove is ICorePlaylist col)
                ((IMergedMutable<ICorePlaylist>)this).RemoveSource(col);
        }

        /// <inheritdoc/>
        public Task AddTrackAsync(ITrack track, int index) => _trackCollectionMap.InsertItem(track, index);

        /// <inheritdoc/>
        public Task ChangeDescriptionAsync(string? description) => _preferredSource.ChangeDescriptionAsync(description);

        /// <inheritdoc/>
        public Task ChangeDurationAsync(TimeSpan duration) => _preferredSource.ChangeDurationAsync(duration);

        /// <inheritdoc/>
        public Task ChangeNameAsync(string name) => _preferredSource.ChangeNameAsync(name);

        /// <inheritdoc/>
        public Task<IReadOnlyList<ITrack>> GetTracksAsync(int limit, int offset) => _trackCollectionMap.GetItemsAsync(limit, offset);

        /// <inheritdoc/>
        public Task<IReadOnlyList<IImage>> GetImagesAsync(int limit, int offset) => _imageCollectionMap.GetItemsAsync(limit, offset);

        /// <inheritdoc/>
        public Task<IReadOnlyList<IUrl>> GetUrlsAsync(int limit, int offset) => _urlCollectionMap.GetItemsAsync(limit, offset);

        /// <inheritdoc/>
        public Task<bool> IsAddTrackAvailableAsync(int index) => _trackCollectionMap.IsAddItemAvailableAsync(index);

        /// <inheritdoc/>
        public Task<bool> IsAddImageAvailableAsync(int index) => _imageCollectionMap.IsAddItemAvailableAsync(index);

        /// <inheritdoc/>
        public Task<bool> IsAddUrlAvailableAsync(int index) => _urlCollectionMap.IsAddItemAvailableAsync(index);

        /// <inheritdoc/>
        public Task<bool> IsRemoveTrackAvailableAsync(int index) => _trackCollectionMap.IsRemoveItemAvailableAsync(index);

        /// <inheritdoc/>
        public Task<bool> IsRemoveImageAvailableAsync(int index) => _imageCollectionMap.IsRemoveItemAvailableAsync(index);

        /// <inheritdoc/>
        public Task<bool> IsRemoveUrlAvailableAsync(int index) => _urlCollectionMap.IsRemoveItemAvailableAsync(index);

        /// <inheritdoc/>
        public Task PauseTrackCollectionAsync() => _preferredSource.PauseTrackCollectionAsync();

        /// <inheritdoc/>
        public Task PlayTrackCollectionAsync() => _preferredSource.PlayTrackCollectionAsync();

        /// <inheritdoc />
        public Task PlayTrackCollectionAsync(ITrack track)
        {
            var targetCore = _preferredSource.SourceCore;
            var source = track.GetSources<ICoreTrack>().FirstOrDefault(x => x.SourceCore.InstanceId == targetCore.InstanceId);

            Guard.IsNotNull(source, nameof(source));

            return _preferredSource.PlayTrackCollectionAsync(source);
        }

        /// <inheritdoc/>
        public Task RemoveTrackAsync(int index) => _trackCollectionMap.RemoveAt(index);

        /// <inheritdoc/>
        public Task RemoveImageAsync(int index) => _imageCollectionMap.RemoveAt(index);

        /// <inheritdoc/>
        public Task RemoveUrlAsync(int index) => _imageCollectionMap.RemoveAt(index);

        /// <inheritdoc/>
        public IUserProfile? Owner { get; }

        /// <inheritdoc/>
        public IPlayableCollectionGroup? RelatedItems { get; }

        /// <inheritdoc/>
        public int TotalTrackCount { get; internal set; }

        /// <inheritdoc/>
        public int TotalImageCount { get; internal set; }

        /// <inheritdoc/>
        public int TotalUrlCount { get; internal set; }

        /// <inheritdoc/>
        public string Id => _preferredSource.Id;

        /// <inheritdoc/>
        public string Name { get; internal set; }

        /// <inheritdoc/>
        public string? Description { get; internal set; }

        /// <inheritdoc/>
        public PlaybackState PlaybackState { get; internal set; }

        /// <inheritdoc/>
        public TimeSpan Duration { get; internal set; }

        /// <inheritdoc />
        public DateTime? LastPlayed { get; internal set; }

        /// <inheritdoc />
        public DateTime? AddedAt { get; internal set; }

        /// <inheritdoc/>
        public bool IsPlayTrackCollectionAsyncAvailable => _preferredSource.IsPlayTrackCollectionAsyncAvailable;

        /// <inheritdoc/>
        public bool IsPauseTrackCollectionAsyncAvailable => _preferredSource.IsPauseTrackCollectionAsyncAvailable;

        /// <inheritdoc/>
        public bool IsChangeNameAsyncAvailable => _preferredSource.IsChangeDescriptionAsyncAvailable;

        /// <inheritdoc/>
        public bool IsChangeDescriptionAsyncAvailable => _preferredSource.IsChangeDescriptionAsyncAvailable;

        /// <inheritdoc/>
        public bool IsChangeDurationAsyncAvailable => _preferredSource.IsChangeDescriptionAsyncAvailable;

        /// <inheritdoc cref="IMerged{T}.SourceCores" />
        public IReadOnlyList<ICore> SourceCores => _sourceCores;

        /// <inheritdoc />
        IReadOnlyList<ICorePlaylistCollectionItem> IMerged<ICorePlaylistCollectionItem>.Sources => Sources;

        /// <inheritdoc/>
        IReadOnlyList<ICoreTrackCollection> IMerged<ICoreTrackCollection>.Sources => Sources;

        /// <inheritdoc/>
        IReadOnlyList<ICoreImageCollection> IMerged<ICoreImageCollection>.Sources => Sources;

        /// <inheritdoc/>
        IReadOnlyList<ICoreUrlCollection> IMerged<ICoreUrlCollection>.Sources => Sources;

        /// <inheritdoc/>
        public IReadOnlyList<ICorePlaylist> Sources => _sources;

        /// <inheritdoc />
        public bool Equals(ICoreImageCollection other) => Equals(other as ICorePlaylistCollectionItem);

        /// <inheritdoc />
        public bool Equals(ICoreUrlCollection other) => Equals(other as ICorePlaylistCollectionItem);

        /// <inheritdoc />
        public bool Equals(ICoreTrackCollection other) => Equals(other as ICorePlaylistCollectionItem);

        /// <inheritdoc cref="Equals(object?)" />
        public bool Equals(ICorePlaylist? other)
        {
            return other?.Name == Name;
        }

        /// <inheritdoc />
        public bool Equals(ICorePlaylistCollectionItem? other)
        {
            if (other is ICorePlaylist corePlaylist)
                return Equals(corePlaylist);

            return false;
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj) || (obj is ICorePlaylistCollectionItem other && Equals(other));
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

            await _sources.InParallel(x => x.DisposeAsync().AsTask());

            await _imageCollectionMap.DisposeAsync();
            await _trackCollectionMap.DisposeAsync();
        }
    }
}