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
    /// Merged multiple <see cref="ICoreAlbum"/> into a single <see cref="IAlbum"/>
    /// </summary>
    public class MergedPlaylist : IPlaylist, IMergedMutable<ICorePlaylist>, IMergedMutable<ICorePlaylistCollectionItem>
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
            LastPlayed = _preferredSource.LastPlayed;
            AddedAt = _preferredSource.AddedAt;

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
                Owner = new CoreUserProfileProxy(_preferredSource.Owner);
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

        private void AttachEvents(ICorePlaylist source)
        {
            AttachPlayableEvents(source);

            source.IsPlayTrackCollectionAsyncAvailableChanged += IsPlayTrackCollectionAsyncAvailableChanged;
            source.IsPauseTrackCollectionAsyncAvailableChanged += IsPauseTrackCollectionAsyncAvailableChanged;

            _trackCollectionMap.ItemsCountChanged += TrackCollectionMap_ItemsCountChanged;
            _trackCollectionMap.ItemsChanged += TrackCollectionMap_ItemsChanged;
            _imageCollectionMap.ItemsChanged += ImageCollectionMap_ItemsChanged;
            _imageCollectionMap.ItemsCountChanged += ImageCollectionMap_ItemsCountChanged;
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

        private void TrackCollectionMap_ItemsChanged(object sender, IReadOnlyList<CollectionChangedItem<ITrack>> addedItems, IReadOnlyList<CollectionChangedItem<ITrack>> removedItems)
        {
            TrackItemsChanged?.Invoke(this, addedItems, removedItems);
        }

        private void TrackCollectionMap_ItemsCountChanged(object sender, int e)
        {
            TotalTracksCount = e;
            TrackItemsCountChanged?.Invoke(this, e);
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

        /// <inheritdoc/>
        public Task AddImageAsync(IImage image, int index) => _imageCollectionMap.InsertItem(image, index);

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
        public Task<IReadOnlyList<IImage>> GetImagesAsync(int limit, int offset) => _imageCollectionMap.GetItems(limit, offset);

        /// <inheritdoc/>
        public Task<IReadOnlyList<ITrack>> GetTracksAsync(int limit, int offset) => _trackCollectionMap.GetItems(limit, offset);

        /// <inheritdoc/>
        public Task<bool> IsAddGenreAvailable(int index) => _preferredSource.IsAddGenreAvailable(index);

        /// <inheritdoc/>
        public Task<bool> IsAddImageAvailable(int index) => _preferredSource.IsAddImageAvailable(index);

        /// <inheritdoc/>
        public Task<bool> IsAddTrackAvailable(int index) => _preferredSource.IsAddTrackAvailable(index);

        /// <inheritdoc/>
        public Task<bool> IsRemoveGenreAvailable(int index) => _preferredSource.IsRemoveGenreAvailable(index);

        /// <inheritdoc/>
        public Task<bool> IsRemoveImageAvailable(int index) => _preferredSource.IsRemoveImageAvailable(index);

        /// <inheritdoc/>
        public Task<bool> IsRemoveTrackAvailable(int index) => _preferredSource.IsRemoveTrackAvailable(index);

        /// <inheritdoc/>
        public Task PauseTrackCollectionAsync() => _preferredSource.PauseTrackCollectionAsync();

        /// <inheritdoc/>
        public Task PlayTrackCollectionAsync() => _preferredSource.PlayTrackCollectionAsync();

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

        /// <inheritdoc/>
        public SynchronizedObservableCollection<string>? Genres => _preferredSource.Genres;

        /// <inheritdoc />
        IReadOnlyList<ICorePlaylistCollectionItem> IMerged<ICorePlaylistCollectionItem>.Sources => Sources;

        /// <inheritdoc/>
        IReadOnlyList<ICoreTrackCollection> IMerged<ICoreTrackCollection>.Sources => Sources;

        /// <inheritdoc/>
        IReadOnlyList<ICoreImageCollection> IMerged<ICoreImageCollection>.Sources => Sources;

        /// <inheritdoc/>
        IReadOnlyList<ICoreGenreCollection> IMerged<ICoreGenreCollection>.Sources => Sources;

        /// <inheritdoc/>
        public IReadOnlyList<ICorePlaylist> Sources => _sources;

        /// <inheritdoc cref="Equals(object?)" />
        public bool Equals(ICorePlaylist? other)
        {
            return other?.Name == Name;
        }

        /// <inheritdoc />
        public bool Equals(ICorePlaylistCollectionItem? other)
        {
            if (other is ICorePlaylist corePlaylist)
            {
                return Equals(corePlaylist);
            }

            return false;
        }

        /// <inheritdoc />
        public bool Equals(ICoreImageCollection other)
        {
            return Equals(other as ICorePlaylistCollectionItem);
        }

        /// <inheritdoc />
        public bool Equals(ICoreTrackCollection other)
        {
            return Equals(other as ICorePlaylistCollectionItem);
        }

        /// <inheritdoc />
        public bool Equals(ICoreGenreCollection other)
        {
            return Equals(other as ICorePlaylistCollectionItem);
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
    }
}