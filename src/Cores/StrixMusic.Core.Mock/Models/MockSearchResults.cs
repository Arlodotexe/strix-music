using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using StrixMusic.Core.Mock.Deserialization;
using StrixMusic.CoreInterfaces;
using StrixMusic.CoreInterfaces.Enums;
using StrixMusic.CoreInterfaces.Interfaces;

namespace StrixMusic.Core.Mock.Models
{
    /// <inheritdoc/>
    public class MockSearchResults : MockPlayableCollectionGroupBase, ISearchResults
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MockSearchResults"/> class.
        /// </summary>
        /// <param name="sourceCore"></param>
        /// <param name="playableCollectionGroup"></param>
        public MockSearchResults(ICore sourceCore)
            : base(sourceCore)
        {
        }

        /// <inheritdoc/>
        public override Task ChangeDescriptionAsync(string? description)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override Task ChangeDurationAsync(TimeSpan duration)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override Task ChangeImagesAsync(IReadOnlyList<IImage> images)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override Task ChangeNameAsync(string name)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override Task PauseAsync()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override Task PlayAsync()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override Task<IReadOnlyList<IAlbum>> PopulateAlbumsAsync(int limit, int offset = 0)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override Task<IReadOnlyList<IArtist>> PopulateArtistsAsync(int limit, int offset = 0)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override Task<IReadOnlyList<IPlayableCollectionGroup>> PopulateChildrenAsync(int limit, int offset = 0)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override Task<IReadOnlyList<IPlaylist>> PopulatePlaylistsAsync(int limit, int offset = 0)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override Task<IReadOnlyList<ITrack>> PopulateTracksAsync(int limit, int offset = 0)
        {
            throw new NotImplementedException();
        }
    }
}
