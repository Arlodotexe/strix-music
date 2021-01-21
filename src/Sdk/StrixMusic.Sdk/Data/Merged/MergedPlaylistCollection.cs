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

            _imageMap.ItemsChanged += ImageCollectionMap_ItemsChanged;
            _imageMap.ItemsCountChanged += ImageCollectionMap_ItemsCountChanged;
            _playlistMap.ItemsChanged += PlaylistMap_ItemsChanged;
            _playlistMap.ItemsCountChanged += PlaylistMap_ItemsCountChanged;
        }

        private void DetachEvents(ICorePlaylistCollection source)
        {
            DetachPlayableEvents(source);

            _imageMap.ItemsChanged -= ImageCollectionMap_ItemsChanged;
            _imageMap.ItemsCountChanged -= ImageCollectionMap_ItemsCountChanged;
            _playlistMap.ItemsChanged -= PlaylistMap_ItemsChanged;
            _playlistMap.ItemsCountChanged -= PlaylistMap_ItemsCountChanged;
        }

        private void AttachPlayableEvents(IPlayable source)
        {
            source.PlaybackStateChanged += PlaybackStateChanged;
            source.NameChanged += NameChanged;
            source.DescriptionChanged += DescriptionChanged;
            source.UrlChanged += UrlChanged;
            source.DurationChanged += DurationChanged;
            source.LastPlayedChanged += LastPlayedChanged;
        }

        private void DetachPlayableEvents(IPlayable source)
        {
            source.PlaybackStateChanged -= PlaybackStateChanged;
            source.NameChanged -= NameChanged;
            source.DescriptionChanged -= DescriptionChanged;
            source.UrlChanged -= UrlChanged;
            source.DurationChanged -= DurationChanged;
            source.LastPlayedChanged -= LastPlayedChanged;
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

        private void ImageCollectionMap_ItemsChanged(object sender, IReadOnlyList<CollectionChangedEventItem<IImage>> addedItems, IReadOnlyList<CollectionChangedEventItem<IImage>> removedItems)
        {
            ImagesChanged?.Invoke(this, addedItems, removedItems);
        }

        private void PlaylistMap_ItemsChanged(object sender, IReadOnlyList<CollectionChangedEventItem<IPlaylistCollectionItem>> addedItems, IReadOnlyList<CollectionChangedEventItem<IPlaylistCollectionItem>> removedItems)
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
        public bool IsPlayAsyncSupported => _preferredSource.IsPlayAsyncSupported;

        /// <inheritdoc />
        public bool IsPauseAsyncSupported => _preferredSource.IsPauseAsyncSupported;

        /// <inheritdoc />
        public bool IsChangeNameAsyncSupported => _preferredSource.IsChangeNameAsyncSupported;

        /// <inheritdoc />
        public bool IsChangeDescriptionAsyncSupported => _preferredSource.IsChangeDescriptionAsyncSupported;

        /// <inheritdoc />
        public bool IsChangeDurationAsyncSupported => _preferredSource.IsChangeDurationAsyncSupported;

        /// <inheritdoc />
        public Task PlayAsync()
        {
            return _preferredSource.PlayAsync();
        }

        /// <inheritdoc />
        public Task PauseAsync()
        {
            return _preferredSource.PauseAsync();
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
        public Task<bool> IsAddPlaylistItemSupported(int index)
        {
            return _preferredSource.IsAddPlaylistItemSupported(index);
        }

        /// <inheritdoc />
        public Task<bool> IsRemovePlaylistItemSupported(int index)
        {
            return _preferredSource.IsRemovePlaylistItemSupported(index);
        }

        /// <inheritdoc />
        public Task<bool> IsAddImageSupported(int index)
        {
            return _preferredSource.IsAddImageSupported(index);
        }

        /// <inheritdoc />
        public Task<bool> IsRemoveImageSupported(int index)
        {
            return _preferredSource.IsRemoveImageSupported(index);
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