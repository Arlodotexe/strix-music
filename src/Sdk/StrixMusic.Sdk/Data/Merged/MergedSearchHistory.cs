using System.Collections.Generic;
using System.Linq;
using StrixMusic.Sdk.Data.Core;

namespace StrixMusic.Sdk.Data.Merged
{
    /// <summary>
    /// A concrete class that merges multiple <see cref="ICoreSearchHistory"/>.
    /// </summary>
    public class MergedSearchHistory : MergedPlayableCollectionGroupBase<ICoreSearchHistory>, ISearchHistory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MergedSearchHistory"/> class.
        /// </summary>
        /// <param name="searchHistories">The search histories to merge.</param>
        public MergedSearchHistory(IEnumerable<ICoreSearchHistory> searchHistories)
            : base(searchHistories.ToArray())
        {
        }

        /// <inheritdoc />
        IReadOnlyList<ICoreSearchHistory> IMerged<ICoreSearchHistory>.Sources => StoredSources;

        /// <inheritdoc cref="Equals(object?)" />
        public override bool Equals(ICoreSearchHistory? other)
        {
            // Search histories should always be merged together.
            return true;
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj) || (obj is ICoreSearchHistory other && Equals(other));
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return PreferredSource.GetHashCode();
        }
    }
}