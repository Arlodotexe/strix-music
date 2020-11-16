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
using StrixMusic.Sdk.Extensions;
using StrixMusic.Sdk.MediaPlayback;

namespace StrixMusic.Sdk.Data.Merged
{
    /// <summary>
    /// A base that merges multiple <see cref="IPlayableCollectionGroupBase"/>s.
    /// </summary>
    public abstract class MergedPlayableCollectionGroupBase<TCoreBase> : IPlayableCollectionGroup
        where TCoreBase : ICorePlayableCollectionGroup
    {
        private readonly List<TCoreBase> _sources;

        private readonly MergedCollectionMap<IAlbumCollection, ICoreAlbumCollection, IAlbumCollectionItem, ICoreAlbumCollectionItem> _albumCollectionMap;
        private readonly MergedCollectionMap<IArtistCollection, ICoreArtistCollection, IArtistCollectionItem, ICoreArtistCollectionItem> _artistCollectionMap;
        private readonly MergedCollectionMap<IPlaylistCollection, ICorePlaylistCollection, IPlaylistCollectionItem, ICorePlaylistCollectionItem> _playlistCollectionMap;
        private readonly MergedCollectionMap<ITrackCollection, ICoreTrackCollection, ITrack, ICoreTrack> _trackCollectionMap;
        private readonly MergedCollectionMap<IPlayableCollectionGroup, ICorePlayableCollectionGroup, IPlayableCollectionGroup, ICorePlayableCollectionGroup> _playableCollectionGroupMap;

        /// <summary>
        /// Initializes a new instance of the <see cref="MergedPlayableCollectionGroupBase{T}"/> class.
        /// </summary>
        /// <param name="sources">The search results to merge.</param>
        protected MergedPlayableCollectionGroupBase(IEnumerable<TCoreBase> sources)
        {
            _sources = sources.ToList();

            // TODO: Use top Preferred core.
            if (sources is null)
                throw new ArgumentNullException(nameof(sources));

            PreferredSource = _sources[0];

            _albumCollectionMap = new MergedCollectionMap<IAlbumCollection, ICoreAlbumCollection, IAlbumCollectionItem, ICoreAlbumCollectionItem>(this);
            _artistCollectionMap = new MergedCollectionMap<IArtistCollection, ICoreArtistCollection, IArtistCollectionItem, ICoreArtistCollectionItem>(this);
            _playlistCollectionMap = new MergedCollectionMap<IPlaylistCollection, ICorePlaylistCollection, IPlaylistCollectionItem, ICorePlaylistCollectionItem>(this);
            _trackCollectionMap = new MergedCollectionMap<ITrackCollection, ICoreTrackCollection, ITrack, ICoreTrack>(this);
            _playableCollectionGroupMap = new MergedCollectionMap<IPlayableCollectionGroup, ICorePlayableCollectionGroup, IPlayableCollectionGroup, ICorePlayableCollectionGroup>(this);

            AttachPropertyChangedEvents(PreferredSource);

            foreach (var item in _sources)
            {
                TotalChildrenCount += item.TotalChildrenCount;
                TotalPlaylistItemsCount += item.TotalPlaylistItemsCount;
                TotalTracksCount += item.TotalTracksCount;
                TotalAlbumItemsCount += item.TotalAlbumItemsCount;
                TotalArtistItemsCount += item.TotalArtistItemsCount;
                Duration += item.Duration;
            }
        }

        /// <summary>
        /// The top preferred source for this item, used for property getters.
        /// </summary>
        protected ICorePlayableCollectionGroup PreferredSource { get; }

        private void AttachPropertyChangedEvents(ICorePlayableCollectionGroup source)
        {
            source.PlaybackStateChanged += PlaybackStateChanged;
            source.NameChanged += NameChanged;
            source.DescriptionChanged += DescriptionChanged;
            source.UrlChanged += UrlChanged;
            source.DurationChanged += DurationChanged;

            source.AlbumItemsCountChanged += AlbumItemsCountChanged;
            source.ArtistItemsCountChanged += ArtistItemsCountChanged;
            source.PlaylistItemsCountChanged += PlaylistItemsCountChanged;
            source.TrackItemsCountChanged += TrackItemsCountChanged;
            source.TotalChildrenCountChanged += TotalChildrenCountChanged;
        }

        private void DetachPropertyChangedEvents(ICorePlayableCollectionGroup source)
        {
            source.PlaybackStateChanged -= PlaybackStateChanged;
            source.NameChanged -= NameChanged;
            source.DescriptionChanged -= DescriptionChanged;
            source.UrlChanged -= UrlChanged;
            source.DurationChanged -= DurationChanged;

            source.AlbumItemsCountChanged -= AlbumItemsCountChanged;
            source.ArtistItemsCountChanged -= ArtistItemsCountChanged;
            source.PlaylistItemsCountChanged -= PlaylistItemsCountChanged;
            source.TrackItemsCountChanged -= TrackItemsCountChanged;
            source.TotalChildrenCountChanged -= TotalChildrenCountChanged;
        }

        private void AttachCollectionChangedEvents(ICorePlayableCollection source)
        {
            _artistCollectionMap.ItemsChanged += ArtistCollectionMap_ItemsChanged;
        }

        private void ArtistCollectionMap_ItemsChanged(object sender, IReadOnlyList<CollectionChangedEventItem<IArtistCollectionItem>> addedItems, IReadOnlyList<CollectionChangedEventItem<IArtistCollectionItem>> removedItems)
        {
            ArtistItemsChanged?.Invoke(this, addedItems, removedItems);
        }

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
        public event EventHandler<int>? TrackItemsCountChanged;

        /// <inheritdoc />
        public event EventHandler<int>? ArtistItemsCountChanged;

        /// <inheritdoc />
        public event EventHandler<int>? AlbumItemsCountChanged;

        /// <inheritdoc />
        public event EventHandler<int>? PlaylistItemsCountChanged;

        /// <inheritdoc />
        public event EventHandler<int>? TotalChildrenCountChanged;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<ITrack>? TrackItemsChanged;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<IArtistCollectionItem>? ArtistItemsChanged;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<IAlbumCollectionItem>? AlbumItemsChanged;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<IPlaylistCollectionItem>? PlaylistItemsChanged;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<IPlayableCollectionGroup>? ChildItemsChanged;

        /// <inheritdoc cref="ISdkMember{T}.SourceCores"/>
        public IReadOnlyList<ICore> SourceCores => Sources.Select(x => x.SourceCore).ToList();

        /// <inheritdoc />
        IReadOnlyList<ICorePlayableCollectionGroupChildren> ISdkMember<ICorePlayableCollectionGroupChildren>.Sources => this.GetSources<ICorePlayableCollectionGroup>();

        /// <inheritdoc />
        IReadOnlyList<ICoreArtistCollection> ISdkMember<ICoreArtistCollection>.Sources => this.GetSources<ICorePlayableCollectionGroup>();

        /// <inheritdoc />
        IReadOnlyList<ICoreArtistCollectionItem> ISdkMember<ICoreArtistCollectionItem>.Sources => this.GetSources<ICorePlayableCollectionGroup>();

        /// <inheritdoc />
        IReadOnlyList<ICoreAlbumCollection> ISdkMember<ICoreAlbumCollection>.Sources => this.GetSources<ICorePlayableCollectionGroup>();

        /// <inheritdoc />
        IReadOnlyList<ICoreTrackCollection> ISdkMember<ICoreTrackCollection>.Sources => this.GetSources<ICorePlayableCollectionGroup>();

        /// <inheritdoc />
        IReadOnlyList<ICorePlaylistCollection> ISdkMember<ICorePlaylistCollection>.Sources => this.GetSources<ICorePlayableCollectionGroup>();

        /// <inheritdoc />
        IReadOnlyList<ICorePlaylistCollectionItem> ISdkMember<ICorePlaylistCollectionItem>.Sources => this.GetSources<ICorePlayableCollectionGroup>();

        /// <inheritdoc />
        IReadOnlyList<ICoreImageCollection> ISdkMember<ICoreImageCollection>.Sources => this.GetSources<ICorePlayableCollectionGroup>();

        /// <inheritdoc />
        IReadOnlyList<ICorePlayableCollectionGroup> ISdkMember<ICorePlayableCollectionGroup>.Sources => this.GetSources<ICorePlayableCollectionGroup>();

        /// <inheritdoc />
        IReadOnlyList<ICoreAlbumCollectionItem> ISdkMember<ICoreAlbumCollectionItem>.Sources => this.GetSources<ICoreAlbumCollectionItem>();

        /// <inheritdoc cref="ISdkMember{T}.Sources"/>
        public virtual IReadOnlyList<TCoreBase> Sources => _sources;

        /// <inheritdoc/>
        public string Id => PreferredSource.Id;

        /// <inheritdoc/>
        public Uri? Url => PreferredSource.Url;

        /// <inheritdoc/>
        public string Name => PreferredSource.Name;

        /// <inheritdoc/>
        public SynchronizedObservableCollection<IImage> Images { get; } = new SynchronizedObservableCollection<IImage>();

        /// <inheritdoc/>
        public string? Description => PreferredSource.Description;

        /// <inheritdoc/>
        public PlaybackState PlaybackState => PreferredSource.PlaybackState;

        /// <inheritdoc/>
        public TimeSpan Duration { get; } = new TimeSpan(0);

        /// <inheritdoc/>
        public int TotalChildrenCount { get; }

        /// <inheritdoc/>
        public int TotalPlaylistItemsCount { get; }

        /// <inheritdoc/>
        public int TotalTracksCount { get; }

        /// <inheritdoc/>
        public int TotalAlbumItemsCount { get; }

        /// <inheritdoc/>
        public int TotalArtistItemsCount { get; }

        /// <inheritdoc/>
        public virtual bool IsPlayAsyncSupported => PreferredSource.IsPlayAsyncSupported;

        /// <inheritdoc/>
        public virtual bool IsPauseAsyncSupported => PreferredSource.IsPauseAsyncSupported;

        /// <inheritdoc/>
        public virtual bool IsChangeNameAsyncSupported => PreferredSource.IsChangeNameAsyncSupported;

        /// <inheritdoc/>
        public virtual bool IsChangeDescriptionAsyncSupported => PreferredSource.IsChangeDescriptionAsyncSupported;

        /// <inheritdoc/>
        public virtual bool IsChangeDurationAsyncSupported => PreferredSource.IsChangeDurationAsyncSupported;

        /// <inheritdoc />
        public Task<bool> IsAddTrackSupported(int index) => PreferredSource.IsAddTrackSupported(index);

        /// <inheritdoc />
        public Task<bool> IsAddAlbumItemSupported(int index) => PreferredSource.IsAddAlbumItemSupported(index);

        /// <inheritdoc />
        public Task<bool> IsAddArtistSupported(int index) => PreferredSource.IsAddArtistSupported(index);

        /// <inheritdoc />
        public Task<bool> IsAddPlaylistItemSupported(int index) => PreferredSource.IsAddPlaylistItemSupported(index);

        /// <inheritdoc />
        public Task<bool> IsAddChildSupported(int index) => PreferredSource.IsAddChildSupported(index);

        /// <inheritdoc />
        public Task<bool> IsAddImageSupported(int index) => PreferredSource.IsAddImageSupported(index);

        /// <inheritdoc/>
        public Task<bool> IsRemoveImageSupported(int index) => PreferredSource.IsRemoveImageSupported(index);

        /// <inheritdoc/>
        public Task<bool> IsRemoveTrackSupported(int index) => PreferredSource.IsRemoveTrackSupported(index);

        /// <inheritdoc/>
        public Task<bool> IsRemoveArtistSupported(int index) => PreferredSource.IsRemoveArtistSupported(index);

        /// <inheritdoc/>
        public Task<bool> IsRemoveAlbumItemSupported(int index) => PreferredSource.IsRemoveAlbumItemSupported(index);

        /// <inheritdoc/>
        public Task<bool> IsRemovePlaylistItemSupported(int index) => PreferredSource.IsRemovePlaylistItemSupported(index);

        /// <inheritdoc/>
        public Task<bool> IsRemoveChildSupported(int index) => PreferredSource.IsRemoveChildSupported(index);

        /// <inheritdoc/>
        public Task PauseAsync() => PreferredSource.PauseAsync();

        /// <inheritdoc/>
        public Task PlayAsync() => PreferredSource.PlayAsync();

        /// <inheritdoc/>
        public Task<IReadOnlyList<IAlbumCollectionItem>> GetAlbumItemsAsync(int limit, int offset)
        {
            return _albumCollectionMap.GetItems(limit, offset);
        }

        /// <inheritdoc/>
        public Task<IReadOnlyList<IArtistCollectionItem>> GetArtistItemsAsync(int limit, int offset)
        {
            return _artistCollectionMap.GetItems(limit, offset);
        }

        /// <inheritdoc/>
        public Task<IReadOnlyList<IPlayableCollectionGroup>> GetChildrenAsync(int limit, int offset = 0)
        {
            return _playableCollectionGroupMap.GetItems(limit, offset);
        }

        /// <inheritdoc/>
        public Task<IReadOnlyList<IPlaylistCollectionItem>> GetPlaylistItemsAsync(int limit, int offset)
        {
            return _playlistCollectionMap.GetItems(limit, offset);
        }

        /// <inheritdoc/>
        public Task<IReadOnlyList<ITrack>> GetTracksAsync(int limit, int offset = 0)
        {
            return _trackCollectionMap.GetItems(limit, offset);
        }

        /// <inheritdoc/>
        public Task ChangeNameAsync(string name)
        {
            return Sources.InParallel(source => source.IsChangeNameAsyncSupported ? source.ChangeNameAsync(name) : Task.CompletedTask);
        }

        /// <inheritdoc/>
        public Task ChangeDescriptionAsync(string? description)
        {
            return Sources.InParallel(source => source.IsChangeDescriptionAsyncSupported ? source.ChangeDescriptionAsync(description) : Task.CompletedTask);
        }

        /// <inheritdoc/>
        public Task ChangeDurationAsync(TimeSpan duration)
        {
            return Sources.InParallel(source => source.IsChangeDurationAsyncSupported ? source.ChangeDurationAsync(duration) : Task.CompletedTask);
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

        /// <inheritdoc/>
        public Task RemoveTrackAsync(int index)
        {
            return _trackCollectionMap.RemoveAt(index);
        }

        /// <inheritdoc/>
        public Task RemoveArtistAsync(int index)
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

        /// <inheritdoc cref="IMerged{TCoreBase}" />
        public void AddSource(TCoreBase itemToAdd)
        {
            if (!Equals(itemToAdd))
                ThrowHelper.ThrowArgumentException<TCoreBase>("Tried to merge an artist that doesn't match. Verify that the item matches before merging the source.");

            _albumCollectionMap.AddSource(itemToAdd);
            _artistCollectionMap.AddSource(itemToAdd);
            _playableCollectionGroupMap.AddSource(itemToAdd);
            _playlistCollectionMap.AddSource(itemToAdd);
            _trackCollectionMap.AddSource(itemToAdd);
            _sources.Add(itemToAdd);
        }
    }
}
