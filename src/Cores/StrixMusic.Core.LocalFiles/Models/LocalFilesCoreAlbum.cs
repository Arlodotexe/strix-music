using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Toolkit.Diagnostics;
using OwlCore.Collections;
using OwlCore.Events;
using StrixMusic.Core.LocalFiles.Services;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.Extensions;
using StrixMusic.Sdk.MediaPlayback;
using StrixMusic.Sdk.Services.FileMetadataManager;
using StrixMusic.Sdk.Services.FileMetadataManager.Models;

namespace StrixMusic.Core.LocalFiles.Models
{
    /// <summary>
    /// Wraps around <see cref="AlbumMetadata"/> to provide album information extracted from a file to the Strix SDK.
    /// </summary>
    public class LocalFilesCoreAlbum : ICoreAlbum
    {
        private readonly IFileMetadataManager _fileMetadataManager;
        private AlbumMetadata _albumMetadata;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalFilesCoreAlbum"/> class.
        /// </summary>
        /// <param name="sourceCore">The core that created this object.</param>
        /// <param name="albumMetadata">The source album metadata to wrap around.</param>
        public LocalFilesCoreAlbum(ICore sourceCore, AlbumMetadata albumMetadata)
        {
            Guard.IsNotNullOrWhiteSpace(albumMetadata.Id, nameof(albumMetadata.Id));

            Id = albumMetadata.Id;
            _fileMetadataManager = sourceCore.GetService<IFileMetadataManager>();
            SourceCore = sourceCore;
            _albumMetadata = albumMetadata;

            Guard.IsNotNull(albumMetadata.ArtistIds, nameof(albumMetadata.ArtistIds));
            Guard.IsNotNull(albumMetadata.TrackIds, nameof(albumMetadata.TrackIds));

            Genres = new SynchronizedObservableCollection<string>(albumMetadata.Genres);

            AttachEvents();
        }

        private void AttachEvents()
        {
            _fileMetadataManager.Albums.MetadataUpdated += Albums_MetadataUpdated;
            _fileMetadataManager.Albums.ArtistItemsChanged += Albums_ArtistItemsChanged;
            _fileMetadataManager.Albums.TracksChanged += Albums_TracksChanged;
        }

        private void DetachEvents()
        {
            _fileMetadataManager.Albums.MetadataUpdated -= Albums_MetadataUpdated;
            _fileMetadataManager.Albums.ArtistItemsChanged -= Albums_ArtistItemsChanged;
            _fileMetadataManager.Albums.TracksChanged -= Albums_TracksChanged;
        }

        private void Albums_TracksChanged(object sender, IReadOnlyList<CollectionChangedItem<(AlbumMetadata Album, TrackMetadata Track)>> addedItems, IReadOnlyList<CollectionChangedItem<(AlbumMetadata Album, TrackMetadata Track)>> removedItems)
        {
            var coreAddedItems = new List<CollectionChangedItem<ICoreTrack>>();
            var coreRemovedItems = new List<CollectionChangedItem<ICoreTrack>>();

            foreach (var item in addedItems)
            {
                if (item.Data.Album.Id == Id)
                {
                    Guard.IsNotNullOrWhiteSpace(item.Data.Track.Id, nameof(item.Data.Track.Id));
                    var coreTrack = InstanceCache.Tracks.GetOrCreate(item.Data.Track.Id, SourceCore, item.Data.Track);
                    coreAddedItems.Add(new CollectionChangedItem<ICoreTrack>(coreTrack, item.Index));
                }
            }

            foreach (var item in removedItems)
            {
                if (item.Data.Album.Id == Id)
                {
                    Guard.IsNotNullOrWhiteSpace(item.Data.Track.Id, nameof(item.Data.Track.Id));
                    var coreTrack = InstanceCache.Tracks.GetOrCreate(item.Data.Track.Id, SourceCore, item.Data.Track);
                    coreRemovedItems.Add(new CollectionChangedItem<ICoreTrack>(coreTrack, item.Index));
                }
            }

            if (coreAddedItems.Count > 0 || coreRemovedItems.Count > 0)
                TrackItemsChanged?.Invoke(this, coreAddedItems, coreRemovedItems);
        }

        private void Albums_ArtistItemsChanged(object sender, IReadOnlyList<CollectionChangedItem<(AlbumMetadata Album, ArtistMetadata Artist)>> addedItems, IReadOnlyList<CollectionChangedItem<(AlbumMetadata Album, ArtistMetadata Artist)>> removedItems)
        {
            var coreAddedItems = new List<CollectionChangedItem<ICoreArtistCollectionItem>>();
            var coreRemovedItems = new List<CollectionChangedItem<ICoreArtistCollectionItem>>();

            foreach (var item in addedItems)
            {
                if (item.Data.Album.Id == Id)
                {
                    Guard.IsNotNullOrWhiteSpace(item.Data.Artist.Id, nameof(item.Data.Artist.Id));
                    var coreArtist = InstanceCache.Artists.GetOrCreate(item.Data.Artist.Id, SourceCore, item.Data.Artist);
                    coreAddedItems.Add(new CollectionChangedItem<ICoreArtistCollectionItem>(coreArtist, item.Index));
                }
            }

            foreach (var item in removedItems)
            {
                if (item.Data.Album.Id == Id)
                {
                    Guard.IsNotNullOrWhiteSpace(item.Data.Artist.Id, nameof(item.Data.Artist.Id));
                    var coreArtist = InstanceCache.Artists.GetOrCreate(item.Data.Artist.Id, SourceCore, item.Data.Artist);
                    coreRemovedItems.Add(new CollectionChangedItem<ICoreArtistCollectionItem>(coreArtist, item.Index));
                }
            }

            if (coreAddedItems.Count > 0 || coreRemovedItems.Count > 0)
                ArtistItemsChanged?.Invoke(this, coreAddedItems, coreRemovedItems);
        }

        private void Albums_MetadataUpdated(object sender, IEnumerable<AlbumMetadata> e)
        {
            foreach (var metadata in e)
            {
                if (metadata.Id != Id)
                    return;

                Guard.IsNotNull(metadata.ArtistIds, nameof(metadata.ArtistIds));
                Guard.IsNotNull(metadata.TrackIds, nameof(metadata.TrackIds));

                var previousData = _albumMetadata;
                _albumMetadata = metadata;

                if (metadata.Title != previousData.Title)
                    NameChanged?.Invoke(this, Name);

                if (metadata.ImageIds != previousData.ImageIds)
                    HandleImageChanged(metadata);

                if (metadata.DatePublished != previousData.DatePublished)
                    DatePublishedChanged?.Invoke(this, DatePublished);

                if (metadata.Description != previousData.Description)
                    DescriptionChanged?.Invoke(this, Description);

                if (metadata.Duration != previousData.Duration)
                    DurationChanged?.Invoke(this, Duration);

                // TODO genres, post genres do-over

                if (metadata.TrackIds.Count != (previousData.TrackIds?.Count ?? 0))
                    TrackItemsCountChanged?.Invoke(this, metadata.TrackIds.Count);

                if (metadata.ArtistIds.Count != (previousData.ArtistIds?.Count ?? 0))
                    ArtistItemsCountChanged?.Invoke(this, metadata.ArtistIds.Count);
            }
        }

        private void HandleImageChanged(AlbumMetadata e)
        {
            var previousImage = _image;

            var removed = new List<CollectionChangedItem<ICoreImage>>();
            var added = new List<CollectionChangedItem<ICoreImage>>();

            if (previousImage != null)
                removed.Add(new CollectionChangedItem<ICoreImage>(previousImage, 0));

            if (e.ImageIds != null)
            {
                var newImage = new LocalFilesCoreImage(SourceCore, null);
                InstanceCache.Images.Replace(Id, newImage);
                added.Add(new CollectionChangedItem<ICoreImage>(newImage, 0));
                _image = newImage;
            }

            if (added.Count > 0 || removed.Count > 0)
            {
                ImagesChanged?.Invoke(this, added, removed);

                if (added.Count != removed.Count)
                    ImagesCountChanged?.Invoke(this, TotalImageCount);
            }
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
        public event EventHandler<int>? TrackItemsCountChanged;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<ICoreTrack>? TrackItemsChanged;

        /// <inheritdoc />
        public event EventHandler<int>? ImagesCountChanged;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<ICoreImage>? ImagesChanged;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<ICoreArtistCollectionItem>? ArtistItemsChanged;

        /// <inheritdoc/>
        public int TotalTracksCount => _albumMetadata.TrackIds?.Count ?? 0;

        /// <inheritdoc/>
        public ICore SourceCore { get; }

        /// <inheritdoc/>
        public string Id { get; }

        /// <inheritdoc/>
        public Uri? Url => null;

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
        public ICorePlayableCollectionGroup? RelatedItems { get; }

        /// <inheritdoc/>
        public SynchronizedObservableCollection<string>? Genres { get; }

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

        /// <inheritdoc />
        public event EventHandler<int>? ArtistItemsCountChanged;

        /// <inheritdoc/>
        public Task<bool> IsAddGenreAvailable(int index)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public Task<bool> IsAddTrackAvailable(int index)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public Task<bool> IsAddImageAvailable(int index)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task<bool> IsRemoveTrackAvailable(int index)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task<bool> IsRemoveImageAvailable(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc />
        public Task<bool> IsRemoveGenreAvailable(int index)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task<bool> IsAddArtistItemAvailable(int index)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task<bool> IsRemoveArtistItemAvailable(int index)
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
            var images = await _fileMetadataManager.Images.GetImagesByAlbumIdAsync(Id, offset, limit);

            foreach (var image in images)
            {
                Guard.IsNotNullOrWhiteSpace(image.Id, nameof(image.Id));
                yield return InstanceCache.Images.GetOrCreate(image.Id, SourceCore, image);
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
        public ValueTask DisposeAsync()
        {
            Dispose(true);
            GC.SuppressFinalize(this);

            return default;
        }

        /// <inheritdoc />
        ~LocalFilesCoreAlbum()
        {
            Dispose(false);
        }
    }
}
