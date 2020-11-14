using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using StrixMusic.Sdk.Data.Base;
using StrixMusic.Sdk.Data.Core;

namespace StrixMusic.Sdk.Data
{
    /// <inheritdoc cref="ITrackCollectionBase"/>
    /// <remarks>This interface should be implemented by a core.</remarks>
    [SuppressMessage("ReSharper", "PossibleInterfaceMemberAmbiguity", Justification = "Ambiguity is handled")]
    public interface ITrackCollection : ITrackCollectionBase, IImageCollection, ISdkMember<ICoreTrackCollection>
    {
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
        /// Fires when the merged <see cref="ITrackCollectionBase.TotalTracksCount"/> changes.
        /// </summary>
        event EventHandler<int> TrackItemsCountChanged;
    }
}