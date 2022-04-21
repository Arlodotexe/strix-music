// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OwlCore.Events;
using StrixMusic.Sdk.AdapterModels;
using StrixMusic.Sdk.BaseModels;
using StrixMusic.Sdk.CoreModels;

namespace StrixMusic.Sdk.AppModels
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
        Task PlayTrackCollectionAsync(ITrack track, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a requested number of <see cref="ITrack"/>s starting at the given offset in the backend.
        /// </summary>
        /// <param name="limit">The max number of items to return.</param>
        /// <param name="offset">Get items starting at this index.</param>
        /// <param name="cancellationToken">A cancellation token that may be used to cancel the ongoing task.</param>
        /// <returns><see cref="IReadOnlyList{T}"/> containing the requested items.</returns>
        Task<IReadOnlyList<ITrack>> GetTracksAsync(int limit, int offset, CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds an <see cref="ITrack"/> to this track collection.
        /// </summary>
        /// <param name="track">The track to add.</param>
        /// <param name="index">the position to insert the track at.</param>
        /// <param name="cancellationToken">A cancellation token that may be used to cancel the ongoing task.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task AddTrackAsync(ITrack track, int index, CancellationToken cancellationToken = default);

        /// <summary>
        /// Fires when the items in the backend are changed by something external.
        /// </summary>
        event CollectionChangedEventHandler<ITrack>? TracksChanged;
    }
}
