using System.Collections.Generic;
using System.Threading.Tasks;
using StrixMusic.Sdk.Data.Base;

namespace StrixMusic.Sdk.Data.Core
{
    /// <inheritdoc cref="ITrackCollectionBase"/>
    /// <remarks>This interface should be implemented by a core.</remarks>
    public interface ICoreTrackCollection : ICorePlayableCollection, ITrackCollectionBase, ICoreImageCollection, ICoreMember
    {
        /// <summary>
        /// Gets a requested number of <see cref="ICoreTrack"/>s starting at the given offset in the backend.
        /// </summary>
        /// <param name="limit">The max number of items to return.</param>
        /// <param name="offset">Get items starting at this index.</param>
        /// <returns><see cref="IAsyncEnumerable{T}"/> that returns the items as they're retrieved.</returns>
        IAsyncEnumerable<ICoreTrack> GetTracksAsync(int limit, int offset);

        /// <summary>
        /// Adds a new track to the collection on the backend.
        /// </summary>
        /// <param name="track">The track to create.</param>
        /// <param name="index">the position to insert the track at.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task AddTrackAsync(ICoreTrack track, int index);
    }
}