using System.Collections.Generic;
using StrixMusic.Sdk.Models.Base;
using StrixMusic.Sdk.Models.Core;
using StrixMusic.Sdk.Services;

namespace StrixMusic.Sdk.Models.Merged
{
    /// <summary>
    /// A concrete class that merges multiple <see cref="IPlayableCollectionGroupBase"/>s.
    /// </summary>
    public class MergedPlayableCollectionGroup : MergedPlayableCollectionGroupBase<ICorePlayableCollectionGroup>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MergedPlayableCollectionGroup"/> class.
        /// </summary>
        public MergedPlayableCollectionGroup(IEnumerable<ICorePlayableCollectionGroup> sources, ISettingsService settingsService)
            : base(sources, settingsService)
        {
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj) || (obj is ICorePlayableCollectionGroup other && Equals(other));
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return PreferredSource.GetHashCode();
        }
    }
}