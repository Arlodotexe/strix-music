// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using OwlCore.ComponentModel;
using StrixMusic.Sdk.AppModels;
using StrixMusic.Sdk.BaseModels;
using StrixMusic.Sdk.CoreModels;
using StrixMusic.Sdk.Extensions;
using StrixMusic.Sdk.MediaPlayback;

namespace StrixMusic.Sdk.AdapterModels
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

        /// <summary>
        /// Creates a new instance of <see cref="MergedTrackCollection"/>.
        /// </summary>
        public MergedTrackCollection(IEnumerable<ICoreTrackCollection> collections, MergedCollectionConfig config)
        {
            _sources = collections.ToList();

            _preferredSource = _sources[0];

            _imageMap = new MergedCollectionMap<IImageCollection, ICoreImageCollection, IImage, ICoreImage>(this, config);
            _trackMap = new MergedCollectionMap<ITrackCollection, ICoreTrackCollection, ITrack, ICoreTrack>(this, config);
            _urlMap = new MergedCollectionMap<IUrlCollection, ICoreUrlCollection, IUrl, ICoreUrl>(this, config);

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
        
        /// <inheritdoc cref="IMerged.SourcesChanged"/>
        public event EventHandler? SourcesChanged;

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

        /// <inheritdoc/>
        public event EventHandler<DownloadInfo>? DownloadInfoChanged;

        /// <inheritdoc />
        public string Id => _preferredSource.Id;

        /// <inheritdoc />
        public string Name => _preferredSource.Name;

        /// <inheritdoc />
        public string? Description => _preferredSource.Description;

        /// <inheritdoc />
        public PlaybackState PlaybackState => _preferredSource.PlaybackState;

        /// <inheritdoc/>
        public DownloadInfo DownloadInfo => default;

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
        public Task PlayTrackCollectionAsync(CancellationToken cancellationToken = default) => _preferredSource.PlayTrackCollectionAsync(cancellationToken);

        /// <inheritdoc />
        public Task PauseTrackCollectionAsync(CancellationToken cancellationToken = default) => _preferredSource.PauseTrackCollectionAsync(cancellationToken);

        /// <inheritdoc />
        public Task PlayTrackCollectionAsync(ITrack track, CancellationToken cancellationToken = default)
        {
            var targetCore = _preferredSource.SourceCore;
            var source = track.GetSources<ICoreTrack>().FirstOrDefault(x => x.SourceCore.InstanceId == targetCore.InstanceId);

            Guard.IsNotNull(source, nameof(source));

            return _preferredSource.PlayTrackCollectionAsync(source, cancellationToken);
        }

        /// <inheritdoc />
        public Task ChangeNameAsync(string name, CancellationToken cancellationToken = default) => _preferredSource.ChangeNameAsync(name, cancellationToken);

        /// <inheritdoc />
        public Task ChangeDescriptionAsync(string? description, CancellationToken cancellationToken = default) => _preferredSource.ChangeDescriptionAsync(description, cancellationToken);

        /// <inheritdoc />
        public Task ChangeDurationAsync(TimeSpan duration, CancellationToken cancellationToken = default) => _preferredSource.ChangeDurationAsync(duration, cancellationToken);

        /// <inheritdoc />
        public Task<bool> IsAddTrackAvailableAsync(int index, CancellationToken cancellationToken = default) => _trackMap.IsAddItemAvailableAsync(index, cancellationToken);

        /// <inheritdoc />
        public Task<bool> IsAddImageAvailableAsync(int index, CancellationToken cancellationToken = default) => _imageMap.IsAddItemAvailableAsync(index, cancellationToken);

        /// <inheritdoc />
        public Task<bool> IsAddUrlAvailableAsync(int index, CancellationToken cancellationToken = default) => _urlMap.IsAddItemAvailableAsync(index, cancellationToken);

        /// <inheritdoc />
        public Task<bool> IsRemoveTrackAvailableAsync(int index, CancellationToken cancellationToken = default) => _trackMap.IsRemoveItemAvailableAsync(index, cancellationToken);

        /// <inheritdoc />
        public Task<bool> IsRemoveImageAvailableAsync(int index, CancellationToken cancellationToken = default) => _imageMap.IsRemoveItemAvailableAsync(index, cancellationToken);

        /// <inheritdoc />
        public Task<bool> IsRemoveUrlAvailableAsync(int index, CancellationToken cancellationToken = default) => _urlMap.IsRemoveItemAvailableAsync(index, cancellationToken);

        /// <inheritdoc/>
        public Task StartDownloadOperationAsync(DownloadOperation operation, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        IReadOnlyList<ICoreTrackCollection> IMerged<ICoreTrackCollection>.Sources => _sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreImageCollection> IMerged<ICoreImageCollection>.Sources => _sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreUrlCollection> IMerged<ICoreUrlCollection>.Sources => _sources;

        /// <inheritdoc />
        public IAsyncEnumerable<ITrack> GetTracksAsync(int limit, int offset, CancellationToken cancellationToken = default) => _trackMap.GetItemsAsync(limit, offset, cancellationToken);

        /// <inheritdoc />
        public IAsyncEnumerable<IImage> GetImagesAsync(int limit, int offset, CancellationToken cancellationToken = default) => _imageMap.GetItemsAsync(limit, offset, cancellationToken);

        /// <inheritdoc />
        public IAsyncEnumerable<IUrl> GetUrlsAsync(int limit, int offset, CancellationToken cancellationToken = default) => _urlMap.GetItemsAsync(limit, offset, cancellationToken);

        /// <inheritdoc />
        public Task AddTrackAsync(ITrack track, int index, CancellationToken cancellationToken = default) => _trackMap.InsertItemAsync(track, index, cancellationToken);

        /// <inheritdoc />
        public Task AddImageAsync(IImage image, int index, CancellationToken cancellationToken = default) => _imageMap.InsertItemAsync(image, index, cancellationToken);

        /// <inheritdoc />
        public Task AddUrlAsync(IUrl url, int index, CancellationToken cancellationToken = default) => _urlMap.InsertItemAsync(url, index, cancellationToken);

        /// <inheritdoc />
        public Task RemoveTrackAsync(int index, CancellationToken cancellationToken = default) => _trackMap.RemoveAtAsync(index, cancellationToken);

        /// <inheritdoc />
        public Task RemoveImageAsync(int index, CancellationToken cancellationToken = default) => _imageMap.RemoveAtAsync(index, cancellationToken);

        /// <inheritdoc />
        public Task RemoveUrlAsync(int index, CancellationToken cancellationToken = default) => _urlMap.RemoveAtAsync(index, cancellationToken);

        /// <inheritdoc />
        public void AddSource(ICoreTrackCollection itemToMerge)
        {
            Guard.IsNotNull(itemToMerge, nameof(itemToMerge));

            _trackMap.AddSource(itemToMerge);
            _imageMap.AddSource(itemToMerge);
            _urlMap.AddSource(itemToMerge);

            _sources.Add(itemToMerge);
            SourcesChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <inheritdoc />
        public void RemoveSource(ICoreTrackCollection itemToRemove)
        {
            Guard.IsNotNull(itemToRemove, nameof(itemToRemove));

            _trackMap.RemoveSource(itemToRemove);
            _imageMap.RemoveSource(itemToRemove);
            _urlMap.RemoveSource(itemToRemove);

            _sources.Remove(itemToRemove);
            SourcesChanged?.Invoke(this, EventArgs.Empty);
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
    }
}
