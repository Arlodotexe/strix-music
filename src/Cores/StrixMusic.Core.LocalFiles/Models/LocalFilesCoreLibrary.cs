using Microsoft.Toolkit.Diagnostics;
using OwlCore.Events;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StrixMusic.Sdk.Services.FileMetadataManager;
using StrixMusic.Sdk.Services.FileMetadataManager.Models;

namespace StrixMusic.Core.LocalFiles.Models
{
    /// <inheritdoc cref="ICoreLibrary"/>
    public class LocalFilesCoreLibrary : LocalFilesCorePlayableCollectionGroupBase, ICoreLibrary
    {

        private IFileMetadataManager? _fileMetadataManager;
        private readonly object _lockObj =new object();

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

        /// <inheritdoc />
        public override int TotalChildrenCount { get; internal set; }

        /// <inheritdoc />
        public override int TotalArtistItemsCount { get; internal set; }

        /// <inheritdoc />
        public override int TotalAlbumItemsCount { get; internal set; }

        /// <inheritdoc />
        public override int TotalPlaylistItemsCount { get; internal set; }

        /// <inheritdoc />
        public override int TotalTracksCount { get; internal set; }

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

                yield return new LocalFilesCoreAlbum(SourceCore, album, album.TrackIds?.Count ?? 0, track?.ImagePath != null ? new LocalFilesCoreImage(SourceCore, track.ImagePath) : null);
            }
        }

        /// <inheritdoc/>
        public override async IAsyncEnumerable<ICoreArtistCollectionItem> GetArtistItemsAsync(int limit, int offset)
        {
            Guard.IsNotNull(_fileMetadataManager, nameof(_fileMetadataManager));

            var artistMetadata = await _fileMetadataManager.Artists.GetArtistMetadata(offset, limit);

            foreach (var artist in artistMetadata)
            {
                if (artist.ImagePath != null)
                    yield return new LocalFilesCoreArtist(SourceCore, artist, artist.TrackIds?.Count ?? 0, new LocalFilesCoreImage(SourceCore, artist.ImagePath));

                yield return new LocalFilesCoreArtist(SourceCore, artist, artist.TrackIds?.Count ?? 0, null);
            }
        }

        /// <inheritdoc/>
        public override async IAsyncEnumerable<ICoreTrack> GetTracksAsync(int limit, int offset = 0)
        {
            Guard.IsNotNull(_fileMetadataManager, nameof(_fileMetadataManager));

            var artistMetadata = await _fileMetadataManager.Tracks.GetTrackMetadata(offset, limit);

            foreach (var track in artistMetadata)
            {
                yield return new LocalFilesCoreTrack(SourceCore, track);
            }
        }

        private void MetadataScannerFileMetadataAdded(object sender, FileMetadata e)
        {
            lock (_lockObj)
            {
                HandleAlbumMetadataAdded(e);

                HandleArtistMetadataAdded(e);

                HandleAddedTrackMetadata(e);
            }
        }

        private void HandleAddedTrackMetadata(FileMetadata e)
        {
            if (e.TrackMetadata == null)
                return;

            var filesCoreTrack = new LocalFilesCoreTrack(SourceCore, e.TrackMetadata);

            var addedItems = new List<CollectionChangedItem<ICoreTrack>>
                {
                    new CollectionChangedItem<ICoreTrack>(filesCoreTrack, 0),
                };

            TrackItemsChanged?.Invoke(this, addedItems, new List<CollectionChangedItem<ICoreTrack>>());
        }

        private void HandleArtistMetadataAdded(FileMetadata e)
        {
            if (e.ArtistMetadata == null)
                return;

            LocalFilesCoreArtist? filesCoreArtist;

            if (e.ArtistMetadata.ImagePath != null)
            {
                filesCoreArtist = new LocalFilesCoreArtist(
                    SourceCore,
                    e.ArtistMetadata,
                    e.ArtistMetadata.TrackIds?.Count ?? 0,
                    new LocalFilesCoreImage(SourceCore, e.ArtistMetadata.ImagePath));
            }
            else
            {
                filesCoreArtist = new LocalFilesCoreArtist(
                    SourceCore,
                    e.ArtistMetadata,
                    e.ArtistMetadata.TrackIds?.Count ?? 0,
                    null);
            }

            var addedItems = new List<CollectionChangedItem<ICoreArtistCollectionItem>>
                {
                    new CollectionChangedItem<ICoreArtistCollectionItem>(filesCoreArtist, 0),
                };

            ArtistItemsChanged?.Invoke(
                this,
                addedItems,
                new List<CollectionChangedItem<ICoreArtistCollectionItem>>());
        }

        private void HandleAlbumMetadataAdded(FileMetadata e)
        {
            if (e.AlbumMetadata == null)
                return;

            var track = e.TrackMetadata;

            var fileCoreAlbum = new LocalFilesCoreAlbum(
                SourceCore,
                e.AlbumMetadata,
                e.AlbumMetadata.TrackIds?.Count ?? 0,
                track?.ImagePath != null ? new LocalFilesCoreImage(SourceCore, track.ImagePath) : null);

            var addedItems = new List<CollectionChangedItem<ICoreAlbumCollectionItem>>
                {
                    new CollectionChangedItem<ICoreAlbumCollectionItem>(fileCoreAlbum, 0),
                };

            AlbumItemsChanged?.Invoke(
                this,
                addedItems,
                new List<CollectionChangedItem<ICoreAlbumCollectionItem>>());
        }
    }
}
