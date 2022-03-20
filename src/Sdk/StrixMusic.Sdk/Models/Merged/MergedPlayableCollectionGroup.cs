// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

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
        public MergedPlayableCollectionGroup(IEnumerable<ICorePlayableCollectionGroup> sources, MergedCollectionConfig config)
            : base(sources, config)
        {
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj) => ReferenceEquals(this, obj) || (obj is ICorePlayableCollectionGroup other && Equals(other));

        /// <inheritdoc/>
        public override int GetHashCode() => PreferredSource.GetHashCode();
    }
}
