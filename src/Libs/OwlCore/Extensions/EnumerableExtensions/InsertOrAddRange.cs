using System.Collections.Generic;
using Microsoft.Toolkit.Diagnostics;

namespace OwlCore.Extensions
{
    public static partial class EnumerableExtensions
    {
        /// <summary>
        /// Inserts a range of items into a collection, or adds if the index is the size of the list.
        /// </summary>
        /// <param name="itemsToAdd">The item to add to the collection.</param>
        /// <param name="source">The collection to modify.</param>
        /// <param name="index">The index to place the item.</param>
        /// <typeparam name="T">The type of the item being added.</typeparam>
        public static void InsertOrAddRange<T>(this IList<T> source, int index, IEnumerable<T> itemsToAdd)
        {
            if (index < 0 || index > source.Count)
                ThrowHelper.ThrowArgumentOutOfRangeException(nameof(index));

            foreach (var item in itemsToAdd)
            {
                if (source.Count == index)
                    source.Add(item);
                else
                    source.Insert(index, item);

                index++;
            }
        }
    }
}