using System;
using System.Collections.Generic;
using System.Linq;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.Extensions;

namespace StrixMusic.Sdk.Data.Merged
{
    /// <summary>
    /// A concrete class that merges multiple <see cref="ICoreRecentlyPlayed"/>.
    /// </summary>
    public class MergedRecentlyPlayed : MergedPlayableCollectionGroupBase<ICoreRecentlyPlayed>, IRecentlyPlayed, IMerged<ICoreRecentlyPlayed>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MergedRecentlyPlayed"/> class.
        /// </summary>
        /// <param name="recentlyPlayed">The <see cref="ICoreRecentlyPlayed"/> objects to merge.</param>
        public MergedRecentlyPlayed(IEnumerable<ICoreRecentlyPlayed> recentlyPlayed)
            : base(recentlyPlayed.ToArray())
        {
        }

        /// <inheritdoc />
        IReadOnlyList<ICoreRecentlyPlayed> ISdkMember<ICoreRecentlyPlayed>.Sources => this.GetSources<ICoreRecentlyPlayed>();

        /// <inheritdoc cref="Equals(object?)" />
        public bool Equals(ICoreRecentlyPlayed? other)
        {
            return other?.Name == Name;
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj) || (obj is ICoreRecentlyPlayed other && Equals(other));
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return PreferredSource.GetHashCode();
        }
    }
}