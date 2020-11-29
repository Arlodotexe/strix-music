using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Toolkit.Diagnostics;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using OwlCore.Events;
using OwlCore.Extensions;
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
    /// <typeparam name="TCoreCollection">The types of items that were merged to form <typeparamref name="TCollection"/>.</typeparam>
    /// <typeparam name="TCollectionItem">The type of the item returned from the merged collection.</typeparam>
    /// <typeparam name="TCoreCollectionItem">The type of the items returned from the original source collections.</typeparam>
    public class MergedCollectionMap<TCollection, TCoreCollection, TCollectionItem, TCoreCollectionItem> : IMerged<TCoreCollection>, IAsyncInit
        where TCollection : class, ICollectionBase, ISdkMember<TCoreCollection>
        where TCoreCollection : class, ICoreCollection
        where TCollectionItem : class, ICollectionItemBase, ISdkMember<TCoreCollectionItem>
        where TCoreCollectionItem : class, ICollectionItemBase, ICoreMember
    {
        private readonly TCollection _collection;
        private readonly ISettingsService _settingsService;

        /// <summary>
        /// A map where each index contains the representation of an item returned from a source collection, where the value is that source collection.
        /// </summary>
        private readonly List<MappedData> _sortedMap = new List<MappedData>();

        private readonly List<MergedMappedData> _mergedMappedData = new List<MergedMappedData>();

        private IReadOnlyList<Type>? _coreRanking;
        private MergedCollectionSorting? _sortingMethod;
        private bool _isInit;

        /// <inheritdoc />
        public IReadOnlyList<TCoreCollection> Sources => _collection.Sources;

        /// <summary>
        /// Initializes a new instance of <see cref="MergedCollectionMap{TCollection, TCoreCollection, TCollectionItem, TCoreCollectionItem}"/>.
        /// </summary>
        /// <param name="collection">The collection that contains the items </param>
        public MergedCollectionMap(TCollection collection)
        {
            _collection = collection;
            _settingsService = Ioc.Default.GetService<ISettingsService>() ?? ThrowHelper.ThrowInvalidOperationException<ISettingsService>();

            AttachEvents();
        }

        /// <inheritdoc />
        public async Task InitAsync()
        {
            if (_isInit)
                return;

            _coreRanking = await GetCoreRankings();
            _sortingMethod = await GetSortingMethod();
            _settingsService.SettingChanged += SettingsServiceOnSettingChanged;

            _isInit = true;
        }

        private Task TryInitAsync() => InitAsync();

        /// <summary>
        /// Fires when a source has been added and the merged collection needs to be re-emitted to include the new source.
        /// </summary>
        public event CollectionChangedEventHandler<TCollectionItem>? ItemsChanged;

        /// <summary>
        /// Fires when the number of items in the merged collection changes, either from a new source being added or from an item getting merged into another.
        /// </summary>
        public event EventHandler<int>? ItemsCountChanged;

        private void AttachEvents()
        {
            foreach (var item in Sources)
            {
                AttachEvents(item);
            }
        }

        private void DetachEvents()
        {
            foreach (var item in Sources)
            {
                DetachEvents(item);
            }
        }

        private void AttachEvents(TCoreCollection item)
        {
            if (typeof(TCoreCollection) == typeof(ICorePlayableCollectionGroup))
            {
                ((ICorePlayableCollectionGroup)item).ChildItemsChanged += MergedCollectionMap_ChildItemsChanged;
                ((ICorePlayableCollectionGroup)item).TotalChildrenCountChanged += MergedCollectionMap_CountChanged;
            }
            else if (typeof(TCoreCollection) == typeof(ICoreAlbumCollection))
            {
                ((ICoreAlbumCollection)item).AlbumItemsCountChanged += MergedCollectionMap_CountChanged;
                ((ICoreAlbumCollection)item).AlbumItemsChanged += MergedCollectionMap_AlbumItemsChanged;
            }
            else if (typeof(TCoreCollection) == typeof(ICoreArtistCollection))
            {
                ((ICoreArtistCollection)item).ArtistItemsCountChanged += MergedCollectionMap_CountChanged;
                ((ICoreArtistCollection)item).ArtistItemsChanged += MergedCollectionMap_ArtistItemsChanged;
            }
            else if (typeof(TCoreCollection) == typeof(ICorePlaylistCollection))
            {
                ((ICoreArtistCollection)item).ArtistItemsCountChanged += MergedCollectionMap_CountChanged;
                ((ICoreArtistCollection)item).ArtistItemsChanged += MergedCollectionMap_ArtistItemsChanged;
            }
            else if (typeof(TCoreCollection) == typeof(ICoreTrackCollection))
            {
                ((ICoreTrackCollection)item).TrackItemsCountChanged += MergedCollectionMap_CountChanged;
                ((ICoreTrackCollection)item).TrackItemsChanged += MergedCollectionMap_TrackItemsChanged;
            }
            else if (typeof(TCoreCollection) == typeof(ICoreImageCollection))
            {
                ((ICoreImageCollection)item).ImagesCountChanged += MergedCollectionMap_CountChanged;
                ((ICoreImageCollection)item).ImagesChanged += MergedCollectionMap_ImagesChanged;
            }
            else
            {
                ThrowHelper.ThrowNotSupportedException<IMerged<TCoreCollection>>(
                    $"Couldn't attach events. Type \"{typeof(TCoreCollection)}\" not supported.");
            }
        }

        private void DetachEvents(TCoreCollection item)
        {
            if (typeof(TCoreCollection) == typeof(ICorePlayableCollectionGroup))
            {
                ((ICorePlayableCollectionGroup)item).ChildItemsChanged -= MergedCollectionMap_ChildItemsChanged;
                ((ICorePlayableCollectionGroup)item).TotalChildrenCountChanged -= MergedCollectionMap_CountChanged;
            }
            else if (typeof(TCoreCollection) == typeof(ICoreAlbumCollection))
            {
                ((ICoreAlbumCollection)item).AlbumItemsCountChanged -= MergedCollectionMap_CountChanged;
                ((ICoreAlbumCollection)item).AlbumItemsChanged -= MergedCollectionMap_AlbumItemsChanged;
            }
            else if (typeof(TCoreCollection) == typeof(ICoreArtistCollection))
            {
                ((ICoreArtistCollection)item).ArtistItemsCountChanged -= MergedCollectionMap_CountChanged;
                ((ICoreArtistCollection)item).ArtistItemsChanged -= MergedCollectionMap_ArtistItemsChanged;
            }
            else if (typeof(TCoreCollection) == typeof(ICorePlaylistCollection))
            {
                ((ICoreArtistCollection)item).ArtistItemsCountChanged -= MergedCollectionMap_CountChanged;
                ((ICoreArtistCollection)item).ArtistItemsChanged -= MergedCollectionMap_ArtistItemsChanged;
            }
            else if (typeof(TCoreCollection) == typeof(ICoreTrackCollection))
            {
                ((ICoreTrackCollection)item).TrackItemsCountChanged -= MergedCollectionMap_CountChanged;
                ((ICoreTrackCollection)item).TrackItemsChanged -= MergedCollectionMap_TrackItemsChanged;
            }
            else if (typeof(TCoreCollection) == typeof(ICoreImageCollection))
            {
                ((ICoreImageCollection)item).ImagesCountChanged -= MergedCollectionMap_CountChanged;
                ((ICoreImageCollection)item).ImagesChanged -= MergedCollectionMap_ImagesChanged;
            }
            else
            {
                ThrowHelper.ThrowNotSupportedException<IMerged<TCoreCollection>>(
                    "Couldn't attach events. Type not supported.");
            }
        }

        private void MergedCollectionMap_ImagesChanged(object sender, IReadOnlyList<CollectionChangedEventItem<ICoreImage>> addedItems, IReadOnlyList<CollectionChangedEventItem<ICoreImage>> removedItems)
        {
            MergedCollectionMap_ItemsChanged(sender, addedItems, removedItems);
        }

        private void MergedCollectionMap_TrackItemsChanged(object sender, IReadOnlyList<CollectionChangedEventItem<ICoreTrack>> addedItems, IReadOnlyList<CollectionChangedEventItem<ICoreTrack>> removedItems)
        {
            MergedCollectionMap_ItemsChanged(sender, addedItems, removedItems);
        }

        private void MergedCollectionMap_ArtistItemsChanged(object sender, IReadOnlyList<CollectionChangedEventItem<ICoreArtistCollectionItem>> addedItems, IReadOnlyList<CollectionChangedEventItem<ICoreArtistCollectionItem>> removedItems)
        {
            MergedCollectionMap_ItemsChanged(sender, addedItems, removedItems);
        }

        private void MergedCollectionMap_AlbumItemsChanged(object sender, IReadOnlyList<CollectionChangedEventItem<ICoreAlbumCollectionItem>> addedItems, IReadOnlyList<CollectionChangedEventItem<ICoreAlbumCollectionItem>> removedItems)
        {
            MergedCollectionMap_ItemsChanged(sender, addedItems, removedItems);
        }

        private void MergedCollectionMap_ChildItemsChanged(object sender, IReadOnlyList<CollectionChangedEventItem<ICorePlayableCollectionGroup>> addedItems, IReadOnlyList<CollectionChangedEventItem<ICorePlayableCollectionGroup>> removedItems)
        {
            MergedCollectionMap_ItemsChanged(sender, addedItems, removedItems);
        }

        private void MergedCollectionMap_ItemsChanged<T>(object sender, IReadOnlyList<CollectionChangedEventItem<T>> addedItems, IReadOnlyList<CollectionChangedEventItem<T>> removedItems)
        where T : class, ICollectionItemBase, ICoreMember
        {
            var addedMergedItems = CheckAddedItems();
            var removedMergedItems = CheckRemovedItems();

            List<CollectionChangedEventItem<TCollectionItem>> CheckAddedItems()
            {
                var added = new List<CollectionChangedEventItem<TCollectionItem>>();
                var newItems = new List<IMerged<TCoreCollectionItem>>();

                foreach (var item in addedItems)
                {
                    if (!(item.Data is TCoreCollectionItem collectionItemData))
                        return ThrowHelper.ThrowInvalidOperationException<List<CollectionChangedEventItem<TCollectionItem>>>($"{nameof(item.Data)} couldn't be cast to {nameof(TCoreCollectionItem)}.");

                    // Check for an existing IMerged that matches this item
                    foreach (var mergedItem in _mergedMappedData)
                    {
                        // Using "?? false" breaks nullable assertion?
                        if (mergedItem?.CollectionItem.Equals(collectionItemData) == true)
                        {
                            mergedItem.CollectionItem.AddSource(collectionItemData);
                            return added;
                        }
                    }

                    // TODO: Sorting is not handled.
                    var mergedImpl = MergeOrAdd(newItems, collectionItemData);

                    var mappedData = new MappedData(item.Index, (TCoreCollection)sender, collectionItemData);
                    _sortedMap.Add(mappedData);
                    _mergedMappedData.Add(new MergedMappedData(mergedImpl, new[] { mappedData }));
                }

                return added;
            }

            List<CollectionChangedEventItem<TCollectionItem>> CheckRemovedItems()
            {
                var removed = new List<CollectionChangedEventItem<TCollectionItem>>();

                foreach (var item in removedItems)
                {
                    var mappedData = _sortedMap.FirstOrDefault(x => x.OriginalIndex == item.Index && item.Data.SourceCore == x.SourceCollection.SourceCore);

                    if (mappedData == null)
                        continue;

                    foreach (var mergedData in _mergedMappedData)
                    {
                        foreach (var mergedSource in mergedData.CollectionItem.Sources)
                        {
                            if (mappedData.CollectionItem != mergedSource)
                                continue;

                            _sortedMap.Remove(mappedData);

                            mergedData.CollectionItem.RemoveSource(mergedSource);

                            mergedData.MergedMapData.RemoveAll(x => x.OriginalIndex == item.Index && item.Data.SourceCore == x.SourceCollection.SourceCore);

                            if (mergedData.CollectionItem.Sources.Count == 0)
                            {
                                _mergedMappedData.Remove(mergedData);

                                var index = _mergedMappedData.IndexOf(mergedData);
                                removed.Add(new CollectionChangedEventItem<TCollectionItem>((TCollectionItem)mergedData.CollectionItem, index));
                            }

                            return removed;
                        }
                    }
                }

                return removed;
            }

            ItemsChanged?.Invoke(this, addedMergedItems, removedMergedItems);
            ItemsCountChanged?.Invoke(this, _mergedMappedData.Count);
        }

        private void MergedCollectionMap_CountChanged(object sender, int e)
        {
            // This is sent from each core.
            // The count would be wrong if we tried to re-emit it as is.
            // We emit CountChanged for the Map elsewhere when items are changed.

            // Instead, maybe we can use it this event verify the size of the collection is correct?
            // Can't think of a better use.
        }

        /// <summary>
        /// Gets a range of items from the collection, merged and sorted from multiple sources.
        /// </summary>
        /// <param name="limit">The max number of items to return.</param>
        /// <param name="offset">Get items starting at this index.</param>
        /// <returns>The requested range of items, sorted and merged from the sources in the input collection.</returns>
        public async Task<IReadOnlyList<TCollectionItem>> GetItems(int limit, int offset)
        {
            await TryInitAsync();
            Guard.IsNotNull(_sortingMethod, nameof(_sortingMethod));

            return _sortingMethod switch
            {
                MergedCollectionSorting.Ranked => await GetItemsByRank(limit, offset),
                _ => ThrowHelper.ThrowNotSupportedException<IReadOnlyList<TCollectionItem>>($"Merged collection sorting by \"{_sortingMethod}\" not supported.")
            };
        }

        /// <summary>
        /// Inserts an item into the compatible source collections on the backend.
        /// </summary>
        /// <param name="item">The item to insert.</param>
        /// <param name="index">The index to place this item at.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task InsertItem(TCollectionItem item, int index)
        {
            await TryInitAsync();
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            // TODO
            // This does not handle adding NEW items to a list, only existing items from elsewhere. 
            // e.g. cannot create a new playlist or playableCollectionGroup.
            // Plan on having a new type that implements ICoreSomething for the UI to create a merged item + fill out data.
            foreach (var source in item.Sources)
            {
                var addedRecord = new Dictionary<TCoreCollection, bool>();

                foreach (var mappedData in _sortedMap)
                {
                    if (mappedData.CollectionItem is null)
                        continue;

                    var sourceCollection = mappedData.SourceCollection;

                    // Make sure the source cores are the same.
                    if (sourceCollection.SourceCore != source.SourceCore)
                        continue;

                    // Only add to this source collection once.
                    if (addedRecord.ContainsKey(sourceCollection))
                        continue;

                    addedRecord.Add(sourceCollection, true);

                    switch (sourceCollection)
                    {
                        case ICorePlayableCollectionGroup playableCollection:
                            if (await playableCollection.IsAddChildSupported(index))
                                await playableCollection.AddChildAsync((ICorePlayableCollectionGroup)source, mappedData.OriginalIndex);
                            break;
                        case ICoreAlbumCollection albumCollection:
                            if (await albumCollection.IsAddAlbumItemSupported(index))
                                await albumCollection.AddAlbumItemAsync((ICoreAlbumCollectionItem)source, mappedData.OriginalIndex);
                            break;
                        case ICoreArtistCollection artistCollection:
                            if (await artistCollection.IsAddArtistSupported(index))
                                await artistCollection.AddArtistItemAsync((ICoreArtistCollectionItem)source, mappedData.OriginalIndex);
                            break;
                        case ICorePlaylistCollection playlistCollection:
                            if (await playlistCollection.IsAddPlaylistItemSupported(index))
                                await playlistCollection.AddPlaylistItemAsync((ICorePlaylistCollectionItem)source, mappedData.OriginalIndex);
                            break;
                        case ICoreTrackCollection trackCollection:
                            if (await trackCollection.IsAddTrackSupported(index))
                                await trackCollection.AddTrackAsync((ICoreTrack)source, mappedData.OriginalIndex);
                            break;
                        case ICoreImageCollection imageCollection:
                            if (await imageCollection.IsAddImageSupported(index))
                                await imageCollection.AddImageAsync((ICoreImage)source, mappedData.OriginalIndex);
                            break;
                        default:
                            ThrowHelper.ThrowNotSupportedException<IMerged<TCoreCollection>>("Couldn't create merged item. Type not supported.");
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Inserts an item into the compatible source collections on the backend.
        /// </summary>
        /// <param name="index">The index to place this item at.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task RemoveAt(int index)
        {
            await TryInitAsync();

            // Externally, the app sees non-core items as this internal collection of merged and sorted items and data.
            // When they ask for an item at an index, they're asking for an item at that index that could be merged.
            // So we go through each of the mapped sources for the item at this index and handle removing from the core side.
            var targetItem = _mergedMappedData[index];

            foreach (var mappedData in targetItem.MergedMapData)
            {
                Guard.IsNotNull(mappedData.CollectionItem, nameof(mappedData.CollectionItem));

                var sourceCollection = mappedData.SourceCollection;
                var source = mappedData.CollectionItem;

                switch (sourceCollection)
                {
                    case ICorePlayableCollectionGroup playableCollection:
                        if (await playableCollection.IsAddChildSupported(index))
                            await playableCollection.AddChildAsync((ICorePlayableCollectionGroup)source, mappedData.OriginalIndex);
                        break;
                    case ICoreAlbumCollection albumCollection:
                        if (await albumCollection.IsAddAlbumItemSupported(index))
                            await albumCollection.AddAlbumItemAsync((ICoreAlbumCollectionItem)source, mappedData.OriginalIndex);
                        break;
                    case ICoreArtistCollection artistCollection:
                        if (await artistCollection.IsAddArtistSupported(index))
                            await artistCollection.AddArtistItemAsync((ICoreArtistCollectionItem)source, mappedData.OriginalIndex);
                        break;
                    case ICorePlaylistCollection playlistCollection:
                        if (await playlistCollection.IsAddPlaylistItemSupported(index))
                            await playlistCollection.AddPlaylistItemAsync((ICorePlaylistCollectionItem)source, mappedData.OriginalIndex);
                        break;
                    case ICoreTrackCollection trackCollection:
                        if (await trackCollection.IsAddTrackSupported(index))
                            await trackCollection.AddTrackAsync((ICoreTrack)source, mappedData.OriginalIndex);
                        break;
                    default:
                        ThrowHelper.ThrowNotSupportedException<IMerged<TCoreCollection>>("Couldn't create merged item. Type not supported.");
                        break;
                }
            }
        }

        /// <summary>
        /// Checks if adding an item to the sorted map is supported.
        /// </summary>
        /// <param name="index">The index to remove.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. Value indicates support.</returns>
        public async Task<bool> IsAddItemSupported(int index)
        {
            await TryInitAsync();

            var sourceResults = await _mergedMappedData[index].MergedMapData
                .InParallel(async x => await x.SourceCollection.IsAddSupported(x.OriginalIndex));

            return sourceResults.Any();
        }

        /// <summary>
        /// Checks if removing an item from the sorted map is supported.
        /// </summary>
        /// <param name="index">The index to remove.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. Value indicates support.</returns>
        public async Task<bool> IsRemoveItemSupport(int index)
        {
            await TryInitAsync();

            var sourceResults = await _mergedMappedData[index].MergedMapData
                .InParallel(async x => await x.SourceCollection.IsAddSupported(x.OriginalIndex));

            return sourceResults.Any();
        }

        private static IMerged<TCoreCollectionItem> MergeOrAdd(List<IMerged<TCoreCollectionItem>> collection, TCoreCollectionItem itemToMerge)
        {
            foreach (var item in collection)
            {
                if (item.Equals(itemToMerge))
                {
                    item.AddSource(itemToMerge);
                    return item;
                }
            }

            IMerged<TCoreCollectionItem>? returnData;

            // if the collection doesn't contain IMerged<TCollectionItem> at all, create a new Merged
            switch (itemToMerge)
            {
                case ICoreArtist artist:
                    returnData = (IMerged<TCoreCollectionItem>)new MergedArtist(artist.IntoList());
                    collection.Add(returnData);
                    break;
                case ICoreAlbum album:
                    returnData = (IMerged<TCoreCollectionItem>)new MergedAlbum(album.IntoList());
                    collection.Add(returnData);
                    break;
                case ICorePlaylist playlist:
                    returnData = (IMerged<TCoreCollectionItem>)new MergedPlaylist(playlist.IntoList());
                    collection.Add(returnData);
                    break;
                case ICoreTrack track:
                    returnData = (IMerged<TCoreCollectionItem>)new MergedTrack(track.IntoList());
                    collection.Add(returnData);
                    break;
                case ICoreDiscoverables discoverables:
                    returnData = (IMerged<TCoreCollectionItem>)new MergedDiscoverables(discoverables.IntoList());
                    collection.Add(returnData);
                    break;
                case ICoreLibrary library:
                    returnData = (IMerged<TCoreCollectionItem>)new MergedLibrary(library.IntoList());
                    collection.Add(returnData);
                    break;
                case ICoreRecentlyPlayed recentlyPlayed:
                    returnData = (IMerged<TCoreCollectionItem>)new MergedRecentlyPlayed(recentlyPlayed.IntoList());
                    collection.Add(returnData);
                    break;
                case ICoreImage coreImage:
                    returnData = (IMerged<TCoreCollectionItem>)new MergedImage(coreImage.IntoList());
                    collection.Add(returnData);
                    break;

                // TODO: Search results post search redo
                case ICorePlayableCollectionGroup playableCollection:
                    returnData = (IMerged<TCoreCollectionItem>)new MergedPlayableCollectionGroup(playableCollection.IntoList());
                    collection.Add(returnData);
                    break;
                case ICoreAlbumCollection albumCollection:
                    returnData = (IMerged<TCoreCollectionItem>)new MergedAlbumCollection(albumCollection.IntoList());
                    collection.Add(returnData);
                    break;
                case ICoreArtistCollection artistCollection:
                    returnData = (IMerged<TCoreCollectionItem>)new MergedArtistCollection(artistCollection.IntoList());
                    collection.Add(returnData);
                    break;
                case ICorePlaylistCollection playlistCollection:
                    returnData = (IMerged<TCoreCollectionItem>)new MergedPlaylistCollection(playlistCollection.IntoList());
                    collection.Add(returnData);
                    break;
                case ICoreTrackCollection trackCollection:
                    returnData = (IMerged<TCoreCollectionItem>)new MergedTrackCollection(trackCollection.IntoList());
                    collection.Add(returnData);
                    break;
                default:
                    // Replace throw with this when verified that this is fully finished.
                    // ThrowHelper.ThrowNotSupportedException<IMerged<TCoreCollection>>("Couldn't create merged item. Type not supported.");
                    throw new NotImplementedException();
            }

            return returnData;
        }

        private async Task<IReadOnlyList<TCollectionItem>> GetItemsByRank(int limit, int offset)
        {
            Guard.IsNotNull(_coreRanking, nameof(_coreRanking));

            // Rebuild the sorted map so we're sure it's sorted correctly.
            _sortedMap.AddRange(BuildSortedMapRanked());

            if (limit > _sortedMap.Count)
                limit = _sortedMap.Count;

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

                var remainingItemsForSource = await OwlCore.Helpers.APIs.GetAllItemsAsync<TCoreCollectionItem>(
                    itemLimitForSource, // Try to get as many items as possible for each page.
                    currentSource.OriginalIndex,
                    async currentOffset => await currentSource.SourceCollection.GetItems<TCoreCollection, TCoreCollectionItem>(itemLimitForSource, currentOffset).ToListAsync().AsTask());

                // For each item that we just retrieved, find the index in the sorted maps and assign the item.
                for (var o = 0; o < remainingItemsForSource.Count; o++)
                {
                    var item = remainingItemsForSource[o];

                    _sortedMap[i + o].CollectionItem = item;
                }
            }

            var merged = MergeMappedData(_sortedMap).Select(x => (TCollectionItem)x).ToList();

            return merged;
        }

        private List<IMerged<TCoreCollectionItem>> MergeMappedData(IList<MappedData> sortedData)
        {
            _mergedMappedData.Clear();

            var returnedData = new List<IMerged<TCoreCollectionItem>>();

            var mergedItemMaps = new Dictionary<IMerged<TCoreCollectionItem>, List<MappedData>>();

            foreach (var item in sortedData)
            {
                if (item.CollectionItem is null)
                    continue;

                var mergedInto = MergeOrAdd(returnedData, item.CollectionItem);

                List<MappedData> mergedMapItems = new List<MappedData>();
                bool exists = mergedInto != null && mergedItemMaps.TryGetValue(mergedInto, out mergedMapItems);
                mergedMapItems ??= new List<MappedData>();

                mergedMapItems.Add(item);
                if (!exists && mergedInto != null)
                    mergedItemMaps.Add(mergedInto, mergedMapItems);
            }

            foreach (var item in mergedItemMaps)
            {
                _mergedMappedData.Add(new MergedMappedData(item.Key, item.Value));
            }

            ItemsCountChanged?.Invoke(this, _mergedMappedData.Count);

            return returnedData;
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
            var rankedSources = new List<TCoreCollection>();
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

        private async Task ResetDataRanked()
        {
            await TryInitAsync();

            // TODO: Optimize this (these instruction for ranked sorting only)
            // Find where this source lies in the ranking
            // If the items have already been requested and another source returned them
            // Get all the items from ONLY the new source 
            // "insert" these and every item that shifted from the insert
            // By firing the event with removed, then again with added
            var previouslySortedItems = _sortedMap.ToList();
            var previousMergedMap = _mergedMappedData.ToList();
            _sortedMap.Clear();

            foreach (var item in previouslySortedItems)
            {
                var i = previouslySortedItems.IndexOf(item);

                // If the currentSource and the previous source are the same, skip this iteration
                // because we get and re-emit the range of items for this source.
                if (i > 0 && item.SourceCollection.SourceCore == _sortedMap[i - 1].SourceCollection.SourceCore)
                    continue;

                // The items retrieved will exist in the sorted map.
                await GetItems(item.OriginalIndex, i);
            }

            var addedItems = new List<CollectionChangedEventItem<TCollectionItem>>();

            // For each item that we just retrieved, find the index in the sorted map and assign the item.
            for (var o = 0; o < _mergedMappedData.Count; o++)
            {
                var addedItem = _mergedMappedData[o];

                Guard.IsNotNull(addedItem.CollectionItem, nameof(addedItem.CollectionItem));

                var x = new CollectionChangedEventItem<TCollectionItem>((TCollectionItem)addedItem.CollectionItem, o);
                addedItems.Add(x);
            }

            // logic for removed was copy-pasted and tweaked from the added logic. Not checked or tested.
            var removedItems = new List<CollectionChangedEventItem<TCollectionItem>>();

            for (var o = 0; o < previousMergedMap.Count; o++)
            {
                var addedItem = previousMergedMap[o];

                Guard.IsNotNull(addedItem.CollectionItem, nameof(addedItem.CollectionItem));

                var x = new CollectionChangedEventItem<TCollectionItem>((TCollectionItem)addedItem.CollectionItem, o);
                removedItems.Add(x);
            }

            ItemsChanged?.Invoke(this, addedItems, removedItems);
            ItemsCountChanged?.Invoke(this, addedItems.Count);
        }

        /// <inheritdoc />
        /// <remarks>
        /// Handles the internal merged map when a source is added (when one collection is merged into another).
        /// <para>
        /// When a new source is added, that source could be anywhere in a ranked map, or the data could be scattered or mingled arbitrarily.
        /// To keep the collection sorted by the user's preferred method
        /// We re-get all the data, which includes rebuilding the collection map 
        /// Then re-emit ALL data
        /// </para>
        /// </remarks>
        public void AddSource(TCoreCollection itemToMerge)
        {
            ResetDataRanked().FireAndForget();
        }

        /// <inheritdoc />
        public void RemoveSource(TCoreCollection itemToRemove)
        {
            ResetDataRanked().FireAndForget();
        }

        /// <inheritdoc />
        /// <remarks>
        /// We're just here for the Merged, not the Equatable.
        /// TSourceCollection and MergedCollectionMap have no overlap and never equal each other.
        /// </remarks>
        public bool Equals(TCoreCollection other)
        {
            return false;
        }

        private class MappedData
        {
            public MappedData(int originalIndex, TCoreCollection sourceCollection, TCoreCollectionItem? collectionItem = null)
            {
                OriginalIndex = originalIndex;
                SourceCollection = sourceCollection;
                CollectionItem = collectionItem;
            }

            public int OriginalIndex { get; }

            public TCoreCollection SourceCollection { get; }

            public TCoreCollectionItem? CollectionItem { get; set; }
        }

        private class MergedMappedData
        {
            public MergedMappedData(IMerged<TCoreCollectionItem> collectionItem, IEnumerable<MappedData> mergedMapData)
            {
                CollectionItem = collectionItem;
                MergedMapData = mergedMapData.ToList();
            }

            public IMerged<TCoreCollectionItem> CollectionItem { get; }

            public List<MappedData> MergedMapData { get; }
        }
    }
}