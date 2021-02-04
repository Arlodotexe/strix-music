using Microsoft.Toolkit.Diagnostics;
using OwlCore.Events;
using OwlCore.Extensions;
using OwlCore.Provisos;
using StrixMusic.Core.LocalFiles.Models;
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
        private readonly object _lockObj = new object();
        private IFileMetadataManager? _fileMetadataManager;
        private IList<ArtistMetadata>? _artistMetadatas;
        private IList<AlbumMetadata>? _albumMetadatas;
        private IList<TrackMetadata>? _trackMetadatas;

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
        public virtual event CollectionChangedEventHandler<ICoreAlbumCollectionItem>? AlbumItemsChanged;

        /// <inheritdoc />
        public virtual event CollectionChangedEventHandler<ICoreArtistCollectionItem>? ArtistItemsChanged;

        /// <inheritdoc />
        public virtual event CollectionChangedEventHandler<ICorePlayableCollectionGroup>? ChildItemsChanged;

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
            var albumMetadata = await _fileMetadataManager.Albums.GetAlbumMetadata(offset, limit);

            foreach (var album in albumMetadata)
            {
                var tracks = await SourceCore.GetService<TrackRepository>().GetTracksByAlbumId(album.Id, 0, album.TrackIds.Count);
                yield return new LocalFilesCoreAlbum(SourceCore, album, tracks.Count);
            }
        }

        /// <inheritdoc/>
        public override async IAsyncEnumerable<ICoreArtistCollectionItem> GetArtistItemsAsync(int limit, int offset)
        {
            var artistMetadata = await _fileMetadataManager.Artists.GetArtistMetadata(offset, limit);

            foreach (var artist in artistMetadata)
            {
                // just to test
                var tracks = await SourceCore.GetService<TrackRepository>().GetTracksByAlbumId(artist.Id, 0, artist.TrackIds.Count);
                yield return new LocalFilesCoreArtist(SourceCore, artist, tracks.Count);
            }
        }

        /// <inheritdoc />?
        public event CollectionChangedEventHandler<ICoreTrack>? TrackItemsChanged;

        /// <inheritdoc/>
        public override async IAsyncEnumerable<ICoreTrack> GetTracksAsync(int limit, int offset = 0)
        {
            var artistMetadata = await _fileMetadataManager.Tracks.GetTrackMetadata(offset, limit);

            foreach (var track in artistMetadata)
            {
                yield return new LocalFilesCoreTrack(SourceCore, track);
            }
        }

        private void MetadataScannerFileMetadataAdded(object sender, FileMetadata e)
        {
            LocalFilesCoreAlbum fileCoreAlbum;
            LocalFilesCoreTrack filesCoreTrack;
            LocalFilesCoreArtist filesCoreArtist;

            lock (_lockObj)
            {
                if (e.AlbumMetadata != null)
                {
                    if (!_albumMetadatas?.Any(c => c.Title?.Equals(e.AlbumMetadata.Title ?? string.Empty) ?? false) ?? false)
                    {
                        fileCoreAlbum =
                            new LocalFilesCoreAlbum(SourceCore, e.AlbumMetadata, 1000); // track count is temporary

                        var addedItems = new List<CollectionChangedEventItem<ICoreAlbumCollectionItem>>
                        {
                            new CollectionChangedEventItem<ICoreAlbumCollectionItem>(fileCoreAlbum, 0),
                        };

                        _albumMetadatas?.Add(e.AlbumMetadata);
                        AlbumItemsChanged?.Invoke(this, addedItems, new List<CollectionChangedEventItem<ICoreAlbumCollectionItem>>()); // nothing is being removed for now.
                    }
                }

                if (e.ArtistMetadata != null)
                {
                    if (!_artistMetadatas?.Any(c => c.Name?.Equals(e.ArtistMetadata.Name ?? string.Empty) ?? false) ?? false)
                    {
                        filesCoreArtist =
                            new LocalFilesCoreArtist(SourceCore, e.ArtistMetadata, 1000); // track count is temporary

                        var addedItems = new List<CollectionChangedEventItem<ICoreArtistCollectionItem>>
                        {
                            new CollectionChangedEventItem<ICoreArtistCollectionItem>(filesCoreArtist, 0),
                        };

                        _artistMetadatas?.Add(e.ArtistMetadata);
                        ArtistItemsChanged?.Invoke(this, addedItems, new List<CollectionChangedEventItem<ICoreArtistCollectionItem>>());
                    }
                }

                if (e.TrackMetadata != null)
                {
                    if (!_trackMetadatas?.Any(c => c.Title?.Contains(e.TrackMetadata.Title ?? string.Empty) ?? false) ?? false)
                    {
                        filesCoreTrack = new LocalFilesCoreTrack(SourceCore, e.TrackMetadata);

                        var addedItems = new List<CollectionChangedEventItem<ICoreTrack>>
                        {
                            new CollectionChangedEventItem<ICoreTrack>(filesCoreTrack, 0),
                        };

                        _trackMetadatas?.Add(e.TrackMetadata);
                        TrackItemsChanged?.Invoke(this, addedItems, new List<CollectionChangedEventItem<ICoreTrack>>()); // nothing is being removed for now.
                    }
                }
            }
        }
    }
}
