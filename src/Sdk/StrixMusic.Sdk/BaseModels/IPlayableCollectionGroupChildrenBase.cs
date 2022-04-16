// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System;
using System.Threading;
using System.Threading.Tasks;

namespace StrixMusic.Sdk.BaseModels
{
    /// <summary>
    /// A collection of <see cref="IPlayableCollectionGroupBase"/>s and the properties and methods for using and manipulating them.
    /// </summary>
    public interface IPlayableCollectionGroupChildrenBase : IPlayableCollectionBase, IAsyncDisposable
    {
        /// <summary>
        /// Attempts to play the playable collection. Resumes if paused.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that may be used to cancel the ongoing task.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task PlayPlayableCollectionGroupAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Attempts to play the playable collection. Resumes if paused.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that may be used to cancel the ongoing task.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task PausePlayableCollectionGroupAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// The total number of available Children.
        /// </summary>
        int TotalChildrenCount { get; }

        /// <summary>
        /// Removes the child from the collection on the backend.
        /// </summary>
        /// <param name="index">The index of the child to remove.</param>
        /// <param name="cancellationToken">A cancellation token that may be used to cancel the ongoing task.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task RemoveChildAsync(int index, CancellationToken cancellationToken = default);

        /// <summary>
        /// Checks if the backend supports adding an <see cref="IPlayableCollectionGroupBase"/> at a specific index.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. If value is true, an item can be added.</returns>
        Task<bool> IsAddChildAvailableAsync(int index, CancellationToken cancellationToken = default);

        /// <summary>
        /// Checks if the backend supports removing an <see cref="IPlayableCollectionGroupBase"/> at a specific index.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. If value is true, the item can be removed.</returns>
        Task<bool> IsRemoveChildAvailableAsync(int index, CancellationToken cancellationToken = default);

        /// <summary>
        /// Fires when the merged <see cref="TotalChildrenCount"/> changes.
        /// </summary>
        event EventHandler<int>? ChildrenCountChanged;
    }
}
