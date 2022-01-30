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
using StrixMusic.Sdk.Services.Settings;

namespace StrixMusic.Sdk.Models.Merged
{
    /// <summary>
    /// Merged multiple <see cref="ICoreAlbum"/> into a single <see cref="IAlbum"/>
    /// </summary>
    public class MergedAlbum : IAlbum, IMergedMutable<ICoreAlbumCollectionItem>, IMergedMutable<ICoreAlbum>
    {
        private readonly ICoreAlbum _preferredSource;
        private readonly List<ICoreAlbum> _sources;
        private readonly List<ICore> _sourceCores;

        private readonly MergedCollectionMap<ITrackCollection, ICoreTrackCollection, ITrack, ICoreTrack> _trackCollectionMap;
        private readonly MergedCollectionMap<IArtistCollection, ICoreArtistCollection, IArtistCollectionItem, ICoreArtistCollectionItem> _artistCollectionMap;
        private readonly MergedCollectionMap<IImageCollection, ICoreImageCollection, IImage, ICoreImage> _imageCollectionMap;
        private readonly MergedCollectionMap<IGenreCollection, ICoreGenreCollection, IGenre, ICoreGenre> _genreCollectionMap;
        private readonly MergedCollectionMap<IUrlCollection, ICoreUrlCollection, IUrl, ICoreUrl> _urlCollectionMap;

        /// <summary>
        /// Initializes a new instance of the <see cref="MergedAlbum"/> class.
        /// </summary>
        public MergedAlbum(IEnumerable<ICoreAlbum> sources, ISettingsService settingsService)
        {
            _sources = sources.ToList();
            _sourceCores = _sources.Select(x => x.SourceCore).ToList();

            var relatedItemsSources = _sources.Select(x => x.RelatedItems).PruneNull().ToList();
            if (relatedItemsSources.Count > 0)
            {
                RelatedItems = new MergedPlayableCollectionGroup(relatedItemsSources, settingsService);
            }

            // TODO: Get the actual preferred source.
            _preferredSource = _sources[0];

            Name = _preferredSource.Name;
            DatePublished = _preferredSource.DatePublished;
            PlaybackState = _preferredSource.PlaybackState;
            LastPlayed = _preferredSource.LastPlayed;
            AddedAt = _preferredSource.AddedAt;

            foreach (var item in sources)
            {
                TotalTrackCount += item.TotalTrackCount;
                TotalImageCount += item.TotalImageCount;
                TotalArtistItemsCount += item.TotalArtistItemsCount;
                TotalGenreCount += item.TotalGenreCount;
                TotalUrlCount += item.TotalUrlCount;
            }

            _trackCollectionMap = new MergedCollectionMap<ITrackCollection, ICoreTrackCollection, ITrack, ICoreTrack>(this, settingsService);
            _imageCollectionMap = new MergedCollectionMap<IImageCollection, ICoreImageCollection, IImage, ICoreImage>(this, settingsService);
            _artistCollectionMap = new MergedCollectionMap<IArtistCollection, ICoreArtistCollection, IArtistCollectionItem, ICoreArtistCollectionItem>(this, settingsService);
            _genreCollectionMap = new MergedCollectionMap<IGenreCollection, ICoreGenreCollection, IGenre, ICoreGenre>(this, settingsService);
            _urlCollectionMap = new MergedCollectionMap<IUrlCollection, ICoreUrlCollection, IUrl, ICoreUrl>(this, settingsService);

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

        /// <inheritdoc cref="IMerged{T}.SourceCores" />
        public IReadOnlyList<ICore> SourceCores => _sourceCores;

        /// <inheritdoc cref="IMerged{T}.Sources"/>
        public IReadOnlyList<ICoreAlbum> Sources => _sources;

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
        public Uri? Url { get; internal set; }

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
        public Task<IReadOnlyList<ITrack>> GetTracksAsync(int limit, int offset) => _trackCollectionMap.GetItemsAsync(limit, offset);

        /// <inheritdoc />
        public Task<IReadOnlyList<IImage>> GetImagesAsync(int limit, int offset) => _imageCollectionMap.GetItemsAsync(limit, offset);

        /// <inheritdoc />
        public Task<IReadOnlyList<IArtistCollectionItem>> GetArtistItemsAsync(int limit, int offset) => _artistCollectionMap.GetItemsAsync(limit, offset);

        /// <inheritdoc />
        public Task<IReadOnlyList<IGenre>> GetGenresAsync(int limit, int offset) => _genreCollectionMap.GetItemsAsync(limit, offset);

        /// <inheritdoc />
        public Task<IReadOnlyList<IUrl>> GetUrlsAsync(int limit, int offset) => _urlCollectionMap.GetItemsAsync(limit, offset);

        /// <inheritdoc/>
        public Task RemoveTrackAsync(int index) => _trackCollectionMap.RemoveAt(index);

        /// <inheritdoc/>
        public Task AddTrackAsync(ITrack track, int index) => _trackCollectionMap.InsertItem(track, index);

        /// <inheritdoc />
        public Task AddArtistItemAsync(IArtistCollectionItem artist, int index) => _artistCollectionMap.InsertItem(artist, index);

        /// <inheritdoc />
        public Task RemoveArtistItemAsync(int index) => _artistCollectionMap.RemoveAt(index);

        /// <inheritdoc />
        public Task AddImageAsync(IImage image, int index) => _imageCollectionMap.InsertItem(image, index);

        /// <inheritdoc />
        public Task RemoveImageAsync(int index) => _imageCollectionMap.RemoveAt(index);

        /// <inheritdoc />
        public Task AddGenreAsync(IGenre genre, int index) => _genreCollectionMap.InsertItem(genre, index);

        /// <inheritdoc />
        public Task RemoveGenreAsync(int index) => _genreCollectionMap.RemoveAt(index);

        /// <inheritdoc />
        public Task AddUrlAsync(IUrl url, int index) => _urlCollectionMap.InsertItem(url, index);

        /// <inheritdoc />
        public Task RemoveUrlAsync(int index) => _urlCollectionMap.RemoveAt(index);

        /// <inheritdoc/>
        public Task ChangeDatePublishedAsync(DateTime datePublished)
        {
            return _preferredSource.ChangeDatePublishedAsync(datePublished);
        }

        /// <inheritdoc/>
        public Task ChangeDescriptionAsync(string? description)
        {
            return _preferredSource.ChangeDescriptionAsync(description);
        }

        /// <inheritdoc/>
        public Task ChangeDurationAsync(TimeSpan duration)
        {
            return _preferredSource.ChangeDurationAsync(duration);
        }

        /// <inheritdoc cref="IPlayableBase.ChangeNameAsync(string)"/>
        public Task ChangeNameAsync(string name)
        {
            return _preferredSource.ChangeNameAsync(name);
        }

        /// <inheritdoc/>
        public Task<bool> IsAddTrackAvailableAsync(int index) => _trackCollectionMap.IsAddItemAvailableAsync(index);

        /// <inheritdoc />
        public Task<bool> IsAddArtistItemAvailableAsync(int index) => _artistCollectionMap.IsAddItemAvailableAsync(index);

        /// <inheritdoc/>
        public Task<bool> IsAddImageAvailableAsync(int index) => _imageCollectionMap.IsAddItemAvailableAsync(index);

        /// <inheritdoc/>
        public Task<bool> IsAddGenreAvailableAsync(int index) => _genreCollectionMap.IsAddItemAvailableAsync(index);

        /// <inheritdoc/>
        public Task<bool> IsAddUrlAvailableAsync(int index) => _urlCollectionMap.IsAddItemAvailableAsync(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveArtistItemAvailableAsync(int index) => _artistCollectionMap.IsRemoveItemAvailableAsync(index);

        /// <inheritdoc/>
        public Task<bool> IsRemoveTrackAvailableAsync(int index) => _trackCollectionMap.IsRemoveItemAvailableAsync(index);

        /// <inheritdoc/>
        public Task<bool> IsRemoveImageAvailableAsync(int index) => _imageCollectionMap.IsRemoveItemAvailableAsync(index);

        /// <inheritdoc/>
        public Task<bool> IsRemoveGenreAvailableAsync(int index) => _genreCollectionMap.IsRemoveItemAvailableAsync(index);

        /// <inheritdoc/>
        public Task<bool> IsRemoveUrlAvailableAsync(int index) => _urlCollectionMap.IsRemoveItemAvailableAsync(index);

        /// <inheritdoc/>
        public Task PauseTrackCollectionAsync() => _preferredSource.PauseTrackCollectionAsync();

        /// <inheritdoc/>
        public Task PlayTrackCollectionAsync() => _preferredSource.PlayTrackCollectionAsync();

        /// <inheritdoc/>
        public Task PauseArtistCollectionAsync() => _preferredSource.PauseArtistCollectionAsync();

        /// <inheritdoc/>
        public Task PlayArtistCollectionAsync() => _preferredSource.PlayArtistCollectionAsync();

        /// <inheritdoc />
        public Task PlayTrackCollectionAsync(ITrack track)
        {
            var targetCore = _preferredSource.SourceCore;
            var source = track.GetSources<ICoreTrack>().FirstOrDefault(x => x.SourceCore.InstanceId == targetCore.InstanceId);

            Guard.IsNotNull(source, nameof(source));

            return _preferredSource.PlayTrackCollectionAsync(source);
        }

        /// <inheritdoc />
        public Task PlayArtistCollectionAsync(IArtistCollectionItem artistItem)
        {
            var targetCore = _preferredSource.SourceCore;

            ICoreArtistCollectionItem? source = null;

            if (artistItem is IArtist artist)
                source = artist.GetSources<ICoreArtist>().FirstOrDefault(x => x.SourceCore.InstanceId == targetCore.InstanceId);

            if (artistItem is IArtistCollection collection)
                source = collection.GetSources<ICoreArtistCollection>().FirstOrDefault(x => x.SourceCore.InstanceId == targetCore.InstanceId);

            Guard.IsNotNull(source, nameof(source));

            return _preferredSource.PlayArtistCollectionAsync(source);
        }

        /// <inheritdoc/>
        void IMergedMutable<ICoreAlbum>.AddSource(ICoreAlbum itemToMerge)
        {
            Guard.IsNotNull(itemToMerge, nameof(itemToMerge));

            _sources.Add(itemToMerge);
            _sourceCores.Add(itemToMerge.SourceCore);

            _imageCollectionMap.Cast<IMergedMutable<ICoreImageCollection>>().AddSource(itemToMerge);
            _artistCollectionMap.Cast<IMergedMutable<ICoreArtistCollection>>().AddSource(itemToMerge);
            _trackCollectionMap.Cast<IMergedMutable<ICoreTrackCollection>>().AddSource(itemToMerge);
            _genreCollectionMap.Cast<IMergedMutable<ICoreGenreCollection>>().AddSource(itemToMerge);
            _urlCollectionMap.Cast<IMergedMutable<ICoreUrlCollection>>().AddSource(itemToMerge);
        }

        /// <inheritdoc />
        void IMergedMutable<ICoreAlbum>.RemoveSource(ICoreAlbum itemToRemove)
        {
            Guard.IsNotNull(itemToRemove, nameof(itemToRemove));

            _sources.Remove(itemToRemove);
            _sourceCores.Remove(itemToRemove.SourceCore);

            _imageCollectionMap.Cast<IMergedMutable<ICoreImageCollection>>().RemoveSource(itemToRemove);
            _artistCollectionMap.Cast<IMergedMutable<ICoreArtistCollection>>().RemoveSource(itemToRemove);
            _trackCollectionMap.Cast<IMergedMutable<ICoreTrackCollection>>().RemoveSource(itemToRemove);
            _genreCollectionMap.Cast<IMergedMutable<ICoreGenreCollection>>().RemoveSource(itemToRemove);
            _urlCollectionMap.Cast<IMergedMutable<ICoreUrlCollection>>().RemoveSource(itemToRemove);
        }

        /// <inheritdoc />
        void IMergedMutable<ICoreAlbumCollectionItem>.AddSource(ICoreAlbumCollectionItem itemToMerge)
        {
            ((IMergedMutable<ICoreAlbum>)this).AddSource((ICoreAlbum)itemToMerge);
        }

        /// <inheritdoc />
        void IMergedMutable<ICoreAlbumCollectionItem>.RemoveSource(ICoreAlbumCollectionItem itemToRemove)
        {
            ((IMergedMutable<ICoreAlbum>)this).RemoveSource((ICoreAlbum)itemToRemove);
        }

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
