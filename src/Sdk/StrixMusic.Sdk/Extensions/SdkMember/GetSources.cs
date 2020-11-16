﻿using System.Collections.Generic;
using StrixMusic.Sdk.Data;
using StrixMusic.Sdk.Data.Core;

namespace StrixMusic.Sdk.Extensions
{
    internal static partial class SdkMember
    {
        /// <summary>
        /// Syntax sugar for getting and casting the sources of an <see cref="ISdkMember{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type to cast to</typeparam>
        /// <param name="sdkMember">The <see cref="ISdkMember{T}"/> to operate on.</param>
        /// <returns>The sources of the given <see cref="ISdkMember{T}"/></returns>
        internal static IReadOnlyList<T> GetSources<T>(this ISdkMember<T> sdkMember)
            where T : ICoreMember
        {
            return sdkMember.Sources;
        }
    }
}
