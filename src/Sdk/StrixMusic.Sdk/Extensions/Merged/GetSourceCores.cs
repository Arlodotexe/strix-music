using System.Collections.Generic;
using StrixMusic.Sdk.Models.Core;
using StrixMusic.Sdk.Models.Merged;

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