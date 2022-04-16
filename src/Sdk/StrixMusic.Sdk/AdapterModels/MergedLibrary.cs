// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System.Collections.Generic;
using StrixMusic.Sdk.Models;
using StrixMusic.Sdk.Models.Base;
using StrixMusic.Sdk.Models.Core;

namespace StrixMusic.Sdk.AdapterModels
{
    /// <summary>
    /// A concrete class that merges multiple <see cref="ILibraryBase"/>.
    /// </summary>
    public class MergedLibrary : MergedPlayableCollectionGroupBase<ICoreLibrary>, ILibrary
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MergedLibrary"/> class.
        /// </summary>
        public MergedLibrary(IEnumerable<ICoreLibrary> sources, MergedCollectionConfig config)
            : base(sources, config)
        {
        }

        /// <inheritdoc cref="IMerged{T}.Sources"/>
        IReadOnlyList<ICoreLibrary> IMerged<ICoreLibrary>.Sources => StoredSources;

        /// <inheritdoc cref="Equals(object?)" />
        public override bool Equals(ICoreLibrary? other)
        {
            return other?.Name == Name;
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj) || (obj is ICoreLibrary other && Equals(other));
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return PreferredSource.GetHashCode();
        }
    }
}
