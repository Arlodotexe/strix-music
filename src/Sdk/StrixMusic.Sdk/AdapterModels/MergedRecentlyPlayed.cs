﻿// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System.Collections.Generic;
using StrixMusic.Sdk.AppModels;
using StrixMusic.Sdk.CoreModels;

namespace StrixMusic.Sdk.AdapterModels
{
    /// <summary>
    /// A concrete class that merges multiple <see cref="ICoreRecentlyPlayed"/>.
    /// </summary>
    public class MergedRecentlyPlayed : MergedPlayableCollectionGroupBase<ICoreRecentlyPlayed>, IRecentlyPlayed
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MergedRecentlyPlayed"/> class.
        /// </summary>
        public MergedRecentlyPlayed(IEnumerable<ICoreRecentlyPlayed> sources, MergedCollectionConfig config)
            : base(sources, config)
        {
        }

        /// <inheritdoc />
        IReadOnlyList<ICoreRecentlyPlayed> IMerged<ICoreRecentlyPlayed>.Sources => StoredSources;

        /// <inheritdoc cref="Equals(object?)" />
        public override bool Equals(ICoreRecentlyPlayed? other) => other?.Name == Name;

        /// <inheritdoc />
        public override bool Equals(object? obj) => ReferenceEquals(this, obj) || (obj is ICoreRecentlyPlayed other && Equals(other));

        /// <inheritdoc />
        public override int GetHashCode() => PreferredSource.GetHashCode();
    }
}
