using System;
using System.Collections.Generic;
using StrixMusic.Sdk.Interfaces;

namespace StrixMusic.Sdk.MergedWrappers
{
    /// <summary>
    /// A concrete class that merges multiple <see cref="IPlayableCollectionGroup"/>s.
    /// </summary>
    public class MergedPlayableCollectionGroup : MergedPlayableCollectionGroupBase, IEquatable<IPlayableCollectionGroup>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MergedPlayableCollectionGroup"/> class.
        /// </summary>
        /// <param name="source"></param>
        public MergedPlayableCollectionGroup(IReadOnlyList<IPlayableCollectionGroup> source)
            : base(source)
        {
        }

        /// <inheritdoc/>
        public bool Equals(IPlayableCollectionGroup? other)
        {
            return other?.Name == Name;
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj) || (obj is IPlayableCollectionGroup other && Equals(other));
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return PreferredSource.GetHashCode();
        }
    }
}