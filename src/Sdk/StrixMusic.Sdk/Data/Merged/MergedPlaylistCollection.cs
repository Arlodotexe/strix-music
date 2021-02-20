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
    /// A concrete class that merged multiple <see cref="ICorePlaylistCollection"/>s.
    /// </summary>
    public class MergedPlaylistCollection : IPlaylistCollection, IMergedMutable<ICorePlaylistCollection>
    {
        private readonly List<ICorePlaylistCollection> _sources;
        private readonly List<ICore> _sourceCores;
        private readonly ICorePlaylistCollection _preferredSource;
        private readonly MergedCollectionMap<IPlaylistCollection, ICorePlaylistCollection, IPlaylistCollectionItem, ICorePlaylistCollectionItem> _playlistMap;
        private readonly MergedCollectionMap<IImageCollection, ICoreImageCollection, IImage, ICoreImage> _imageMap;

        /// <summary>
        /// Creates a new instance of <see cref="MergedPlaylistCollection"/>.
        /// </summary>
        /// <param name="collections">The collections to merge in.</param>
        public MergedPlaylistCollection(IEnumerable<ICorePlaylistCollection> collections)
        {
            _sources = collections?.ToList() ?? ThrowHelper.ThrowArgumentNullException<List<ICorePlaylistCollection>>(nameof(collections));
            _sourceCores = _sources.Select(x => x.SourceCore).ToList();

            _preferredSource = _sources[0];

            _imageMap = new MergedCollectionMap<IImageCollection, ICoreImageCollection, IImage, ICoreImage>(this);
            _playlistMap = new MergedCollectionMap<IPlaylistCollection, ICorePlaylistCollection, IPlaylistCollectionItem, ICorePlaylistCollectionItem>(this);

            foreach (var item in _sources)
            {
                TotalPlaylistItemsCount += item.TotalPlaylistItemsCount;
                TotalImageCount += item.TotalImageCount;
            }

            Url = _preferredSource.Url;
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

            _imageMap.ItemsChanged += ImageCollectionMap_ItemsChanged;
            _imageMap.ItemsCountChanged += ImageCollectionMap_ItemsCountChanged;
            _playlistMap.ItemsChanged += PlaylistMap_ItemsChanged;
            _playlistMap.ItemsCountChanged += PlaylistMap_ItemsCountChanged;
        }

        private void DetachEvents(ICorePlaylistCollection source)
        {
            DetachPlayableEvents(source);

            source.IsPlayPlaylistCollectionAsyncAvailableChanged -= IsPlayPlaylistCollectionAsyncAvailableChanged;
            source.IsPausePlaylistCollectionAsyncAvailableChanged -= IsPausePlaylistCollectionAsyncAvailableChanged;

            _imageMap.ItemsChanged -= ImageCollectionMap_ItemsChanged;
            _imageMap.ItemsCountChanged -= ImageCollectionMap_ItemsCountChanged;
            _playlistMap.ItemsChanged -= PlaylistMap_ItemsChanged;
            _playlistMap.ItemsCountChanged -= PlaylistMap_ItemsCountChanged;
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

        private void ImageCollectionMap_ItemsCountChanged(object sender, int e)
        {
            TotalImageCount = e;
            ImagesCountChanged?.Invoke(this, e);
        }

        private void PlaylistMap_ItemsCountChanged(object sender, int e)
        {
            TotalPlaylistItemsCount = e;
            PlaylistItemsCountChanged?.Invoke(this, e);
        }

        private void ImageCollectionMap_ItemsChanged(object sender, IReadOnlyList<CollectionChangedItem<IImage>> addedItems, IReadOnlyList<CollectionChangedItem<IImage>> removedItems)
        {
            ImagesChanged?.Invoke(this, addedItems, removedItems);
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
        public event EventHandler<Uri?>? UrlChanged;

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
        public event EventHandler<int>? PlaylistItemsCountChanged;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<IImage>? ImagesChanged;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<IPlaylistCollectionItem>? PlaylistItemsChanged;

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
        public int TotalPlaylistItemsCount { get; internal set; }

        /// <inheritdoc />
        public int TotalImageCount { get; internal set; }

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
        public Task PlayPlaylistCollectionAsync(IPlaylist playlist)
        {
            var targetCore = _preferredSource.SourceCore;
            var source = playlist.GetSources<ICorePlaylist>().FirstOrDefault(x => x.SourceCore.InstanceId == targetCore.InstanceId);

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
        public Task<bool> IsAddPlaylistItemAvailable(int index)
        {
            return _preferredSource.IsAddPlaylistItemAvailable(index);
        }

        /// <inheritdoc />
        public Task<bool> IsRemovePlaylistItemAvailable(int index)
        {
            return _preferredSource.IsRemovePlaylistItemAvailable(index);
        }

        /// <inheritdoc />
        public Task<bool> IsAddImageAvailable(int index)
        {
            return _preferredSource.IsAddImageAvailable(index);
        }

        /// <inheritdoc />
        public Task<bool> IsRemoveImageAvailable(int index)
        {
            return _preferredSource.IsRemoveImageAvailable(index);
        }

        /// <inheritdoc />
        public Task RemovePlaylistItemAsync(int index)
        {
            return _preferredSource.RemovePlaylistItemAsync(index);
        }

        /// <inheritdoc />
        public Task RemoveImageAsync(int index)
        {
            return _preferredSource.RemoveImageAsync(index);
        }

        /// <inheritdoc cref="IMerged{T}.SourceCores" />
        public IReadOnlyList<ICore> SourceCores => _sourceCores;

        /// <inheritdoc cref="IMerged{T}.Sources" />
        public IReadOnlyList<ICorePlaylistCollection> Sources => _sources;

        /// <inheritdoc />
        IReadOnlyList<ICorePlaylistCollectionItem> IMerged<ICorePlaylistCollectionItem>.Sources => _sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreImageCollection> IMerged<ICoreImageCollection>.Sources => _sources;

        /// <inheritdoc />
        IReadOnlyList<ICorePlaylistCollection> IMerged<ICorePlaylistCollection>.Sources => _sources;

        /// <inheritdoc />
        public Task AddImageAsync(IImage image, int index)
        {
            return _imageMap.InsertItem(image, index);
        }

        /// <inheritdoc />
        public Task<IReadOnlyList<IImage>> GetImagesAsync(int limit, int offset)
        {
            return _imageMap.GetItems(limit, offset);
        }

        /// <inheritdoc />
        public Task<IReadOnlyList<IPlaylistCollectionItem>> GetPlaylistItemsAsync(int limit, int offset)
        {
            return _playlistMap.GetItems(limit, offset);
        }

        /// <inheritdoc />
        public Task AddPlaylistItemAsync(IPlaylistCollectionItem playlist, int index)
        {
            return _playlistMap.InsertItem(playlist, index);
        }

        /// <inheritdoc />
        void IMergedMutable<ICorePlaylistCollection>.AddSource(ICorePlaylistCollection itemToMerge)
        {
            Guard.IsNotNull(itemToMerge, nameof(itemToMerge));

            _sources.Add(itemToMerge);
            _sourceCores.Remove(itemToMerge.SourceCore);

            _playlistMap.Cast<IMergedMutable<ICorePlaylistCollection>>().AddSource(itemToMerge);
            _imageMap.Cast<IMergedMutable<ICorePlaylistCollection>>().AddSource(itemToMerge);
        }

        /// <inheritdoc />
        void IMergedMutable<ICorePlaylistCollection>.RemoveSource(ICorePlaylistCollection itemToRemove)
        {
            Guard.IsNotNull(itemToRemove, nameof(itemToRemove));

            _sources.Remove(itemToRemove);
            _sourceCores.Remove(itemToRemove.SourceCore);

            _playlistMap.Cast<IMergedMutable<ICorePlaylistCollection>>().RemoveSource(itemToRemove);
            _imageMap.Cast<IMergedMutable<ICorePlaylistCollection>>().RemoveSource(itemToRemove);
        }

        /// <inheritdoc />
        public bool Equals(ICorePlaylistCollection? other)
        {
            return other?.Name == Name;
        }

        /// <inheritdoc />
        public bool Equals(ICoreImageCollection other)
        {
            return Equals(other as ICorePlaylistCollection);
        }

        /// <inheritdoc />
        public bool Equals(ICorePlaylistCollectionItem other)
        {
            return Equals(other as ICorePlaylistCollection);
        }
    }
}