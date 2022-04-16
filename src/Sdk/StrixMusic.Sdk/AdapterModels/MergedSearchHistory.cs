// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System.Collections.Generic;
using StrixMusic.Sdk.Models;
using StrixMusic.Sdk.Models.Core;

namespace StrixMusic.Sdk.AdapterModels
{
    /// <summary>
    /// A concrete class that merges multiple <see cref="ICoreSearchHistory"/>.
    /// </summary>
    public sealed class MergedSearchHistory : MergedPlayableCollectionGroupBase<ICoreSearchHistory>, ISearchHistory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MergedSearchHistory"/> class.
        /// </summary>
        public MergedSearchHistory(IEnumerable<ICoreSearchHistory> searchHistories, MergedCollectionConfig config)
            : base(searchHistories, config)
        {
        }

        /// <inheritdoc />
        IReadOnlyList<ICoreSearchHistory> IMerged<ICoreSearchHistory>.Sources => StoredSources;

        /// <inheritdoc cref="Equals(object?)" />
        public override bool Equals(ICoreSearchHistory? other)
        {
            // Search history should always be merged together.
            return true;
        }

        /// <inheritdoc />
        public override bool Equals(object? obj) => ReferenceEquals(this, obj) || (obj is ICoreSearchHistory other && Equals(other));

        /// <inheritdoc />
        public override int GetHashCode() => PreferredSource.GetHashCode();
    }
}
