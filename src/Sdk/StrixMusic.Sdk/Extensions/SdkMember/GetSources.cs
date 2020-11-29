using System.Collections.Generic;
using StrixMusic.Sdk.Data;
using StrixMusic.Sdk.Data.Core;

namespace StrixMusic.Sdk.Extensions
{
    internal static partial class SdkMember
    {
        /// <summary>
        /// Syntax sugar for getting and casting the sources of an <see cref="ISdkMember{T}"/>.
        /// </summary>
        /// <typeparam name="TSource">The explicit source type to get sources from.</typeparam>
        /// <param name="sdkMember">The <see cref="ISdkMember{T}"/> to operate on.</param>
        /// <returns>The sources of the given <see cref="ISdkMember{T}"/></returns>
        internal static IReadOnlyList<TSource> GetSources<TSource>(this ISdkMember<TSource> sdkMember)
            where TSource : ICoreMember
        {
            return sdkMember.Sources;
        }
    }
}
