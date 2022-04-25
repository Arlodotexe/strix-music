// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System.Collections.Generic;
using StrixMusic.Sdk.AdapterModels;
using StrixMusic.Sdk.CoreModels;

namespace StrixMusic.Sdk.Extensions
{
    internal static partial class Merged
    {
        /// <summary>
        /// Syntax sugar for getting and casting the sources of an <see cref="IMerged{T}"/>.
        /// </summary>
        /// <typeparam name="TSource">The explicit source type to get sources from.</typeparam>
        /// <param name="merged">The <see cref="IMerged{T}"/> to operate on.</param>
        /// <returns>The sources of the given <see cref="IMerged{T}"/></returns>
        internal static IReadOnlyList<TSource> GetSources<TSource>(this IMerged<TSource> merged)
            where TSource : ICoreMember => merged.Sources;
    }
}
