using System;
using System.Collections.Generic;
using System.Globalization;
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
    /// A concrete class that merged multiple <see cref="ICoreTrack"/>s.
    /// </summary>
    public class MergedTrack : ITrack, IMergedMutable<ICoreTrack>
    {
        private readonly ICoreTrack _preferredSource;
        private readonly List<ICoreTrack> _sources;

        private readonly MergedCollectionMap<IArtistCollection, ICoreArtistCollection, IArtistCollectionItem, ICoreArtistCollectionItem> _artistMap;
        private readonly MergedCollectionMap<IImageCollection, ICoreImageCollection, IImage, ICoreImage> _imageCollectionMap;
        private readonly MergedCollectionMap<IGenreCollection, ICoreGenreCollection, IGenre, ICoreGenre> _genreCollectionMap;
        private readonly MergedCollectionMap<IUrlCollection, ICoreUrlCollection, IUrl, ICoreUrl> _urlCollectionMap;

        private readonly List<ICore> _sourceCores;
        private readonly ISettingsService _settingsService;

        /// <summary>
        /// Initializes a new instance of the <see cref="MergedTrack"/> class.
        /// </summary>
        public MergedTrack(IEnumerable<ICoreTrack> tracks, ISettingsService settingsService)
        {
            _sources = tracks.ToList();
            _sourceCores = _sources.Select(x => x.SourceCore).ToList();

            // TODO: Use top Preferred core.
            _preferredSource = _sources.First();

            _artistMap = new MergedCollectionMap<IArtistCollection, ICoreArtistCollection, IArtistCollectionItem, ICoreArtistCollectionItem>(this, settingsService);
            _imageCollectionMap = new MergedCollectionMap<IImageCollection, ICoreImageCollection, IImage, ICoreImage>(this, settingsService);
            _genreCollectionMap = new MergedCollectionMap<IGenreCollection, ICoreGenreCollection, IGenre, ICoreGenre>(this, settingsService);
            _urlCollectionMap = new MergedCollectionMap<IUrlCollection, ICoreUrlCollection, IUrl, ICoreUrl>(this, settingsService);

            foreach (var item in _sources)
            {
                TotalArtistItemsCount += item.TotalArtistItemsCount;
                TotalImageCount += item.TotalImageCount;
                TotalGenreCount += item.TotalGenreCount;
                TotalUrlCount += item.TotalUrlCount;

                if (item.IsExplicit)
                    IsExplicit = true;
            }

            AttachEvents(_preferredSource);
            _settingsService = settingsService;
        }

        private void AttachEvents(ICoreTrack source)
        {
            AttachPlayableEvents(source);

            source.IsPlayArtistCollectionAsyncAvailableChanged += IsPlayArtistCollectionAsyncAvailableChanged;
            source.IsPauseArtistCollectionAsyncAvailableChanged += IsPauseArtistCollectionAsyncAvailableChanged;

            source.LanguageChanged += LanguageChanged;
            source.LyricsChanged += Source_LyricsChanged;
            source.IsExplicitChanged += IsExplicitChanged;
            source.AlbumChanged += Source_AlbumChanged;
            source.TrackNumberChanged += TrackNumberChanged;

            _imageCollectionMap.ItemsChanged += ImageCollectionMap_ItemsChanged;
            _imageCollectionMap.ItemsCountChanged += ImageCollectionMap_ItemsCountChanged;
            _artistMap.ItemsChanged += ArtistMap_ItemsChanged;
            _artistMap.ItemsCountChanged += ArtistMap_ItemsCountChanged;
            _genreCollectionMap.ItemsChanged += GenreCollectionMap_ItemsChanged;
            _genreCollectionMap.ItemsCountChanged += GenreCollectionMap_ItemsCountChanged;
            _urlCollectionMap.ItemsChanged += UrlCollectionMap_ItemsChanged;
            _urlCollectionMap.ItemsCountChanged += UrlCollectionMap_ItemsCountChanged;
        }

        private void DetachEvents(ICoreTrack source)
        {
            DetachPlayableEvents(source);

            source.IsPlayArtistCollectionAsyncAvailableChanged -= IsPlayArtistCollectionAsyncAvailableChanged;
            source.IsPauseArtistCollectionAsyncAvailableChanged -= IsPauseArtistCollectionAsyncAvailableChanged;

            source.LanguageChanged -= LanguageChanged;
            source.LyricsChanged -= Source_LyricsChanged;
            source.IsExplicitChanged -= IsExplicitChanged;
            source.AlbumChanged -= Source_AlbumChanged;
            source.TrackNumberChanged -= TrackNumberChanged;

            _imageCollectionMap.ItemsChanged -= ImageCollectionMap_ItemsChanged;
            _imageCollectionMap.ItemsCountChanged -= ImageCollectionMap_ItemsCountChanged;
            _artistMap.ItemsChanged -= ArtistMap_ItemsChanged;
            _artistMap.ItemsCountChanged -= ArtistMap_ItemsCountChanged;
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

        private void ArtistMap_ItemsCountChanged(object sender, int e)
        {
            TotalArtistItemsCount = e;
            ArtistItemsCountChanged?.Invoke(this, e);
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

        private void ArtistMap_ItemsChanged(object sender, IReadOnlyList<CollectionChangedItem<IArtistCollectionItem>> addedItems, IReadOnlyList<CollectionChangedItem<IArtistCollectionItem>> removedItems)
        {
            ArtistItemsChanged?.Invoke(this, addedItems, removedItems);
        }

        private void GenreCollectionMap_ItemsChanged(object sender, IReadOnlyList<CollectionChangedItem<IGenre>> addedItems, IReadOnlyList<CollectionChangedItem<IGenre>> removedItems)
        {
            GenresChanged?.Invoke(this, addedItems, removedItems);
        }

        private void ImageCollectionMap_ItemsChanged(object sender, IReadOnlyList<CollectionChangedItem<IImage>> addedItems, IReadOnlyList<CollectionChangedItem<IImage>> removedItems)
        {
            ImagesChanged?.Invoke(this, addedItems, removedItems);
        }

        private void UrlCollectionMap_ItemsChanged(object sender, IReadOnlyList<CollectionChangedItem<IUrl>> addedItems, IReadOnlyList<CollectionChangedItem<IUrl>> removedItems)
        {
            UrlsChanged?.Invoke(this, addedItems, removedItems);
        }

        private void Source_LyricsChanged(object sender, ICoreLyrics? e)
        {
            if (e is null)
                return;

            var merged = new MergedLyrics(e, _settingsService);
            LyricsChanged?.Invoke(this, merged);
        }

        private void Source_AlbumChanged(object sender, ICoreAlbum? e)
        {
            if (e is null)
                return;

            var merged = new MergedAlbum(e.IntoList(), _settingsService);
            AlbumChanged?.Invoke(this, merged);
        }

        /// <inheritdoc/>
        public event EventHandler<IAlbum?>? AlbumChanged;

        /// <inheritdoc />
        public event EventHandler<int?>? TrackNumberChanged;

        /// <inheritdoc />
        public event EventHandler<PlaybackState>? PlaybackStateChanged;

        /// <inheritdoc />
        public event EventHandler<CultureInfo?>? LanguageChanged;

        /// <inheritdoc />
        public event EventHandler<ILyrics?>? LyricsChanged;

        /// <inheritdoc />
        public event EventHandler<bool>? IsExplicitChanged;

        /// <inheritdoc />
        public event EventHandler<string>? NameChanged;

        /// <inheritdoc />
        public event EventHandler<string?>? DescriptionChanged;

        /// <inheritdoc />
        public event EventHandler<TimeSpan>? DurationChanged;

        /// <inheritdoc />
        public event EventHandler<DateTime?>? LastPlayedChanged;

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
        public event EventHandler<int>? ArtistItemsCountChanged;

        /// <inheritdoc />
        public event EventHandler<int>? GenresCountChanged;

        /// <inheritdoc />
        public event EventHandler<int>? ImagesCountChanged;

        /// <inheritdoc />
        public event EventHandler<int>? UrlsCountChanged;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<IArtistCollectionItem>? ArtistItemsChanged;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<IGenre>? GenresChanged;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<IImage>? ImagesChanged;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<IUrl>? UrlsChanged;

        /// <inheritdoc cref="IMerged{T}.SourceCores"/>
        public IReadOnlyList<ICore> SourceCores => _sourceCores;

        /// <inheritdoc />
        IReadOnlyList<ICoreTrack> IMerged<ICoreTrack>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreArtistCollection> IMerged<ICoreArtistCollection>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreGenreCollection> IMerged<ICoreGenreCollection>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreImageCollection> IMerged<ICoreImageCollection>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreUrlCollection> IMerged<ICoreUrlCollection>.Sources => Sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreArtistCollectionItem> IMerged<ICoreArtistCollectionItem>.Sources => Sources;

        /// <summary>
        /// The original sources for this merged item.
        /// </summary>
        public IReadOnlyList<ICoreTrack> Sources => _sources;

        /// <inheritdoc/>
        public string Id => _preferredSource.Id;

        /// <inheritdoc/>
        public string Name => _preferredSource.Name;

        /// <inheritdoc/>
        public TrackType Type => _preferredSource.Type;

        /// <inheritdoc/>
        public int TotalArtistItemsCount { get; private set; }

        /// <inheritdoc />
        public int TotalImageCount { get; private set; }

        /// <inheritdoc />
        public int TotalGenreCount { get; private set; }

        /// <inheritdoc />
        public int TotalUrlCount { get; private set; }

        /// <inheritdoc/>
        public IPlayableCollectionGroup? RelatedItems { get; }

        /// <inheritdoc/>
        public IAlbum? Album { get; }

        /// <inheritdoc/>
        public int? TrackNumber => _preferredSource.TrackNumber;

        /// <inheritdoc/>
        public int? DiscNumber => _preferredSource.DiscNumber;

        /// <inheritdoc/>
        public CultureInfo? Language => _preferredSource.Language;

        /// <inheritdoc/>
        public ILyrics? Lyrics { get; }

        /// <inheritdoc/>
        public bool IsExplicit { get; }

        /// <inheritdoc/>
        public string? Description => _preferredSource.Description;

        /// <inheritdoc/>
        public PlaybackState PlaybackState => _preferredSource.PlaybackState;

        /// <inheritdoc/>
        public TimeSpan Duration => _preferredSource.Duration;

        /// <inheritdoc />
        public DateTime? LastPlayed => _preferredSource.LastPlayed;

        /// <inheritdoc />
        public DateTime? AddedAt => _preferredSource.AddedAt;

        /// <inheritdoc/>
        public bool IsChangeAlbumAsyncAvailable => _preferredSource.IsChangeAlbumAsyncAvailable;

        /// <inheritdoc/>
        public bool IsChangeTrackNumberAsyncAvailable => _preferredSource.IsChangeTrackNumberAsyncAvailable;

        /// <inheritdoc/>
        public bool IsChangeLanguageAsyncAvailable => _preferredSource.IsChangeLanguageAsyncAvailable;

        /// <inheritdoc/>
        public bool IsChangeLyricsAsyncAvailable => _preferredSource.IsChangeLyricsAsyncAvailable;

        /// <inheritdoc/>
        public bool IsChangeIsExplicitAsyncAvailable => _preferredSource.IsChangeIsExplicitAsyncAvailable;

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
        public Task PauseArtistCollectionAsync() => _preferredSource.PauseArtistCollectionAsync();

        /// <inheritdoc/>
        public Task PlayArtistCollectionAsync() => _preferredSource.PlayArtistCollectionAsync();

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
        public Task ChangeAlbumAsync(IAlbum? album)
        {
            return _sources.InParallel(x =>
            {
                if (!x.IsChangeAlbumAsyncAvailable)
                    return Task.CompletedTask;

                var sourceAlbum = album?.GetSources<ICoreAlbum>().First(y => y.SourceCore == x.SourceCore);
                return x.ChangeAlbumAsync(sourceAlbum);
            });
        }

        /// <inheritdoc/>
        public Task ChangeTrackNumberAsync(int? trackNumber)
        {
            return _sources.InParallel(x => x.ChangeTrackNumberAsync(trackNumber));
        }

        /// <inheritdoc/>
        public Task ChangeLanguageAsync(CultureInfo language)
        {
            return _preferredSource.ChangeLanguageAsync(language);
        }

        /// <inheritdoc/>
        public Task ChangeLyricsAsync(ILyrics? lyrics)
        {
            var sourceToChange = lyrics?.GetSources().First(x => x.SourceCore == _preferredSource.SourceCore);

            Guard.IsNotNull(sourceToChange, nameof(sourceToChange));

            return _preferredSource.ChangeLyricsAsync(sourceToChange);
        }

        /// <inheritdoc/>
        public Task ChangeIsExplicitAsync(bool isExplicit) => _preferredSource.ChangeIsExplicitAsync(isExplicit);

        /// <inheritdoc/>
        public Task ChangeNameAsync(string name) => _preferredSource.ChangeNameAsync(name);

        /// <inheritdoc/>
        public Task ChangeDescriptionAsync(string? description) => _preferredSource.ChangeDescriptionAsync(description);

        /// <inheritdoc/>
        public Task ChangeDurationAsync(TimeSpan duration) => _preferredSource.ChangeDurationAsync(duration);

        /// <inheritdoc/>
        public Task<bool> IsAddArtistItemAvailableAsync(int index) => _preferredSource.IsAddArtistItemAvailableAsync(index);

        /// <inheritdoc/>
        public Task<bool> IsAddImageAvailableAsync(int index) => _preferredSource.IsAddImageAvailableAsync(index);

        /// <inheritdoc/>
        public Task<bool> IsAddGenreAvailableAsync(int index) => _genreCollectionMap.IsAddItemAvailableAsync(index);

        /// <inheritdoc/>
        public Task<bool> IsAddUrlAvailableAsync(int index) => _urlCollectionMap.IsAddItemAvailableAsync(index);

        /// <inheritdoc/>
        public Task<bool> IsRemoveArtistItemAvailableAsync(int index) => _artistMap.IsRemoveItemAvailableAsync(index);

        /// <inheritdoc/>
        public Task<bool> IsRemoveImageAvailableAsync(int index) => _imageCollectionMap.IsRemoveItemAvailableAsync(index);

        /// <inheritdoc/>
        public Task<bool> IsRemoveGenreAvailableAsync(int index) => _genreCollectionMap.IsRemoveItemAvailableAsync(index);

        /// <inheritdoc/>
        public Task<bool> IsRemoveUrlAvailableAsync(int index) => _urlCollectionMap.IsRemoveItemAvailableAsync(index);

        /// <inheritdoc/>
        public Task<IReadOnlyList<IArtistCollectionItem>> GetArtistItemsAsync(int limit, int offset) => _artistMap.GetItemsAsync(limit, offset);

        /// <inheritdoc />
        public Task<IReadOnlyList<IImage>> GetImagesAsync(int limit, int offset) => _imageCollectionMap.GetItemsAsync(limit, offset);

        /// <inheritdoc />
        public Task<IReadOnlyList<IGenre>> GetGenresAsync(int limit, int offset) => _genreCollectionMap.GetItemsAsync(limit, offset);

        /// <inheritdoc />
        public Task<IReadOnlyList<IUrl>> GetUrlsAsync(int limit, int offset) => _urlCollectionMap.GetItemsAsync(limit, offset);

        /// <inheritdoc/>
        public Task AddArtistItemAsync(IArtistCollectionItem artist, int index) => _artistMap.InsertItem(artist, index);

        /// <inheritdoc />
        public Task AddImageAsync(IImage image, int index) => _imageCollectionMap.InsertItem(image, index);

        /// <inheritdoc />
        public Task AddGenreAsync(IGenre genre, int index) => _genreCollectionMap.InsertItem(genre, index);

        /// <inheritdoc />
        public Task AddUrlAsync(IUrl genre, int index) => _urlCollectionMap.InsertItem(genre, index);

        /// <inheritdoc/>
        public Task RemoveArtistItemAsync(int index) => _artistMap.RemoveAt(index);

        /// <inheritdoc />
        public Task RemoveImageAsync(int index) => _imageCollectionMap.RemoveAt(index);

        /// <inheritdoc />
        public Task RemoveGenreAsync(int index) => _genreCollectionMap.RemoveAt(index);

        /// <inheritdoc />
        public Task RemoveUrlAsync(int index) => _urlCollectionMap.RemoveAt(index);

        /// <inheritdoc />
        public bool Equals(ICoreArtistCollectionItem other) => Equals(other as ICoreTrack);

        /// <inheritdoc />
        public bool Equals(ICoreImageCollection other) => Equals(other as ICoreTrack);

        /// <inheritdoc />
        public bool Equals(ICoreUrlCollection other) => Equals(other as ICoreTrack);

        /// <inheritdoc />
        public bool Equals(ICoreArtistCollection other) => Equals(other as ICoreTrack);

        /// <inheritdoc />
        public bool Equals(ICoreGenreCollection other) => Equals(other as ICoreTrack);

        /// <inheritdoc/>
        public override bool Equals(object? obj) => Equals(obj as ICoreTrack);

        /// <inheritdoc/>
        public override int GetHashCode() => _preferredSource.Id.GetHashCode();

        /// <inheritdoc />
        void IMergedMutable<ICoreTrack>.AddSource(ICoreTrack itemToMerge)
        {
            Guard.IsNotNull(itemToMerge, nameof(itemToMerge));

            _sources.Add(itemToMerge);
            _sourceCores.Add(itemToMerge.SourceCore);

            _artistMap.Cast<IMergedMutable<ICoreArtistCollection>>().AddSource(itemToMerge);
            _imageCollectionMap.Cast<IMergedMutable<ICoreImageCollection>>().AddSource(itemToMerge);
            _urlCollectionMap.Cast<IMergedMutable<ICoreUrlCollection>>().AddSource(itemToMerge);
        }

        /// <inheritdoc />
        void IMergedMutable<ICoreTrack>.RemoveSource(ICoreTrack itemToRemove)
        {
            Guard.IsNotNull(itemToRemove, nameof(itemToRemove));
            
            _sources.Remove(itemToRemove);
            _sourceCores.Remove(itemToRemove.SourceCore);

            _artistMap.Cast<IMergedMutable<ICoreArtistCollection>>().RemoveSource(itemToRemove);
            _imageCollectionMap.Cast<IMergedMutable<ICoreImageCollection>>().RemoveSource(itemToRemove);
            _urlCollectionMap.Cast<IMergedMutable<ICoreUrlCollection>>().RemoveSource(itemToRemove);
        }

        /// <inheritdoc />
        public bool Equals(ICoreTrack? other)
        {
            return other != null &&
                   other.Name == Name &&
                   other.TrackNumber == TrackNumber &&
                   other.Type == Type &&
                   other.DiscNumber == DiscNumber &&
                   other.Duration == Duration &&
                   Album is MergedAlbum album &&
                   !(other.Album is null) &&
                   album.Equals(other.Album);
        }

        /// <inheritdoc />
        public async ValueTask DisposeAsync()
        {
            DetachEvents(_preferredSource);

            await _sources.InParallel(x => x.DisposeAsync().AsTask());

            await _imageCollectionMap.DisposeAsync();
            await _artistMap.DisposeAsync();
            await _genreCollectionMap.DisposeAsync();
            await _urlCollectionMap.DisposeAsync();
        }
    }
}