using System.Collections.Generic;
using System.Threading.Tasks;
using OwlCore.Events;
using StrixMusic.Sdk.Models.Base;
using StrixMusic.Sdk.Models.Core;
using StrixMusic.Sdk.Models.Merged;

namespace StrixMusic.Sdk.Models
{
    /// <summary>
    /// A collection of <see cref="ITrack"/>s and the properties and methods for using and manipulating them.
    /// </summary>
    /// <remarks>Instances of this class may contain data merged from one or more sources.</remarks>
    public interface ITrackCollection : ITrackCollectionBase, IImageCollection, IUrlCollection, IPlayable, ISdkMember, IMerged<ICoreTrackCollection>
    {
        /// <summary>
        /// Attempts to play a specific item in the track collection. Restarts playback if already playing.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task PlayTrackCollectionAsync(ITrack track);

        /// <summary>
        /// Gets a requested number of <see cref="ITrack"/>s starting at the given offset in the backend.
        /// </summary>
        /// <param name="limit">The max number of items to return.</param>
        /// <param name="offset">Get items starting at this index.</param>
        /// <returns><see cref="IReadOnlyList{T}"/> containing the requested items.</returns>
        Task<IReadOnlyList<ITrack>> GetTracksAsync(int limit, int offset);

        /// <summary>
        /// Adds a new track to the collection on the backend.
        /// </summary>
        /// <param name="track">The track to create.</param>
        /// <param name="index">the position to insert the track at.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task AddTrackAsync(ITrack track, int index);

        /// <summary>
        /// Fires when the items in the backend are changed by something external.
        /// </summary>
        event CollectionChangedEventHandler<ITrack>? TracksChanged;
    }
}