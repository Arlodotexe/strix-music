// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System.Collections.Generic;
using StrixMusic.Sdk.AdapterModels;
using StrixMusic.Sdk.Models.Core;

namespace StrixMusic.Sdk.Extensions
{
    internal static partial class Merged
    {
        /// <summary>
        /// Syntax sugar for getting and casting the source cores of an <see cref="IMerged{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type to cast to</typeparam>
        /// <param name="merged">The <see cref="IMerged{T}"/> to operate on.</param>
        /// <returns>The source cores of the given <see cref="IMerged{T}"/></returns>
        internal static IReadOnlyList<ICore> GetSourceCores<T>(this IMerged<T> merged)
            where T : ICoreMember
        {
            return merged.SourceCores;
        }
    }
}
