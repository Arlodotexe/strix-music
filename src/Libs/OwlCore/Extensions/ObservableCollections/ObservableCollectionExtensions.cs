using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace OwlCore.Extensions.ObservableCollections
{
    /// <summary>
    /// Extension methods for <see cref="ObservableCollection{T}"/>.
    /// </summary>
    public static class ObservableCollectionExtensions
    {
        /// <summary>
        /// Extension method to sort <see cref="ObservableCollection{T}"/>.
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="comparison"></param>
        public static void Sort<T>(this ObservableCollection<T> collection, Comparer<T> comparison)
        {
            var sortableList = new List<T>(collection);
            sortableList.Sort(comparison);

            for (var i = 0; i < sortableList.Count; i++)
            {
                collection.Move(collection.IndexOf(sortableList[i]), i);
            }
        }
    }
}
