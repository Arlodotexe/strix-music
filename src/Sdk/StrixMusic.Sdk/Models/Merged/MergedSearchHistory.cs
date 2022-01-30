using System.Collections.Generic;
using StrixMusic.Sdk.Models.Core;
using StrixMusic.Sdk.Services.Settings;

namespace StrixMusic.Sdk.Models.Merged
{
    /// <summary>
    /// A concrete class that merges multiple <see cref="ICoreSearchHistory"/>.
    /// </summary>
    public sealed class MergedSearchHistory : MergedPlayableCollectionGroupBase<ICoreSearchHistory>, ISearchHistory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MergedSearchHistory"/> class.
        /// </summary>
        public MergedSearchHistory(IEnumerable<ICoreSearchHistory> searchHistories, ISettingsService settingsService)
            : base(searchHistories, settingsService)
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