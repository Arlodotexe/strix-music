﻿using System;
using System.Threading.Tasks;

namespace StrixMusic.Sdk.Models.Base
{
    /// <summary>
    /// A collection of <see cref="IPlayableCollectionGroupBase"/>s and the properties and methods for using and manipulating them.
    /// </summary>
    public interface IPlayableCollectionGroupChildrenBase : IPlayableCollectionBase, IAsyncDisposable
    {
        /// <summary>
        /// Attempts to play the playable collection. Resumes if paused.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task PlayPlayableCollectionGroupAsync();

        /// <summary>
        /// Attempts to play the playable collection. Resumes if paused.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task PausePlayableCollectionGroupAsync();

        /// <summary>
        /// The total number of available Children.
        /// </summary>
        int TotalChildrenCount { get; }

        /// <summary>
        /// Removes the child from the collection on the backend.
        /// </summary>
        /// <param name="index">The index of the child to remove.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task RemoveChildAsync(int index);

        /// <summary>
        /// Checks if the backend supports adding an <see cref="IPlayableCollectionGroupBase"/> at a specific index.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. If value is true, an item can be added.</returns>
        Task<bool> IsAddChildAvailableAsync(int index);

        /// <summary>
        /// Checks if the backend supports removing an <see cref="IPlayableCollectionGroupBase"/> at a specific index.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. If value is true, the item can be removed.</returns>
        Task<bool> IsRemoveChildAvailableAsync(int index);

        /// <summary>
        /// Fires when the merged <see cref="TotalChildrenCount"/> changes.
        /// </summary>
        event EventHandler<int>? ChildrenCountChanged;
    }
}