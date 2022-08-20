// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System;
using System.Threading;
using System.Threading.Tasks;

namespace StrixMusic.Sdk.BaseModels
{
    /// <summary>
    /// A collection of <see cref="IPlaylistCollectionItemBase"/>s and the properties and methods for using and manipulating them.
    /// </summary>
    public interface IPlaylistCollectionBase : IPlayableCollectionItem, IPlaylistCollectionItemBase
    {
        /// <summary>
        /// The total number of available Playlists.
        /// </summary>
        int TotalPlaylistItemsCount { get; }

        /// <summary>
        /// If true, <see cref="PlayPlaylistCollectionAsync(CancellationToken)"/> can be used.
        /// </summary>
        bool IsPlayPlaylistCollectionAsyncAvailable { get; }

        /// <summary>
        /// If true, <see cref="PausePlaylistCollectionAsync(CancellationToken)"/> can be used.
        /// </summary>
        bool IsPausePlaylistCollectionAsyncAvailable { get; }

        /// <summary>
        /// Attempts to play the Playlist collection, or resumes playback if already playing.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that may be used to cancel the ongoing task.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task PlayPlaylistCollectionAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Attempts to pause the Playlist collection.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that may be used to cancel the ongoing task.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task PausePlaylistCollectionAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Removes the playlist from the collection on the backend.
        /// </summary>
        /// <param name="index">The index of the playlist to remove.</param>
        /// <param name="cancellationToken">A cancellation token that may be used to cancel the ongoing task.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task RemovePlaylistItemAsync(int index, CancellationToken cancellationToken = default);

        /// <summary>
        /// Checks if the backend supports adding an <see cref="IPlaylistCollectionItemBase"/> at a specific index.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. If value is true, a new <see cref="IPlaylistCollectionItemBase"/> can be added.</returns>
        Task<bool> IsAddPlaylistItemAvailableAsync(int index, CancellationToken cancellationToken = default);

        /// <summary>
        /// Checks if the backend supports removing an <see cref="IPlaylistCollectionItemBase"/> at a specific index.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. If value is true, the <see cref="IPlaylistCollectionItemBase"/> can be removed.</returns>
        Task<bool> IsRemovePlaylistItemAvailableAsync(int index, CancellationToken cancellationToken = default);

        /// <summary>
        /// Raised when <see cref="IsPlayPlaylistCollectionAsyncAvailable"/> changes.
        /// </summary>
        event EventHandler<bool>? IsPlayPlaylistCollectionAsyncAvailableChanged;

        /// <summary>
        /// Raised when <see cref="IsPausePlaylistCollectionAsyncAvailable"/> changes.
        /// </summary>
        event EventHandler<bool>? IsPausePlaylistCollectionAsyncAvailableChanged;

        /// <summary>
        /// Fires when the merged <see cref="TotalPlaylistItemsCount"/> changes.
        /// </summary>
        event EventHandler<int>? PlaylistItemsCountChanged;
    }
}
