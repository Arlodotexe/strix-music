using Microsoft.Toolkit.Diagnostics;
using OwlCore.Provisos;
using StrixMusic.Core.LocalFiles.Backing.Models;
using StrixMusic.Core.LocalFiles.Backing.Services;
using StrixMusic.Core.LocalFiles.Models;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrixMusic.Core.LocalFiles.Models
{
    /// <inheritdoc/>
    public class LocalFilesCoreLibrary : LocalFilesCorePlayableCollectionGroupBase, ICoreLibrary, IAsyncInit
    {
        private LocalFilesCoreTrack _localFileCoreTrack;
        private IEnumerable<TrackMetadata> _trackMetadatas;
        private IEnumerable<AlbumMetadata> _albumMetadatas;
        private IEnumerable<ArtistMetadata> _artistMetadatas;

        private ArtistService _artistService;
        private TrackService _trackService;
        private AlbumService _albumService;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalFilesCoreLibrary"/> class.
        /// </summary>
        /// <param name="sourceCore"></param>
        public LocalFilesCoreLibrary(ICore sourceCore)
            : base(sourceCore)
        {
        }

        /// <inheritdoc/>
        public async Task InitAsync()
        {
            _trackService = SourceCore.GetService<TrackService>();
            _artistService = SourceCore.GetService<ArtistService>();
            _albumService = SourceCore.GetService<AlbumService>();

            _trackMetadatas = await _trackService.GetTrackMetadata(0, int.MaxValue);
            _albumMetadatas = await _albumService.GetAlbumMetadata(0, int.MaxValue);
            _artistMetadatas = await _artistService.GetArtistMetadata(0, int.MaxValue);

            TotalTracksCount = _trackMetadatas.Count();
            TotalArtistItemsCount = _artistMetadatas.Count();
            TotalAlbumItemsCount = _albumMetadatas.Count();
        }

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
        public bool IsInitialized => false;

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
        public async override IAsyncEnumerable<ICoreAlbumCollectionItem> GetAlbumItemsAsync(int limit, int offset)
        {
            foreach (var album in _albumMetadatas)
            {
                yield return new LocalFilesCoreAlbum(SourceCore, album);
            }
        }

        /// <inheritdoc/>
        public async override IAsyncEnumerable<ICoreArtistCollectionItem> GetArtistItemsAsync(int limit, int offset)
        {
            foreach (var artist in _artistMetadatas)
            {
                yield return new LocalFilesCoreArtist(SourceCore, artist);
            }
        }

        /// <inheritdoc/>
        public async override IAsyncEnumerable<ICoreTrack> GetTracksAsync(int limit, int offset = 0)
        {
            foreach (var track in _trackMetadatas)
            {
                yield return new LocalFilesCoreTrack(SourceCore, track);
            }
        }
    }
}
