using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Toolkit.Diagnostics;
using OwlCore.Collections;
using OwlCore.Events;
using OwlCore.Extensions;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.MediaPlayback;

namespace StrixMusic.Sdk.Data.Merged
{
    /// <summary>
    /// Merged multiple <see cref="ICoreAlbum"/> into a single <see cref="IAlbum"/>
    /// </summary>
    public class MergedPlaylist : IPlaylist, IMerged<ICorePlaylist>
    {
        private readonly List<ICorePlaylist> _sources;
        private readonly MergedCollectionMap<ITrackCollection, ICoreTrackCollection, ITrack, ICoreTrack> _trackCollectionMap;
        private readonly MergedCollectionMap<IImageCollection, ICoreImageCollection, IImage, ICoreImage> _imageCollectionMap;
        private readonly ICorePlaylist _preferredSource;
        private readonly List<ICore> _sourceCores;

        /// <summary>
        /// Creates a new instance of <see cref="MergedPlaylist"/>.
        /// </summary>
        /// <param name="sources"></param>
        public MergedPlaylist(IEnumerable<ICorePlaylist> sources)
        {
            _sources = sources?.ToList() ?? throw new ArgumentNullException();
            _sourceCores = _sources.Select(x => x.SourceCore).ToList();

            _trackCollectionMap = new MergedCollectionMap<ITrackCollection, ICoreTrackCollection, ITrack, ICoreTrack>(this);
            _imageCollectionMap = new MergedCollectionMap<IImageCollection, ICoreImageCollection, IImage, ICoreImage>(this);

            // TODO: Get the actual preferred source.
            _preferredSource = _sources[0];

            Url = _preferredSource.Url;
            Name = _preferredSource.Name;
            Description = _preferredSource.Description;
            PlaybackState = _preferredSource.PlaybackState;

            foreach (var source in _sources)
            {
                TotalImageCount += source.TotalImageCount;
                TotalTracksCount += source.TotalTracksCount;
            }

            if (_preferredSource.RelatedItems != null)
            {
                RelatedItems = new MergedPlayableCollectionGroup(_preferredSource.RelatedItems.IntoList());
            }

            if (_preferredSource.Owner != null)
            {
                Owner = new MergedUserProfile(_preferredSource.Owner);
            }

            AttachEvents(_preferredSource);
        }

        /// <inheritdoc/>
        public event CollectionChangedEventHandler<ITrack>? TrackItemsChanged;

        /// <inheritdoc/>
        public event EventHandler<int>? TrackItemsCountChanged;

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

        /// <inheritdoc/>
        public event CollectionChangedEventHandler<IImage>? ImagesChanged;

        /// <inheritdoc/>
        public event EventHandler<int>? ImagesCountChanged;

        private void TrackCollectionMap_ItemsChanged(object sender, IReadOnlyList<CollectionChangedEventItem<ITrack>> addedItems, IReadOnlyList<CollectionChangedEventItem<ITrack>> removedItems)
        {
            TrackItemsChanged?.Invoke(this, addedItems, removedItems);
        }

        private void TrackCollectionMap_ItemsCountChanged(object sender, int e)
        {
            TotalTracksCount = e;
            TrackItemsCountChanged?.Invoke(this, e);
        }

        private void AttachEvents(ICorePlaylist preferredSource)
        {
            AttachPropertyEvents(preferredSource);
            _trackCollectionMap.ItemsCountChanged += TrackCollectionMap_ItemsCountChanged;
            _trackCollectionMap.ItemsChanged += TrackCollectionMap_ItemsChanged;
            _imageCollectionMap.ItemsChanged += ImageCollectionMap_ItemsChanged;
            _imageCollectionMap.ItemsCountChanged += ImageCollectionMap_ItemsCountChanged;
        }

        private void ImageCollectionMap_ItemsCountChanged(object sender, int e)
        {
            TotalImageCount = e;
            ImagesCountChanged?.Invoke(this, e);
        }

        private void ImageCollectionMap_ItemsChanged(object sender, IReadOnlyList<CollectionChangedEventItem<IImage>> addedItems, IReadOnlyList<CollectionChangedEventItem<IImage>> removedItems)
        {
            ImagesChanged?.Invoke(this, addedItems, removedItems);
        }

        private void DetachEvents(ICoreAlbum source)
        {
            DetachPropertyEvents(source);
            _trackCollectionMap.ItemsChanged -= TrackCollectionMap_ItemsChanged;
            _trackCollectionMap.ItemsCountChanged -= TrackCollectionMap_ItemsCountChanged;
            _imageCollectionMap.ItemsChanged -= ImageCollectionMap_ItemsChanged;
            _imageCollectionMap.ItemsCountChanged -= ImageCollectionMap_ItemsCountChanged;
        }

        private void DetachPropertyEvents(ICoreAlbum source)
        {
            source.PlaybackStateChanged -= PlaybackStateChanged;
            source.NameChanged -= NameChanged;
            source.DescriptionChanged -= DescriptionChanged;
            source.UrlChanged -= UrlChanged;
            source.DurationChanged -= DurationChanged;
        }

        private void AttachPropertyEvents(ICorePlaylist source)
        {
            source.PlaybackStateChanged += PlaybackStateChanged;
            source.NameChanged += NameChanged;
            source.DescriptionChanged += DescriptionChanged;
            source.UrlChanged += UrlChanged;
            source.DurationChanged += DurationChanged;
        }

        /// <inheritdoc/>
        public Task AddImageAsync(IImage image, int index) => _imageCollectionMap.InsertItem(image, index);

        /// <inheritdoc/>
        public void AddSource(ICorePlaylist itemToMerge)
        {
            Guard.IsNotNull(itemToMerge, nameof(itemToMerge));

            if (!Equals(itemToMerge))
                ThrowHelper.ThrowArgumentException(nameof(itemToMerge), "Tried to merge an artist that doesn't match. Verify that the item matches before merging the source.");

            _sources.Add(itemToMerge);
            _sourceCores.Add(itemToMerge.SourceCore);

            _trackCollectionMap.AddSource(itemToMerge);
            _imageCollectionMap.AddSource(itemToMerge);
        }

        /// <inheritdoc />
        public void RemoveSource(ICorePlaylist itemToRemove)
        {
            Guard.IsNotNull(itemToRemove, nameof(itemToRemove));

            _sources.Remove(itemToRemove);
            _sourceCores.Remove(itemToRemove.SourceCore);

            _trackCollectionMap.RemoveSource(itemToRemove);
            _imageCollectionMap.RemoveSource(itemToRemove);
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
        public Task<IReadOnlyList<IImage>> GetImagesAsync(int limit, int offset) => _imageCollectionMap.GetItems(limit, offset);

        /// <inheritdoc/>
        public Task<IReadOnlyList<ITrack>> GetTracksAsync(int limit, int offset) => _trackCollectionMap.GetItems(limit, offset);

        /// <inheritdoc/>
        public Task<bool> IsAddGenreSupported(int index) => _preferredSource.IsAddGenreSupported(index);

        /// <inheritdoc/>
        public Task<bool> IsAddImageSupported(int index) => _preferredSource.IsAddImageSupported(index);

        /// <inheritdoc/>
        public Task<bool> IsAddTrackSupported(int index) => _preferredSource.IsAddTrackSupported(index);

        /// <inheritdoc/>
        public Task<bool> IsRemoveGenreSupported(int index) => _preferredSource.IsRemoveGenreSupported(index);

        /// <inheritdoc/>
        public Task<bool> IsRemoveImageSupported(int index) => _preferredSource.IsRemoveImageSupported(index);

        /// <inheritdoc/>
        public Task<bool> IsRemoveTrackSupported(int index) => _preferredSource.IsRemoveTrackSupported(index);

        /// <inheritdoc/>
        public Task PauseAsync()
        {
            return _preferredSource.PauseAsync();
        }

        /// <inheritdoc/>
        public Task PlayAsync()
        {
            return _preferredSource.PlayAsync();
        }

        /// <inheritdoc/>
        public Task RemoveImageAsync(int index)
        {
            return _preferredSource.RemoveImageAsync(index);
        }

        /// <inheritdoc/>
        public Task RemoveTrackAsync(int index)
        {
            return _preferredSource.RemoveTrackAsync(index);
        }

        /// <inheritdoc/>
        public IUserProfile? Owner { get; }

        /// <inheritdoc/>
        public IPlayableCollectionGroup? RelatedItems { get; }

        /// <inheritdoc/>
        public int TotalTracksCount { get; internal set; }

        /// <inheritdoc/>
        public int TotalImageCount { get; internal set; }

        /// <inheritdoc/>
        public string Id => _preferredSource.Id;

        /// <inheritdoc/>
        public Uri? Url { get; internal set; }

        /// <inheritdoc/>
        public string Name { get; internal set; }

        /// <inheritdoc/>
        public string? Description { get; internal set; }

        /// <inheritdoc/>
        public PlaybackState PlaybackState { get; internal set; }

        /// <inheritdoc/>
        public TimeSpan Duration { get; internal set; }

        /// <inheritdoc/>
        public bool IsPlayAsyncSupported => _preferredSource.IsPlayAsyncSupported;

        /// <inheritdoc/>
        public bool IsPauseAsyncSupported => _preferredSource.IsPauseAsyncSupported;

        /// <inheritdoc/>
        public bool IsChangeNameAsyncSupported => _preferredSource.IsChangeDescriptionAsyncSupported;

        /// <inheritdoc/>
        public bool IsChangeDescriptionAsyncSupported => _preferredSource.IsChangeDescriptionAsyncSupported;

        /// <inheritdoc/>
        public bool IsChangeDurationAsyncSupported => _preferredSource.IsChangeDescriptionAsyncSupported;

        /// <inheritdoc cref="ISdkMember{T}.SourceCores" />
        public IReadOnlyList<ICore> SourceCores => _sourceCores;

        /// <inheritdoc/>
        public SynchronizedObservableCollection<string>? Genres => _preferredSource.Genres;

        /// <inheritdoc/>
        IReadOnlyList<ICoreTrackCollection> ISdkMember<ICoreTrackCollection>.Sources => Sources;

        /// <inheritdoc/>
        IReadOnlyList<ICoreImageCollection> ISdkMember<ICoreImageCollection>.Sources => Sources;

        /// <inheritdoc/>
        IReadOnlyList<ICoreGenreCollection> ISdkMember<ICoreGenreCollection>.Sources => Sources;

        /// <inheritdoc/>
        IReadOnlyList<ICorePlaylistCollectionItem> ISdkMember<ICorePlaylistCollectionItem>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICorePlaylist> ISdkMember<ICorePlaylist>.Sources => Sources;

        /// <inheritdoc/>
        public IReadOnlyList<ICorePlaylist> Sources => _sources;

        /// <inheritdoc cref="Equals(object?)" />
        public bool Equals(ICorePlaylist? other)
        {
            return other?.Name == Name;
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj) || (obj is ICorePlaylist other && Equals(other));
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return _preferredSource.GetHashCode();
        }
    }
}