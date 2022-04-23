// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using OwlCore.Events;
using OwlCore.Extensions;
using StrixMusic.Sdk.AppModels;
using StrixMusic.Sdk.BaseModels;
using StrixMusic.Sdk.CoreModels;
using StrixMusic.Sdk.Extensions;
using StrixMusic.Sdk.MediaPlayback;

namespace StrixMusic.Sdk.AdapterModels
{
    /// <summary>
    /// Merged multiple <see cref="ICoreArtist"/> into a single <see cref="IArtist"/>
    /// </summary>
    public class MergedArtist : IArtist, IMergedMutable<ICoreArtist>, IMergedMutable<ICoreArtistCollectionItem>
    {
        private readonly List<ICoreArtist> _sources;
        private readonly List<ICore> _sourceCores;
        private readonly MergedCollectionMap<ITrackCollection, ICoreTrackCollection, ITrack, ICoreTrack> _trackCollectionMap;
        private readonly MergedCollectionMap<IImageCollection, ICoreImageCollection, IImage, ICoreImage> _imageCollectionMap;
        private readonly MergedCollectionMap<IAlbumCollection, ICoreAlbumCollection, IAlbumCollectionItem, ICoreAlbumCollectionItem> _albumCollectionItemMap;
        private readonly MergedCollectionMap<IGenreCollection, ICoreGenreCollection, IGenre, ICoreGenre> _genreCollectionMap;
        private readonly MergedCollectionMap<IUrlCollection, ICoreUrlCollection, IUrl, ICoreUrl> _urlCollectionMap;
        private readonly ICoreArtist _preferredSource;

        /// <summary>
        /// Creates a new instance of <see cref="MergedArtist"/>.
        /// </summary>
        public MergedArtist(IEnumerable<ICoreArtist> sources, MergedCollectionConfig config)
        {
            _sources = sources.ToList();
            _sourceCores = _sources.Select(x => x.SourceCore).ToList();

            // TODO: Get the actual preferred source.
            _preferredSource = _sources[0];

            foreach (var source in _sources)
            {
                TotalAlbumItemsCount += source.TotalAlbumItemsCount;
                TotalImageCount += source.TotalImageCount;
                TotalTrackCount += source.TotalTrackCount;
                TotalGenreCount += source.TotalGenreCount;
                TotalUrlCount += source.TotalUrlCount;
            }

            Name = _preferredSource.Name;
            Description = _preferredSource.Description;
            PlaybackState = _preferredSource.PlaybackState;
            Duration = _preferredSource.Duration;
            LastPlayed = _preferredSource.LastPlayed;
            AddedAt = _preferredSource.AddedAt;

            _trackCollectionMap = new MergedCollectionMap<ITrackCollection, ICoreTrackCollection, ITrack, ICoreTrack>(this, config);
            _imageCollectionMap = new MergedCollectionMap<IImageCollection, ICoreImageCollection, IImage, ICoreImage>(this, config);
            _albumCollectionItemMap = new MergedCollectionMap<IAlbumCollection, ICoreAlbumCollection, IAlbumCollectionItem, ICoreAlbumCollectionItem>(this, config);
            _genreCollectionMap = new MergedCollectionMap<IGenreCollection, ICoreGenreCollection, IGenre, ICoreGenre>(this, config);
            _urlCollectionMap = new MergedCollectionMap<IUrlCollection, ICoreUrlCollection, IUrl, ICoreUrl>(this, config);

            var relatedItems = _sources.Select(x => x.RelatedItems).PruneNull().ToList();

            if (relatedItems.Count > 0)
                RelatedItems = new MergedPlayableCollectionGroup(relatedItems, config);

            AttachEvents(_preferredSource);
        }

        private void AttachEvents(ICoreArtist source)
        {
            AttachPlayableEvents(source);

            source.IsPlayTrackCollectionAsyncAvailableChanged += IsPlayTrackCollectionAsyncAvailableChanged;
            source.IsPauseTrackCollectionAsyncAvailableChanged += IsPauseTrackCollectionAsyncAvailableChanged;
            source.IsPlayAlbumCollectionAsyncAvailableChanged += IsPlayAlbumCollectionAsyncAvailableChanged;
            source.IsPauseAlbumCollectionAsyncAvailableChanged += IsPauseAlbumCollectionAsyncAvailableChanged;

            _trackCollectionMap.ItemsChanged += TrackCollectionMap_ItemsChanged;
            _trackCollectionMap.ItemsCountChanged += TrackCollectionMap_ItemsCountChanged;
            _imageCollectionMap.ItemsChanged += ImageCollectionMap_ItemsChanged;
            _imageCollectionMap.ItemsCountChanged += ImageCollectionMap_ItemsCountChanged;
            _albumCollectionItemMap.ItemsChanged += AlbumCollectionItemsChanged;
            _albumCollectionItemMap.ItemsCountChanged += AlbumCollectionMap_ItemsCountChanged;
            _genreCollectionMap.ItemsChanged += GenreCollectionMap_ItemsChanged;
            _genreCollectionMap.ItemsCountChanged += GenreCollectionMap_ItemsCountChanged;
            _urlCollectionMap.ItemsChanged += UrlCollectionMap_ItemsChanged;
            _urlCollectionMap.ItemsCountChanged += UrlCollectionMap_ItemsCountChanged;
        }

        private void DetachEvents(ICoreArtist source)
        {
            DetachPlayableEvents(source);

            source.IsPlayTrackCollectionAsyncAvailableChanged -= IsPlayTrackCollectionAsyncAvailableChanged;
            source.IsPauseTrackCollectionAsyncAvailableChanged -= IsPauseTrackCollectionAsyncAvailableChanged;
            source.IsPlayAlbumCollectionAsyncAvailableChanged += IsPlayAlbumCollectionAsyncAvailableChanged;
            source.IsPauseAlbumCollectionAsyncAvailableChanged += IsPauseAlbumCollectionAsyncAvailableChanged;

            _trackCollectionMap.ItemsChanged -= TrackCollectionMap_ItemsChanged;
            _trackCollectionMap.ItemsCountChanged -= TrackCollectionMap_ItemsCountChanged;
            _imageCollectionMap.ItemsChanged -= ImageCollectionMap_ItemsChanged;
            _imageCollectionMap.ItemsCountChanged -= ImageCollectionMap_ItemsCountChanged;
            _albumCollectionItemMap.ItemsChanged -= AlbumCollectionItemsChanged;
            _genreCollectionMap.ItemsChanged -= GenreCollectionMap_ItemsChanged;
            _genreCollectionMap.ItemsCountChanged -= GenreCollectionMap_ItemsCountChanged;
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

        private void AlbumCollectionMap_ItemsCountChanged(object sender, int e)
        {
            TotalAlbumItemsCount = e;
            AlbumItemsCountChanged?.Invoke(this, e);
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

        private void GenreCollectionMap_ItemsCountChanged(object sender, int e)
        {
            TotalGenreCount = e;
            GenresCountChanged?.Invoke(this, e);
        }

        private void UrlCollectionMap_ItemsCountChanged(object sender, int e)
        {
            TotalUrlCount = e;
            UrlsCountChanged?.Invoke(this, e);
        }

        private void AlbumCollectionItemsChanged(object sender, IReadOnlyList<CollectionChangedItem<IAlbumCollectionItem>> addedItems, IReadOnlyList<CollectionChangedItem<IAlbumCollectionItem>> removedItems)
        {
            AlbumItemsChanged?.Invoke(this, addedItems, removedItems);
        }

        private void TrackCollectionMap_ItemsChanged(object sender, IReadOnlyList<CollectionChangedItem<ITrack>> addedItems, IReadOnlyList<CollectionChangedItem<ITrack>> removedItems)
        {
            TracksChanged?.Invoke(this, addedItems, removedItems);
        }

        private void ImageCollectionMap_ItemsChanged(object sender, IReadOnlyList<CollectionChangedItem<IImage>> addedItems, IReadOnlyList<CollectionChangedItem<IImage>> removedItems)
        {
            ImagesChanged?.Invoke(this, addedItems, removedItems);
        }

        private void GenreCollectionMap_ItemsChanged(object sender, IReadOnlyList<CollectionChangedItem<IGenre>> addedItems, IReadOnlyList<CollectionChangedItem<IGenre>> removedItems)
        {
            GenresChanged?.Invoke(this, addedItems, removedItems);
        }

        private void UrlCollectionMap_ItemsChanged(object sender, IReadOnlyList<CollectionChangedItem<IUrl>> addedItems, IReadOnlyList<CollectionChangedItem<IUrl>> removedItems)
        {
            UrlsChanged?.Invoke(this, addedItems, removedItems);
        }

        /// <inheritdoc />
        public event EventHandler<int>? AlbumItemsCountChanged;

        /// <inheritdoc />
        public event EventHandler<int>? TracksCountChanged;

        /// <inheritdoc />
        public event EventHandler<int>? ImagesCountChanged;

        /// <inheritdoc />
        public event EventHandler<int>? GenresCountChanged;

        /// <inheritdoc />
        public event EventHandler<int>? UrlsCountChanged;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<IAlbumCollectionItem>? AlbumItemsChanged;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<ITrack>? TracksChanged;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<IImage>? ImagesChanged;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<IGenre>? GenresChanged;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<IUrl>? UrlsChanged;

        /// <inheritdoc />
        public event EventHandler<TimeSpan>? DurationChanged;

        /// <inheritdoc />
        public event EventHandler<DateTime?>? LastPlayedChanged;

        /// <inheritdoc />
        public event EventHandler<bool>? IsPlayTrackCollectionAsyncAvailableChanged;

        /// <inheritdoc />
        public event EventHandler<bool>? IsPauseTrackCollectionAsyncAvailableChanged;

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
        public event EventHandler<string>? NameChanged;

        /// <inheritdoc />
        public event EventHandler<string?>? DescriptionChanged;

        /// <inheritdoc/>
        public event EventHandler<DownloadInfo>? DownloadInfoChanged
        {
            add => throw new NotSupportedException();
            remove => throw new NotSupportedException();
        }

        /// <inheritdoc />
        public event EventHandler<PlaybackState>? PlaybackStateChanged;

        /// <inheritdoc cref="IMerged{T}.SourceCores" />
        public IReadOnlyList<ICore> SourceCores => _sourceCores;

        /// <inheritdoc />
        IReadOnlyList<ICoreArtistCollectionItem> IMerged<ICoreArtistCollectionItem>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreTrackCollection> IMerged<ICoreTrackCollection>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreAlbumCollection> IMerged<ICoreAlbumCollection>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreImageCollection> IMerged<ICoreImageCollection>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreGenreCollection> IMerged<ICoreGenreCollection>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreUrlCollection> IMerged<ICoreUrlCollection>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreAlbumCollectionItem> IMerged<ICoreAlbumCollectionItem>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreArtist> IMerged<ICoreArtist>.Sources => Sources;

        /// <summary>
        /// The merged sources for this <see cref="IArtist"/>
        /// </summary>
        public IReadOnlyList<ICoreArtist> Sources => _sources;

        /// <inheritdoc />
        public string Id => _preferredSource.Id;

        /// <inheritdoc />
        public string Name { get; internal set; }

        /// <inheritdoc />
        public string? Description { get; internal set; }

        /// <inheritdoc />
        public PlaybackState PlaybackState { get; internal set; }

        /// <inheritdoc/>
        public DownloadInfo DownloadInfo => throw new NotSupportedException();

        /// <inheritdoc />
        public TimeSpan Duration { get; internal set; }

        /// <inheritdoc />
        public DateTime? LastPlayed { get; internal set; }

        /// <inheritdoc />
        public DateTime? AddedAt { get; internal set; }

        /// <inheritdoc />
        public int TotalTrackCount { get; internal set; }

        /// <inheritdoc />
        public int TotalAlbumItemsCount { get; internal set; }

        /// <inheritdoc />
        public int TotalImageCount { get; internal set; }

        /// <inheritdoc />
        public int TotalGenreCount { get; internal set; }

        /// <inheritdoc />
        public int TotalUrlCount { get; internal set; }

        /// <inheritdoc />
        public IPlayableCollectionGroup? RelatedItems { get; }

        /// <inheritdoc />
        public bool IsPlayTrackCollectionAsyncAvailable => _preferredSource.IsPlayTrackCollectionAsyncAvailable;

        /// <inheritdoc />
        public bool IsPauseTrackCollectionAsyncAvailable => _preferredSource.IsPauseTrackCollectionAsyncAvailable;

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
        public Task<bool> IsAddTrackAvailableAsync(int index, CancellationToken cancellationToken = default) => _trackCollectionMap.IsAddItemAvailableAsync(index, cancellationToken);

        /// <inheritdoc />
        public Task<bool> IsAddAlbumItemAvailableAsync(int index, CancellationToken cancellationToken = default) => _albumCollectionItemMap.IsAddItemAvailableAsync(index, cancellationToken);

        /// <inheritdoc />
        public Task<bool> IsAddImageAvailableAsync(int index, CancellationToken cancellationToken = default) => _imageCollectionMap.IsAddItemAvailableAsync(index, cancellationToken);

        /// <inheritdoc />
        public Task<bool> IsAddGenreAvailableAsync(int index, CancellationToken cancellationToken = default) => _genreCollectionMap.IsAddItemAvailableAsync(index, cancellationToken);

        /// <inheritdoc />
        public Task<bool> IsAddUrlAvailableAsync(int index, CancellationToken cancellationToken = default) => _genreCollectionMap.IsAddItemAvailableAsync(index, cancellationToken);

        /// <inheritdoc />
        public Task<bool> IsRemoveTrackAvailableAsync(int index, CancellationToken cancellationToken = default) => _trackCollectionMap.IsRemoveItemAvailableAsync(index, cancellationToken);

        /// <inheritdoc />
        public Task<bool> IsRemoveAlbumItemAvailableAsync(int index, CancellationToken cancellationToken = default) => _albumCollectionItemMap.IsRemoveItemAvailableAsync(index, cancellationToken);

        /// <inheritdoc />
        public Task<bool> IsRemoveImageAvailableAsync(int index, CancellationToken cancellationToken = default) => _imageCollectionMap.IsRemoveItemAvailableAsync(index, cancellationToken);

        /// <inheritdoc />
        public Task<bool> IsRemoveGenreAvailableAsync(int index, CancellationToken cancellationToken = default) => _genreCollectionMap.IsRemoveItemAvailableAsync(index, cancellationToken);

        /// <inheritdoc />
        public Task<bool> IsRemoveUrlAvailableAsync(int index, CancellationToken cancellationToken = default) => _urlCollectionMap.IsRemoveItemAvailableAsync(index, cancellationToken);

        /// <inheritdoc />
        public IAsyncEnumerable<IAlbumCollectionItem> GetAlbumItemsAsync(int limit, int offset, CancellationToken cancellationToken = default) => _albumCollectionItemMap.GetItemsAsync(limit, offset, cancellationToken);

        /// <inheritdoc />
        public Task AddAlbumItemAsync(IAlbumCollectionItem albumItem, int index, CancellationToken cancellationToken = default) => _albumCollectionItemMap.InsertItemAsync(albumItem, index, cancellationToken);

        /// <inheritdoc />
        public IAsyncEnumerable<ITrack> GetTracksAsync(int limit, int offset, CancellationToken cancellationToken = default) => _trackCollectionMap.GetItemsAsync(limit, offset, cancellationToken);

        /// <inheritdoc />
        public Task AddTrackAsync(ITrack track, int index, CancellationToken cancellationToken = default) => _trackCollectionMap.InsertItemAsync(track, index, cancellationToken);

        /// <inheritdoc />
        public Task PlayTrackCollectionAsync(CancellationToken cancellationToken = default) => _preferredSource.PlayTrackCollectionAsync(cancellationToken);

        /// <inheritdoc />
        public Task PauseTrackCollectionAsync(CancellationToken cancellationToken = default) => _preferredSource.PauseTrackCollectionAsync(cancellationToken);

        /// <inheritdoc />
        public Task PlayAlbumCollectionAsync(CancellationToken cancellationToken = default) => _preferredSource.PlayAlbumCollectionAsync(cancellationToken);

        /// <inheritdoc />
        public Task PauseAlbumCollectionAsync(CancellationToken cancellationToken = default) => _preferredSource.PauseAlbumCollectionAsync(cancellationToken);

        /// <inheritdoc />
        public Task PlayTrackCollectionAsync(ITrack track, CancellationToken cancellationToken = default)
        {
            var targetCore = _preferredSource.SourceCore;
            var source = track.GetSources<ICoreTrack>()
                .FirstOrDefault(x => x.SourceCore.InstanceId == targetCore.InstanceId);

            Guard.IsNotNull(source, nameof(source));

            return _preferredSource.PlayTrackCollectionAsync(source, cancellationToken);
        }

        /// <inheritdoc />
        public Task PlayAlbumCollectionAsync(IAlbumCollectionItem albumItem, CancellationToken cancellationToken = default)
        {
            var targetCore = _preferredSource.SourceCore;

            ICoreAlbumCollectionItem? source = null;

            if (albumItem is IAlbum album)
                source = album.GetSources<ICoreAlbum>().FirstOrDefault(x => x.SourceCore.InstanceId == targetCore.InstanceId);

            if (albumItem is IAlbumCollection collection)
                source = collection.GetSources<ICoreAlbumCollection>().FirstOrDefault(x => x.SourceCore.InstanceId == targetCore.InstanceId);

            Guard.IsNotNull(source, nameof(source));

            return _preferredSource.PlayAlbumCollectionAsync(source, cancellationToken);
        }

        /// <inheritdoc />
        public Task ChangeNameAsync(string name, CancellationToken cancellationToken = default) => _preferredSource.ChangeNameAsync(name, cancellationToken);

        /// <inheritdoc />
        public IAsyncEnumerable<IImage> GetImagesAsync(int limit, int offset, CancellationToken cancellationToken = default) => _imageCollectionMap.GetItemsAsync(limit, offset, cancellationToken);

        /// <inheritdoc />
        public IAsyncEnumerable<IGenre> GetGenresAsync(int limit, int offset, CancellationToken cancellationToken = default) => _genreCollectionMap.GetItemsAsync(limit, offset, cancellationToken);

        /// <inheritdoc />
        public IAsyncEnumerable<IUrl> GetUrlsAsync(int limit, int offset, CancellationToken cancellationToken = default) => _urlCollectionMap.GetItemsAsync(limit, offset, cancellationToken);

        /// <inheritdoc />
        public Task AddImageAsync(IImage image, int index, CancellationToken cancellationToken = default) => _imageCollectionMap.InsertItemAsync(image, index, cancellationToken);

        /// <inheritdoc />
        public Task AddGenreAsync(IGenre genre, int index, CancellationToken cancellationToken = default) => _genreCollectionMap.InsertItemAsync(genre, index, cancellationToken);

        /// <inheritdoc />
        public Task AddUrlAsync(IUrl url, int index, CancellationToken cancellationToken = default) => _urlCollectionMap.InsertItemAsync(url, index, cancellationToken);

        /// <inheritdoc />
        public Task ChangeDescriptionAsync(string? description, CancellationToken cancellationToken = default) => _preferredSource.ChangeDescriptionAsync(description, cancellationToken);

        /// <inheritdoc />
        public Task ChangeDurationAsync(TimeSpan duration, CancellationToken cancellationToken = default) => _preferredSource.ChangeDurationAsync(duration, cancellationToken);

        /// <inheritdoc />
        public Task RemoveTrackAsync(int index, CancellationToken cancellationToken = default) => _trackCollectionMap.RemoveAtAsync(index, cancellationToken);

        /// <inheritdoc />
        public Task RemoveImageAsync(int index, CancellationToken cancellationToken = default) => _trackCollectionMap.RemoveAtAsync(index, cancellationToken);

        /// <inheritdoc />
        public Task RemoveAlbumItemAsync(int index, CancellationToken cancellationToken = default) => _albumCollectionItemMap.RemoveAtAsync(index, cancellationToken);

        /// <inheritdoc />
        public Task RemoveGenreAsync(int index, CancellationToken cancellationToken = default) => _genreCollectionMap.RemoveAtAsync(index, cancellationToken);

        /// <inheritdoc />
        public Task RemoveUrlAsync(int index, CancellationToken cancellationToken = default) => _urlCollectionMap.RemoveAtAsync(index, cancellationToken);
        
        /// <inheritdoc/>
        public Task StartDownloadOperationAsync(DownloadOperation operation, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Adds a new source to this merged item.
        /// </summary>
        /// <param name="itemToMerge">The item to merge into this Artist</param>
        void IMergedMutable<ICoreArtist>.AddSource(ICoreArtist itemToMerge)
        {
            Guard.IsNotNull(itemToMerge, nameof(itemToMerge));

            if (!Equals(itemToMerge))
                ThrowHelper.ThrowArgumentException(nameof(itemToMerge), "Tried to merge an artist that doesn't match. Verify that the item matches before merging the source.");

            _sources.Add(itemToMerge);
            _sourceCores.Add(itemToMerge.SourceCore);
            _albumCollectionItemMap.Cast<IMergedMutable<ICoreAlbumCollection>>().AddSource(itemToMerge);
            _trackCollectionMap.Cast<IMergedMutable<ICoreTrackCollection>>().AddSource(itemToMerge);
            _imageCollectionMap.Cast<IMergedMutable<ICoreImageCollection>>().AddSource(itemToMerge);
            _genreCollectionMap.Cast<IMergedMutable<ICoreGenreCollection>>().AddSource(itemToMerge);
            _urlCollectionMap.Cast<IMergedMutable<ICoreUrlCollection>>().AddSource(itemToMerge);
        }

        /// <inheritdoc />
        void IMergedMutable<ICoreArtist>.RemoveSource(ICoreArtist itemToRemove)
        {
            Guard.IsNotNull(itemToRemove, nameof(itemToRemove));

            _sources.Remove(itemToRemove);
            _sourceCores.Remove(itemToRemove.SourceCore);
            _albumCollectionItemMap.Cast<IMergedMutable<ICoreAlbumCollection>>().RemoveSource(itemToRemove);
            _trackCollectionMap.Cast<IMergedMutable<ICoreTrackCollection>>().RemoveSource(itemToRemove);
            _imageCollectionMap.Cast<IMergedMutable<ICoreImageCollection>>().RemoveSource(itemToRemove);
            _genreCollectionMap.Cast<IMergedMutable<ICoreGenreCollection>>().RemoveSource(itemToRemove);
            _urlCollectionMap.Cast<IMergedMutable<ICoreUrlCollection>>().RemoveSource(itemToRemove);
        }

        /// <inheritdoc />
        void IMergedMutable<ICoreArtistCollectionItem>.AddSource(ICoreArtistCollectionItem itemToMerge)
        {
            if (itemToMerge is ICoreArtist col)
                ((IMergedMutable<ICoreArtist>)this).AddSource(col);
        }

        /// <inheritdoc />
        void IMergedMutable<ICoreArtistCollectionItem>.RemoveSource(ICoreArtistCollectionItem itemToRemove)
        {
            if (itemToRemove is ICoreArtist col)
                ((IMergedMutable<ICoreArtist>)this).RemoveSource(col);
        }

        /// <inheritdoc cref="Equals(object?)" />
        public bool Equals(ICoreArtist? other)
        {
            return other?.Name == Name;
        }

        /// <inheritdoc />
        public bool Equals(ICoreArtistCollectionItem other) => Equals(other as ICoreArtist);

        /// <inheritdoc />
        public bool Equals(ICoreAlbumCollectionItem other) => Equals(other as ICoreArtist);

        /// <inheritdoc />
        public bool Equals(ICoreImageCollection other) => Equals(other as ICoreArtist);

        /// <inheritdoc />
        public bool Equals(ICoreAlbumCollection other) => Equals(other as ICoreArtist);

        /// <inheritdoc />
        public bool Equals(ICoreTrackCollection other) => Equals(other as ICoreArtist);

        /// <inheritdoc />
        public bool Equals(ICoreGenreCollection other) => Equals(other as ICoreArtist);

        /// <inheritdoc />
        public bool Equals(ICoreUrlCollection other) => Equals(other as ICoreArtist);

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj) || (obj is ICoreArtist other && Equals(other));
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

            await _albumCollectionItemMap.DisposeAsync();
            await _trackCollectionMap.DisposeAsync();
            await _imageCollectionMap.DisposeAsync();
            await _genreCollectionMap.DisposeAsync();
            await _urlCollectionMap.DisposeAsync();

            await Sources.InParallel(x => x.DisposeAsync().AsTask());
        }
    }
}
