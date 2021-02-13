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
    /// A concrete class that merged multiple <see cref="ICoreArtistCollection"/>s.
    /// </summary>
    public class MergedTrackCollection : ITrackCollection, IMergedMutable<ICoreTrackCollection>
    {
        private readonly List<ICoreTrackCollection> _sources;
        private readonly ICoreTrackCollection _preferredSource;
        private readonly MergedCollectionMap<ITrackCollection, ICoreTrackCollection, ITrack, ICoreTrack> _trackMap;
        private readonly MergedCollectionMap<IImageCollection, ICoreImageCollection, IImage, ICoreImage> _imageMap;
        private readonly List<ICore> _sourceCores;

        /// <summary>
        /// Creates a new instance of <see cref="MergedTrackCollection"/>.
        /// </summary>
        /// <param name="collections">The collections to merge in.</param>
        public MergedTrackCollection(IEnumerable<ICoreTrackCollection> collections)
        {
            _sources = collections?.ToList() ?? ThrowHelper.ThrowArgumentNullException<List<ICoreTrackCollection>>(nameof(collections));
            _sourceCores = _sources.Select(x => x.SourceCore).ToList();

            _preferredSource = _sources[0];

            _imageMap = new MergedCollectionMap<IImageCollection, ICoreImageCollection, IImage, ICoreImage>(this);
            _trackMap = new MergedCollectionMap<ITrackCollection, ICoreTrackCollection, ITrack, ICoreTrack>(this);

            foreach (var item in _sources)
            {
                TotalTracksCount += item.TotalTracksCount;
                TotalImageCount += item.TotalImageCount;
            }

            AttachEvents(_preferredSource);
        }

        private void AttachEvents(ICoreTrackCollection source)
        {
            AttachPlayableEvents(source);

            _imageMap.ItemsChanged += ImageCollectionMap_ItemsChanged;
            _imageMap.ItemsCountChanged += ImageCollectionMap_ItemsCountChanged;
            _trackMap.ItemsChanged += TrackMap_ItemsChanged;
            _trackMap.ItemsCountChanged += TrackMap_ItemsCountChanged;
        }

        private void DetachEvents(ICoreTrackCollection source)
        {
            DetachPlayableEvents(source);

            _imageMap.ItemsChanged -= ImageCollectionMap_ItemsChanged;
            _imageMap.ItemsCountChanged -= ImageCollectionMap_ItemsCountChanged;
            _trackMap.ItemsChanged -= TrackMap_ItemsChanged;
            _trackMap.ItemsCountChanged -= TrackMap_ItemsCountChanged;
        }

        private void AttachPlayableEvents(IPlayable source)
        {
            source.PlaybackStateChanged += PlaybackStateChanged;
            source.NameChanged += NameChanged;
            source.DescriptionChanged += DescriptionChanged;
            source.UrlChanged += UrlChanged;
            source.DurationChanged += DurationChanged;
            source.LastPlayedChanged += LastPlayedChanged;
            source.IsPlayAsyncAvailableChanged += IsPlayAsyncAvailableChanged;
            source.IsPauseAsyncAvailableChanged += IsPauseAsyncAvailableChanged;
            source.IsChangeNameAsyncAvailableChanged += IsChangeNameAsyncAvailableChanged;
            source.IsChangeDurationAsyncAvailableChanged += IsChangeDurationAsyncAvailableChanged;
            source.IsChangeDescriptionAsyncAvailableChanged += IsChangeDescriptionAsyncAvailableChanged;
        }

        private void DetachPlayableEvents(IPlayable source)
        {
            source.PlaybackStateChanged -= PlaybackStateChanged;
            source.NameChanged -= NameChanged;
            source.DescriptionChanged -= DescriptionChanged;
            source.UrlChanged -= UrlChanged;
            source.DurationChanged -= DurationChanged;
            source.LastPlayedChanged -= LastPlayedChanged;
            source.IsPlayAsyncAvailableChanged -= IsPlayAsyncAvailableChanged;
            source.IsPauseAsyncAvailableChanged -= IsPauseAsyncAvailableChanged;
            source.IsChangeNameAsyncAvailableChanged -= IsChangeNameAsyncAvailableChanged;
            source.IsChangeDurationAsyncAvailableChanged -= IsChangeDurationAsyncAvailableChanged;
            source.IsChangeDescriptionAsyncAvailableChanged -= IsChangeDescriptionAsyncAvailableChanged;
        }

        private void ImageCollectionMap_ItemsCountChanged(object sender, int e)
        {
            TotalImageCount = e;
            ImagesCountChanged?.Invoke(this, e);
        }

        private void TrackMap_ItemsCountChanged(object sender, int e)
        {
            TotalTracksCount = e;
            TrackItemsCountChanged?.Invoke(this, e);
        }

        private void ImageCollectionMap_ItemsChanged(object sender, IReadOnlyList<CollectionChangedItem<IImage>> addedItems, IReadOnlyList<CollectionChangedItem<IImage>> removedItems)
        {
            ImagesChanged?.Invoke(this, addedItems, removedItems);
        }

        private void TrackMap_ItemsChanged(object sender, IReadOnlyList<CollectionChangedItem<ITrack>> addedItems, IReadOnlyList<CollectionChangedItem<ITrack>> removedItems)
        {
            TrackItemsChanged?.Invoke(this, addedItems, removedItems);
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
        public event EventHandler<bool>? IsPlayAsyncAvailableChanged;

        /// <inheritdoc />
        public event EventHandler<bool>? IsPauseAsyncAvailableChanged;

        /// <inheritdoc />
        public event EventHandler<bool>? IsChangeNameAsyncAvailableChanged;

        /// <inheritdoc />
        public event EventHandler<bool>? IsChangeDescriptionAsyncAvailableChanged;

        /// <inheritdoc />
        public event EventHandler<bool>? IsChangeDurationAsyncAvailableChanged;

        /// <inheritdoc />
        public event EventHandler<int>? ImagesCountChanged;

        /// <inheritdoc />
        public event EventHandler<int>? TrackItemsCountChanged;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<IImage>? ImagesChanged;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<ITrack>? TrackItemsChanged;

        /// <inheritdoc />
        public string Id => _preferredSource.Id;

        /// <inheritdoc />
        public Uri? Url => _preferredSource.Url;

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
        public int TotalTracksCount { get; private set; }

        /// <inheritdoc />
        public int TotalImageCount { get; private set; }

        /// <inheritdoc />
        public bool IsPlayAsyncAvailable => _preferredSource.IsPlayAsyncAvailable;

        /// <inheritdoc />
        public bool IsPauseAsyncAvailable => _preferredSource.IsPauseAsyncAvailable;

        /// <inheritdoc />
        public bool IsChangeNameAsyncAvailable => _preferredSource.IsChangeNameAsyncAvailable;

        /// <inheritdoc />
        public bool IsChangeDescriptionAsyncAvailable => _preferredSource.IsChangeDescriptionAsyncAvailable;

        /// <inheritdoc />
        public bool IsChangeDurationAsyncAvailable => _preferredSource.IsChangeDurationAsyncAvailable;

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
        public Task<bool> IsAddTrackAvailable(int index)
        {
            return _preferredSource.IsAddTrackAvailable(index);
        }

        /// <inheritdoc />
        public Task<bool> IsRemoveTrackAvailable(int index)
        {
            return _preferredSource.IsRemoveTrackAvailable(index);
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
        public Task RemoveTrackAsync(int index)
        {
            return _preferredSource.RemoveTrackAsync(index);
        }

        /// <inheritdoc />
        public Task RemoveImageAsync(int index)
        {
            return _preferredSource.RemoveImageAsync(index);
        }

        /// <inheritdoc cref="IMerged{T}.Sources" />
        public IReadOnlyList<ICore> SourceCores => _sourceCores;

        /// <inheritdoc />
        IReadOnlyList<ICoreImageCollection> IMerged<ICoreImageCollection>.Sources => _sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreTrackCollection> IMerged<ICoreTrackCollection>.Sources => _sources;

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
        public Task<IReadOnlyList<ITrack>> GetTracksAsync(int limit, int offset)
        {
            return _trackMap.GetItems(limit, offset);
        }

        /// <inheritdoc />
        public Task AddTrackAsync(ITrack track, int index)
        {
            return _trackMap.InsertItem(track, index);
        }

        /// <inheritdoc />
        void IMergedMutable<ICoreTrackCollection>.AddSource(ICoreTrackCollection itemToMerge)
        {
            Guard.IsNotNull(itemToMerge, nameof(itemToMerge));

            _sources.Add(itemToMerge);
            _sourceCores.Add(itemToMerge.SourceCore);

            _trackMap.Cast<IMergedMutable<ICoreTrackCollection>>().AddSource(itemToMerge);
            _imageMap.Cast<IMergedMutable<ICoreImageCollection>>().AddSource(itemToMerge);
        }

        /// <inheritdoc />
        void IMergedMutable<ICoreTrackCollection>.RemoveSource(ICoreTrackCollection itemToRemove)
        {
            Guard.IsNotNull(itemToRemove, nameof(itemToRemove));

            _sources.Remove(itemToRemove);
            _sourceCores.Remove(itemToRemove.SourceCore);

            _trackMap.Cast<IMergedMutable<ICoreTrackCollection>>().RemoveSource(itemToRemove);
            _imageMap.Cast<IMergedMutable<ICoreImageCollection>>().RemoveSource(itemToRemove);
        }

        /// <inheritdoc />
        public bool Equals(ICoreTrackCollection? other)
        {
            return other?.Name == Name;
        }

        /// <inheritdoc />
        public bool Equals(ICoreImageCollection other) => Equals(other as ICoreTrackCollection);
    }
}