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
    {
        /// <summary>
        /// Adds a new source to this merged item.
        /// </summary>
        /// <param name="itemToMerge"></param>
        void AddSource(T itemToMerge);

        /// <summary>
        /// The sources that make up this merged item.
        /// </summary>
        IReadOnlyList<T> Sources { get; }
    }

    /// <summary>
    /// helpers for merging together 
    /// </summary>
    public static partial class MergedHelpers
    {
        /// <summary>
        /// Takes a list of <typeparamref name="TCoreMember"/> and makes items distinct by merging items together.
        /// </summary>
        /// <typeparam name="TCoreMember">The type of the member to merge together.</typeparam>
        /// <param name="sources">The items to merge</param>
        /// <returns>A list of distinct, merged items.</returns>
        public static IEnumerable<ISdkMember<TCoreMember>> UnionAndMerge<TCoreMember>(this IEnumerable<TCoreMember> sources)
            where TCoreMember : ICoreMember
        {

        }
    }
}