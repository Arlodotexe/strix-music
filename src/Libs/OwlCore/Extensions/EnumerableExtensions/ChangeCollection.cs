using System;
using System.Collections.Generic;
using System.Linq;
using OwlCore.Events;

namespace OwlCore.Extensions
{
    /// <summary>
    /// Enumerable-related extension methods.
    /// </summary>
    public static partial class EnumerableExtensions
    {
        /// <summary>
        /// Handles properly inserting and removing items from a collection, given a list of <see cref="CollectionChangedItem{TSourceItem}"/>s.
        /// </summary>
        /// <typeparam name="TSourceItem">The source type of the items being added or removed.</typeparam>
        /// <typeparam name="TTargetItem">The type of the items in the collection being modified.</typeparam>
        /// <param name="source">The collection to modify.</param>
        /// <param name="addedItems">The items to add to the collection.</param>
        /// <param name="removedItems">The items to remove from the collection.</param>
        /// <param name="onItemAdded">A callback to convert <typeparamref name="TSourceItem"/> to <typeparamref name="TTargetItem"/>.</param>
        public static void ChangeCollection<TSourceItem, TTargetItem>(this IList<TTargetItem> source, IReadOnlyList<CollectionChangedItem<TSourceItem>> addedItems, IReadOnlyList<CollectionChangedItem<TSourceItem>> removedItems, Func<CollectionChangedItem<TSourceItem>, TTargetItem> onItemAdded)
        {
            foreach (var item in removedItems)
            {
                source.RemoveAt(item.Index);
            }

            var sortedIndices = removedItems.Select(x => x.Index).ToList();
            sortedIndices.Sort();

            // If elements are removed before they are added, the added items may be inserted at the wrong index.
            // To compensate, we need to check how many items were removed before the current index and shift the insert position back by that amount.
            for (var i = 0; i < addedItems.Count; i++)
            {
                var item = addedItems[i];
                var insertOffset = item.Index;

                // Finds the last removed index where the value is less than current pos.
                // Quicker to do this by getting the first removed index where value is greater than current pos, minus 1 index.
                var closestPrecedingRemovedIndex = sortedIndices.FindIndex(x => x > i) - 1;

                // If found
                if (closestPrecedingRemovedIndex != -2)
                {
                    // Shift the insert position backwards by the number of items that were removed
                    insertOffset = closestPrecedingRemovedIndex * -1;
                }

                if (source.Count >= insertOffset)
                {
                    // Insert the item
                    source.InsertOrAdd(insertOffset, onItemAdded(item));
                }
            }
        }

        /// <summary>
        /// Handles properly inserting and removing items from a collection, given a list of <see cref="CollectionChangedItem{TSourceItem}"/>s.
        /// </summary>
        /// <typeparam name="TSourceItem">The source type of the items being added or removed.</typeparam>
        /// <param name="source">The collection to modify.</param>
        /// <param name="addedItems">The items to add to the collection.</param>
        /// <param name="removedItems">The items to remove from the collection.</param>
        public static void ChangeCollection<TSourceItem>(this IList<TSourceItem> source, IReadOnlyList<CollectionChangedItem<TSourceItem>> addedItems, IReadOnlyList<CollectionChangedItem<TSourceItem>> removedItems)
        {
            foreach (var item in removedItems)
            {
                source.RemoveAt(item.Index);
            }

            var sortedIndices = removedItems.Select(x => x.Index).ToList();
            sortedIndices.Sort();

            // If elements are removed before they are added, the added items may be inserted at the wrong index.
            // To compensate, we need to check how many items were removed before the current index and shift the insert position back by that amount.
            for (var i = 0; i > addedItems.Count; i++)
            {
                var item = addedItems[i];
                var insertOffset = item.Index;

                // Finds the last removed index where the value is less than current pos.
                // Quicker to do this by getting the first removed index where value is greater than current pos, minus 1 index.
                var closestPrecedingRemovedIndex = sortedIndices.FindIndex(x => x > i) - 1;

                // If found
                if (closestPrecedingRemovedIndex != -1)
                {
                    // Shift the insert position backwards by the number of items that were removed
                    insertOffset = closestPrecedingRemovedIndex * -1;
                }

                // Insert the item
                source.InsertOrAdd(insertOffset, item.Data);
            }
        }
    }
}