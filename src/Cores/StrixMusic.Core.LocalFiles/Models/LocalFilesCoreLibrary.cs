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
        private readonly IList<ArtistMetadata> _artistMetadatas;
        private readonly IList<AlbumMetadata> _albumMetadatas;
        private readonly IList<TrackMetadata> _trackMetadatas;

        private readonly object _lockObj = new object();
        private IFileMetadataManager? _fileMetadataManager;

        private Dictionary<TrackMetadata, LocalFilesCoreTrack> _metadataCoreTrackDictionary;
        private Dictionary<AlbumMetadata, LocalFilesCoreAlbum> _metadataCoreAlbumDictionary;
        private Dictionary<ArtistMetadata, LocalFilesCoreArtist> _metadataCoreArtistDictionary;

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
            _metadataCoreTrackDictionary = new Dictionary<TrackMetadata, LocalFilesCoreTrack>();
            _metadataCoreAlbumDictionary = new Dictionary<AlbumMetadata, LocalFilesCoreAlbum>();
            _metadataCoreArtistDictionary = new Dictionary<ArtistMetadata, LocalFilesCoreArtist>();
        }

        /// <inheritdoc/>
        public override async Task InitAsync()
        {
            _fileMetadataManager = SourceCore.GetService<IFileMetadataManager>();

            _fileMetadataManager.FileMetadataAdded += MetadataScannerFileMetadataAdded;
            _fileMetadataManager.FileMetadataUpdated += MetadataScannerFileMetadataUpdated;

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

        private void MetadataScannerFileMetadataUpdated(object sender, FileMetadata e)
        {
            lock (_metadataCoreTrackDictionary)
            {
                if (e.TrackMetadata != null)
                {
                    var targetTrackMetadata = _trackMetadatas?.FirstOrDefault(c => c.Title?.Equals(e.TrackMetadata.Title, StringComparison.OrdinalIgnoreCase) ?? false);

                    if (targetTrackMetadata != null)
                    {
                        if (_metadataCoreTrackDictionary.Count > 0)
                            _metadataCoreTrackDictionary[targetTrackMetadata].ChangeTotalArtistCount(targetTrackMetadata.ArtistIds.Count());
                    }
                }
            }

            lock (_metadataCoreAlbumDictionary)
            {
                if (e.AlbumMetadata != null)
                {
                    var targetAlbumMetadata = _albumMetadatas?.FirstOrDefault(c => c.Title?.Equals(e.AlbumMetadata.Title, StringComparison.OrdinalIgnoreCase) ?? false);

                    if (targetAlbumMetadata != null)
                    {
                        if (_metadataCoreAlbumDictionary.Count > 0)
                        {
                            _metadataCoreAlbumDictionary[targetAlbumMetadata].ChangeTotalArtistCount(targetAlbumMetadata.ArtistIds.Count());
                            _metadataCoreAlbumDictionary[targetAlbumMetadata].ChangeTotalTrackCount(targetAlbumMetadata.TrackIds.Count());
                        }
                    }
                }
            }

            lock (_metadataCoreArtistDictionary)
            {
                if (e.ArtistMetadata != null)
                {
                    var targetArtistMetadata = _artistMetadatas?.FirstOrDefault(c => c.Name?.Equals(e.ArtistMetadata.Name, StringComparison.OrdinalIgnoreCase) ?? false);

                    if (targetArtistMetadata != null)
                    {
                        if (_metadataCoreArtistDictionary.Count > 0)
                        {
                            _metadataCoreArtistDictionary[targetArtistMetadata].ChangeTotalTrackCount(targetArtistMetadata.TrackIds.Count());
                        }
                    }
                }
            }
        }

        private void MetadataScannerFileMetadataAdded(object sender, FileMetadata e)
        {
            HandleAlbumMetadataAdded(e);

            HandleArtistMetadataAdded(e);

            HandleAddedTrackMetadata(e);
        }

        private void HandleAddedTrackMetadata(FileMetadata e)
        {
            lock (_metadataCoreTrackDictionary)
            {
                if (e.TrackMetadata is null || (_trackMetadatas?.Any(c => c.Title?.Contains(e.TrackMetadata.Title ?? string.Empty) ?? false) ?? false))
                    return;

                var filesCoreTrack = new LocalFilesCoreTrack(SourceCore, e.TrackMetadata);

                var addedItems = new List<CollectionChangedEventItem<ICoreTrack>>
                {
                    new CollectionChangedEventItem<ICoreTrack>(filesCoreTrack, 0),
                };

                if (_trackMetadatas == null)
                    return;

                _trackMetadatas?.Add(e.TrackMetadata);
                _metadataCoreTrackDictionary.Add(e.TrackMetadata, filesCoreTrack);

                TrackItemsChanged?.Invoke(this, addedItems, new List<CollectionChangedEventItem<ICoreTrack>>());
            }
        }

        private void HandleArtistMetadataAdded(FileMetadata e)
        {
            lock (_metadataCoreArtistDictionary)
            {
                if (e.ArtistMetadata == null)
                    return;

                if (!(!_artistMetadatas?.Any(c => c.Name?.Equals(e.ArtistMetadata.Name ?? string.Empty) ?? false) ??
                      false))
                    return;

                var filesCoreArtist =
                    new LocalFilesCoreArtist(SourceCore, e.ArtistMetadata, e.ArtistMetadata.TrackIds?.Count ?? 0);

                var addedItems = new List<CollectionChangedEventItem<ICoreArtistCollectionItem>>
                {
                    new CollectionChangedEventItem<ICoreArtistCollectionItem>(filesCoreArtist, 0),
                };

                if (_artistMetadatas == null)
                    return;
                _artistMetadatas?.Add(e.ArtistMetadata);
                _metadataCoreArtistDictionary.Add(e.ArtistMetadata, filesCoreArtist);
                ArtistItemsChanged?.Invoke(this, addedItems, new List<CollectionChangedEventItem<ICoreArtistCollectionItem>>());
            }
        }

        private void HandleAlbumMetadataAdded(FileMetadata e)
        {
            lock (_metadataCoreAlbumDictionary)
            {
                foreach (var album in _albumMetadatas)
                {
                    if (album.Title == e.AlbumMetadata?.Title)
                        return;
                }

                Guard.IsNotNull(e.AlbumMetadata, nameof(e.AlbumMetadata));
                Guard.IsNotNull(_fileMetadataManager, nameof(_fileMetadataManager));
                Guard.IsNotNull(e.AlbumMetadata.Id, nameof(e.AlbumMetadata.Id));

                var track = e.TrackMetadata;

                var fileCoreAlbum = new LocalFilesCoreAlbum(SourceCore, e.AlbumMetadata, e.AlbumMetadata.TrackIds?.Count ?? 0, track?.ImagePath != null ? new LocalFilesCoreImage(SourceCore, track.ImagePath) : null);

                var addedItems = new List<CollectionChangedEventItem<ICoreAlbumCollectionItem>>
                {
                    new CollectionChangedEventItem<ICoreAlbumCollectionItem>(fileCoreAlbum, 0),
                };
                _albumMetadatas.Add(e.AlbumMetadata);
                _metadataCoreAlbumDictionary.Add(e.AlbumMetadata, fileCoreAlbum);

                AlbumItemsChanged?.Invoke(this, addedItems, new List<CollectionChangedEventItem<ICoreAlbumCollectionItem>>());
            }
        }
    }
}
