using System.Collections.Generic;
using StrixMusic.Sdk.Data;
using StrixMusic.Sdk.Data.Core;

namespace StrixMusic.Sdk.Extensions
{
    internal static partial class SdkMember
    {
        /// <summary>
        /// Syntax sugar for getting and casting the source cores of an <see cref="ISdkMember{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type to cast to</typeparam>
        /// <param name="sdkMember">The <see cref="ISdkMember{T}"/> to operate on.</param>
        /// <returns>The source cores of the given <see cref="ISdkMember{T}"/></returns>
        internal static IReadOnlyList<ICore> GetSourceCores<T>(this ISdkMember<T> sdkMember)
            where T : ICoreMember
        {
            return sdkMember.SourceCores;
        }
    }
}