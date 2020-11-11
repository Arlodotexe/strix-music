using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Diagnostics;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using OwlCore.Events;
using OwlCore.Extensions.AsyncExtensions;
using OwlCore.Provisos;
using StrixMusic.Sdk.Data.Base;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.Extensions;
using StrixMusic.Sdk.Services.Settings;

namespace StrixMusic.Sdk.Data.Merged
{
    /// <summary>
    /// Maps indices for sources in an an <see cref="IMerged{T}"/>.
    /// </summary>
    /// <typeparam name="TCollection">The collection type that this is part of.</typeparam>
    /// <typeparam name="TCollectionItem">The type of the items returned from the collection.</typeparam>
    /// <typeparam name="TSourceCollection">The types of items that were merged to form <typeparamref name="TCollection"/>.</typeparam>
    public class MergedCollectionMap<TCollection, TCollectionItem, TSourceCollection> : IMerged<TSourceCollection>, IAsyncInit
        where TCollection : class, IPlayableCollectionBase, ISdkMember<TSourceCollection>
        where TCollectionItem : class, ICollectionItemBase
        where TSourceCollection : class, ICorePlayableCollection
    {
        private readonly TCollection _collection;
        private readonly ISettingsService _settingsService;

        /// <summary>
        /// A map where each index contains the representation of an item returned from a source collection, where the value is that source collection.
        /// </summary>
        private readonly List<MappedData> _sortedMap = new List<MappedData>();

        private IReadOnlyList<Type>? _coreRanking;
        private MergedCollectionSorting? _sortingMethod;

        /// <inheritdoc />
        public IReadOnlyList<TSourceCollection> Sources => _collection.Sources;

        /// <summary>
        /// Fires when a source has been added and the merged collection needs to be re-emitted to include the new source.
        /// </summary>
        public event CollectionChangedEventHandler<TCollectionItem>? ItemsChanged;

        /// <summary>
        /// Initializes a new instance of <see cref="MergedCollectionMap{TCollection,TCollectionItem,TMerged}"/>.
        /// </summary>
        /// <param name="collection">The collection that contains the items </param>
        public MergedCollectionMap(TCollection collection)
        {
            _collection = collection;
            _settingsService = Ioc.Default.GetService<ISettingsService>();
        }

        /// <inheritdoc />
        public async Task InitAsync()
        {
            _coreRanking = await GetCoreRankings();
            _sortingMethod = await GetSortingMethod();
            _settingsService.SettingChanged += SettingsServiceOnSettingChanged;
        }

        /// <summary>
        /// Gets a range of items from the collection, merged and sorted from multiple sources.
        /// </summary>
        /// <param name="limit">The max number of items to return.</param>
        /// <param name="offset">Get items starting at this index.</param>
        /// <returns>The requested range of items, sorted and merged from the sources in the input collection.</returns>
        public Task<IReadOnlyList<TCollectionItem>> GetItems(int limit, int offset)
        {
            Guard.IsNotNull(_sortingMethod, nameof(_sortingMethod));

            return _sortingMethod switch
            {
                MergedCollectionSorting.Ranked => GetItemsByRank(limit, offset),
                _ => ThrowHelper.ThrowNotSupportedException<Task<IReadOnlyList<TCollectionItem>>>($"Merged collection sorting by \"{_sortingMethod}\" not supported.")
            };
        }

        private async Task<IReadOnlyList<TCollectionItem>> GetItemsByRank(int limit, int offset)
        {
            Guard.IsNotNull(_coreRanking, nameof(_coreRanking));

            var returnList = new List<TCollectionItem>();

            // Rebuild the sorted map so we're sure it's sorted correctly.
            _sortedMap.Clear();
            _sortedMap.AddRange(BuildSortedMapRanked());

            // Get all requested items using the sorted map
            for (var i = offset; i < limit; i++)
            {
                var currentSource = _sortedMap[i];
                var itemsCountForSource = currentSource.SourceCollection.GetItemsCount<TCollection>();
                var itemLimitForSource = limit;

                // If the currentSource and the previous source are the same, skip this iteration
                // because we get the max items from each source once per collection.
                if (i > 0 && currentSource.SourceCollection.SourceCore == _sortedMap[i - 1].SourceCollection.SourceCore)
                    continue;

                // do we end up outside the range if we try getting all items from this source?
                if (currentSource.OriginalIndex + limit > itemsCountForSource)
                {
                    // If so, reduce limit so it only gets the remaining items in this collection.
                    itemLimitForSource = itemsCountForSource - currentSource.OriginalIndex;
                }

                var remainingItemsForSource = await OwlCore.Helpers.APIs.GetAllItemsAsync<TCollectionItem>(
                    itemLimitForSource, // Try to get as many items as possible for each page.
                    currentSource.OriginalIndex,
                    async currentOffset => await currentSource.SourceCollection.GetItems<TSourceCollection, TCollectionItem>(itemLimitForSource, currentOffset).ToListAsync().AsTask());

                // For each item that we just retrieved, find the index in the sorted map and assign the item.
                for (var o = 0; o < remainingItemsForSource.Count; o++)
                {
                    var item = remainingItemsForSource[o];

                    _sortedMap[i + o].CollectionItem = item;
                }

                returnList.AddRange(remainingItemsForSource);
            }

            return returnList;
        }

        private List<MappedData> BuildSortedMap()
        {
            Guard.IsNotNull(_sortingMethod, nameof(_sortingMethod));

            return _sortingMethod switch
            {
                MergedCollectionSorting.Ranked => BuildSortedMapRanked(),
                _ => throw new NotSupportedException($"Merged collection sorting by \"{_sortingMethod}\" not supported.")
            };
        }

        private List<MappedData> BuildSortedMapRanked()
        {
            Guard.IsNotNull(_coreRanking, nameof(_coreRanking));

            // Rank the sources by core
            var rankedSources = new List<TSourceCollection>();
            foreach (var coreType in _coreRanking)
            {
                var source = Sources.First(x => x.SourceCore.GetType() == coreType);
                rankedSources.Add(source);
            }

            // Create the map for each possible item returned from a source collection.
            var itemsMap = new List<MappedData>();

            foreach (var source in rankedSources)
            {
                var itemsCount = source.GetItemsCount<TCollection>();

                for (var i = 0; i < itemsCount; i++)
                {
                    itemsMap.Add(new MappedData(i, source));
                }
            }

            return itemsMap;
        }

        private async Task<IReadOnlyList<Type>> GetCoreRankings()
        {
            return await _settingsService.GetValue<IReadOnlyList<Type>>(nameof(SettingsKeys.CoreRanking));
        }

        private async Task<MergedCollectionSorting> GetSortingMethod()
        {
            return await _settingsService.GetValue<MergedCollectionSorting>(nameof(SettingsKeys.MergedCollectionSorting));
        }

        private void SettingsServiceOnSettingChanged(object sender, SettingChangedEventArgs e)
        {
            switch (e.Key)
            {
                case nameof(SettingsKeys.CoreRanking):
                    _coreRanking = e.Value as IReadOnlyList<Type>;
                    break;
                case nameof(SettingsKeys.MergedCollectionSorting) when e.Value != null:
                    _sortingMethod = (MergedCollectionSorting)e.Value;
                    break;
            }
        }

        /// <inheritdoc />
        /// <remarks>
        /// Handles the internal merged map when a source is added (when one collection is merged into another)
        /// </remarks>
        public void AddSource(TSourceCollection itemToMerge)
        {
            // When a new source is added, that source could be anywhere in a ranked map, or the data could be scattered or mingled arbitrarily.
            // To keep the collection sorted by the user's preferred method
            // We re-get all the data, which includes rebuilding the collection map 
            // Then re-emit ALL data

            // TODO: Optimize this (these instruction for ranked sorting only)
            // Find where this source lies in the ranking
            // If the items have already been requested and another source returned them
            // Get all the items from ONLY the new source 
            // "insert" these and every item that shifted from the insert
            // By firing the event with removed, then again with added
            Task.Run(async () =>
                {
                    var itemsFromPreviousMerge = _sortedMap.ToList();
                    _sortedMap.Clear();

                    foreach (var item in itemsFromPreviousMerge)
                    {
                        var i = itemsFromPreviousMerge.IndexOf(item);

                        // If the currentSource and the previous source are the same, skip this iteration
                        // because we get and re-emit the range of items for this source.
                        if (i > 0 && item.SourceCollection.SourceCore == _sortedMap[i - 1].SourceCollection.SourceCore)
                            continue;

                        // The items retrieved will exist in the sorted map.
                        await GetItems(item.OriginalIndex, i);
                    }

                    var addedItems = new List<CollectionChangedEventItem<TCollectionItem>>();

                    // For each item that we just retrieved, find the index in the sorted map and assign the item.
                    for (var o = 0; o < _sortedMap.Count; o++)
                    {
                        var addedItem = _sortedMap[o];

                        Guard.IsNotNull(addedItem.CollectionItem, nameof(addedItem.CollectionItem));

                        var x = new CollectionChangedEventItem<TCollectionItem>(addedItem.CollectionItem, o);
                        addedItems.Add(x);
                    }
                    
                    // logic for removed was copy-pasted and tweaked from the added logic. Not checked or tested.
                    var removedItems = new List<CollectionChangedEventItem<TCollectionItem>>();

                    for (var o = 0; o < itemsFromPreviousMerge.Count; o++)
                    {
                        var addedItem = itemsFromPreviousMerge[o];

                        Guard.IsNotNull(addedItem.CollectionItem, nameof(addedItem.CollectionItem));

                        var x = new CollectionChangedEventItem<TCollectionItem>(addedItem.CollectionItem, o);
                        removedItems.Add(x);
                    }

                    ItemsChanged?.Invoke(this, addedItems, removedItems);
                })
                .FireAndForget();
        }

        /// <inheritdoc />
        public bool Equals(TSourceCollection other)
        {
            // We're just here for the Merged, not the Equatable.
            // TSourceCollection and MergedCollectionMap have no overlap and never equal each other.
            return false;
        }

        private class MappedData
        {
            public MappedData(int originalIndex, TSourceCollection sourceCollection, TCollectionItem? collectionItem = null)
            {
                OriginalIndex = originalIndex;
                SourceCollection = sourceCollection;
                CollectionItem = collectionItem;
            }

            public int OriginalIndex { get; }

            public TSourceCollection SourceCollection { get; }

            public TCollectionItem? CollectionItem { get; set; }
        }
    }
}