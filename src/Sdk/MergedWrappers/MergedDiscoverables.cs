using System;
using System.Collections.Generic;
using System.Linq;
using StrixMusic.Sdk.Interfaces;

namespace StrixMusic.Sdk.MergedWrappers
{
    /// <summary>
    /// A concrete class that merges multiple <see cref="IDiscoverables"/>.
    /// </summary>
    public class MergedDiscoverables : MergedPlayableCollectionGroupBase, IDiscoverables, IEquatable<IDiscoverables>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MergedDiscoverables"/> class.
        /// </summary>
        /// <param name="source">The <see cref="IDiscoverables"/> objects to merge.</param>
        public MergedDiscoverables(IEnumerable<IDiscoverables> source)
            : base(source.ToArray<IPlayableCollectionGroup>())
        {
        }

        /// <inheritdoc cref="Equals(object?)" />
        public bool Equals(IDiscoverables? other)
        {
            return other?.Name == Name;
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj) || (obj is IDiscoverables other && Equals(other));
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return PreferredSource.GetHashCode();
        }
    }
}