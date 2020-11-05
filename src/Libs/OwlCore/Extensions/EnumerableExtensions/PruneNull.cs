using System;
using System.Collections.Generic;
using System.Linq;

namespace OwlCore.Extensions
{
    /// <summary>
    /// Enumerable-related extension methods.
    /// </summary>
    public static partial class EnumerableExtensions
    {
        /// <summary>
        /// Syntax sugar for pruning null values returned from a LINQ select.
        /// </summary>
        /// <typeparam name="T">The input type.</typeparam>
        /// <param name="source">The source to enumerate.</param>
        /// <returns></returns>
        public static IEnumerable<T> PruneNull<T>(this IEnumerable<T?> source)
            where T : class
        {
            return source.Where(x => !(x is null))!;
        }
    }
}