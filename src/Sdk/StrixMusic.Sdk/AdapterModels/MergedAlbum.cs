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
    /// Merged multiple <see cref="ICoreAlbum"/> into a single <see cref="IAlbum"/>
    /// </summary>
    public class MergedAlbum : IAlbum, IMergedMutable<ICoreAlbumCollectionItem>, IMergedMutable<ICoreAlbum>
    {
        private readonly ICoreAlbum _preferredSource;
        private readonly List<ICoreAlbum> _sources;

        private readonly MergedCollectionMap<ITrackCollection, ICoreTrackCollection, ITrack, ICoreTrack> _trackCollectionMap;
        private readonly MergedCollectionMap<IArtistCollection, ICoreArtistCollection, IArtistCollectionItem, ICoreArtistCollectionItem> _artistCollectionMap;
        private readonly MergedCollectionMap<IImageCollection, ICoreImageCollection, IImage, ICoreImage> _imageCollectionMap;
        private readonly MergedCollectionMap<IGenreCollection, ICoreGenreCollection, IGenre, ICoreGenre> _genreCollectionMap;
        private readonly MergedCollectionMap<IUrlCollection, ICoreUrlCollection, IUrl, ICoreUrl> _urlCollectionMap;

        /// <summary>
        /// Initializes a new instance of the <see cref="MergedAlbum"/> class.
        /// </summary>
        public MergedAlbum(IEnumerable<ICoreAlbum> sources, MergedCollectionConfig config)
        {
            _sources = sources.ToList();

            var relatedItemsSources = _sources.Select(x => x.RelatedItems).PruneNull().ToList();
            if (relatedItemsSources.Count > 0)
            {
                RelatedItems = new MergedPlayableCollectionGroup(relatedItemsSources, config);
            }

            // TODO: Get the actual preferred source.
            _preferredSource = _sources[0];

            Name = _preferredSource.Name;
            DatePublished = _preferredSource.DatePublished;
            PlaybackState = _preferredSource.PlaybackState;
            LastPlayed = _preferredSource.LastPlayed;
            AddedAt = _preferredSource.AddedAt;

            foreach (var item in _sources)
            {
                TotalTrackCount += item.TotalTrackCount;
                TotalImageCount += item.TotalImageCount;
                TotalArtistItemsCount += item.TotalArtistItemsCount;
                TotalGenreCount += item.TotalGenreCount;
                TotalUrlCount += item.TotalUrlCount;
            }

            _trackCollectionMap = new MergedCollectionMap<ITrackCollection, ICoreTrackCollection, ITrack, ICoreTrack>(this, config);
            _imageCollectionMap = new MergedCollectionMap<IImageCollection, ICoreImageCollection, IImage, ICoreImage>(this, config);
            _artistCollectionMap = new MergedCollectionMap<IArtistCollection, ICoreArtistCollection, IArtistCollectionItem, ICoreArtistCollectionItem>(this, config);
            _genreCollectionMap = new MergedCollectionMap<IGenreCollection, ICoreGenreCollection, IGenre, ICoreGenre>(this, config);
            _urlCollectionMap = new MergedCollectionMap<IUrlCollection, ICoreUrlCollection, IUrl, ICoreUrl>(this, config);

            AttachEvents(_preferredSource);
        }

        private void AttachEvents(ICoreAlbum source)
        {
            AttachPlayableEvents(source);

            source.DatePublishedChanged += DatePublishedChanged;
            source.IsChangeDatePublishedAsyncAvailableChanged += IsChangeDatePublishedAsyncAvailableChanged;

            source.IsPlayTrackCollectionAsyncAvailableChanged += IsPlayTrackCollectionAsyncAvailableChanged;
            source.IsPauseTrackCollectionAsyncAvailableChanged += IsPauseTrackCollectionAsyncAvailableChanged;
            source.IsPlayArtistCollectionAsyncAvailableChanged += IsPlayArtistCollectionAsyncAvailableChanged;
            source.IsPauseArtistCollectionAsyncAvailableChanged += IsPauseArtistCollectionAsyncAvailableChanged;

            _trackCollectionMap.ItemsChanged += TrackCollectionMap_ItemsChanged;
            _trackCollectionMap.ItemsCountChanged += TrackCollectionMap_ItemsCountChanged;
            _imageCollectionMap.ItemsChanged += ImageCollectionMap_ItemsChanged;
            _imageCollectionMap.ItemsCountChanged += ImageCollectionMap_ItemsCountChanged;
            _artistCollectionMap.ItemsChanged += ArtistCollectionMap_ItemsChanged;
            _artistCollectionMap.ItemsCountChanged += ArtistCollectionMap_ItemsCountChanged;
            _genreCollectionMap.ItemsChanged += GenreCollectionMap_ItemsChanged;
            _genreCollectionMap.ItemsCountChanged += GenreCollectionMap_ItemsCountChanged;
            _urlCollectionMap.ItemsChanged += UrlCollectionMap_ItemsChanged;
            _urlCollectionMap.ItemsCountChanged += UrlCollectionMap_ItemsCountChanged;
        }

        private void DetachEvents(ICoreAlbum source)
        {
            DetachPlayableEvents(source);

            source.DatePublishedChanged -= DatePublishedChanged;
            source.IsChangeDatePublishedAsyncAvailableChanged -= IsChangeDatePublishedAsyncAvailableChanged;

            source.IsPlayTrackCollectionAsyncAvailableChanged -= IsPlayTrackCollectionAsyncAvailableChanged;
            source.IsPauseTrackCollectionAsyncAvailableChanged -= IsPauseTrackCollectionAsyncAvailableChanged;
            source.IsPlayArtistCollectionAsyncAvailableChanged -= IsPlayArtistCollectionAsyncAvailableChanged;
            source.IsPauseArtistCollectionAsyncAvailableChanged -= IsPauseArtistCollectionAsyncAvailableChanged;

            _trackCollectionMap.ItemsChanged -= TrackCollectionMap_ItemsChanged;
            _trackCollectionMap.ItemsCountChanged -= TrackCollectionMap_ItemsCountChanged;
            _imageCollectionMap.ItemsChanged -= ImageCollectionMap_ItemsChanged;
            _imageCollectionMap.ItemsCountChanged -= ImageCollectionMap_ItemsCountChanged;
            _artistCollectionMap.ItemsChanged -= ArtistCollectionMap_ItemsChanged;
            _artistCollectionMap.ItemsCountChanged -= ArtistCollectionMap_ItemsCountChanged;
            _genreCollectionMap.ItemsChanged -= GenreCollectionMap_ItemsChanged;
            _genreCollectionMap.ItemsCountChanged -= GenreCollectionMap_ItemsCountChanged;
            _urlCollectionMap.ItemsChanged -= UrlCollectionMap_ItemsChanged;
            _urlCollectionMap.ItemsCountChanged -= UrlCollectionMap_ItemsCountChanged;
        }

        private void AttachPlayableEvents(IPlayableBase source)
        {
            source.PlaybackStateChanged -= PlaybackStateChanged;
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

        private void ArtistCollectionMap_ItemsChanged(object sender, IReadOnlyList<CollectionChangedItem<IArtistCollectionItem>> addedItems, IReadOnlyList<CollectionChangedItem<IArtistCollectionItem>> removedItems)
        {
            ArtistItemsChanged?.Invoke(this, addedItems, removedItems);
        }

        private void ArtistCollectionMap_ItemsCountChanged(object sender, int e)
        {
            TotalArtistItemsCount = e;
            ArtistItemsCountChanged?.Invoke(this, e);
        }

        private void ImageCollectionMap_ItemsChanged(object sender, IReadOnlyList<CollectionChangedItem<IImage>> addedItems, IReadOnlyList<CollectionChangedItem<IImage>> removedItems)
        {
            ImagesChanged?.Invoke(this, addedItems, removedItems);
        }

        private void ImageCollectionMap_ItemsCountChanged(object sender, int e)
        {
            TotalImageCount = e;
            ImagesCountChanged?.Invoke(this, e);
        }

        private void GenreCollectionMap_ItemsChanged(object sender, IReadOnlyList<CollectionChangedItem<IGenre>> addedItems, IReadOnlyList<CollectionChangedItem<IGenre>> removedItems)
        {
            GenresChanged?.Invoke(this, addedItems, removedItems);
        }

        private void GenreCollectionMap_ItemsCountChanged(object sender, int e)
        {
            TotalGenreCount = e;
            GenresCountChanged?.Invoke(this, e);
        }

        private void UrlCollectionMap_ItemsChanged(object sender, IReadOnlyList<CollectionChangedItem<IUrl>> addedItems, IReadOnlyList<CollectionChangedItem<IUrl>> removedItems)
        {
            UrlsChanged?.Invoke(this, addedItems, removedItems);
        }

        private void UrlCollectionMap_ItemsCountChanged(object sender, int e)
        {
            TotalUrlCount = e;
            UrlsCountChanged?.Invoke(this, e);
        }

        /// <inheritdoc cref="IMerged{T}.Sources"/>
        public IReadOnlyList<ICoreAlbum> Sources => _sources;

        /// <inheritdoc cref="IMerged.SourcesChanged" />
        public event EventHandler? SourcesChanged;

        /// <inheritdoc/>
        public IPlayableCollectionGroup? RelatedItems { get; }

        /// <inheritdoc/>
        public DateTime? DatePublished { get; internal set; }

        /// <inheritdoc/>
        public bool IsChangeDatePublishedAsyncAvailable => _preferredSource.IsChangeDatePublishedAsyncAvailable;

        /// <inheritdoc/>
        IReadOnlyList<ICoreAlbumCollectionItem> IMerged<ICoreAlbumCollectionItem>.Sources => Sources;

        /// <inheritdoc/>
        IReadOnlyList<ICoreTrackCollection> IMerged<ICoreTrackCollection>.Sources => Sources;

        /// <inheritdoc/>
        IReadOnlyList<ICoreImageCollection> IMerged<ICoreImageCollection>.Sources => Sources;

        /// <inheritdoc/>
        IReadOnlyList<ICoreGenreCollection> IMerged<ICoreGenreCollection>.Sources => Sources;

        /// <inheritdoc/>
        IReadOnlyList<ICoreUrlCollection> IMerged<ICoreUrlCollection>.Sources => Sources;

        /// <inheritdoc/>
        IReadOnlyList<ICoreAlbum> IMerged<ICoreAlbum>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreArtistCollection> IMerged<ICoreArtistCollection>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreArtistCollectionItem> IMerged<ICoreArtistCollectionItem>.Sources => Sources;

        /// <inheritdoc/>
        public string Id => _preferredSource.Id;

        /// <inheritdoc/>
        public string Name { get; internal set; }

        /// <inheritdoc/>
        public int TotalTrackCount { get; internal set; }

        /// <inheritdoc />
        public int TotalImageCount { get; internal set; }

        /// <inheritdoc />
        public int TotalArtistItemsCount { get; internal set; }

        /// <inheritdoc />
        public int TotalGenreCount { get; internal set; }

        /// <inheritdoc />
        public int TotalUrlCount { get; internal set; }

        /// <inheritdoc/>
        public string? Description { get; internal set; }

        /// <inheritdoc />
        public DateTime? LastPlayed { get; internal set; }

        /// <inheritdoc />
        public DateTime? AddedAt { get; internal set; }

        /// <inheritdoc/>
        public PlaybackState PlaybackState { get; internal set; }

        /// <inheritdoc/>
        public TimeSpan Duration => _preferredSource.Duration;

        /// <inheritdoc/>
        public bool IsPlayTrackCollectionAsyncAvailable => _preferredSource.IsPlayTrackCollectionAsyncAvailable;

        /// <inheritdoc/>
        public bool IsPauseTrackCollectionAsyncAvailable => _preferredSource.IsPauseTrackCollectionAsyncAvailable;

        /// <inheritdoc/>
        public bool IsPlayArtistCollectionAsyncAvailable => _preferredSource.IsPlayArtistCollectionAsyncAvailable;

        /// <inheritdoc/>
        public bool IsPauseArtistCollectionAsyncAvailable => _preferredSource.IsPauseArtistCollectionAsyncAvailable;

        /// <inheritdoc/>
        public bool IsChangeNameAsyncAvailable => _preferredSource.IsChangeNameAsyncAvailable;

        /// <inheritdoc/>
        public bool IsChangeDescriptionAsyncAvailable => _preferredSource.IsChangeDescriptionAsyncAvailable;

        /// <inheritdoc/>
        public bool IsChangeDurationAsyncAvailable => _preferredSource.IsChangeDurationAsyncAvailable;

        /// <inheritdoc/>
        public DownloadInfo DownloadInfo => throw new NotSupportedException();

        /// <inheritdoc />
        public event EventHandler<DateTime?>? DatePublishedChanged;

        /// <inheritdoc />
        public event EventHandler<string>? NameChanged;

        /// <inheritdoc />
        public event EventHandler<string?>? DescriptionChanged;

        /// <inheritdoc />
        public event EventHandler<PlaybackState>? PlaybackStateChanged;

        /// <inheritdoc />
        public event EventHandler<TimeSpan>? DurationChanged;

        /// <inheritdoc />
        public event EventHandler<DateTime?>? LastPlayedChanged;

        /// <inheritdoc />
        public event EventHandler<bool>? IsPlayTrackCollectionAsyncAvailableChanged;

        /// <inheritdoc />
        public event EventHandler<bool>? IsPauseTrackCollectionAsyncAvailableChanged;

        /// <inheritdoc />
        public event EventHandler<bool>? IsPlayArtistCollectionAsyncAvailableChanged;

        /// <inheritdoc />
        public event EventHandler<bool>? IsPauseArtistCollectionAsyncAvailableChanged;

        /// <inheritdoc />
        public event EventHandler<bool>? IsChangeNameAsyncAvailableChanged;

        /// <inheritdoc />
        public event EventHandler<bool>? IsChangeDescriptionAsyncAvailableChanged;

        /// <inheritdoc />
        public event EventHandler<bool>? IsChangeDurationAsyncAvailableChanged;

        /// <inheritdoc />
        public event EventHandler<bool>? IsChangeDatePublishedAsyncAvailableChanged;

        /// <inheritdoc />
        public event EventHandler<int>? TracksCountChanged;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<ITrack>? TracksChanged;

        /// <inheritdoc />
        public event EventHandler<int>? ArtistItemsCountChanged;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<IArtistCollectionItem>? ArtistItemsChanged;

        /// <inheritdoc />
        public event EventHandler<int>? ImagesCountChanged;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<IImage>? ImagesChanged;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<IGenre>? GenresChanged;

        /// <inheritdoc />
        public event EventHandler<int>? GenresCountChanged;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<IUrl>? UrlsChanged;

        /// <inheritdoc />
        public event EventHandler<int>? UrlsCountChanged;

        /// <inheritdoc/>
        public event EventHandler<DownloadInfo>? DownloadInfoChanged
        {
            add => throw new NotSupportedException();
            remove => throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public IAsyncEnumerable<ITrack> GetTracksAsync(int limit, int offset, CancellationToken cancellationToken = default) => _trackCollectionMap.GetItemsAsync(limit, offset, cancellationToken);

        /// <inheritdoc />
        public IAsyncEnumerable<IImage> GetImagesAsync(int limit, int offset, CancellationToken cancellationToken = default) => _imageCollectionMap.GetItemsAsync(limit, offset, cancellationToken);

        /// <inheritdoc />
        public IAsyncEnumerable<IArtistCollectionItem> GetArtistItemsAsync(int limit, int offset, CancellationToken cancellationToken = default) => _artistCollectionMap.GetItemsAsync(limit, offset, cancellationToken);

        /// <inheritdoc />
        public IAsyncEnumerable<IGenre> GetGenresAsync(int limit, int offset, CancellationToken cancellationToken = default) => _genreCollectionMap.GetItemsAsync(limit, offset, cancellationToken);

        /// <inheritdoc />
        public IAsyncEnumerable<IUrl> GetUrlsAsync(int limit, int offset, CancellationToken cancellationToken = default) => _urlCollectionMap.GetItemsAsync(limit, offset, cancellationToken);

        /// <inheritdoc/>
        public Task RemoveTrackAsync(int index, CancellationToken cancellationToken = default) => _trackCollectionMap.RemoveAtAsync(index, cancellationToken);

        /// <inheritdoc/>
        public Task AddTrackAsync(ITrack track, int index, CancellationToken cancellationToken = default) => _trackCollectionMap.InsertItemAsync(track, index, cancellationToken);

        /// <inheritdoc />
        public Task AddArtistItemAsync(IArtistCollectionItem artistItem, int index, CancellationToken cancellationToken = default) => _artistCollectionMap.InsertItemAsync(artistItem, index, cancellationToken);

        /// <inheritdoc />
        public Task RemoveArtistItemAsync(int index, CancellationToken cancellationToken = default) => _artistCollectionMap.RemoveAtAsync(index, cancellationToken);

        /// <inheritdoc />
        public Task AddImageAsync(IImage image, int index, CancellationToken cancellationToken = default) => _imageCollectionMap.InsertItemAsync(image, index, cancellationToken);

        /// <inheritdoc />
        public Task RemoveImageAsync(int index, CancellationToken cancellationToken = default) => _imageCollectionMap.RemoveAtAsync(index, cancellationToken);

        /// <inheritdoc />
        public Task AddGenreAsync(IGenre genre, int index, CancellationToken cancellationToken = default) => _genreCollectionMap.InsertItemAsync(genre, index, cancellationToken);

        /// <inheritdoc />
        public Task RemoveGenreAsync(int index, CancellationToken cancellationToken = default) => _genreCollectionMap.RemoveAtAsync(index, cancellationToken);

        /// <inheritdoc />
        public Task AddUrlAsync(IUrl url, int index, CancellationToken cancellationToken = default) => _urlCollectionMap.InsertItemAsync(url, index, cancellationToken);

        /// <inheritdoc />
        public Task RemoveUrlAsync(int index, CancellationToken cancellationToken = default) => _urlCollectionMap.RemoveAtAsync(index, cancellationToken);

        /// <inheritdoc/>
        public Task ChangeDatePublishedAsync(DateTime datePublished, CancellationToken cancellationToken = default)
        {
            return _preferredSource.ChangeDatePublishedAsync(datePublished, cancellationToken);
        }

        /// <inheritdoc/>
        public Task ChangeDescriptionAsync(string? description, CancellationToken cancellationToken = default)
        {
            return _preferredSource.ChangeDescriptionAsync(description, cancellationToken);
        }

        /// <inheritdoc/>
        public Task ChangeDurationAsync(TimeSpan duration, CancellationToken cancellationToken = default)
        {
            return _preferredSource.ChangeDurationAsync(duration, cancellationToken);
        }

        /// <inheritdoc cref="IPlayableBase.ChangeNameAsync(string, CancellationToken)"/>
        public Task ChangeNameAsync(string name, CancellationToken cancellationToken = default)
        {
            return _preferredSource.ChangeNameAsync(name, cancellationToken);
        }

        /// <inheritdoc/>
        public Task StartDownloadOperationAsync(DownloadOperation operation, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public Task<bool> IsAddTrackAvailableAsync(int index, CancellationToken cancellationToken = default) => _trackCollectionMap.IsAddItemAvailableAsync(index, cancellationToken);

        /// <inheritdoc />
        public Task<bool> IsAddArtistItemAvailableAsync(int index, CancellationToken cancellationToken = default) => _artistCollectionMap.IsAddItemAvailableAsync(index, cancellationToken);

        /// <inheritdoc/>
        public Task<bool> IsAddImageAvailableAsync(int index, CancellationToken cancellationToken = default) => _imageCollectionMap.IsAddItemAvailableAsync(index, cancellationToken);

        /// <inheritdoc/>
        public Task<bool> IsAddGenreAvailableAsync(int index, CancellationToken cancellationToken = default) => _genreCollectionMap.IsAddItemAvailableAsync(index, cancellationToken);

        /// <inheritdoc/>
        public Task<bool> IsAddUrlAvailableAsync(int index, CancellationToken cancellationToken = default) => _urlCollectionMap.IsAddItemAvailableAsync(index, cancellationToken);

        /// <inheritdoc />
        public Task<bool> IsRemoveArtistItemAvailableAsync(int index, CancellationToken cancellationToken = default) => _artistCollectionMap.IsRemoveItemAvailableAsync(index, cancellationToken);

        /// <inheritdoc/>
        public Task<bool> IsRemoveTrackAvailableAsync(int index, CancellationToken cancellationToken = default) => _trackCollectionMap.IsRemoveItemAvailableAsync(index, cancellationToken);

        /// <inheritdoc/>
        public Task<bool> IsRemoveImageAvailableAsync(int index, CancellationToken cancellationToken = default) => _imageCollectionMap.IsRemoveItemAvailableAsync(index, cancellationToken);

        /// <inheritdoc/>
        public Task<bool> IsRemoveGenreAvailableAsync(int index, CancellationToken cancellationToken = default) => _genreCollectionMap.IsRemoveItemAvailableAsync(index, cancellationToken);

        /// <inheritdoc/>
        public Task<bool> IsRemoveUrlAvailableAsync(int index, CancellationToken cancellationToken = default) => _urlCollectionMap.IsRemoveItemAvailableAsync(index, cancellationToken);

        /// <inheritdoc/>
        public Task PauseTrackCollectionAsync(CancellationToken cancellationToken = default) => _preferredSource.PauseTrackCollectionAsync(cancellationToken);

        /// <inheritdoc/>
        public Task PlayTrackCollectionAsync(CancellationToken cancellationToken = default) => _preferredSource.PlayTrackCollectionAsync(cancellationToken);

        /// <inheritdoc/>
        public Task PauseArtistCollectionAsync(CancellationToken cancellationToken = default) => _preferredSource.PauseArtistCollectionAsync(cancellationToken);

        /// <inheritdoc/>
        public Task PlayArtistCollectionAsync(CancellationToken cancellationToken = default) => _preferredSource.PlayArtistCollectionAsync(cancellationToken);

        /// <inheritdoc />
        public Task PlayTrackCollectionAsync(ITrack track, CancellationToken cancellationToken = default)
        {
            var targetCore = _preferredSource.SourceCore;
            var source = track.GetSources<ICoreTrack>().FirstOrDefault(x => x.SourceCore.InstanceId == targetCore.InstanceId);

            Guard.IsNotNull(source, nameof(source));

            return _preferredSource.PlayTrackCollectionAsync(source, cancellationToken);
        }

        /// <inheritdoc />
        public Task PlayArtistCollectionAsync(IArtistCollectionItem artistItem, CancellationToken cancellationToken = default)
        {
            var targetCore = _preferredSource.SourceCore;

            ICoreArtistCollectionItem? source = null;

            if (artistItem is IArtist artist)
                source = artist.GetSources<ICoreArtist>().FirstOrDefault(x => x.SourceCore.InstanceId == targetCore.InstanceId);

            if (artistItem is IArtistCollection collection)
                source = collection.GetSources<ICoreArtistCollection>().FirstOrDefault(x => x.SourceCore.InstanceId == targetCore.InstanceId);

            Guard.IsNotNull(source, nameof(source));

            return _preferredSource.PlayArtistCollectionAsync(source, cancellationToken);
        }

        /// <inheritdoc/>
        public void AddSource(ICoreAlbum itemToMerge)
        {
            Guard.IsNotNull(itemToMerge, nameof(itemToMerge));

            _imageCollectionMap.AddSource(itemToMerge);
            _artistCollectionMap.AddSource(itemToMerge);
            _trackCollectionMap.AddSource(itemToMerge);
            _genreCollectionMap.AddSource(itemToMerge);
            _urlCollectionMap.AddSource(itemToMerge);
            
            _sources.Add(itemToMerge);
            SourcesChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <inheritdoc />
        public void RemoveSource(ICoreAlbum itemToRemove)
        {
            Guard.IsNotNull(itemToRemove, nameof(itemToRemove));

            _imageCollectionMap.RemoveSource(itemToRemove);
            _artistCollectionMap.RemoveSource(itemToRemove);
            _trackCollectionMap.RemoveSource(itemToRemove);
            _genreCollectionMap.RemoveSource(itemToRemove);
            _urlCollectionMap.RemoveSource(itemToRemove);
            
            _sources.Remove(itemToRemove);
            SourcesChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <inheritdoc />
        public void AddSource(ICoreAlbumCollectionItem itemToMerge) => AddSource((ICoreAlbum)itemToMerge);

        /// <inheritdoc />
        public void RemoveSource(ICoreAlbumCollectionItem itemToRemove) => RemoveSource((ICoreAlbum)itemToRemove);

        /// <inheritdoc/>
        public bool Equals(ICoreAlbum? other)
        {
            return other?.Name == Name;
        }

        /// <inheritdoc />
        public bool Equals(ICoreAlbumCollectionItem other) => Equals(other as ICoreAlbum);

        /// <inheritdoc />
        public bool Equals(ICoreArtistCollectionItem other) => Equals(other as ICoreAlbum);

        /// <inheritdoc />
        public bool Equals(ICoreImageCollection other) => Equals(other as ICoreAlbum);

        /// <inheritdoc />
        public bool Equals(ICoreArtistCollection other) => Equals(other as ICoreAlbum);

        /// <inheritdoc />
        public bool Equals(ICoreTrackCollection other) => Equals(other as ICoreAlbum);

        /// <inheritdoc />
        public bool Equals(ICoreGenreCollection other) => Equals(other as ICoreAlbum);

        /// <inheritdoc />
        public bool Equals(ICoreUrlCollection other) => Equals(other as ICoreAlbum);

        /// <inheritdoc />
        public async ValueTask DisposeAsync()
        {
            DetachEvents(_preferredSource);

            await _artistCollectionMap.DisposeAsync();
            await _imageCollectionMap.DisposeAsync();
            await _genreCollectionMap.DisposeAsync();
            await _urlCollectionMap.DisposeAsync();

            await Sources.InParallel(x => x.DisposeAsync().AsTask());
        }
    }
}
