using Microsoft.Toolkit.Diagnostics;
using StrixMusic.Core.LocalFiles.Backing.Services;
using StrixMusic.Core.LocalFiles.Models;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StrixMusic.Core.LocalFiles.Models
{
    /// <inheritdoc/>
    public class LocalFilesCoreLibrary : LocalFilesCorePlayableCollectionGroupBase, ICoreLibrary
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LocalFilesCoreLibrary"/> class.
        /// </summary>
        /// <param name="sourceCore"></param>
        public LocalFilesCoreLibrary(ICore sourceCore)
            : base(sourceCore)
        {
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
        public override int TotalChildrenCount { get; internal set; } = 0;

        /// <inheritdoc />
        public override int TotalArtistItemsCount { get; internal set; }

        /// <inheritdoc />
        public override int TotalAlbumItemsCount { get; internal set; }

        /// <inheritdoc />
        public override int TotalPlaylistItemsCount { get; internal set; } = 0;

        /// <inheritdoc />
        public override int TotalTracksCount { get; internal set; }

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
        public override IAsyncEnumerable<ICoreAlbumCollectionItem> GetAlbumItemsAsync(int limit, int offset)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public async override IAsyncEnumerable<ICoreArtistCollectionItem> GetArtistItemsAsync(int limit, int offset)
        {
            var artistService = SourceCore.GetService<ArtistService>();

            var artists = await artistService.GetArtistMetadata(limit, offset);

            foreach (var artist in artists)
            {
                yield return new LocalFilesCoreArtist(SourceCore, artist);
            }
        }

        /// <inheritdoc/>
        public override IAsyncEnumerable<ICoreTrack> GetTracksAsync(int limit, int offset = 0)
        {
            throw new NotImplementedException();
        }
    }
}
