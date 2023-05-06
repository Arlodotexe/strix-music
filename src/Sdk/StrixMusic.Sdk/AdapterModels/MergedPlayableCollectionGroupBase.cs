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
using OwlCore.Extensions;
using StrixMusic.Sdk.AppModels;
using StrixMusic.Sdk.BaseModels;
using StrixMusic.Sdk.CoreModels;
using StrixMusic.Sdk.Extensions;
using StrixMusic.Sdk.MediaPlayback;

namespace StrixMusic.Sdk.AdapterModels
{
    /// <summary>
    /// A base that merges multiple <see cref="IPlayableCollectionGroupBase"/>s.
    /// </summary>
    public abstract class MergedPlayableCollectionGroupBase<TCoreBase> : IPlayableCollectionGroup, IMergedMutable<TCoreBase>
        where TCoreBase : class, ICorePlayableCollectionGroup
    {
        private readonly MergedCollectionMap<IAlbumCollection, ICoreAlbumCollection, IAlbumCollectionItem, ICoreAlbumCollectionItem> _albumCollectionMap;
        private readonly MergedCollectionMap<IArtistCollection, ICoreArtistCollection, IArtistCollectionItem, ICoreArtistCollectionItem> _artistCollectionMap;
        private readonly MergedCollectionMap<IPlaylistCollection, ICorePlaylistCollection, IPlaylistCollectionItem, ICorePlaylistCollectionItem> _playlistCollectionMap;
        private readonly MergedCollectionMap<ITrackCollection, ICoreTrackCollection, ITrack, ICoreTrack> _trackCollectionMap;
        private readonly MergedCollectionMap<IPlayableCollectionGroup, ICorePlayableCollectionGroup, IPlayableCollectionGroup, ICorePlayableCollectionGroup> _playableCollectionGroupMap;
        private readonly MergedCollectionMap<IImageCollection, ICoreImageCollection, IImage, ICoreImage> _imageCollectionMap;
        private readonly MergedCollectionMap<IUrlCollection, ICoreUrlCollection, IUrl, ICoreUrl> _urlCollectionMap;

        /// <summary>
        /// Initializes a new instance of the <see cref="MergedPlayableCollectionGroupBase{T}"/> class.
        /// </summary>
        protected MergedPlayableCollectionGroupBase(IEnumerable<TCoreBase> sources, MergedCollectionConfig config)
        {
            // TODO: Use top Preferred core.
            if (sources is null)
                throw new ArgumentNullException(nameof(sources));

            StoredSources = sources.ToList();
            Guard.HasSizeGreaterThan(StoredSources, 0, nameof(StoredSources));

            PreferredSource = StoredSources[0];

            _albumCollectionMap = new MergedCollectionMap<IAlbumCollection, ICoreAlbumCollection, IAlbumCollectionItem, ICoreAlbumCollectionItem>(this, config);
            _artistCollectionMap = new MergedCollectionMap<IArtistCollection, ICoreArtistCollection, IArtistCollectionItem, ICoreArtistCollectionItem>(this, config);
            _playlistCollectionMap = new MergedCollectionMap<IPlaylistCollection, ICorePlaylistCollection, IPlaylistCollectionItem, ICorePlaylistCollectionItem>(this, config);
            _trackCollectionMap = new MergedCollectionMap<ITrackCollection, ICoreTrackCollection, ITrack, ICoreTrack>(this, config);
            _playableCollectionGroupMap = new MergedCollectionMap<IPlayableCollectionGroup, ICorePlayableCollectionGroup, IPlayableCollectionGroup, ICorePlayableCollectionGroup>(this, config);
            _imageCollectionMap = new MergedCollectionMap<IImageCollection, ICoreImageCollection, IImage, ICoreImage>(this, config);
            _urlCollectionMap = new MergedCollectionMap<IUrlCollection, ICoreUrlCollection, IUrl, ICoreUrl>(this, config);

            AttachPropertyChangedEvents(PreferredSource);
            AttachCollectionChangedEvents();

            foreach (var item in StoredSources)
            {
                TotalChildrenCount += item.TotalChildrenCount;
                TotalPlaylistItemsCount += item.TotalPlaylistItemsCount;
                TotalTrackCount += item.TotalTrackCount;
                TotalAlbumItemsCount += item.TotalAlbumItemsCount;
                TotalArtistItemsCount += item.TotalArtistItemsCount;
                TotalImageCount += item.TotalImageCount;
                TotalUrlCount += item.TotalUrlCount;
                Duration += item.Duration;
            }

            Name = PreferredSource.Name;
            Description = PreferredSource.Description;
            PlaybackState = PreferredSource.PlaybackState;
            LastPlayed = PreferredSource.LastPlayed;
            AddedAt = PreferredSource.AddedAt;
        }

        /// <summary>
        /// The top preferred source for this item, used for property getters.
        /// </summary>
        protected ICorePlayableCollectionGroup PreferredSource { get; }

        /// <summary>
        /// The source items that were merged to create this <see cref="MergedPlayableCollectionGroupBase{T}"/>
        /// </summary>
        protected List<TCoreBase> StoredSources { get; }

        private void AttachPropertyChangedEvents(ICorePlayableCollectionGroup source)
        {
            source.PlaybackStateChanged += PlaybackStateChanged;
            source.NameChanged += NameChanged;
            source.DescriptionChanged += DescriptionChanged;
            source.DurationChanged += DurationChanged;
            source.LastPlayedChanged += LastPlayedChanged;
            source.IsPlayTrackCollectionAsyncAvailableChanged += IsPlayTrackCollectionAsyncAvailableChanged;
            source.IsPauseTrackCollectionAsyncAvailableChanged += IsPauseTrackCollectionAsyncAvailableChanged;
            source.IsPlayArtistCollectionAsyncAvailableChanged += IsPlayArtistCollectionAsyncAvailableChanged;
            source.IsPauseArtistCollectionAsyncAvailableChanged += IsPauseArtistCollectionAsyncAvailableChanged;
            source.IsPlayAlbumCollectionAsyncAvailableChanged += IsPlayAlbumCollectionAsyncAvailableChanged;
            source.IsPauseAlbumCollectionAsyncAvailableChanged += IsPauseAlbumCollectionAsyncAvailableChanged;
            source.IsChangeNameAsyncAvailableChanged += IsChangeNameAsyncAvailableChanged;
            source.IsChangeDurationAsyncAvailableChanged += IsChangeDurationAsyncAvailableChanged;
            source.IsChangeDescriptionAsyncAvailableChanged += IsChangeDescriptionAsyncAvailableChanged;
        }

        private void DetachPropertyChangedEvents(ICorePlayableCollectionGroup source)
        {
            source.PlaybackStateChanged -= PlaybackStateChanged;
            source.NameChanged -= NameChanged;
            source.DescriptionChanged -= DescriptionChanged;
            source.DurationChanged -= DurationChanged;
            source.LastPlayedChanged -= LastPlayedChanged;
            source.IsPlayTrackCollectionAsyncAvailableChanged -= IsPlayTrackCollectionAsyncAvailableChanged;
            source.IsPauseTrackCollectionAsyncAvailableChanged -= IsPauseTrackCollectionAsyncAvailableChanged;
            source.IsPlayArtistCollectionAsyncAvailableChanged -= IsPlayArtistCollectionAsyncAvailableChanged;
            source.IsPauseArtistCollectionAsyncAvailableChanged -= IsPauseArtistCollectionAsyncAvailableChanged;
            source.IsPlayAlbumCollectionAsyncAvailableChanged -= IsPlayAlbumCollectionAsyncAvailableChanged;
            source.IsPauseAlbumCollectionAsyncAvailableChanged -= IsPauseAlbumCollectionAsyncAvailableChanged;
            source.IsChangeNameAsyncAvailableChanged -= IsChangeNameAsyncAvailableChanged;
            source.IsChangeDurationAsyncAvailableChanged -= IsChangeDurationAsyncAvailableChanged;
            source.IsChangeDescriptionAsyncAvailableChanged -= IsChangeDescriptionAsyncAvailableChanged;
        }

        private void AttachCollectionChangedEvents()
        {
            _albumCollectionMap.ItemsChanged += AlbumCollectionMap_ItemsChanged;
            _artistCollectionMap.ItemsChanged += ArtistCollectionMap_ItemsChanged;
            _playlistCollectionMap.ItemsChanged += PlaylistCollectionMap_ItemsChanged;
            _playableCollectionGroupMap.ItemsChanged += PlayableCollectionGroupMap_ItemsChanged;
            _trackCollectionMap.ItemsChanged += TrackCollectionMap_ItemsChanged;
            _imageCollectionMap.ItemsChanged += ImagesCollectionMap_ItemsChanged;
            _urlCollectionMap.ItemsChanged += UrlsCollectionMap_ItemsChanged;

            _albumCollectionMap.ItemsCountChanged += AlbumCollectionMap_ItemsCountChanged;
            _artistCollectionMap.ItemsCountChanged += ArtistCollectionMap_ItemsCountChanged;
            _trackCollectionMap.ItemsCountChanged += TrackCollectionMap_ItemsCountChanged;
            _playlistCollectionMap.ItemsCountChanged += PlaylistCollectionMap_ItemsCountChanged;
            _playableCollectionGroupMap.ItemsCountChanged += PlayableCollectionGroupMap_ItemsCountChanged;
            _imageCollectionMap.ItemsCountChanged += ImagesCollectionMap_ItemsCountChanged;
            _urlCollectionMap.ItemsCountChanged += UrlsCollectionMap_ItemsCountChanged;
        }

        private void DetachCollectionChangedEvents()
        {
            _albumCollectionMap.ItemsChanged -= AlbumCollectionMap_ItemsChanged;
            _artistCollectionMap.ItemsChanged -= ArtistCollectionMap_ItemsChanged;
            _playlistCollectionMap.ItemsChanged -= PlaylistCollectionMap_ItemsChanged;
            _playableCollectionGroupMap.ItemsChanged -= PlayableCollectionGroupMap_ItemsChanged;
            _trackCollectionMap.ItemsChanged -= TrackCollectionMap_ItemsChanged;
            _imageCollectionMap.ItemsChanged -= ImagesCollectionMap_ItemsChanged;
            _urlCollectionMap.ItemsChanged -= UrlsCollectionMap_ItemsChanged;

            _albumCollectionMap.ItemsCountChanged -= AlbumCollectionMap_ItemsCountChanged;
            _artistCollectionMap.ItemsCountChanged -= ArtistCollectionMap_ItemsCountChanged;
            _playlistCollectionMap.ItemsCountChanged -= PlaylistCollectionMap_ItemsCountChanged;
            _playableCollectionGroupMap.ItemsCountChanged -= PlayableCollectionGroupMap_ItemsCountChanged;
            _imageCollectionMap.ItemsCountChanged -= ImagesCollectionMap_ItemsCountChanged;
            _trackCollectionMap.ItemsCountChanged -= TrackCollectionMap_ItemsCountChanged;
            _urlCollectionMap.ItemsCountChanged -= UrlsCollectionMap_ItemsCountChanged;
        }

        /// <inheritdoc/>
        public event EventHandler<PlaybackState>? PlaybackStateChanged;

        /// <inheritdoc/>
        public event EventHandler<string>? NameChanged;

        /// <inheritdoc/>
        public event EventHandler<string?>? DescriptionChanged;

        /// <inheritdoc/>
        public event EventHandler<TimeSpan>? DurationChanged;

        /// <inheritdoc />
        public event EventHandler<DateTime?>? LastPlayedChanged;

        /// <inheritdoc />
        public event EventHandler<bool>? IsPlayAlbumCollectionAsyncAvailableChanged;

        /// <inheritdoc />
        public event EventHandler<bool>? IsPauseAlbumCollectionAsyncAvailableChanged;

        /// <inheritdoc />
        public event EventHandler<bool>? IsPlayArtistCollectionAsyncAvailableChanged;

        /// <inheritdoc />
        public event EventHandler<bool>? IsPauseArtistCollectionAsyncAvailableChanged;

        /// <inheritdoc />
        public event EventHandler<bool>? IsPlayPlaylistCollectionAsyncAvailableChanged;

        /// <inheritdoc />
        public event EventHandler<bool>? IsPausePlaylistCollectionAsyncAvailableChanged;

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
        public event EventHandler<int>? ArtistItemsCountChanged;

        /// <inheritdoc />
        public event EventHandler<int>? AlbumItemsCountChanged;

        /// <inheritdoc />
        public event EventHandler<int>? PlaylistItemsCountChanged;

        /// <inheritdoc />
        public event EventHandler<int>? ChildrenCountChanged;

        /// <inheritdoc />
        public event EventHandler<int>? ImagesCountChanged;

        /// <inheritdoc />
        public event EventHandler<int>? UrlsCountChanged;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<ITrack>? TracksChanged;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<IArtistCollectionItem>? ArtistItemsChanged;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<IAlbumCollectionItem>? AlbumItemsChanged;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<IPlaylistCollectionItem>? PlaylistItemsChanged;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<IPlayableCollectionGroup>? ChildItemsChanged;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<IImage>? ImagesChanged;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<IUrl>? UrlsChanged;

        /// <inheritdoc/>
        public event EventHandler<DownloadInfo>? DownloadInfoChanged;

        private void ImagesCollectionMap_ItemsCountChanged(object? sender, int e)
        {
            TotalImageCount = e;
            ImagesCountChanged?.Invoke(this, e);
        }

        private void UrlsCollectionMap_ItemsCountChanged(object? sender, int e)
        {
            TotalUrlCount = e;
            UrlsCountChanged?.Invoke(this, e);
        }

        private void PlayableCollectionGroupMap_ItemsCountChanged(object? sender, int e)
        {
            TotalChildrenCount = e;
            ChildrenCountChanged?.Invoke(this, e);
        }

        private void PlaylistCollectionMap_ItemsCountChanged(object? sender, int e)
        {
            TotalPlaylistItemsCount = e;
            PlaylistItemsCountChanged?.Invoke(this, e);
        }

        private void ArtistCollectionMap_ItemsCountChanged(object? sender, int e)
        {
            TotalArtistItemsCount = e;
            ArtistItemsCountChanged?.Invoke(this, e);
        }

        private void AlbumCollectionMap_ItemsCountChanged(object? sender, int e)
        {
            TotalAlbumItemsCount = e;
            AlbumItemsCountChanged?.Invoke(this, e);
        }

        private void TrackCollectionMap_ItemsCountChanged(object? sender, int e)
        {
            TotalTrackCount = e;
            TracksCountChanged?.Invoke(this, e);
        }

        private void TrackCollectionMap_ItemsChanged(object sender, IReadOnlyList<CollectionChangedItem<ITrack>> addedItems, IReadOnlyList<CollectionChangedItem<ITrack>> removedItems)
        {
            TracksChanged?.Invoke(this, addedItems, removedItems);
        }

        private void PlayableCollectionGroupMap_ItemsChanged(object sender, IReadOnlyList<CollectionChangedItem<IPlayableCollectionGroup>> addedItems, IReadOnlyList<CollectionChangedItem<IPlayableCollectionGroup>> removedItems)
        {
            ChildItemsChanged?.Invoke(this, addedItems, removedItems);
        }

        private void PlaylistCollectionMap_ItemsChanged(object sender, IReadOnlyList<CollectionChangedItem<IPlaylistCollectionItem>> addedItems, IReadOnlyList<CollectionChangedItem<IPlaylistCollectionItem>> removedItems)
        {
            PlaylistItemsChanged?.Invoke(this, addedItems, removedItems);
        }

        private void AlbumCollectionMap_ItemsChanged(object sender, IReadOnlyList<CollectionChangedItem<IAlbumCollectionItem>> addedItems, IReadOnlyList<CollectionChangedItem<IAlbumCollectionItem>> removedItems)
        {
            AlbumItemsChanged?.Invoke(this, addedItems, removedItems);
        }

        private void ArtistCollectionMap_ItemsChanged(object sender, IReadOnlyList<CollectionChangedItem<IArtistCollectionItem>> addedItems, IReadOnlyList<CollectionChangedItem<IArtistCollectionItem>> removedItems)
        {
            ArtistItemsChanged?.Invoke(this, addedItems, removedItems);
        }

        private void ImagesCollectionMap_ItemsChanged(object sender, IReadOnlyList<CollectionChangedItem<IImage>> addedItems, IReadOnlyList<CollectionChangedItem<IImage>> removedItems)
        {
            ImagesChanged?.Invoke(this, addedItems, removedItems);
        }

        private void UrlsCollectionMap_ItemsChanged(object sender, IReadOnlyList<CollectionChangedItem<IUrl>> addedItems, IReadOnlyList<CollectionChangedItem<IUrl>> removedItems)
        {
            UrlsChanged?.Invoke(this, addedItems, removedItems);
        }
        
        /// <inheritdoc cref="IMerged.SourcesChanged"/>
        public event EventHandler? SourcesChanged;

        /// <inheritdoc />
        IReadOnlyList<ICorePlayableCollectionGroupChildren> IMerged<ICorePlayableCollectionGroupChildren>.Sources => StoredSources;

        /// <inheritdoc />
        IReadOnlyList<ICoreArtistCollection> IMerged<ICoreArtistCollection>.Sources => StoredSources;

        /// <inheritdoc />
        IReadOnlyList<ICoreArtistCollectionItem> IMerged<ICoreArtistCollectionItem>.Sources => StoredSources;

        /// <inheritdoc />
        IReadOnlyList<ICoreAlbumCollection> IMerged<ICoreAlbumCollection>.Sources => StoredSources;

        /// <inheritdoc />
        IReadOnlyList<ICoreTrackCollection> IMerged<ICoreTrackCollection>.Sources => StoredSources;

        /// <inheritdoc />
        IReadOnlyList<ICorePlaylistCollection> IMerged<ICorePlaylistCollection>.Sources => StoredSources;

        /// <inheritdoc />
        IReadOnlyList<ICorePlaylistCollectionItem> IMerged<ICorePlaylistCollectionItem>.Sources => StoredSources;

        /// <inheritdoc />
        IReadOnlyList<ICoreImageCollection> IMerged<ICoreImageCollection>.Sources => StoredSources;

        /// <inheritdoc />
        IReadOnlyList<ICoreUrlCollection> IMerged<ICoreUrlCollection>.Sources => StoredSources;

        /// <inheritdoc />
        IReadOnlyList<ICorePlayableCollectionGroup> IMerged<ICorePlayableCollectionGroup>.Sources => StoredSources;

        /// <inheritdoc />
        IReadOnlyList<ICoreAlbumCollectionItem> IMerged<ICoreAlbumCollectionItem>.Sources => StoredSources;

        /// <inheritdoc cref="IMerged{T}.Sources"/>
        public virtual IReadOnlyList<TCoreBase> Sources => StoredSources;

        /// <inheritdoc/>
        public string Id => PreferredSource.Id;

        /// <inheritdoc/>
        public string Name { get; internal set; }

        /// <inheritdoc/>
        public string? Description { get; internal set; }

        /// <inheritdoc/>
        public PlaybackState PlaybackState { get; internal set; }

        /// <inheritdoc/>
        public DownloadInfo DownloadInfo => default;

        /// <inheritdoc/>
        public TimeSpan Duration { get; internal set; } = new TimeSpan(0);

        /// <inheritdoc />
        public DateTime? LastPlayed { get; internal set; }

        /// <inheritdoc />
        public DateTime? AddedAt { get; internal set; }

        /// <inheritdoc/>
        public int TotalChildrenCount { get; internal set; }

        /// <inheritdoc/>
        public int TotalPlaylistItemsCount { get; internal set; }

        /// <inheritdoc/>
        public int TotalTrackCount { get; internal set; }

        /// <inheritdoc/>
        public int TotalAlbumItemsCount { get; internal set; }

        /// <inheritdoc/>
        public int TotalArtistItemsCount { get; internal set; }

        /// <inheritdoc />
        public int TotalImageCount { get; internal set; }

        /// <inheritdoc />
        public int TotalUrlCount { get; internal set; }

        /// <inheritdoc/>
        public virtual bool IsPlayAlbumCollectionAsyncAvailable => PreferredSource.IsPlayAlbumCollectionAsyncAvailable;

        /// <inheritdoc/>
        public virtual bool IsPauseAlbumCollectionAsyncAvailable => PreferredSource.IsPauseAlbumCollectionAsyncAvailable;

        /// <inheritdoc/>
        public virtual bool IsPlayArtistCollectionAsyncAvailable => PreferredSource.IsPlayArtistCollectionAsyncAvailable;

        /// <inheritdoc/>
        public virtual bool IsPauseArtistCollectionAsyncAvailable => PreferredSource.IsPauseArtistCollectionAsyncAvailable;

        /// <inheritdoc/>
        public virtual bool IsPlayPlaylistCollectionAsyncAvailable => PreferredSource.IsPlayPlaylistCollectionAsyncAvailable;

        /// <inheritdoc/>
        public virtual bool IsPausePlaylistCollectionAsyncAvailable => PreferredSource.IsPausePlaylistCollectionAsyncAvailable;

        /// <inheritdoc/>
        public virtual bool IsPlayTrackCollectionAsyncAvailable => PreferredSource.IsPlayTrackCollectionAsyncAvailable;

        /// <inheritdoc/>
        public virtual bool IsPauseTrackCollectionAsyncAvailable => PreferredSource.IsPauseTrackCollectionAsyncAvailable;

        /// <inheritdoc/>
        public virtual bool IsChangeNameAsyncAvailable => PreferredSource.IsChangeNameAsyncAvailable;

        /// <inheritdoc/>
        public virtual bool IsChangeDescriptionAsyncAvailable => PreferredSource.IsChangeDescriptionAsyncAvailable;

        /// <inheritdoc/>
        public virtual bool IsChangeDurationAsyncAvailable => PreferredSource.IsChangeDurationAsyncAvailable;

        /// <inheritdoc />
        public Task<bool> IsAddTrackAvailableAsync(int index, CancellationToken cancellationToken = default) => _trackCollectionMap.IsAddItemAvailableAsync(index, cancellationToken);

        /// <inheritdoc />
        public Task<bool> IsAddAlbumItemAvailableAsync(int index, CancellationToken cancellationToken = default) => _albumCollectionMap.IsAddItemAvailableAsync(index, cancellationToken);

        /// <inheritdoc />
        public Task<bool> IsAddArtistItemAvailableAsync(int index, CancellationToken cancellationToken = default) => _artistCollectionMap.IsAddItemAvailableAsync(index, cancellationToken);

        /// <inheritdoc />
        public Task<bool> IsAddPlaylistItemAvailableAsync(int index, CancellationToken cancellationToken = default) => _playlistCollectionMap.IsAddItemAvailableAsync(index, cancellationToken);

        /// <inheritdoc />
        public Task<bool> IsAddChildAvailableAsync(int index, CancellationToken cancellationToken = default) => _playableCollectionGroupMap.IsAddItemAvailableAsync(index, cancellationToken);

        /// <inheritdoc />
        public Task<bool> IsAddImageAvailableAsync(int index, CancellationToken cancellationToken = default) => _imageCollectionMap.IsAddItemAvailableAsync(index, cancellationToken);

        /// <inheritdoc />
        public Task<bool> IsAddUrlAvailableAsync(int index, CancellationToken cancellationToken = default) => _urlCollectionMap.IsAddItemAvailableAsync(index, cancellationToken);

        /// <inheritdoc/>
        public Task<bool> IsRemoveTrackAvailableAsync(int index, CancellationToken cancellationToken = default) => _trackCollectionMap.IsRemoveItemAvailableAsync(index, cancellationToken);

        /// <inheritdoc/>
        public Task<bool> IsRemoveArtistItemAvailableAsync(int index, CancellationToken cancellationToken = default) => _artistCollectionMap.IsRemoveItemAvailableAsync(index, cancellationToken);

        /// <inheritdoc/>
        public Task<bool> IsRemoveAlbumItemAvailableAsync(int index, CancellationToken cancellationToken = default) => _albumCollectionMap.IsRemoveItemAvailableAsync(index, cancellationToken);

        /// <inheritdoc/>
        public Task<bool> IsRemovePlaylistItemAvailableAsync(int index, CancellationToken cancellationToken = default) => _playableCollectionGroupMap.IsRemoveItemAvailableAsync(index, cancellationToken);

        /// <inheritdoc/>
        public Task<bool> IsRemoveChildAvailableAsync(int index, CancellationToken cancellationToken = default) => _playableCollectionGroupMap.IsRemoveItemAvailableAsync(index, cancellationToken);

        /// <inheritdoc/>
        public Task<bool> IsRemoveImageAvailableAsync(int index, CancellationToken cancellationToken = default) => _imageCollectionMap.IsRemoveItemAvailableAsync(index, cancellationToken);

        /// <inheritdoc/>
        public Task<bool> IsRemoveUrlAvailableAsync(int index, CancellationToken cancellationToken = default) => _urlCollectionMap.IsRemoveItemAvailableAsync(index, cancellationToken);

        /// <inheritdoc/>
        public Task PauseAlbumCollectionAsync(CancellationToken cancellationToken) => PreferredSource.PauseAlbumCollectionAsync(cancellationToken);

        /// <inheritdoc/>
        public Task PlayAlbumCollectionAsync(CancellationToken cancellationToken) => PreferredSource.PlayAlbumCollectionAsync(cancellationToken);

        /// <inheritdoc/>
        public Task PauseArtistCollectionAsync(CancellationToken cancellationToken) => PreferredSource.PauseArtistCollectionAsync(cancellationToken);

        /// <inheritdoc/>
        public Task PlayArtistCollectionAsync(CancellationToken cancellationToken) => PreferredSource.PlayArtistCollectionAsync(cancellationToken);

        /// <inheritdoc/>
        public Task PausePlaylistCollectionAsync(CancellationToken cancellationToken) => PreferredSource.PausePlaylistCollectionAsync(cancellationToken);

        /// <inheritdoc/>
        public Task PlayPlaylistCollectionAsync(CancellationToken cancellationToken) => PreferredSource.PlayPlaylistCollectionAsync(cancellationToken);

        /// <inheritdoc/>
        public Task PauseTrackCollectionAsync(CancellationToken cancellationToken) => PreferredSource.PauseTrackCollectionAsync(cancellationToken);

        /// <inheritdoc/>
        public Task PlayTrackCollectionAsync(CancellationToken cancellationToken) => PreferredSource.PlayTrackCollectionAsync(cancellationToken);

        /// <inheritdoc />
        public Task PlayPlayableCollectionGroupAsync(CancellationToken cancellationToken) => PreferredSource.PlayPlayableCollectionGroupAsync(cancellationToken);

        /// <inheritdoc />
        public Task PausePlayableCollectionGroupAsync(CancellationToken cancellationToken) => PreferredSource.PausePlayableCollectionGroupAsync(cancellationToken);

        /// <inheritdoc />
        public Task PlayTrackCollectionAsync(ITrack track, CancellationToken cancellationToken = default)
        {
            var targetCore = PreferredSource.SourceCore;
            var source = track.GetSources<ICoreTrack>().FirstOrDefault(x => x.SourceCore.InstanceId == targetCore.InstanceId);

            Guard.IsNotNull(source, nameof(source));

            return PreferredSource.PlayTrackCollectionAsync(source, cancellationToken);
        }

        /// <inheritdoc />
        public Task PlayArtistCollectionAsync(IArtistCollectionItem artistItem, CancellationToken cancellationToken = default)
        {
            var targetCore = PreferredSource.SourceCore;

            ICoreArtistCollectionItem? source = null;

            if (artistItem is IArtist artist)
                source = artist.GetSources<ICoreArtist>().FirstOrDefault(x => x.SourceCore.InstanceId == targetCore.InstanceId);

            if (artistItem is IArtistCollection collection)
                source = collection.GetSources<ICoreArtistCollection>().FirstOrDefault(x => x.SourceCore.InstanceId == targetCore.InstanceId);

            Guard.IsNotNull(source, nameof(source));

            return PreferredSource.PlayArtistCollectionAsync(source, cancellationToken);
        }

        /// <inheritdoc />
        public Task PlayAlbumCollectionAsync(IAlbumCollectionItem albumItem, CancellationToken cancellationToken = default)
        {
            var targetCore = PreferredSource.SourceCore;

            ICoreAlbumCollectionItem? source = null;

            if (albumItem is IAlbum album)
                source = album.GetSources<ICoreAlbum>().FirstOrDefault(x => x.SourceCore.InstanceId == targetCore.InstanceId);

            if (albumItem is IAlbumCollection collection)
                source = collection.GetSources<ICoreAlbumCollection>().FirstOrDefault(x => x.SourceCore.InstanceId == targetCore.InstanceId);

            Guard.IsNotNull(source, nameof(source));

            return PreferredSource.PlayAlbumCollectionAsync(source, cancellationToken);
        }

        /// <inheritdoc />
        public Task PlayPlaylistCollectionAsync(IPlaylistCollectionItem playlistItem, CancellationToken cancellationToken = default)
        {
            var targetCore = PreferredSource.SourceCore;

            ICorePlaylistCollectionItem? source = null;

            if (playlistItem is IPlaylist playlist)
                source = playlist.GetSources<ICorePlaylist>().FirstOrDefault(x => x.SourceCore.InstanceId == targetCore.InstanceId);

            if (playlistItem is IPlaylistCollection collection)
                source = collection.GetSources<ICorePlaylistCollection>().FirstOrDefault(x => x.SourceCore.InstanceId == targetCore.InstanceId);

            Guard.IsNotNull(source, nameof(source));

            return PreferredSource.PlayPlaylistCollectionAsync(source, cancellationToken);
        }

        /// <inheritdoc />
        public Task PlayPlayableCollectionGroupAsync(IPlayableCollectionGroup collectionGroup, CancellationToken cancellationToken = default)
        {
            var targetCore = PreferredSource.SourceCore;
            var source = collectionGroup.GetSources<ICorePlayableCollectionGroup>().FirstOrDefault(x => x.SourceCore.InstanceId == targetCore.InstanceId);

            Guard.IsNotNull(source, nameof(source));

            return PreferredSource.PlayPlayableCollectionGroupAsync(source, cancellationToken);
        }

        /// <inheritdoc/>
        public IAsyncEnumerable<IAlbumCollectionItem> GetAlbumItemsAsync(int limit, int offset, CancellationToken cancellationToken = default)
        {
            return _albumCollectionMap.GetItemsAsync(limit, offset, cancellationToken);
        }

        /// <inheritdoc/>
        public IAsyncEnumerable<IArtistCollectionItem> GetArtistItemsAsync(int limit, int offset, CancellationToken cancellationToken = default)
        {
            return _artistCollectionMap.GetItemsAsync(limit, offset, cancellationToken);
        }

        /// <inheritdoc/>
        public IAsyncEnumerable<IPlayableCollectionGroup> GetChildrenAsync(int limit, int offset, CancellationToken cancellationToken = default)
        {
            return _playableCollectionGroupMap.GetItemsAsync(limit, offset, cancellationToken);
        }

        /// <inheritdoc/>
        public IAsyncEnumerable<IPlaylistCollectionItem> GetPlaylistItemsAsync(int limit, int offset, CancellationToken cancellationToken = default)
        {
            return _playlistCollectionMap.GetItemsAsync(limit, offset, cancellationToken);
        }

        /// <inheritdoc/>
        public IAsyncEnumerable<ITrack> GetTracksAsync(int limit, int offset, CancellationToken cancellationToken = default)
        {
            return _trackCollectionMap.GetItemsAsync(limit, offset, cancellationToken);
        }

        /// <inheritdoc />
        public IAsyncEnumerable<IImage> GetImagesAsync(int limit, int offset, CancellationToken cancellationToken = default)
        {
            return _imageCollectionMap.GetItemsAsync(limit, offset, cancellationToken);
        }

        /// <inheritdoc />
        public IAsyncEnumerable<IUrl> GetUrlsAsync(int limit, int offset, CancellationToken cancellationToken = default)
        {
            return _urlCollectionMap.GetItemsAsync(limit, offset, cancellationToken);
        }

        /// <inheritdoc/>
        public Task ChangeNameAsync(string name, CancellationToken cancellationToken = default)
        {
            return Sources.InParallel(source => source.IsChangeNameAsyncAvailable ? source.ChangeNameAsync(name) : Task.CompletedTask);
        }

        /// <inheritdoc/>
        public Task ChangeDescriptionAsync(string? description, CancellationToken cancellationToken = default)
        {
            return Sources.InParallel(source => source.IsChangeDescriptionAsyncAvailable ? source.ChangeDescriptionAsync(description) : Task.CompletedTask);
        }

        /// <inheritdoc/>
        public Task ChangeDurationAsync(TimeSpan duration, CancellationToken cancellationToken = default)
        {
            return Sources.InParallel(source => source.IsChangeDurationAsyncAvailable ? source.ChangeDurationAsync(duration) : Task.CompletedTask);
        }

        /// <inheritdoc/>
        public Task AddTrackAsync(ITrack track, int index, CancellationToken cancellationToken = default)
        {
            return _trackCollectionMap.InsertItemAsync(track, index, cancellationToken);
        }

        /// <inheritdoc/>
        public Task AddArtistItemAsync(IArtistCollectionItem artistItem, int index, CancellationToken cancellationToken = default)
        {
            return _artistCollectionMap.InsertItemAsync(artistItem, index, cancellationToken);
        }

        /// <inheritdoc/>
        public Task AddAlbumItemAsync(IAlbumCollectionItem albumItem, int index, CancellationToken cancellationToken = default)
        {
            return _albumCollectionMap.InsertItemAsync(albumItem, index, cancellationToken);
        }

        /// <inheritdoc/>
        public Task AddPlaylistItemAsync(IPlaylistCollectionItem playlistItem, int index, CancellationToken cancellationToken = default)
        {
            return _playlistCollectionMap.InsertItemAsync(playlistItem, index, cancellationToken);
        }

        /// <inheritdoc/>
        public Task AddChildAsync(IPlayableCollectionGroup child, int index, CancellationToken cancellationToken = default)
        {
            return _playableCollectionGroupMap.InsertItemAsync(child, index, cancellationToken);
        }

        /// <inheritdoc />
        public Task AddImageAsync(IImage image, int index, CancellationToken cancellationToken = default)
        {
            return _imageCollectionMap.InsertItemAsync(image, index, cancellationToken);
        }

        /// <inheritdoc />
        public Task AddUrlAsync(IUrl url, int index, CancellationToken cancellationToken = default)
        {
            return _urlCollectionMap.InsertItemAsync(url, index, cancellationToken);
        }

        /// <inheritdoc/>
        public Task RemoveTrackAsync(int index, CancellationToken cancellationToken = default)
        {
            return _trackCollectionMap.RemoveAtAsync(index, cancellationToken);
        }

        /// <inheritdoc/>
        public Task RemoveArtistItemAsync(int index, CancellationToken cancellationToken = default)
        {
            return _artistCollectionMap.RemoveAtAsync(index, cancellationToken);
        }

        /// <inheritdoc/>
        public Task RemoveAlbumItemAsync(int index, CancellationToken cancellationToken = default)
        {
            return _albumCollectionMap.RemoveAtAsync(index, cancellationToken);
        }

        /// <inheritdoc/>
        public Task RemovePlaylistItemAsync(int index, CancellationToken cancellationToken = default)
        {
            return _playlistCollectionMap.RemoveAtAsync(index, cancellationToken);
        }

        /// <inheritdoc/>
        public Task RemoveChildAsync(int index, CancellationToken cancellationToken = default)
        {
            return _playableCollectionGroupMap.RemoveAtAsync(index, cancellationToken);
        }

        /// <inheritdoc />
        public Task RemoveImageAsync(int index, CancellationToken cancellationToken = default)
        {
            return _imageCollectionMap.RemoveAtAsync(index, cancellationToken);
        }

        /// <inheritdoc />
        public Task RemoveUrlAsync(int index, CancellationToken cancellationToken = default)
        {
            return _urlCollectionMap.RemoveAtAsync(index, cancellationToken);
        }

        /// <inheritdoc/>
        public Task StartDownloadOperationAsync(DownloadOperation operation, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public void AddSource(TCoreBase itemToAdd)
        {
            Guard.IsNotNull(itemToAdd, nameof(itemToAdd));

            if (!Equals(itemToAdd))
                ThrowHelper.ThrowArgumentException<TCoreBase>("Tried to merge an artistItem that doesn't match. Verify that the item matches before merging the source.");

            StoredSources.Add(itemToAdd);

            _albumCollectionMap.AddSource(itemToAdd);
            _artistCollectionMap.AddSource(itemToAdd);
            _playableCollectionGroupMap.AddSource(itemToAdd);
            _playlistCollectionMap.AddSource(itemToAdd);
            _trackCollectionMap.AddSource(itemToAdd);
            _imageCollectionMap.AddSource(itemToAdd);
            _urlCollectionMap.AddSource(itemToAdd);
            
            SourcesChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <inheritdoc />
        public void RemoveSource(TCoreBase itemToRemove)
        {
            Guard.IsNotNull(itemToRemove, nameof(itemToRemove));

            StoredSources.Remove(itemToRemove);

            _albumCollectionMap.RemoveSource(itemToRemove);
            _artistCollectionMap.RemoveSource(itemToRemove);
            _playableCollectionGroupMap.RemoveSource(itemToRemove);
            _playlistCollectionMap.RemoveSource(itemToRemove);
            _trackCollectionMap.RemoveSource(itemToRemove);
            _imageCollectionMap.RemoveSource(itemToRemove);
            _urlCollectionMap.RemoveSource(itemToRemove);

            SourcesChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <inheritdoc />
        public bool Equals(ICorePlaylistCollectionItem? other) => Equals(other as ICorePlayableCollectionGroup);

        /// <inheritdoc />
        public bool Equals(ICorePlaylistCollection? other) => Equals(other as ICorePlayableCollectionGroup);

        /// <inheritdoc />
        public bool Equals(ICoreTrackCollection? other) => Equals(other as ICorePlayableCollectionGroup);

        /// <inheritdoc />
        public bool Equals(ICoreAlbumCollectionItem? other) => Equals(other as ICorePlayableCollectionGroup);

        /// <inheritdoc />
        public bool Equals(ICoreAlbumCollection? other) => Equals(other as ICorePlayableCollectionGroup);

        /// <inheritdoc />
        public bool Equals(ICoreArtistCollectionItem? other) => Equals(other as ICorePlayableCollectionGroup);

        /// <inheritdoc />
        public bool Equals(ICoreArtistCollection? other) => Equals(other as ICorePlayableCollectionGroup);

        /// <inheritdoc />
        public bool Equals(ICorePlayableCollectionGroupChildren? other) => Equals(other as ICorePlayableCollectionGroup);

        /// <inheritdoc />
        public bool Equals(ICoreImageCollection? other) => Equals(other as ICorePlayableCollectionGroup);

        /// <inheritdoc />
        public bool Equals(ICoreUrlCollection? other) => Equals(other as ICorePlayableCollectionGroup);

        /// <inheritdoc />
        public bool Equals(ICorePlayableCollectionGroup? other)
        {
            return Equals(other as TCoreBase);
        }

        /// <summary>
        /// Overrides the default equality comparer.
        /// </summary>
        /// <param name="other">The object to compare.</param>
        /// <returns>True if this is a match, otherwise false.</returns>
        public virtual bool Equals(TCoreBase? other)
        {
            return other != null && other.Name.Equals(Name, StringComparison.InvariantCulture);
        }
    }
}
