using System;
using System.Collections.Generic;
using System.Text;

namespace System.Collections.Generic
{
    /// <summary>
    /// A collection of extension methods.
    /// </summary>
    public static class ListExtensions
    {
        /// <summary>
        /// Splits a list into <paramref name="n"/> sub lists.
        /// </summary>
        /// <param name="list">The original list.</param>
        /// <param name="n">The amount of sub lists to create.</param>
        /// <returns>A list size N of sub lists.</returns>
        public static List<List<T>> Split<T>(this List<T> list, int n)
        {
            List<List<T>> value = new List<List<T>>();
            int subSize = list.Count / n;

            int totalCount = 0;
            for (int i = 0; i < n; i++)
            {
                List<T> currentList = new List<T>();
                for (int j = 0; j < subSize && totalCount < list.Count; j++)
                {
                    currentList.Add(list[totalCount]);
                    totalCount++;
                }
                value.Add(currentList);
            }
            return value;
        }
    }
}
