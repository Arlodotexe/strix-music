using System;
using System.Collections.Generic;
using StrixMusic.Sdk.Data.Core;

namespace StrixMusic.Sdk.Data.Merged
{
    /// <summary>
    /// An item that has been merged.
    /// </summary>
    /// <typeparam name="T">The type that makes up this merged item.</typeparam>
    public interface IMerged<T> : IEquatable<T>
        where T : ICoreMember
    {
        /// <summary>
        /// Adds a new source to this merged item.
        /// </summary>
        /// <param name="itemToMerge">The source to remove.</param>
        void AddSource(T itemToMerge);

        /// <summary>
        /// Removes a source from the merged collection.
        /// </summary>
        /// <param name="itemToRemove">The source to remove.</param>
        void RemoveSource(T itemToRemove);

        /// <summary>
        /// The sources that make up this merged item.
        /// </summary>
        IReadOnlyList<T> Sources { get; }
    }
}