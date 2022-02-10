using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Toolkit.Diagnostics;
using OwlCore.Events;
using StrixMusic.Cores.Files.Services;
using StrixMusic.Sdk.Extensions;
using StrixMusic.Sdk.FileMetadata;
using StrixMusic.Sdk.FileMetadata.Models;
using StrixMusic.Sdk.MediaPlayback;
using StrixMusic.Sdk.Models.Core;

namespace StrixMusic.Cores.Files.Models
{
    /// <summary>
    /// Wraps around <see cref="AlbumMetadata"/> to provide album information extracted from a file to the Strix SDK.
    /// </summary>
    public sealed class FilesCoreAlbum : ICoreAlbum
    {
        private readonly IFileMetadataManager _fileMetadataManager;
        private AlbumMetadata _albumMetadata;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilesCoreAlbum"/> class.
        /// </summary>
        /// <param name="sourceCore">The core that created this object.</param>
        /// <param name="albumMetadata">The source album metadata to wrap around.</param>
        public FilesCoreAlbum(ICore sourceCore, AlbumMetadata albumMetadata)
        {
            Guard.IsNotNullOrWhiteSpace(albumMetadata.Id, nameof(albumMetadata.Id));

            Id = albumMetadata.Id;
            _fileMetadataManager = sourceCore.GetService<IFileMetadataManager>();
            SourceCore = sourceCore;
            _albumMetadata = albumMetadata;

            Guard.IsNotNull(albumMetadata.ArtistIds, nameof(albumMetadata.ArtistIds));
            Guard.IsNotNull(albumMetadata.TrackIds, nameof(albumMetadata.TrackIds));

            AttachEvents();
        }

        private void AttachEvents()
        {
            _fileMetadataManager.Albums.MetadataUpdated += Albums_MetadataUpdated;
        }

        private void DetachEvents()
        {
            _fileMetadataManager.Albums.MetadataUpdated -= Albums_MetadataUpdated;
        }

        private void Albums_MetadataUpdated(object sender, IEnumerable<AlbumMetadata> e)
        {
            foreach (var metadata in e)
            {
                if (metadata.Id != Id)
                    return;

                Guard.IsNotNull(metadata.ArtistIds, nameof(metadata.ArtistIds));
                Guard.IsNotNull(metadata.ImageIds, nameof(metadata.ImageIds));
                Guard.IsNotNull(metadata.TrackIds, nameof(metadata.TrackIds));
                Guard.IsNotNull(metadata.Genres, nameof(metadata.Genres));

                var previousData = _albumMetadata;
                _albumMetadata = metadata;

                Guard.IsNotNull(previousData.TrackIds, nameof(previousData.TrackIds));
                Guard.IsNotNull(previousData.ArtistIds, nameof(previousData.ArtistIds));
                Guard.IsNotNull(previousData.ImageIds, nameof(previousData.ImageIds));
                Guard.IsNotNull(previousData.Genres, nameof(previousData.Genres));

                if (metadata.Title != previousData.Title)
                    NameChanged?.Invoke(this, Name);

                if (metadata.DatePublished != previousData.DatePublished)
                    DatePublishedChanged?.Invoke(this, DatePublished);

                if (metadata.Description != previousData.Description)
                    DescriptionChanged?.Invoke(this, Description);

                if (metadata.Duration != previousData.Duration)
                    DurationChanged?.Invoke(this, Duration);

                if (metadata.Genres.Count != previousData.Genres.Count)
                    GenresCountChanged?.Invoke(this, metadata.Genres.Count);

                if (metadata.TrackIds.Count != previousData.TrackIds.Count)
                    TracksCountChanged?.Invoke(this, metadata.TrackIds.Count);

                if (metadata.ArtistIds.Count != previousData.ArtistIds.Count)
                    ArtistItemsCountChanged?.Invoke(this, metadata.ArtistIds.Count);

                _ = HandleImagesChangedAsync(previousData.ImageIds, metadata.ImageIds);
                _ = HandleArtistsChangedAsync(previousData.ArtistIds, metadata.ArtistIds);
                _ = HandleTracksChangedAsync(previousData.TrackIds, metadata.TrackIds);
            }
        }

        private async Task HandleTracksChangedAsync(HashSet<string> oldTrackIds, HashSet<string> newTrackIds)
        {
            if (oldTrackIds.OrderBy(s => s).SequenceEqual(newTrackIds.OrderBy(s => s)))
            {
                // Lists have identical content, so no images have changed.
                return;
            }

            var addedImages = newTrackIds.Except(oldTrackIds);
            var removedImages = oldTrackIds.Except(newTrackIds);

            if (oldTrackIds.Count != newTrackIds.Count)
            {
                TracksChanged?.Invoke(this, await TransformAsync(addedImages), await TransformAsync(removedImages));
                TracksCountChanged?.Invoke(this, newTrackIds.Count);
            }

            async Task<IReadOnlyList<CollectionChangedItem<ICoreTrack>>> TransformAsync(IEnumerable<string> ids)
            {
                var idArray = ids as string[] ?? ids.ToArray();
                var collectionChangedItems = new List<CollectionChangedItem<ICoreTrack>>(idArray.Length);

                foreach (var id in idArray)
                {
                    var track = await _fileMetadataManager.Tracks.GetByIdAsync(id);

                    Guard.IsNotNullOrWhiteSpace(track?.Id, nameof(track.Id));
                    collectionChangedItems.Add(new CollectionChangedItem<ICoreTrack>(InstanceCache.Tracks.GetOrCreate(track.Id, SourceCore, track), collectionChangedItems.Count));
                }

                return collectionChangedItems;
            }
        }

        private async Task HandleArtistsChangedAsync(HashSet<string> newArtistIds, HashSet<string> oldArtistIds)
        {
            if (oldArtistIds.OrderBy(s => s).SequenceEqual(newArtistIds.OrderBy(s => s)))
            {
                // Lists have identical content, so no images have changed.
                return;
            }

            var addedImages = newArtistIds.Except(oldArtistIds);
            var removedImages = oldArtistIds.Except(newArtistIds);

            if (oldArtistIds.Count != newArtistIds.Count)
            {
                ArtistItemsChanged?.Invoke(this, await TransformAsync(addedImages), await TransformAsync(removedImages));
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

        private async Task HandleImagesChangedAsync(HashSet<string> oldImageIds, HashSet<string> newImageIds)
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
        public event EventHandler<PlaybackState>? PlaybackStateChanged;

        /// <inheritdoc/>
        public event EventHandler<string>? NameChanged;

        /// <inheritdoc/>
        public event EventHandler<string?>? DescriptionChanged;

        /// <inheritdoc/>
        public event EventHandler<DateTime?>? DatePublishedChanged;

        /// <inheritdoc/>
        public event EventHandler<TimeSpan>? DurationChanged;

        /// <inheritdoc />
        public event EventHandler<DateTime?>? LastPlayedChanged;

        /// <inheritdoc />
        public event EventHandler<bool>? IsPlayArtistCollectionAsyncAvailableChanged;

        /// <inheritdoc />
        public event EventHandler<bool>? IsPauseArtistCollectionAsyncAvailableChanged;

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
        public event EventHandler<int>? ArtistItemsCountChanged;

        /// <inheritdoc />
        public event EventHandler<int>? TracksCountChanged;

        /// <inheritdoc />
        public event EventHandler<int>? ImagesCountChanged;

        /// <inheritdoc />
        public event EventHandler<int>? UrlsCountChanged;

        /// <inheritdoc/>
        public event EventHandler<int>? GenresCountChanged;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<ICoreArtistCollectionItem>? ArtistItemsChanged;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<ICoreTrack>? TracksChanged;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<ICoreImage>? ImagesChanged;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<ICoreUrl>? UrlsChanged;

        /// <inheritdoc/>
        public event CollectionChangedEventHandler<ICoreGenre>? GenresChanged;

        /// <inheritdoc/>
        public ICore SourceCore { get; }

        /// <inheritdoc/>
        public string Id { get; }

        /// <inheritdoc/>
        public string Name => _albumMetadata.Title ?? string.Empty;

        /// <inheritdoc/>
        public DateTime? DatePublished => _albumMetadata.DatePublished;

        /// <inheritdoc/>
        public string? Description => _albumMetadata.Description;

        /// <inheritdoc/>
        public PlaybackState PlaybackState { get; }

        /// <inheritdoc/>
        public TimeSpan Duration => _albumMetadata.Duration ?? new TimeSpan(0, 0, 0);

        /// <inheritdoc />
        public DateTime? LastPlayed { get; }

        /// <inheritdoc />
        public DateTime? AddedAt { get; }

        /// <inheritdoc />
        public int TotalImageCount => _albumMetadata.ImageIds?.Count ?? 0;

        /// <inheritdoc />
        public int TotalArtistItemsCount => _albumMetadata.ArtistIds?.Count ?? 0;

        /// <inheritdoc/>
        public int TotalTrackCount => _albumMetadata.TrackIds?.Count ?? 0;

        /// <inheritdoc/>
        public int TotalGenreCount => _albumMetadata.Genres?.Count ?? 0;

        /// <inheritdoc/>
        public int TotalUrlCount => 0;

        /// <inheritdoc/>
        public ICorePlayableCollectionGroup? RelatedItems { get; }

        /// <inheritdoc/>
        public bool IsPlayTrackCollectionAsyncAvailable => false;

        /// <inheritdoc/>
        public bool IsPauseTrackCollectionAsyncAvailable => false;

        /// <inheritdoc/>
        public bool IsPlayArtistCollectionAsyncAvailable => false;

        /// <inheritdoc/>
        public bool IsPauseArtistCollectionAsyncAvailable => false;

        /// <inheritdoc/>
        public bool IsChangeNameAsyncAvailable => false;

        /// <inheritdoc/>
        public bool IsChangeDatePublishedAsyncAvailable => false;

        /// <inheritdoc/>
        public bool IsChangeDescriptionAsyncAvailable => false;

        /// <inheritdoc/>
        public bool IsChangeDurationAsyncAvailable => false;

        /// <inheritdoc/>
        public event EventHandler<bool>? IsChangeDatePublishedAsyncAvailableChanged;

        /// <inheritdoc/>
        public Task<bool> IsAddGenreAvailableAsync(int index)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public Task<bool> IsAddTrackAvailableAsync(int index)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public Task<bool> IsAddImageAvailableAsync(int index)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public Task<bool> IsAddUrlAvailableAsync(int index)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task<bool> IsRemoveTrackAvailableAsync(int index)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task<bool> IsRemoveImageAvailableAsync(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc />
        public Task<bool> IsRemoveUrlAvailableAsync(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc />
        public Task<bool> IsRemoveGenreAvailableAsync(int index)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task<bool> IsAddArtistItemAvailableAsync(int index)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task<bool> IsRemoveArtistItemAvailableAsync(int index)
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
        public Task ChangeDatePublishedAsync(DateTime datePublished)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public Task ChangeNameAsync(string name)
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

        /// <inheritdoc/>
        public Task PauseTrackCollectionAsync()
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public Task PlayTrackCollectionAsync()
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task PlayArtistCollectionAsync(ICoreArtistCollectionItem artistItem)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task PlayTrackCollectionAsync(ICoreTrack track)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task AddArtistItemAsync(ICoreArtistCollectionItem artist, int index)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task AddTrackAsync(ICoreTrack track, int index)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task AddImageAsync(ICoreImage image, int index)
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
        public Task RemoveTrackAsync(int index)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task RemoveImageAsync(int index)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task RemoveUrlAsync(int index)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public Task AddGenreAsync(ICoreGenre genre, int index)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public Task RemoveGenreAsync(int index)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public async IAsyncEnumerable<ICoreTrack> GetTracksAsync(int limit, int offset)
        {
            var tracks = await _fileMetadataManager.Tracks.GetTracksByAlbumId(Id, offset, limit);

            foreach (var track in tracks)
            {
                Guard.IsNotNullOrWhiteSpace(track.Id, nameof(track.Id));
                yield return InstanceCache.Tracks.GetOrCreate(track.Id, SourceCore, track);
            }
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<ICoreArtistCollectionItem> GetArtistItemsAsync(int limit, int offset)
        {
            var artists = await _fileMetadataManager.Artists.GetArtistsByAlbumId(Id, offset, limit);

            foreach (var artist in artists)
            {
                Guard.IsNotNullOrWhiteSpace(artist.Id, nameof(artist.Id));
                yield return InstanceCache.Artists.GetOrCreate(artist.Id, SourceCore, artist);
            }
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<ICoreImage> GetImagesAsync(int limit, int offset)
        {
            if (_albumMetadata.ImageIds == null)
                yield break;

            foreach (var imageId in _albumMetadata.ImageIds.Skip(offset).Take(limit))
            {
                var image = await _fileMetadataManager.Images.GetByIdAsync(imageId);
                Guard.IsNotNullOrWhiteSpace(image?.Id, nameof(image.Id));

                yield return InstanceCache.Images.GetOrCreate(imageId, SourceCore, image);
            }
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<ICoreUrl> GetUrlsAsync(int limit, int offset)
        {
            await Task.CompletedTask;
            yield break;
        }

        /// <inheritdoc/>
        public async IAsyncEnumerable<ICoreGenre> GetGenresAsync(int limit, int offset)
        {
            foreach (var genre in _albumMetadata.Genres ?? Enumerable.Empty<string>())
            {
                yield return new FilesCoreGenre(SourceCore, genre);
            }

            await Task.CompletedTask;
        }

        /// <inheritdoc />
        public ValueTask DisposeAsync()
        {
            DetachEvents();
            return default;
        }
    }
}
