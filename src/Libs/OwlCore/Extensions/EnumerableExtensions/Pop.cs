using System.Collections.Generic;
using Microsoft.Toolkit.Diagnostics;

namespace OwlCore.Extensions
{
    public static partial class EnumerableExtensions
    {
        /// <summary>
        /// Removes and returns the last item in the <paramref name="source"/>.
        /// </summary>
        /// <param name="source">The collection to modify.</param>
        /// <typeparam name="T">The type of the item being returned.</typeparam>
        public static T Pop<T>(this IList<T> source)
        {
            var lastIndex = source.Count - 1;
            var itemToReturn = source[lastIndex];

            source.RemoveAt(lastIndex);
            return itemToReturn;
        }
    }
}