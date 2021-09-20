using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Toolkit.Diagnostics;
using OwlCore.Events;
using StrixMusic.Cores.Files.Services;
using StrixMusic.Sdk.Data;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.Extensions;
using StrixMusic.Sdk.MediaPlayback;
using StrixMusic.Sdk.Services.FileMetadataManager;
using StrixMusic.Sdk.Services.FileMetadataManager.Models;

namespace StrixMusic.Cores.Files.Models
{
    /// <summary>
    /// Wraps around <see cref="TrackMetadata"/> to provide track information extracted from a file to the Strix SDK.
    /// </summary>
    public sealed class FilesCoreTrack : ICoreTrack
    {
        private readonly IFileMetadataManager _fileMetadataManager;
        private TrackMetadata _trackMetadata;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilesCoreTrack"/> class.
        /// </summary>
        /// <param name="sourceCore">The source core.</param>
        /// <param name="trackMetadata">The track metadata to wrap around</param>
        public FilesCoreTrack(ICore sourceCore, TrackMetadata trackMetadata)
        {
            SourceCore = sourceCore;
            _trackMetadata = trackMetadata;

            _fileMetadataManager = SourceCore.GetService<IFileMetadataManager>();
            AttachEvents();
        }

        private void AttachEvents()
        {
            _fileMetadataManager.Tracks.MetadataUpdated += Tracks_MetadataUpdated;
        }

        private void DetachEvents()
        {
            _fileMetadataManager.Tracks.MetadataUpdated -= Tracks_MetadataUpdated;
        }

        private void Tracks_MetadataUpdated(object sender, IEnumerable<TrackMetadata> e)
        {
            foreach (var metadata in e)
            {
                if (metadata.Id != Id)
                    return;

                Guard.IsNotNull(metadata.ArtistIds, nameof(metadata.ArtistIds));
                Guard.IsNotNull(metadata.ImageIds, nameof(metadata.ImageIds));
                Guard.IsNotNull(metadata.Genres, nameof(metadata.Genres));

                var previousData = _trackMetadata;
                _trackMetadata = metadata;

                Guard.IsNotNull(previousData.ArtistIds, nameof(previousData.ArtistIds));
                Guard.IsNotNull(previousData.ImageIds, nameof(previousData.ImageIds));
                Guard.IsNotNull(previousData.Genres, nameof(previousData.Genres));

                if (metadata.Title != previousData.Title)
                    NameChanged?.Invoke(this, Name);

                if (metadata.Description != previousData.Description)
                    DescriptionChanged?.Invoke(this, Description);

                if (metadata.DiscNumber != previousData.DiscNumber)
                    TrackNumberChanged?.Invoke(this, TrackNumber);

                if (!Equals(metadata.Language, previousData.Language))
                    LanguageChanged?.Invoke(this, Language);

                if (!ReferenceEquals(metadata.Lyrics, previousData.Lyrics))
                    LyricsChanged?.Invoke(this, Lyrics);

                if (metadata.TrackNumber != previousData.TrackNumber)
                    TrackNumberChanged?.Invoke(this, TrackNumber);

                if (metadata.Duration != previousData.Duration)
                    DurationChanged?.Invoke(this, Duration);

                if (metadata.ArtistIds.Count != previousData.ArtistIds.Count)
                    ArtistItemsCountChanged?.Invoke(this, metadata.ArtistIds.Count);

                if (metadata.ImageIds.Count != previousData.ImageIds.Count)
                    ImagesCountChanged?.Invoke(this, metadata.ImageIds.Count);

                _ = HandleImagesChanged(previousData.ImageIds, metadata.ImageIds);
                _ = HandleArtistsChanged(previousData.ArtistIds, metadata.ArtistIds);
            }
        }

        private async Task HandleArtistsChanged(HashSet<string> oldArtistIds, HashSet<string> newArtistIds)
        {
            if (oldArtistIds.OrderBy(s => s).SequenceEqual(newArtistIds.OrderBy(s => s)))
            {
                // Lists have identical content, so no items have changed.
                return;
            }

            var addedArtists = newArtistIds.Except(oldArtistIds);
            var removedArtists = oldArtistIds.Except(newArtistIds);

            if (oldArtistIds.Count != newArtistIds.Count)
            {
                ArtistItemsChanged?.Invoke(this, await TransformAsync(addedArtists), await TransformAsync(removedArtists));
                ArtistItemsCountChanged?.Invoke(this, newArtistIds.Count);
            }

            async Task<IReadOnlyList<CollectionChangedItem<ICoreArtistCollectionItem>>> TransformAsync(IEnumerable<string> ids)
            {
                var idArray = ids as string[] ?? ids.ToArray();
                var collectionChangedItems = new List<CollectionChangedItem<ICoreArtistCollectionItem>>(idArray.Length);

                foreach (var id in idArray)
                {
                    var artist = await _fileMetadataManager.Artists.GetByIdAsync(id);
                    Guard.IsNotNullOrWhiteSpace(artist?.Id, nameof(artist.Id));

                    collectionChangedItems.Add(new CollectionChangedItem<ICoreArtistCollectionItem>(InstanceCache.Artists.GetOrCreate(artist.Id, SourceCore, artist), collectionChangedItems.Count));
                }

                return collectionChangedItems;
            }
        }

        private async Task HandleImagesChanged(HashSet<string> oldImageIds, HashSet<string> newImageIds)
        {
            if (oldImageIds.OrderBy(s => s).SequenceEqual(newImageIds.OrderBy(s => s)))
            {
	            // Lists have identical content, so no images have changed.
	            return;
            }

            var addedImages = newImageIds.Except(oldImageIds);
            var removedImages = oldImageIds.Except(newImageIds);

            if (oldImageIds.Count != newImageIds.Count)
            {
                ImagesChanged?.Invoke(this, await TransformAsync(addedImages), await TransformAsync(removedImages));
                ImagesCountChanged?.Invoke(this, newImageIds.Count);
            }

            async Task<IReadOnlyList<CollectionChangedItem<ICoreImage>>> TransformAsync(IEnumerable<string> ids)
            {
                var idArray = ids as string[] ?? ids.ToArray();
                var collectionChangedItems = new List<CollectionChangedItem<ICoreImage>>(idArray.Length);

                foreach (var id in idArray)
                {
                    var image = await _fileMetadataManager.Images.GetByIdAsync(id);

                    Guard.IsNotNullOrWhiteSpace(image?.Id, nameof(image.Id));
                    collectionChangedItems.Add(new CollectionChangedItem<ICoreImage>(InstanceCache.Images.GetOrCreate(image.Id, SourceCore, image), collectionChangedItems.Count));
                }

                return collectionChangedItems;
            }
        }

        /// <inheritdoc/>
        public event EventHandler<ICoreAlbum?>? AlbumChanged;

        /// <inheritdoc/>
        public event EventHandler<int?>? TrackNumberChanged;

        /// <inheritdoc/>
        public event EventHandler<CultureInfo?>? LanguageChanged;

        /// <inheritdoc/>
        public event EventHandler<ICoreLyrics?>? LyricsChanged;

        /// <inheritdoc/>
        public event EventHandler<bool>? IsExplicitChanged;

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
        public event CollectionChangedEventHandler<ICoreArtistCollectionItem>? ArtistItemsChanged;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<ICoreGenre>? GenresChanged;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<ICoreImage>? ImagesChanged;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<ICoreUrl>? UrlsChanged;

        /// <inheritdoc/>
        public string Id => _trackMetadata.Id ?? string.Empty;

        /// <inheritdoc/>
        public TrackType Type => TrackType.Song;

        /// <inheritdoc />
        public int TotalArtistItemsCount => _trackMetadata.ArtistIds?.Count ?? 0;

        /// <inheritdoc />
        public int TotalImageCount => _trackMetadata.ImageIds?.Count ?? 0;

        /// <inheritdoc/>
        public int TotalGenreCount => _trackMetadata.Genres?.Count ?? 0;

        /// <inheritdoc />
        public int TotalUrlCount => 0;

        /// <inheritdoc/>
        public ICoreAlbum? Album { get; }

        /// <inheritdoc/>
        /// <remarks>Is not passed into the constructor. Should be set on object creation.</remarks>
        public int? TrackNumber => Convert.ToInt32(_trackMetadata.TrackNumber);

        /// <inheritdoc />
        public int? DiscNumber { get; }

        /// <inheritdoc/>
        public CultureInfo? Language { get; }

        /// <inheritdoc/>
        public ICoreLyrics? Lyrics => null;

        /// <inheritdoc/>
        public bool IsExplicit => false;

        /// <inheritdoc/>
        public ICore SourceCore { get; }

        /// <summary>
        /// The path to the playable music file on disk.
        /// </summary>
        public Uri? LocalTrackPath => _trackMetadata.Url;

        /// <inheritdoc/>
        public string Name => _trackMetadata.Title ?? string.Empty;

        /// <inheritdoc/>
        public string? Description => _trackMetadata.Description;

        /// <inheritdoc/>
        public PlaybackState PlaybackState { get; }

        /// <inheritdoc/>
        public TimeSpan Duration => _trackMetadata.Duration ?? new TimeSpan(0, 0, 0);

        /// <inheritdoc />
        public DateTime? LastPlayed { get; }

        /// <inheritdoc />
        public DateTime? AddedAt { get; }

        /// <inheritdoc/>
        public ICorePlayableCollectionGroup? RelatedItems => null;

        /// <inheritdoc/>
        public bool IsChangeAlbumAsyncAvailable => false;

        /// <inheritdoc/>
        public bool IsChangeTrackNumberAsyncAvailable => false;

        /// <inheritdoc/>
        public bool IsChangeLanguageAsyncAvailable => false;

        /// <inheritdoc/>
        public bool IsChangeLyricsAsyncAvailable => false;

        /// <inheritdoc/>
        public bool IsChangeIsExplicitAsyncAvailable => false;

        /// <inheritdoc/>
        public bool IsPlayArtistCollectionAsyncAvailable => false;

        /// <inheritdoc/>
        public bool IsPauseArtistCollectionAsyncAvailable => false;

        /// <inheritdoc/>
        public bool IsChangeNameAsyncAvailable => false;

        /// <inheritdoc/>
        public bool IsChangeDescriptionAsyncAvailable => false;

        /// <inheritdoc/>
        public bool IsChangeDurationAsyncAvailable => false;

        /// <inheritdoc/>
        public Task<bool> IsAddArtistItemAvailableAsync(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc/>
        public Task<bool> IsAddImageAvailableAsync(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc/>
        public Task<bool> IsAddGenreAvailableAsync(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc/>
        public Task<bool> IsAddUrlAvailableAsync(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc />
        public Task<bool> IsRemoveArtistItemAvailableAsync(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc />
        public Task<bool> IsRemoveImageAvailableAsync(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc />
        public Task<bool> IsRemoveGenreAvailableAsync(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc />
        public Task<bool> IsRemoveUrlAvailableAsync(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc/>
        public Task ChangeAlbumAsync(ICoreAlbum? albums)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public Task ChangeDescriptionAsync(string? description)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public Task ChangeDurationAsync(TimeSpan duration)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public Task ChangeIsExplicitAsync(bool isExplicit)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public Task ChangeLanguageAsync(CultureInfo language)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public Task ChangeLyricsAsync(ICoreLyrics? lyrics)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public Task ChangeNameAsync(string name)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public Task ChangeTrackNumberAsync(int? trackNumber)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public Task PauseArtistCollectionAsync()
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public Task PlayArtistCollectionAsync()
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task PlayArtistCollectionAsync(ICoreArtistCollectionItem artistItem)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task AddArtistItemAsync(ICoreArtistCollectionItem artist, int index)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task AddImageAsync(ICoreImage image, int index)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task AddGenreAsync(ICoreGenre genre, int index)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task AddUrlAsync(ICoreUrl image, int index)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task RemoveArtistItemAsync(int index)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task RemoveImageAsync(int index)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task RemoveGenreAsync(int index)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task RemoveUrlAsync(int index)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public async IAsyncEnumerable<ICoreArtistCollectionItem> GetArtistItemsAsync(int limit, int offset)
        {
            var artists = await _fileMetadataManager.Artists.GetArtistsByTrackId(Id, offset, limit);

            foreach (var artist in artists)
            {
                Guard.IsNotNullOrWhiteSpace(artist.Id, nameof(artist.Id));
                yield return InstanceCache.Artists.GetOrCreate(artist.Id, SourceCore, artist);
            }
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<ICoreImage> GetImagesAsync(int limit, int offset)
        {
            if (_trackMetadata.ImageIds == null)
                yield break;

            foreach (var imageId in _trackMetadata.ImageIds)
            {
                var image = await _fileMetadataManager.Images.GetByIdAsync(imageId);
                Guard.IsNotNull(image, nameof(image));

                yield return InstanceCache.Images.GetOrCreate(imageId, SourceCore, image);
            }
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<ICoreGenre> GetGenresAsync(int limit, int offset)
        {
            foreach (var genre in _trackMetadata.Genres ?? Enumerable.Empty<string>())
            {
                yield return new FilesCoreGenre(SourceCore, genre);
            }

            await Task.CompletedTask;
        }

        /// <inheritdoc/>
        public IAsyncEnumerable<ICoreUrl> GetUrlsAsync(int limit, int offset = 0)
        {
            return AsyncEnumerable.Empty<ICoreUrl>();
        }

        /// <inheritdoc />
        public ValueTask DisposeAsync()
        {
            DetachEvents();
            return default;
        }
    }
}
