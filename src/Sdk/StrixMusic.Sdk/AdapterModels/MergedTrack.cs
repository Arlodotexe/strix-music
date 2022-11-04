// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Globalization;
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
    /// A concrete class that merged multiple <see cref="ICoreTrack"/>s.
    /// </summary>
    public class MergedTrack : ITrack, IMergedMutable<ICoreTrack>
    {
        private readonly MergedCollectionConfig _config;
        private readonly ICoreTrack _preferredSource;
        private readonly List<ICoreTrack> _sources;

        private readonly MergedCollectionMap<IArtistCollection, ICoreArtistCollection, IArtistCollectionItem, ICoreArtistCollectionItem> _artistMap;
        private readonly MergedCollectionMap<IImageCollection, ICoreImageCollection, IImage, ICoreImage> _imageCollectionMap;
        private readonly MergedCollectionMap<IGenreCollection, ICoreGenreCollection, IGenre, ICoreGenre> _genreCollectionMap;
        private readonly MergedCollectionMap<IUrlCollection, ICoreUrlCollection, IUrl, ICoreUrl> _urlCollectionMap;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="MergedTrack"/> class.
        /// </summary>
        public MergedTrack(IEnumerable<ICoreTrack> tracks, MergedCollectionConfig config)
        {
            _config = config;
            _sources = tracks.ToList();

            // TODO: Use top Preferred core.
            _preferredSource = _sources.First();

            _artistMap = new MergedCollectionMap<IArtistCollection, ICoreArtistCollection, IArtistCollectionItem, ICoreArtistCollectionItem>(this, config);
            _imageCollectionMap = new MergedCollectionMap<IImageCollection, ICoreImageCollection, IImage, ICoreImage>(this, config);
            _genreCollectionMap = new MergedCollectionMap<IGenreCollection, ICoreGenreCollection, IGenre, ICoreGenre>(this, config);
            _urlCollectionMap = new MergedCollectionMap<IUrlCollection, ICoreUrlCollection, IUrl, ICoreUrl>(this, config);

            Name = _preferredSource.Name;

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

            var merged = new MergedLyrics(e, _config);
            LyricsChanged?.Invoke(this, merged);
        }

        private void Source_AlbumChanged(object sender, ICoreAlbum? e)
        {
            if (e is null)
                return;

            var merged = new MergedAlbum(e.IntoList(), _config);
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

        /// <inheritdoc/>
        public event EventHandler<DownloadInfo>? DownloadInfoChanged;
        
        /// <inheritdoc cref="IMerged.SourcesChanged"/>
        public event EventHandler? SourcesChanged;

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
        public string Name { get; internal set; }

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
        public DownloadInfo DownloadInfo => default;

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
        public Task PauseArtistCollectionAsync(CancellationToken cancellationToken = default) => _preferredSource.PauseArtistCollectionAsync(cancellationToken);

        /// <inheritdoc/>
        public Task PlayArtistCollectionAsync(CancellationToken cancellationToken = default) => _preferredSource.PlayArtistCollectionAsync(cancellationToken);

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
        public Task ChangeAlbumAsync(IAlbum? album, CancellationToken cancellationToken = default) => _sources.InParallel(x =>
        {
            if (!x.IsChangeAlbumAsyncAvailable)
                return Task.CompletedTask;

            var sourceAlbum = album?.GetSources<ICoreAlbum>().First(y => y.SourceCore == x.SourceCore);
            return x.ChangeAlbumAsync(sourceAlbum, cancellationToken);
        });

        /// <inheritdoc/>
        public Task ChangeTrackNumberAsync(int? trackNumber, CancellationToken cancellationToken = default)
        {
            return _sources.InParallel(x => x.ChangeTrackNumberAsync(trackNumber, cancellationToken));
        }

        /// <inheritdoc/>
        public Task ChangeLanguageAsync(CultureInfo language, CancellationToken cancellationToken = default)
        {
            return _preferredSource.ChangeLanguageAsync(language, cancellationToken);
        }

        /// <inheritdoc/>
        public Task ChangeLyricsAsync(ILyrics? lyrics, CancellationToken cancellationToken = default)
        {
            var sourceToChange = lyrics?.GetSources().First(x => x.SourceCore == _preferredSource.SourceCore);

            Guard.IsNotNull(sourceToChange, nameof(sourceToChange));

            return _preferredSource.ChangeLyricsAsync(sourceToChange, cancellationToken);
        }

        /// <inheritdoc/>
        public Task ChangeIsExplicitAsync(bool isExplicit, CancellationToken cancellationToken = default) => _preferredSource.ChangeIsExplicitAsync(isExplicit, cancellationToken);

        /// <inheritdoc/>
        public Task ChangeNameAsync(string name, CancellationToken cancellationToken = default) => _preferredSource.ChangeNameAsync(name, cancellationToken);

        /// <inheritdoc/>
        public Task ChangeDescriptionAsync(string? description, CancellationToken cancellationToken = default) => _preferredSource.ChangeDescriptionAsync(description, cancellationToken);

        /// <inheritdoc/>
        public Task ChangeDurationAsync(TimeSpan duration, CancellationToken cancellationToken = default) => _preferredSource.ChangeDurationAsync(duration, cancellationToken);

        /// <inheritdoc/>
        public Task<bool> IsAddArtistItemAvailableAsync(int index, CancellationToken cancellationToken = default) => _preferredSource.IsAddArtistItemAvailableAsync(index, cancellationToken);

        /// <inheritdoc/>
        public Task<bool> IsAddImageAvailableAsync(int index, CancellationToken cancellationToken = default) => _preferredSource.IsAddImageAvailableAsync(index, cancellationToken);

        /// <inheritdoc/>
        public Task<bool> IsAddGenreAvailableAsync(int index, CancellationToken cancellationToken = default) => _genreCollectionMap.IsAddItemAvailableAsync(index, cancellationToken);

        /// <inheritdoc/>
        public Task<bool> IsAddUrlAvailableAsync(int index, CancellationToken cancellationToken = default) => _urlCollectionMap.IsAddItemAvailableAsync(index, cancellationToken);

        /// <inheritdoc/>
        public Task<bool> IsRemoveArtistItemAvailableAsync(int index, CancellationToken cancellationToken = default) => _artistMap.IsRemoveItemAvailableAsync(index, cancellationToken);

        /// <inheritdoc/>
        public Task<bool> IsRemoveImageAvailableAsync(int index, CancellationToken cancellationToken = default) => _imageCollectionMap.IsRemoveItemAvailableAsync(index, cancellationToken);

        /// <inheritdoc/>
        public Task<bool> IsRemoveGenreAvailableAsync(int index, CancellationToken cancellationToken = default) => _genreCollectionMap.IsRemoveItemAvailableAsync(index, cancellationToken);

        /// <inheritdoc/>
        public Task<bool> IsRemoveUrlAvailableAsync(int index, CancellationToken cancellationToken = default) => _urlCollectionMap.IsRemoveItemAvailableAsync(index, cancellationToken);

        /// <inheritdoc/>
        public IAsyncEnumerable<IArtistCollectionItem> GetArtistItemsAsync(int limit, int offset, CancellationToken cancellationToken = default) => _artistMap.GetItemsAsync(limit, offset, cancellationToken);

        /// <inheritdoc />
        public IAsyncEnumerable<IImage> GetImagesAsync(int limit, int offset, CancellationToken cancellationToken = default) => _imageCollectionMap.GetItemsAsync(limit, offset, cancellationToken);

        /// <inheritdoc />
        public IAsyncEnumerable<IGenre> GetGenresAsync(int limit, int offset, CancellationToken cancellationToken = default) => _genreCollectionMap.GetItemsAsync(limit, offset, cancellationToken);

        /// <inheritdoc />
        public IAsyncEnumerable<IUrl> GetUrlsAsync(int limit, int offset, CancellationToken cancellationToken = default) => _urlCollectionMap.GetItemsAsync(limit, offset, cancellationToken);

        /// <inheritdoc/>
        public Task AddArtistItemAsync(IArtistCollectionItem artistItem, int index, CancellationToken cancellationToken = default) => _artistMap.InsertItemAsync(artistItem, index, cancellationToken);

        /// <inheritdoc />
        public Task AddImageAsync(IImage image, int index, CancellationToken cancellationToken = default) => _imageCollectionMap.InsertItemAsync(image, index, cancellationToken);

        /// <inheritdoc />
        public Task AddGenreAsync(IGenre genre, int index, CancellationToken cancellationToken = default) => _genreCollectionMap.InsertItemAsync(genre, index, cancellationToken);

        /// <inheritdoc />
        public Task AddUrlAsync(IUrl genre, int index, CancellationToken cancellationToken = default) => _urlCollectionMap.InsertItemAsync(genre, index, cancellationToken);

        /// <inheritdoc/>
        public Task RemoveArtistItemAsync(int index, CancellationToken cancellationToken = default) => _artistMap.RemoveAtAsync(index, cancellationToken);

        /// <inheritdoc />
        public Task RemoveImageAsync(int index, CancellationToken cancellationToken = default) => _imageCollectionMap.RemoveAtAsync(index, cancellationToken);

        /// <inheritdoc />
        public Task RemoveGenreAsync(int index, CancellationToken cancellationToken = default) => _genreCollectionMap.RemoveAtAsync(index, cancellationToken);

        /// <inheritdoc />
        public Task RemoveUrlAsync(int index, CancellationToken cancellationToken = default) => _urlCollectionMap.RemoveAtAsync(index, cancellationToken);

        /// <inheritdoc/>
        public Task StartDownloadOperationAsync(DownloadOperation operation, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

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
        public void AddSource(ICoreTrack itemToMerge)
        {
            Guard.IsNotNull(itemToMerge, nameof(itemToMerge));

            _artistMap.AddSource(itemToMerge);
            _imageCollectionMap.AddSource(itemToMerge);
            _urlCollectionMap.AddSource(itemToMerge);

            _sources.Add(itemToMerge);
            SourcesChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <inheritdoc />
        public void RemoveSource(ICoreTrack itemToRemove)
        {
            Guard.IsNotNull(itemToRemove, nameof(itemToRemove));

            _artistMap.RemoveSource(itemToRemove);
            _imageCollectionMap.RemoveSource(itemToRemove);
            _urlCollectionMap.RemoveSource(itemToRemove);

            _sources.Remove(itemToRemove);
            SourcesChanged?.Invoke(this, EventArgs.Empty);
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
    }
}
