// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Toolkit.Diagnostics;
using OwlCore.Events;
using OwlCore.Extensions;
using StrixMusic.Sdk.Extensions;
using StrixMusic.Sdk.MediaPlayback;
using StrixMusic.Sdk.Models.Base;
using StrixMusic.Sdk.Models.Core;
using StrixMusic.Sdk.Services;

namespace StrixMusic.Sdk.Models.Merged
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
        private readonly List<ICore> _sourceCores;

        /// <summary>
        /// Initializes a new instance of the <see cref="MergedPlayableCollectionGroupBase{T}"/> class.
        /// </summary>
        protected MergedPlayableCollectionGroupBase(IEnumerable<TCoreBase> sources, ISettingsService settingsService)
        {
            // TODO: Use top Preferred core.
            if (sources is null)
                throw new ArgumentNullException(nameof(sources));

            StoredSources = sources.ToList();
            Guard.HasSizeGreaterThan(StoredSources, 0, nameof(StoredSources));

            PreferredSource = StoredSources[0];
            _sourceCores = StoredSources.Select(x => x.SourceCore).ToList();

            _albumCollectionMap = new MergedCollectionMap<IAlbumCollection, ICoreAlbumCollection, IAlbumCollectionItem, ICoreAlbumCollectionItem>(this, settingsService);
            _artistCollectionMap = new MergedCollectionMap<IArtistCollection, ICoreArtistCollection, IArtistCollectionItem, ICoreArtistCollectionItem>(this, settingsService);
            _playlistCollectionMap = new MergedCollectionMap<IPlaylistCollection, ICorePlaylistCollection, IPlaylistCollectionItem, ICorePlaylistCollectionItem>(this, settingsService);
            _trackCollectionMap = new MergedCollectionMap<ITrackCollection, ICoreTrackCollection, ITrack, ICoreTrack>(this, settingsService);
            _playableCollectionGroupMap = new MergedCollectionMap<IPlayableCollectionGroup, ICorePlayableCollectionGroup, IPlayableCollectionGroup, ICorePlayableCollectionGroup>(this, settingsService);
            _imageCollectionMap = new MergedCollectionMap<IImageCollection, ICoreImageCollection, IImage, ICoreImage>(this, settingsService);
            _urlCollectionMap = new MergedCollectionMap<IUrlCollection, ICoreUrlCollection, IUrl, ICoreUrl>(this, settingsService);

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

        private void ImagesCollectionMap_ItemsCountChanged(object sender, int e)
        {
            TotalImageCount = e;
            ImagesCountChanged?.Invoke(this, e);
        }

        private void UrlsCollectionMap_ItemsCountChanged(object sender, int e)
        {
            TotalUrlCount = e;
            UrlsCountChanged?.Invoke(this, e);
        }

        private void PlayableCollectionGroupMap_ItemsCountChanged(object sender, int e)
        {
            TotalChildrenCount = e;
            ChildrenCountChanged?.Invoke(this, e);
        }

        private void PlaylistCollectionMap_ItemsCountChanged(object sender, int e)
        {
            TotalPlaylistItemsCount = e;
            PlaylistItemsCountChanged?.Invoke(this, e);
        }

        private void ArtistCollectionMap_ItemsCountChanged(object sender, int e)
        {
            TotalArtistItemsCount = e;
            ArtistItemsCountChanged?.Invoke(this, e);
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

        /// <inheritdoc cref="IMerged{T}.SourceCores"/>
        public IReadOnlyList<ICore> SourceCores => _sourceCores;

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
        public Task<bool> IsAddTrackAvailableAsync(int index) => _trackCollectionMap.IsAddItemAvailableAsync(index);

        /// <inheritdoc />
        public Task<bool> IsAddAlbumItemAvailableAsync(int index) => _albumCollectionMap.IsAddItemAvailableAsync(index);

        /// <inheritdoc />
        public Task<bool> IsAddArtistItemAvailableAsync(int index) => _artistCollectionMap.IsAddItemAvailableAsync(index);

        /// <inheritdoc />
        public Task<bool> IsAddPlaylistItemAvailableAsync(int index) => _playlistCollectionMap.IsAddItemAvailableAsync(index);

        /// <inheritdoc />
        public Task<bool> IsAddChildAvailableAsync(int index) => _playableCollectionGroupMap.IsAddItemAvailableAsync(index);

        /// <inheritdoc />
        public Task<bool> IsAddImageAvailableAsync(int index) => _imageCollectionMap.IsAddItemAvailableAsync(index);

        /// <inheritdoc />
        public Task<bool> IsAddUrlAvailableAsync(int index) => _urlCollectionMap.IsAddItemAvailableAsync(index);

        /// <inheritdoc/>
        public Task<bool> IsRemoveTrackAvailableAsync(int index) => _trackCollectionMap.IsRemoveItemAvailableAsync(index);

        /// <inheritdoc/>
        public Task<bool> IsRemoveArtistItemAvailableAsync(int index) => _artistCollectionMap.IsRemoveItemAvailableAsync(index);

        /// <inheritdoc/>
        public Task<bool> IsRemoveAlbumItemAvailableAsync(int index) => _albumCollectionMap.IsRemoveItemAvailableAsync(index);

        /// <inheritdoc/>
        public Task<bool> IsRemovePlaylistItemAvailableAsync(int index) => _playableCollectionGroupMap.IsRemoveItemAvailableAsync(index);

        /// <inheritdoc/>
        public Task<bool> IsRemoveChildAvailableAsync(int index) => _playableCollectionGroupMap.IsRemoveItemAvailableAsync(index);

        /// <inheritdoc/>
        public Task<bool> IsRemoveImageAvailableAsync(int index) => _imageCollectionMap.IsRemoveItemAvailableAsync(index);

        /// <inheritdoc/>
        public Task<bool> IsRemoveUrlAvailableAsync(int index) => _urlCollectionMap.IsRemoveItemAvailableAsync(index);

        /// <inheritdoc/>
        public Task PauseAlbumCollectionAsync() => PreferredSource.PauseAlbumCollectionAsync();

        /// <inheritdoc/>
        public Task PlayAlbumCollectionAsync() => PreferredSource.PlayAlbumCollectionAsync();

        /// <inheritdoc/>
        public Task PauseArtistCollectionAsync() => PreferredSource.PauseArtistCollectionAsync();

        /// <inheritdoc/>
        public Task PlayArtistCollectionAsync() => PreferredSource.PlayArtistCollectionAsync();

        /// <inheritdoc/>
        public Task PausePlaylistCollectionAsync() => PreferredSource.PausePlaylistCollectionAsync();

        /// <inheritdoc/>
        public Task PlayPlaylistCollectionAsync() => PreferredSource.PlayPlaylistCollectionAsync();

        /// <inheritdoc/>
        public Task PauseTrackCollectionAsync() => PreferredSource.PauseTrackCollectionAsync();

        /// <inheritdoc/>
        public Task PlayTrackCollectionAsync() => PreferredSource.PlayTrackCollectionAsync();

        /// <inheritdoc />
        public Task PlayPlayableCollectionGroupAsync() => PreferredSource.PlayPlayableCollectionGroupAsync();

        /// <inheritdoc />
        public Task PausePlayableCollectionGroupAsync() => PreferredSource.PausePlayableCollectionGroupAsync();

        /// <inheritdoc />
        public Task PlayTrackCollectionAsync(ITrack track)
        {
            var targetCore = PreferredSource.SourceCore;
            var source = track.GetSources<ICoreTrack>().FirstOrDefault(x => x.SourceCore.InstanceId == targetCore.InstanceId);

            Guard.IsNotNull(source, nameof(source));

            return PreferredSource.PlayTrackCollectionAsync(source);
        }

        /// <inheritdoc />
        public Task PlayArtistCollectionAsync(IArtistCollectionItem artistItem)
        {
            var targetCore = PreferredSource.SourceCore;

            ICoreArtistCollectionItem? source = null;

            if (artistItem is IArtist artist)
                source = artist.GetSources<ICoreArtist>().FirstOrDefault(x => x.SourceCore.InstanceId == targetCore.InstanceId);

            if (artistItem is IArtistCollection collection)
                source = collection.GetSources<ICoreArtistCollection>().FirstOrDefault(x => x.SourceCore.InstanceId == targetCore.InstanceId);

            Guard.IsNotNull(source, nameof(source));

            return PreferredSource.PlayArtistCollectionAsync(source);
        }

        /// <inheritdoc />
        public Task PlayAlbumCollectionAsync(IAlbumCollectionItem albumItem)
        {
            var targetCore = PreferredSource.SourceCore;

            ICoreAlbumCollectionItem? source = null;

            if (albumItem is IAlbum album)
                source = album.GetSources<ICoreAlbum>().FirstOrDefault(x => x.SourceCore.InstanceId == targetCore.InstanceId);

            if (albumItem is IAlbumCollection collection)
                source = collection.GetSources<ICoreAlbumCollection>().FirstOrDefault(x => x.SourceCore.InstanceId == targetCore.InstanceId);

            Guard.IsNotNull(source, nameof(source));

            return PreferredSource.PlayAlbumCollectionAsync(source);
        }

        /// <inheritdoc />
        public Task PlayPlaylistCollectionAsync(IPlaylistCollectionItem playlistItem)
        {
            var targetCore = PreferredSource.SourceCore;

            ICorePlaylistCollectionItem? source = null;

            if (playlistItem is IPlaylist playlist)
                source = playlist.GetSources<ICorePlaylist>().FirstOrDefault(x => x.SourceCore.InstanceId == targetCore.InstanceId);

            if (playlistItem is IPlaylistCollection collection)
                source = collection.GetSources<ICorePlaylistCollection>().FirstOrDefault(x => x.SourceCore.InstanceId == targetCore.InstanceId);

            Guard.IsNotNull(source, nameof(source));

            return PreferredSource.PlayPlaylistCollectionAsync(source);
        }

        /// <inheritdoc />
        public Task PlayPlayableCollectionGroupAsync(IPlayableCollectionGroup collectionGroup)
        {
            var targetCore = PreferredSource.SourceCore;
            var source = collectionGroup.GetSources<ICorePlayableCollectionGroup>().FirstOrDefault(x => x.SourceCore.InstanceId == targetCore.InstanceId);

            Guard.IsNotNull(source, nameof(source));

            return PreferredSource.PlayPlayableCollectionGroupAsync(source);
        }

        /// <inheritdoc/>
        public Task<IReadOnlyList<IAlbumCollectionItem>> GetAlbumItemsAsync(int limit, int offset)
        {
            return _albumCollectionMap.GetItemsAsync(limit, offset);
        }

        /// <inheritdoc/>
        public Task<IReadOnlyList<IArtistCollectionItem>> GetArtistItemsAsync(int limit, int offset)
        {
            return _artistCollectionMap.GetItemsAsync(limit, offset);
        }

        /// <inheritdoc/>
        public Task<IReadOnlyList<IPlayableCollectionGroup>> GetChildrenAsync(int limit, int offset = 0)
        {
            return _playableCollectionGroupMap.GetItemsAsync(limit, offset);
        }

        /// <inheritdoc/>
        public Task<IReadOnlyList<IPlaylistCollectionItem>> GetPlaylistItemsAsync(int limit, int offset)
        {
            return _playlistCollectionMap.GetItemsAsync(limit, offset);
        }

        /// <inheritdoc/>
        public Task<IReadOnlyList<ITrack>> GetTracksAsync(int limit, int offset = 0)
        {
            return _trackCollectionMap.GetItemsAsync(limit, offset);
        }

        /// <inheritdoc />
        public Task<IReadOnlyList<IImage>> GetImagesAsync(int limit, int offset)
        {
            return _imageCollectionMap.GetItemsAsync(limit, offset);
        }

        /// <inheritdoc />
        public Task<IReadOnlyList<IUrl>> GetUrlsAsync(int limit, int offset)
        {
            return _urlCollectionMap.GetItemsAsync(limit, offset);
        }

        /// <inheritdoc/>
        public Task ChangeNameAsync(string name)
        {
            return Sources.InParallel(source => source.IsChangeNameAsyncAvailable ? source.ChangeNameAsync(name) : Task.CompletedTask);
        }

        /// <inheritdoc/>
        public Task ChangeDescriptionAsync(string? description)
        {
            return Sources.InParallel(source => source.IsChangeDescriptionAsyncAvailable ? source.ChangeDescriptionAsync(description) : Task.CompletedTask);
        }

        /// <inheritdoc/>
        public Task ChangeDurationAsync(TimeSpan duration)
        {
            return Sources.InParallel(source => source.IsChangeDurationAsyncAvailable ? source.ChangeDurationAsync(duration) : Task.CompletedTask);
        }

        /// <inheritdoc/>
        public Task AddTrackAsync(ITrack track, int index)
        {
            return _trackCollectionMap.InsertItem(track, index);
        }

        /// <inheritdoc/>
        public Task AddArtistItemAsync(IArtistCollectionItem artist, int index)
        {
            return _artistCollectionMap.InsertItem(artist, index);
        }

        /// <inheritdoc/>
        public Task AddAlbumItemAsync(IAlbumCollectionItem album, int index)
        {
            return _albumCollectionMap.InsertItem(album, index);
        }

        /// <inheritdoc/>
        public Task AddPlaylistItemAsync(IPlaylistCollectionItem playlist, int index)
        {
            return _playlistCollectionMap.InsertItem(playlist, index);
        }

        /// <inheritdoc/>
        public Task AddChildAsync(IPlayableCollectionGroup child, int index)
        {
            return _playableCollectionGroupMap.InsertItem(child, index);
        }

        /// <inheritdoc />
        public Task AddImageAsync(IImage image, int index)
        {
            return _imageCollectionMap.InsertItem(image, index);
        }

        /// <inheritdoc />
        public Task AddUrlAsync(IUrl url, int index)
        {
            return _urlCollectionMap.InsertItem(url, index);
        }

        /// <inheritdoc/>
        public Task RemoveTrackAsync(int index)
        {
            return _trackCollectionMap.RemoveAt(index);
        }

        /// <inheritdoc/>
        public Task RemoveArtistItemAsync(int index)
        {
            return _artistCollectionMap.RemoveAt(index);
        }

        /// <inheritdoc/>
        public Task RemoveAlbumItemAsync(int index)
        {
            return _albumCollectionMap.RemoveAt(index);
        }

        /// <inheritdoc/>
        public Task RemovePlaylistItemAsync(int index)
        {
            return _playlistCollectionMap.RemoveAt(index);
        }

        /// <inheritdoc/>
        public Task RemoveChildAsync(int index)
        {
            return _playableCollectionGroupMap.RemoveAt(index);
        }

        /// <inheritdoc />
        public Task RemoveImageAsync(int index)
        {
            return _imageCollectionMap.RemoveAt(index);
        }

        /// <inheritdoc />
        public Task RemoveUrlAsync(int index)
        {
            return _urlCollectionMap.RemoveAt(index);
        }

        /// <inheritdoc />
        void IMergedMutable<TCoreBase>.AddSource(TCoreBase itemToAdd)
        {
            Guard.IsNotNull(itemToAdd, nameof(itemToAdd));

            if (!Equals(itemToAdd))
                ThrowHelper.ThrowArgumentException<TCoreBase>("Tried to merge an artist that doesn't match. Verify that the item matches before merging the source.");

            StoredSources.Add(itemToAdd);
            _sourceCores.Add(itemToAdd.SourceCore);

            _albumCollectionMap.Cast<IMergedMutable<ICoreAlbumCollection>>().AddSource(itemToAdd);
            _artistCollectionMap.Cast<IMergedMutable<ICoreArtistCollection>>().AddSource(itemToAdd);
            _playableCollectionGroupMap.Cast<IMergedMutable<ICorePlayableCollectionGroup>>().AddSource(itemToAdd);
            _playlistCollectionMap.Cast<IMergedMutable<ICorePlaylistCollection>>().AddSource(itemToAdd);
            _trackCollectionMap.Cast<IMergedMutable<ICoreTrackCollection>>().AddSource(itemToAdd);
            _imageCollectionMap.Cast<IMergedMutable<ICoreImageCollection>>().AddSource(itemToAdd);
            _urlCollectionMap.Cast<IMergedMutable<ICoreUrlCollection>>().AddSource(itemToAdd);
        }

        /// <inheritdoc />
        void IMergedMutable<TCoreBase>.RemoveSource(TCoreBase itemToRemove)
        {
            Guard.IsNotNull(itemToRemove, nameof(itemToRemove));

            StoredSources.Remove(itemToRemove);
            _sourceCores.Remove(itemToRemove.SourceCore);

            _albumCollectionMap.Cast<IMergedMutable<ICoreAlbumCollection>>().RemoveSource(itemToRemove);
            _artistCollectionMap.Cast<IMergedMutable<ICoreArtistCollection>>().RemoveSource(itemToRemove);
            _playableCollectionGroupMap.Cast<IMergedMutable<ICorePlayableCollectionGroup>>().RemoveSource(itemToRemove);
            _playlistCollectionMap.Cast<IMergedMutable<ICorePlaylistCollection>>().RemoveSource(itemToRemove);
            _trackCollectionMap.Cast<IMergedMutable<ICoreTrackCollection>>().RemoveSource(itemToRemove);
            _imageCollectionMap.Cast<IMergedMutable<ICoreImageCollection>>().RemoveSource(itemToRemove);
            _urlCollectionMap.Cast<IMergedMutable<ICoreUrlCollection>>().RemoveSource(itemToRemove);
        }

        /// <inheritdoc />
        public bool Equals(ICorePlaylistCollectionItem other) => Equals(other as ICorePlayableCollectionGroup);

        /// <inheritdoc />
        public bool Equals(ICorePlaylistCollection other) => Equals(other as ICorePlayableCollectionGroup);

        /// <inheritdoc />
        public bool Equals(ICoreTrackCollection other) => Equals(other as ICorePlayableCollectionGroup);

        /// <inheritdoc />
        public bool Equals(ICoreAlbumCollectionItem other) => Equals(other as ICorePlayableCollectionGroup);

        /// <inheritdoc />
        public bool Equals(ICoreAlbumCollection other) => Equals(other as ICorePlayableCollectionGroup);

        /// <inheritdoc />
        public bool Equals(ICoreArtistCollectionItem other) => Equals(other as ICorePlayableCollectionGroup);

        /// <inheritdoc />
        public bool Equals(ICoreArtistCollection other) => Equals(other as ICorePlayableCollectionGroup);

        /// <inheritdoc />
        public bool Equals(ICorePlayableCollectionGroupChildren other) => Equals(other as ICorePlayableCollectionGroup);

        /// <inheritdoc />
        public bool Equals(ICoreImageCollection other) => Equals(other as ICorePlayableCollectionGroup);

        /// <inheritdoc />
        public bool Equals(ICoreUrlCollection other) => Equals(other as ICorePlayableCollectionGroup);

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

        /// <inheritdoc />
        public async ValueTask DisposeAsync()
        {
            DetachCollectionChangedEvents();
            DetachPropertyChangedEvents(PreferredSource);

            await _albumCollectionMap.DisposeAsync();
            await _artistCollectionMap.DisposeAsync();
            await _playableCollectionGroupMap.DisposeAsync();
            await _playlistCollectionMap.DisposeAsync();
            await _trackCollectionMap.DisposeAsync();
            await _imageCollectionMap.DisposeAsync();
            await _urlCollectionMap.DisposeAsync();

            await Sources.InParallel(x => x.Cast<ICorePlayableCollectionGroup>().DisposeAsync().AsTask());
        }
    }
}
