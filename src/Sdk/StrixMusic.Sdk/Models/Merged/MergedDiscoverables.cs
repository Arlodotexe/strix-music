using System.Collections.Generic;
using StrixMusic.Sdk.Models.Core;
using StrixMusic.Sdk.Services.Settings;

namespace StrixMusic.Sdk.Models.Merged
{
    /// <summary>
    /// A concrete class that merges multiple <see cref="ICoreDiscoverables"/>.
    /// </summary>
    public class MergedDiscoverables : MergedPlayableCollectionGroupBase<ICoreDiscoverables>, IDiscoverables
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MergedDiscoverables"/> class.
        /// </summary>
        public MergedDiscoverables(IEnumerable<ICoreDiscoverables> sources, ISettingsService settingsService)
            : base(sources, settingsService)
        {
        }

        /// <inheritdoc />
        IReadOnlyList<ICoreDiscoverables> IMerged<ICoreDiscoverables>.Sources => StoredSources;

        /// <inheritdoc cref="Equals(object?)" />
        public override bool Equals(ICoreDiscoverables? other)
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