using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Toolkit.Diagnostics;
using OwlCore;
using OwlCore.Events;
using StrixMusic.Core.LocalFiles.Services;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.Extensions;
using StrixMusic.Sdk.Services.FileMetadataManager;
using StrixMusic.Sdk.Services.FileMetadataManager.Models;

namespace StrixMusic.Core.LocalFiles.Models
{
    /// <inheritdoc cref="ICoreLibrary"/>
    public class LocalFilesCoreLibrary : LocalFilesCorePlayableCollectionGroupBase, ICoreLibrary
    {
        private readonly List<FileMetadata> _batchItemsToEmit = new List<FileMetadata>();
        private readonly SemaphoreSlim _batchItemsLock = new SemaphoreSlim(1, 1);
        private IFileMetadataManager? _fileMetadataManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalFilesCoreLibrary"/> class.
        /// </summary>
        /// <param name="sourceCore">The core that created this instance.</param>
        public LocalFilesCoreLibrary(ICore sourceCore)
            : base(sourceCore)
        {
        }

        /// <inheritdoc/>
        public override async Task InitAsync()
        {
            _fileMetadataManager = SourceCore.GetService<IFileMetadataManager>();
            _fileMetadataManager.FileMetadataAdded += MetadataScannerFileMetadataAdded;

            IsInitialized = true;

            await base.InitAsync();
        }

        /// <summary>
        /// Determines if collection base is initialized or not.
        /// </summary>
        public override bool IsInitialized { get; protected set; }

        /// <inheritdoc />
        public override string Id { get; protected set; } = "library";

        /// <inheritdoc />
        public override Uri? Url { get; protected set; } = null;

        /// <inheritdoc />
        public override string Name { get; protected set; } = "Library";

        /// <inheritdoc />
        public override string? Description { get; protected set; } = null;

        /// <inheritdoc />?
        public virtual event CollectionChangedEventHandler<ICorePlaylistCollectionItem>? PlaylistItemsChanged;

        /// <inheritdoc />
        public override event CollectionChangedEventHandler<ICoreAlbumCollectionItem>? AlbumItemsChanged;

        /// <inheritdoc />
        public override event CollectionChangedEventHandler<ICoreArtistCollectionItem>? ArtistItemsChanged;

        /// <inheritdoc />?
        public override event CollectionChangedEventHandler<ICoreTrack>? TrackItemsChanged;

        /// <inheritdoc/>
        public override IAsyncEnumerable<ICorePlayableCollectionGroup> GetChildrenAsync(int limit, int offset = 0)
        {
            return AsyncEnumerable.Empty<ICorePlayableCollectionGroup>();
        }

        /// <inheritdoc/>
        public override IAsyncEnumerable<ICorePlaylistCollectionItem> GetPlaylistItemsAsync(int limit, int offset)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override async IAsyncEnumerable<ICoreAlbumCollectionItem> GetAlbumItemsAsync(int limit, int offset)
        {
            Guard.IsNotNull(_fileMetadataManager, nameof(_fileMetadataManager));
            var albumMetadata = await _fileMetadataManager.Albums.GetAlbumMetadata(offset, limit);

            foreach (var album in albumMetadata)
            {
                Guard.IsNotNull(album.Id, nameof(album.Id));

                var tracks = await _fileMetadataManager.Tracks.GetTracksByAlbumId(album.Id, 0, 1);
                var track = tracks.FirstOrDefault();

                Guard.IsNotNullOrWhiteSpace(track?.Id, nameof(track.Id));

                yield return InstanceCache.Albums.GetOrCreate(album.Id, SourceCore, album, album.TrackIds?.Count ?? 0, track.ImagePath != null ? InstanceCache.Images.GetOrCreate(track.Id, SourceCore, track.ImagePath) : null);
            }
        }

        /// <inheritdoc/>
        public override async IAsyncEnumerable<ICoreArtistCollectionItem> GetArtistItemsAsync(int limit, int offset)
        {
            Guard.IsNotNull(_fileMetadataManager, nameof(_fileMetadataManager));
            var artistMetadata = await _fileMetadataManager.Artists.GetArtistMetadata(offset, limit);

            foreach (var artist in artistMetadata)
            {
                Guard.IsNotNullOrWhiteSpace(artist.Id, nameof(artist.Id));

                if (artist.ImagePath != null)
                    yield return InstanceCache.Artists.GetOrCreate(artist.Id, SourceCore, artist, artist.TrackIds?.Count ?? 0, InstanceCache.Images.GetOrCreate(artist.Id, SourceCore, artist.ImagePath));

                yield return InstanceCache.Artists.GetOrCreate(artist.Id, SourceCore, artist, artist.TrackIds?.Count ?? 0);
            }
        }

        /// <inheritdoc/>
        public override async IAsyncEnumerable<ICoreTrack> GetTracksAsync(int limit, int offset = 0)
        {
            Guard.IsNotNull(_fileMetadataManager, nameof(_fileMetadataManager));
            var artistMetadata = await _fileMetadataManager.Tracks.GetTrackMetadata(offset, limit);

            foreach (var track in artistMetadata)
            {
                Guard.IsNotNullOrWhiteSpace(track.Id, nameof(track.Id));
                yield return InstanceCache.Tracks.GetOrCreate(track.Id, SourceCore, track);
            }
        }

        private async void MetadataScannerFileMetadataAdded(object sender, FileMetadata e)
        {
            // Emitting the data in batches was required for performance reasons.
            // The MergedCollectionMap was not optimized very well for many quick collection changes, so we need to emit many items at once.
            await _batchItemsLock.WaitAsync();
            _batchItemsToEmit.Add(e);
            _batchItemsLock.Release();

            if (!await Threading.Debounce($"{nameof(LocalFilesCoreLibrary)}.{SourceCore.InstanceId}", TimeSpan.FromSeconds(2)) || _batchItemsToEmit.Count == 0)
                return;

            await _batchItemsLock.WaitAsync();

            HandleAlbumMetadataAdded();
            HandleArtistMetadataAdded();
            HandleTrackMetadataAdded();
            HandlePlaylistMetadataAdded();

            _batchItemsToEmit.Clear();
            _batchItemsLock.Release();
        }

        private void HandlePlaylistMetadataAdded()
        {
            var addedItems = new List<CollectionChangedItem<ICorePlaylistCollectionItem>>();

            foreach (var e in _batchItemsToEmit)
            {
                if (e.PlaylistMetadata == null)
                    continue;

                Guard.IsNotNullOrWhiteSpace(e.TrackMetadata?.Id, nameof(e.PlaylistMetadata.Id));

                var filesCoreTrack = InstanceCache.PlayLists.GetOrCreate(e.TrackMetadata.Id, SourceCore, e.PlaylistMetadata);

                addedItems.Add(new CollectionChangedItem<ICorePlaylistCollectionItem>(filesCoreTrack, addedItems.Count));
            }

            var removedItems = Array.Empty<CollectionChangedItem<ICorePlaylistCollectionItem>>();

            TotalPlaylistItemsCount += addedItems.Count;
            PlaylistItemsChanged?.Invoke(this, addedItems, removedItems);
        }

        private void HandleTrackMetadataAdded()
        {
            var addedItems = new List<CollectionChangedItem<ICoreTrack>>();

            foreach (var e in _batchItemsToEmit)
            {
                if (e.TrackMetadata == null)
                    continue;

                Guard.IsNotNullOrWhiteSpace(e.TrackMetadata?.Id, nameof(e.TrackMetadata.Id));

                var filesCoreTrack = InstanceCache.Tracks.GetOrCreate(e.TrackMetadata.Id, SourceCore, e.TrackMetadata);

                addedItems.Add(new CollectionChangedItem<ICoreTrack>(filesCoreTrack, addedItems.Count));
            }

            var removedItems = Array.Empty<CollectionChangedItem<ICoreTrack>>();

            TotalTracksCount += addedItems.Count;
            TrackItemsChanged?.Invoke(this, addedItems, removedItems);
        }

        private void HandleArtistMetadataAdded()
        {
            var addedItems = new List<CollectionChangedItem<ICoreArtistCollectionItem>>();

            foreach (var e in _batchItemsToEmit)
            {
                if (e.ArtistMetadata == null)
                    continue;

                Guard.IsNotNullOrWhiteSpace(e.ArtistMetadata?.Id, nameof(e.ArtistMetadata.Id));

                var filesCoreArtist = InstanceCache.Artists.GetOrCreate(
                    e.ArtistMetadata.Id,
                    SourceCore,
                    e.ArtistMetadata,
                    e.ArtistMetadata.TrackIds?.Count ?? 0,
                    e.ArtistMetadata.ImagePath == null
                        ? null
                        : InstanceCache.Images.GetOrCreate(e.ArtistMetadata.Id, SourceCore, e.ArtistMetadata.ImagePath));

                addedItems.Add(new CollectionChangedItem<ICoreArtistCollectionItem>(filesCoreArtist, addedItems.Count));
            }

            var removedItems = Array.Empty<CollectionChangedItem<ICoreArtistCollectionItem>>();

            TotalArtistItemsCount += addedItems.Count;
            ArtistItemsChanged?.Invoke(this, addedItems, removedItems);
        }

        private void HandleAlbumMetadataAdded()
        {
            var addedItems = new List<CollectionChangedItem<ICoreAlbumCollectionItem>>();

            foreach (var e in _batchItemsToEmit)
            {
                if (e.AlbumMetadata == null)
                    continue;

                var track = e.TrackMetadata;

                Guard.IsNotNullOrWhiteSpace(e.AlbumMetadata.Id, nameof(e.AlbumMetadata.Id));

                var fileCoreAlbum = InstanceCache.Albums.GetOrCreate(
                    e.AlbumMetadata.Id,
                    SourceCore,
                    e.AlbumMetadata,
                    e.AlbumMetadata.TrackIds?.Count ?? 0,
                    track?.ImagePath != null ? InstanceCache.Images.GetOrCreate(e.AlbumMetadata.Id, SourceCore, track.ImagePath) : null);

                addedItems.Add(new CollectionChangedItem<ICoreAlbumCollectionItem>(fileCoreAlbum, addedItems.Count));
            }

            var removedItems = Array.Empty<CollectionChangedItem<ICoreAlbumCollectionItem>>();

            TotalAlbumItemsCount += addedItems.Count;
            AlbumItemsChanged?.Invoke(this, addedItems, removedItems);
        }
    }
}
