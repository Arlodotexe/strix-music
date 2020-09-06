using System;
using System.Collections.Generic;
using System.Resources;
using System.Threading.Tasks;
using Newtonsoft.Json;
using StrixMusic.CoreInterfaces.Interfaces;

namespace StrixMusic.Core.Mock.Models
{
    /// <summary>
    /// The lists of tracks in the dummy core's library.
    /// </summary>
    public class MockLibrary : MockPlayableCollectionGroupBase, ILibrary
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MockLibrary"/> class.
        /// </summary>
        /// <param name="sourceCore"></param>
        public MockLibrary(ICore sourceCore)
            : base(sourceCore)
        {
            SourceCore = sourceCore;
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
