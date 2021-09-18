using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Toolkit.Diagnostics;
using OwlCore.Collections;
using OwlCore.Events;
using StrixMusic.Core.LocalFiles.Services;
using StrixMusic.Sdk.Data;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.Extensions;
using StrixMusic.Sdk.MediaPlayback;
using StrixMusic.Sdk.Services.FileMetadataManager;
using StrixMusic.Sdk.Services.FileMetadataManager.Models;

namespace StrixMusic.Core.LocalFiles.Models
{
    /// <summary>
    /// Wraps around <see cref="TrackMetadata"/> to provide track information extracted from a file to the Strix SDK.
    /// </summary>
    public class LocalFilesCoreTrack : ICoreTrack
    {
        private readonly IFileMetadataManager _fileMetadataManager;
        private TrackMetadata _trackMetadata;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalFilesCoreTrack"/> class.
        /// </summary>
        /// <param name="sourceCore">The source core.</param>
        /// <param name="trackMetadata">The track metadata to wrap around</param>
        public LocalFilesCoreTrack(ICore sourceCore, TrackMetadata trackMetadata)
        {
            SourceCore = sourceCore;
            _trackMetadata = trackMetadata;
            Genres = new SynchronizedObservableCollection<string>(trackMetadata.Genres);

            _fileMetadataManager = SourceCore.GetService<IFileMetadataManager>();
            AttachEvents();
        }

        private void AttachEvents()
        {
            _fileMetadataManager.Tracks.MetadataUpdated += Tracks_MetadataUpdated;
            _fileMetadataManager.Tracks.ArtistItemsChanged += Tracks_ArtistItemsChanged;
        }

        private void DetachEvents()
        {
            _fileMetadataManager.Tracks.MetadataUpdated -= Tracks_MetadataUpdated;
            _fileMetadataManager.Tracks.ArtistItemsChanged -= Tracks_ArtistItemsChanged;
        }

        private void Tracks_ArtistItemsChanged(object sender, IReadOnlyList<CollectionChangedItem<(TrackMetadata Track, ArtistMetadata Artist)>> addedItems, IReadOnlyList<CollectionChangedItem<(TrackMetadata Track, ArtistMetadata Artist)>> removedItems)
        {
            var coreAddedItems = new List<CollectionChangedItem<ICoreArtistCollectionItem>>();
            var coreRemovedItems = new List<CollectionChangedItem<ICoreArtistCollectionItem>>();

            foreach (var item in addedItems)
            {
                if (item.Data.Track.Id == Id)
                {
                    Guard.IsNotNullOrWhiteSpace(item.Data.Artist.Id, nameof(item.Data.Artist.Id));
                    var coreArtist = InstanceCache.Artists.GetOrCreate(item.Data.Artist.Id, SourceCore, item.Data.Artist);
                    coreAddedItems.Add(new CollectionChangedItem<ICoreArtistCollectionItem>(coreArtist, item.Index));
                }
            }

            foreach (var item in removedItems)
            {
                if (item.Data.Track.Id == Id)
                {
                    Guard.IsNotNullOrWhiteSpace(item.Data.Artist.Id, nameof(item.Data.Artist.Id));
                    var coreArtist = InstanceCache.Artists.GetOrCreate(item.Data.Artist.Id, SourceCore, item.Data.Artist);
                    coreRemovedItems.Add(new CollectionChangedItem<ICoreArtistCollectionItem>(coreArtist, item.Index));
                }
            }

            if (coreAddedItems.Count + coreRemovedItems.Count > 0)
                ArtistItemsChanged?.Invoke(this, coreAddedItems, coreRemovedItems);
        }

        private void Tracks_MetadataUpdated(object sender, IEnumerable<TrackMetadata> e)
        {
            foreach (var metadata in e)
            {
                if (metadata.Id != Id)
                    return;

                Guard.IsNotNull(metadata.ArtistIds, nameof(metadata.ArtistIds));

                var previousData = _trackMetadata;
                _trackMetadata = metadata;

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

                // TODO genres, post genres do-over

                if (metadata.ArtistIds.Count != (previousData.ArtistIds?.Count ?? 0))
                    ArtistItemsCountChanged?.Invoke(this, metadata.ArtistIds.Count);

                HandleImagesChanged(previousData.ImageIds, metadata.ImageIds);
            }
        }

        private async void HandleImagesChanged(IReadOnlyList<string>? oldImageIds, IReadOnlyList<string>? newImageIds)
        {
            async Task<IReadOnlyList<CollectionChangedItem<ICoreImage>>> TransformAsync(IEnumerable<string> ids)
            {
	            var idArray = ids as string[] ?? ids.ToArray();
	            var collectionChangedItems = new List<CollectionChangedItem<ICoreImage>>(idArray.Length);

                foreach (var id in idArray)
                {
                    var image = await _fileMetadataManager.Images.GetImageByIdAsync(id);
                    
                    Guard.IsNotNullOrWhiteSpace(image.Id, nameof(image.Id));
                    collectionChangedItems.Add(new CollectionChangedItem<ICoreImage>(InstanceCache.Images.GetOrCreate(image.Id, SourceCore, image), collectionChangedItems.Count));
                }

                return collectionChangedItems;
            }

            // Null and empty lists should be handled the same.
            oldImageIds ??= new List<string>();
            newImageIds ??= new List<string>();

            if (oldImageIds.OrderBy(s => s).SequenceEqual(newImageIds.OrderBy(s => s)))
            {
	            // Lists have identical content, so no images have changed.
	            return;
            }

            var addedImages = newImageIds.Except(oldImageIds);
            var removedImages = oldImageIds.Except(newImageIds);

            ImagesChanged?.Invoke(this, await TransformAsync(addedImages), await TransformAsync(removedImages));

            if (oldImageIds.Count != newImageIds.Count)
			{
                ImagesCountChanged?.Invoke(this, newImageIds.Count);
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
        public event EventHandler<Uri?>? UrlChanged;

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
        public event EventHandler<int>? ImagesCountChanged;

        /// <inheritdoc />
        public event EventHandler<int>? ArtistItemsCountChanged;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<ICoreImage>? ImagesChanged;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<ICoreArtistCollectionItem>? ArtistItemsChanged;

        /// <inheritdoc/>
        public string Id => _trackMetadata.Id ?? string.Empty;

        /// <inheritdoc/>
        public TrackType Type => TrackType.Song;

        /// <inheritdoc />
        public int TotalArtistItemsCount => _trackMetadata.ArtistIds?.Count ?? 0;

        /// <inheritdoc />
        public int TotalImageCount => _trackMetadata.ImageIds?.Count ?? 0;

        /// <inheritdoc/>
        public ICoreAlbum? Album { get; }

        /// <inheritdoc/>
        public SynchronizedObservableCollection<string>? Genres { get; }

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
        public Uri? LocalTrackPath => _trackMetadata.Source;

        /// <inheritdoc/>
        public Uri? Url => null;

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
        public Task<bool> IsAddImageAvailable(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc/>
        public Task<bool> IsAddGenreAvailable(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc/>
        public Task<bool> IsAddArtistItemAvailable(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc />
        public Task<bool> IsRemoveImageAvailable(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc />
        public Task<bool> IsRemoveArtistItemAvailable(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc />
        public Task<bool> IsRemoveGenreAvailable(int index)
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
        public Task RemoveArtistItemAsync(int index)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task AddImageAsync(ICoreImage image, int index)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task RemoveImageAsync(int index)
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
            {
                yield break;
            }

            foreach (var imageId in _trackMetadata.ImageIds)
            {
                yield return InstanceCache.Images.GetOrCreate(
                    imageId,
                    SourceCore,
                    await _fileMetadataManager.Images.GetImageByIdAsync(imageId));
            }
        }

        private void ReleaseUnmanagedResources()
        {
            DetachEvents();
        }

        private void Dispose(bool disposing)
        {
            ReleaseUnmanagedResources();
            if (disposing)
            {
                Genres?.Dispose();
            }
        }

        /// <inheritdoc />
        ~LocalFilesCoreTrack()
        {
            Dispose(false);
        }

        /// <inheritdoc />
        public ValueTask DisposeAsync()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
            return default;
        }
    }
}
