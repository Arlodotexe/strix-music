// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OwlCore.Events;
using StrixMusic.Sdk.Models.Base;

namespace StrixMusic.Sdk.Models.Core
{
    /// <summary>
    /// A collection of <see cref="ICorePlaylistCollectionItem"/>s and the properties and methods for using and manipulating them.
    /// </summary>
    /// <remarks>This interface should be implemented by a core.</remarks>
    public interface ICorePlaylistCollection : ICorePlayableCollection, IPlaylistCollectionBase, ICoreImageCollection, ICoreUrlCollection, ICorePlaylistCollectionItem, ICoreMember
    {
        /// <summary>
        /// Attempts to play a specific item in the playlist collection. Restarts playback if already playing.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task PlayPlaylistCollectionAsync(ICorePlaylistCollectionItem playlistItem, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a requested number of <see cref="IPlaylistCollectionItemBase"/>s starting at the given offset in the backend.
        /// </summary>
        /// <param name="limit">The max number of items to return.</param>
        /// <param name="offset">Get items starting at this index.</param>
        /// <param name="cancellationToken">A cancellation token that may be used to cancel the ongoing task.</param>
        /// <returns><see cref="IAsyncEnumerable{T}"/> that returns the items as they're retrieved.</returns>
        IAsyncEnumerable<ICorePlaylistCollectionItem> GetPlaylistItemsAsync(int limit, int offset, CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds a new playlist to the collection on the backend.
        /// </summary>
        /// <param name="playlist">The playlist to create.</param>
        /// <param name="index">the position to insert the playlist at.</param>
        /// <param name="cancellationToken">A cancellation token that may be used to cancel the ongoing task.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task AddPlaylistItemAsync(ICorePlaylistCollectionItem playlist, int index, CancellationToken cancellationToken = default);

        /// <summary>
        /// Fires when the items in the backend are changed by something external.
        /// </summary>
        event CollectionChangedEventHandler<ICorePlaylistCollectionItem>? PlaylistItemsChanged;
    }
}
