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
            _artistMetadatas = new List<ArtistMetadata>();
            _trackMetadatas = new List<TrackMetadata>();
            _albumMetadatas = new List<AlbumMetadata>();
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
                yield return new LocalFilesCoreAlbum(SourceCore, album, album.TrackIds?.Count ?? 0);
            }
        }

        /// <inheritdoc/>
        public override async IAsyncEnumerable<ICoreArtistCollectionItem> GetArtistItemsAsync(int limit, int offset)
        {
            Guard.IsNotNull(_fileMetadataManager, nameof(_fileMetadataManager));

            var artistMetadata = await _fileMetadataManager.Artists.GetArtistMetadata(offset, limit);

            foreach (var artist in artistMetadata)
            {
                yield return new LocalFilesCoreArtist(SourceCore, artist, artist.TrackIds?.Count ?? 0);
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
                            new LocalFilesCoreAlbum(SourceCore, e.AlbumMetadata, e.AlbumMetadata.TrackIds?.Count ?? 0); 

                        var addedItems = new List<CollectionChangedEventItem<ICoreAlbumCollectionItem>>
                        {
                            new CollectionChangedEventItem<ICoreAlbumCollectionItem>(fileCoreAlbum, 0),
                        };

                        _albumMetadatas?.Add(e.AlbumMetadata);
                        AlbumItemsChanged?.Invoke(this, addedItems, new List<CollectionChangedEventItem<ICoreAlbumCollectionItem>>());
                    }
                }

                if (e.ArtistMetadata != null)
                {
                    if (!_artistMetadatas?.Any(c => c.Name?.Equals(e.ArtistMetadata.Name ?? string.Empty) ?? false) ?? false)
                    {
                        filesCoreArtist =
                            new LocalFilesCoreArtist(SourceCore, e.ArtistMetadata, e.ArtistMetadata.TrackIds?.Count ?? 0); 

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
                        TrackItemsChanged?.Invoke(this, addedItems, new List<CollectionChangedEventItem<ICoreTrack>>()); 
                    }
                }
            }
        }
    }
}
