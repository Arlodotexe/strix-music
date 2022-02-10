using StrixMusic.Sdk.Models.Core;

namespace StrixMusic.Sdk.Models.Merged
{
    /// <summary>
    /// Provides accessors for modifying the sources in a <see cref="IMerged{T}"/> class.
    /// </summary>
    /// <typeparam name="T">The type that makes up this merged item.</typeparam>
    internal interface IMergedMutable<T>
        where T : ICoreMember
    {
        /// <summary>
        /// Adds a new source to this merged item.
        /// </summary>
        /// <param name="itemToMerge">The source to remove.</param>
        internal void AddSource(T itemToMerge);

        /// <summary>
        /// Removes a source from the merged collection.
        /// </summary>
        /// <param name="itemToRemove">The source to remove.</param>
        internal void RemoveSource(T itemToRemove);
    }
}