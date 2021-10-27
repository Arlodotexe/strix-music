using System.Collections.Generic;
using Microsoft.Toolkit.Diagnostics;

namespace OwlCore.Extensions
{
    public static partial class EnumerableExtensions
    {
        /// <summary>
        /// Replaces an item in a collection, or adds if the index is the size of the list.
        /// </summary>
        /// <param name="itemToAdd">The item to add to the collection.</param>
        /// <param name="source">The collection to modify.</param>
        /// <param name="index">The index to place the item.</param>
        /// <typeparam name="T">The type of the item being added.</typeparam>
        public static void ReplaceOrAdd<T>(this IList<T> source, int index, T itemToAdd)
        {
            if (index < 0 || index > source.Count)
                ThrowHelper.ThrowArgumentOutOfRangeException(nameof(index));

            if (source.Count == index)
                source.Add(itemToAdd);
            else
                source[index] = itemToAdd;
        }
    }
}