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
        /// <param name="collection">The collection to modify.</param>
        /// <param name="index"></param>
        /// <typeparam name="T"></typeparam>
        public static void InsertOrAdd<T>(this ICollection<T> collection, int index, T itemToAdd)
        {
            if (index < 0)
                return;

            if (collection.Count <= index)
            {
                collection.Add(itemToAdd);
            }
            else if (collection is IList<T> list)
            {
                list.Insert(index, itemToAdd);
            }
            else
            {
                ThrowHelper.ThrowNotSupportedException($"Type {collection.GetType()} not supported by {nameof(InsertOrAdd)}");
            }
        }
    }
}