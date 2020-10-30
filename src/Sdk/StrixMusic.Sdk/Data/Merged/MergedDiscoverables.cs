using System;
using System.Collections.Generic;
using System.Linq;
using StrixMusic.Sdk.Core.Data;

namespace StrixMusic.Sdk.Core.Merged
{
    /// <summary>
    /// A concrete class that merges multiple <see cref="ICoreDiscoverables"/>.
    /// </summary>
    public class MergedDiscoverables : MergedPlayableCollectionGroupBase, ICoreDiscoverables, IEquatable<ICoreDiscoverables>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MergedDiscoverables"/> class.
        /// </summary>
        /// <param name="source">The <see cref="ICoreDiscoverables"/> objects to merge.</param>
        public MergedDiscoverables(IEnumerable<ICoreDiscoverables> source)
            : base(source.ToArray<IPlayableCollectionGroup>())
        {
        }

        /// <inheritdoc cref="Equals(object?)" />
        public bool Equals(ICoreDiscoverables? other)
        {
            return other?.Name == Name;
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj) || (obj is ICoreDiscoverables other && Equals(other));
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return PreferredSource.GetHashCode();
        }
    }
}