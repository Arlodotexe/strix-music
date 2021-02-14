using System.Collections.Generic;
using Microsoft.Toolkit.Diagnostics;

namespace OwlCore.Extensions
{
    public static partial class EnumerableExtensions
    {
        /// <summary>
        /// Inserts an item into a collection, or adds if the index is out of bounds.
        /// </summary>
        /// <param name="itemToAdd">The item to add to the collection.</param>
        /// <param name="source">The collection to modify.</param>
        /// <param name="index"></param>
        /// <typeparam name="T"></typeparam>
        public static void InsertOrAdd<T>(this IList<T> source, int index, T itemToAdd)
        {
            if (index < 0)
                return;

            Guard.HasSizeLessThanOrEqualTo(source, index, nameof(source));

            if (source.Count == index)
                source.Add(itemToAdd);
            else
                source.Insert(index, itemToAdd);
        }
    }
}