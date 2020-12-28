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
        /// Creates a new <see cref="List{T}"/> from an <see cref="IEnumerable{T}"/>, or casts the given <paramref name="enumerable"/> if it's already a <see cref="List{T}"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <returns></returns>
        public static List<T> ToOrAsList<T>(this IEnumerable<T> enumerable)
        {
            if (enumerable is List<T> list)
            {
                return list;
            }

            return enumerable.ToList();
        }
    }
}