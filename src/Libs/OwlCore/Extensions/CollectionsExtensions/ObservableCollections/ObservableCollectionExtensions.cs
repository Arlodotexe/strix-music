using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

// ReSharper disable once CheckNamespace
namespace OwlCore.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="ObservableCollection{T}"/>.
    /// </summary>
    public static class ObservableCollectionExtensions
    {
        /// <summary>
        /// Extension method to sort <see cref="ObservableCollection{T}"/>.
        /// </summary>
        /// <param name="collection">The <see cref="ObservableCollection{T}"/> to sort.</param>
        /// <param name="comparison">The <see cref="Comparer{T}"/> to sort the </param><paramref name="collection"/>.
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
