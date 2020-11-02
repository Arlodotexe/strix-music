using System;
using System.Collections.Generic;
using StrixMusic.Sdk.Data.Base;
using StrixMusic.Sdk.Data.Core;

namespace StrixMusic.Sdk.Data.Merged
{
    /// <summary>
    /// A concrete class that merges multiple <see cref="IPlayableCollectionGroupBase"/>s.
    /// </summary>
    public class MergedPlayableCollectionGroup : MergedPlayableCollectionGroupBase<ICorePlayableCollectionGroup>, IEquatable<IPlayableCollectionGroupBase>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MergedPlayableCollectionGroup"/> class.
        /// </summary>
        /// <param name="source"></param>
        public MergedPlayableCollectionGroup(IReadOnlyList<ICorePlayableCollectionGroup> source)
            : base(source)
        {
        }

        /// <inheritdoc/>
        public bool Equals(IPlayableCollectionGroupBase? other)
        {
            return other?.Name == Name;
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj) || (obj is IPlayableCollectionGroupBase other && Equals(other));
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return PreferredSource.GetHashCode();
        }
    }
}