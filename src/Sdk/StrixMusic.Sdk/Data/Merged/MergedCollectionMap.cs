using Microsoft.Toolkit.Diagnostics;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using OwlCore.Events;
using OwlCore.Extensions;
using OwlCore.Provisos;
using StrixMusic.Sdk.Data.Base;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.Extensions;
using StrixMusic.Sdk.Services.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StrixMusic.Sdk.Data.Merged
{
    /// <summary>
    /// Manages the merging of multiple <typeparamref name="TCoreCollection"/>s and presents them as single <typeparamref name="TCollection"/>.
    /// </summary>
    /// <typeparam name="TCollection">The collection type that this is part of.</typeparam>
    /// <typeparam name="TCoreCollection">The types of items that were merged to form <typeparamref name="TCollection"/>.</typeparam>
    /// <typeparam name="TCollectionItem">The type of the item returned from the merged collection.</typeparam>
    /// <typeparam name="TCoreCollectionItem">The type of the items returned from the original source collections.</typeparam>
    internal class MergedCollectionMap<TCollection, TCoreCollection, TCollectionItem, TCoreCollectionItem> : IMerged<TCoreCollection>, IMergedMutable<TCoreCollection>, IAsyncInit, IAsyncDisposable
        where TCollection : class, ICollectionBase, IMerged<TCoreCollection>
        where TCoreCollection : class, ICoreCollection
        where TCollectionItem : class, ICollectionItemBase, IMerged<TCoreCollectionItem>
        where TCoreCollectionItem : class, ICollectionItemBase, ICoreMember
    {
        // ReSharper disable StaticMemberInGenericType
        private static bool _isInitialized;
        private static TaskCompletionSource<bool>? _initCompletionSource;
        private static IReadOnlyList<string>? _coreRanking;
        private static Dictionary<string, CoreAssemblyInfo>? _coreInstanceRegistry;
        private static MergedCollectionSorting? _sortingMethod;

        private readonly TCollection _collection;
        private readonly ISettingsService _settingsService;

        /// <summary>
        /// A map where each index contains the representation of an item returned from a source collection, where the value is that source collection.
        /// </summary>
        private readonly List<MappedData> _sortedMap = new List<MappedData>();
        private readonly List<MergedMappedData> _mergedMappedData = new List<MergedMappedData>();

        /// <inheritdoc />
        public IReadOnlyList<TCoreCollection> Sources => _collection.Sources;

        /// <inheritdoc />
        public IReadOnlyList<ICore> SourceCores => _collection.SourceCores;

        /// <summary>
        /// Initializes a new instance of <see cref="MergedCollectionMap{TCollection, TCoreCollection, TCollectionItem, TCoreCollectionItem}"/>.
        /// </summary>
        /// <param name="collection">The collection that contains the items </param>
        public MergedCollectionMap(TCollection collection)
        {
            _collection = collection;
            _settingsService = Ioc.Default.GetRequiredService<ISettingsService>();

            AttachEvents();
        }

        private static async Task InsertItemIntoCollection<T>(TCoreCollection sourceCollection, T itemToAdd, int originalIndex)
            where T : class, ICollectionItemBase, ICoreMember // https://twitter.com/Arlodottxt/status/1351317100959326213?s=20
        {
            switch (sourceCollection)
            {
                case ICorePlayableCollectionGroup playableCollection:
                    if (await playableCollection.IsAddChildAvailable(originalIndex))
                        await playableCollection.AddChildAsync((ICorePlayableCollectionGroup)itemToAdd, originalIndex);
                    break;
                case ICoreAlbumCollection albumCollection:
                    if (await albumCollection.IsAddAlbumItemAvailable(originalIndex))
                        await albumCollection.AddAlbumItemAsync((ICoreAlbumCollectionItem)itemToAdd, originalIndex);
                    break;
                case ICoreArtistCollection artistCollection:
                    if (await artistCollection.IsAddArtistItemAvailable(originalIndex))
                        await artistCollection.AddArtistItemAsync((ICoreArtistCollectionItem)itemToAdd, originalIndex);
                    break;
                case ICorePlaylistCollection playlistCollection:
                    if (await playlistCollection.IsAddPlaylistItemAvailable(originalIndex))
                        await playlistCollection.AddPlaylistItemAsync((ICorePlaylistCollectionItem)itemToAdd, originalIndex);
                    break;
                case ICoreTrackCollection trackCollection:
                    if (await trackCollection.IsAddTrackAvailable(originalIndex))
                        await trackCollection.AddTrackAsync((ICoreTrack)itemToAdd, originalIndex);
                    break;
                case ICoreImageCollection imageCollection:
                    if (await imageCollection.IsAddImageAvailable(originalIndex))
                        await imageCollection.AddImageAsync((ICoreImage)itemToAdd, originalIndex);
                    break;
                case ICoreGenreCollection genreCollection:
                    if (await genreCollection.IsAddGenreAvailable(originalIndex))
                        await genreCollection.AddGenreAsync((ICoreGenre)itemToAdd, originalIndex);
                    break;
                default:
                    ThrowHelper.ThrowNotSupportedException<IMergedMutable<TCoreCollection>>($"Couldn't add item to collection. Type {sourceCollection.GetType()} not supported.");
                    break;
            }
        }

        private static async Task InsertExistingItem(TCollectionItem itemToInsert, MappedData mappedData)
        {
            foreach (var source in itemToInsert.Sources)
            {
                var addedRecord = new Dictionary<TCoreCollection, bool>();

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

                var originalIndex = mappedData.OriginalIndex;

                await InsertItemIntoCollection(sourceCollection, source, originalIndex);
            }
        }

        private static async Task InsertNewItem(IEnumerable<TCoreCollection> sourceCollections, IReadOnlyList<ICore> sourceCores, IInitialData dataToInsert, int index)
        {
            // TODO create setting to let user decide default
            foreach (var source in sourceCollections)
            {
                IEnumerable<ICore> targetSources = sourceCores;

                if (dataToInsert.TargetSourceCores != null && dataToInsert.TargetSourceCores.Count > 0)
                {
                    targetSources = dataToInsert.TargetSourceCores;
                }

                // Try adding to all by default
                foreach (var targetCore in targetSources)
                {
                    if (dataToInsert is InitialPlaylistData playlistData)
                    {
                        var coreData = new InitialCorePlaylistData(playlistData, targetCore);

                        await InsertItemIntoCollection(source, coreData, index);
                    }
                }
            }
        }

        /// <inheritdoc />
        public async Task InitAsync()
        {
            if (IsInitialized)
                return;

            if (_initCompletionSource?.Task.Status == TaskStatus.Running || _initCompletionSource?.Task.Status == TaskStatus.WaitingForActivation || _initCompletionSource?.Task.Status == TaskStatus.RanToCompletion)
            {
                await _initCompletionSource.Task;
                return;
            }

            _initCompletionSource = new TaskCompletionSource<bool>();

            _coreRanking = await GetCoreRankings();

            _coreInstanceRegistry = await GetConfiguredCoreRegistry();

            _sortingMethod = await GetSortingMethod();
            _settingsService.SettingChanged += SettingsServiceOnSettingChanged;

            Guard.IsNotNull(_coreRanking, nameof(_coreRanking));
            Guard.HasSizeGreaterThan(_coreRanking, 0, nameof(_coreRanking));

            Guard.IsNotNull(_coreInstanceRegistry, nameof(_coreInstanceRegistry));
            Guard.IsGreaterThan(_coreInstanceRegistry.Count, 0, nameof(_coreInstanceRegistry.Count));

            _initCompletionSource.SetResult(true);
            IsInitialized = true;
        }

        /// <inheritdoc />
        public bool IsInitialized
        {
            get => _isInitialized;
            set => _isInitialized = value;
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
                ((ICorePlayableCollectionGroup)item).ChildrenCountChanged += MergedCollectionMap_CountChanged;
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
                ((ICorePlaylistCollection)item).PlaylistItemsCountChanged += MergedCollectionMap_CountChanged;
                ((ICorePlaylistCollection)item).PlaylistItemsChanged += MergedCollectionMap_PlaylistItemsChanged;
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
            else if (typeof(TCoreCollection) == typeof(ICoreGenreCollection))
            {
                ((ICoreGenreCollection)item).GenresCountChanged += MergedCollectionMap_CountChanged;
                ((ICoreGenreCollection)item).GenresChanged += MergedCollectionMap_GenresChanged;
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
                ((ICorePlayableCollectionGroup)item).ChildrenCountChanged -= MergedCollectionMap_CountChanged;
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

        private void MergedCollectionMap_ImagesChanged(object sender, IReadOnlyList<CollectionChangedItem<ICoreImage>> addedItems, IReadOnlyList<CollectionChangedItem<ICoreImage>> removedItems)
        {
            MergedCollectionMap_ItemsChanged(sender, addedItems, removedItems);
        }

        private void MergedCollectionMap_GenresChanged(object sender, IReadOnlyList<CollectionChangedItem<ICoreGenre>> addedItems, IReadOnlyList<CollectionChangedItem<ICoreGenre>> removedItems)
        {
            MergedCollectionMap_ItemsChanged(sender, addedItems, removedItems);
        }

        private void MergedCollectionMap_TrackItemsChanged(object sender, IReadOnlyList<CollectionChangedItem<ICoreTrack>> addedItems, IReadOnlyList<CollectionChangedItem<ICoreTrack>> removedItems)
        {
            MergedCollectionMap_ItemsChanged(sender, addedItems, removedItems);
        }

        private void MergedCollectionMap_ArtistItemsChanged(object sender, IReadOnlyList<CollectionChangedItem<ICoreArtistCollectionItem>> addedItems, IReadOnlyList<CollectionChangedItem<ICoreArtistCollectionItem>> removedItems)
        {
            MergedCollectionMap_ItemsChanged(sender, addedItems, removedItems);
        }

        private void MergedCollectionMap_AlbumItemsChanged(object sender, IReadOnlyList<CollectionChangedItem<ICoreAlbumCollectionItem>> addedItems, IReadOnlyList<CollectionChangedItem<ICoreAlbumCollectionItem>> removedItems)
        {
            MergedCollectionMap_ItemsChanged(sender, addedItems, removedItems);
        }

        private void MergedCollectionMap_ChildItemsChanged(object sender, IReadOnlyList<CollectionChangedItem<ICorePlayableCollectionGroup>> addedItems, IReadOnlyList<CollectionChangedItem<ICorePlayableCollectionGroup>> removedItems)
        {
            var changedItemsCount = addedItems.Count + removedItems.Count;

            Guard.IsGreaterThan(changedItemsCount, 0, nameof(changedItemsCount));

            MergedCollectionMap_ItemsChanged(sender, addedItems, removedItems);
        }

        private void MergedCollectionMap_PlaylistItemsChanged(object sender, IReadOnlyList<CollectionChangedItem<ICorePlaylistCollectionItem>> addedItems, IReadOnlyList<CollectionChangedItem<ICorePlaylistCollectionItem>> removedItems)
        {
            MergedCollectionMap_ItemsChanged(sender, addedItems, removedItems);
        }

        private void MergedCollectionMap_ItemsChanged<T>(object sender, IReadOnlyList<CollectionChangedItem<T>> addedItems, IReadOnlyList<CollectionChangedItem<T>> removedItems)
            where T : class, ICollectionItemBase, ICoreMember
        {
            Guard.IsGreaterThan(addedItems.Count + removedItems.Count, 0, "Total changed items count");

            lock (_mergedMappedData)
            {
                var addedMergedItems = ItemsAdded_CheckAddedItems(addedItems, sender);
                var removedMergedItems = ItemsChanged_CheckRemovedItems(removedItems);

                Guard.IsGreaterThan(_mergedMappedData.Count, 0, nameof(_mergedMappedData.Count));

                ItemsChanged?.Invoke(this, addedMergedItems, removedMergedItems);
                ItemsCountChanged?.Invoke(this, _mergedMappedData.Count);
            }
        }

        private List<CollectionChangedItem<TCollectionItem>> ItemsAdded_CheckAddedItems<T>(IReadOnlyList<CollectionChangedItem<T>> addedItems, object sender)
            where T : class, ICollectionItemBase, ICoreMember
        {
            var added = new List<CollectionChangedItem<TCollectionItem>>();
            var newItems = new List<IMergedMutable<TCoreCollectionItem>>();

            foreach (var item in addedItems)
            {
                var newItemsCount = newItems.Count;

                if (!(item.Data is TCoreCollectionItem collectionItemData))
                    return ThrowHelper.ThrowInvalidOperationException<List<CollectionChangedItem<TCollectionItem>>>($"{nameof(item.Data)} couldn't be cast to {nameof(TCoreCollectionItem)}.");

                // TODO: Sorting is not handled.
                var mappedData = new MappedData(item.Index, (TCoreCollection)sender, collectionItemData);
                var mergedImpl = MergeOrAdd(newItems, collectionItemData);

                _sortedMap.Add(mappedData);

                // If the number of items in this list changes, the item was not merged and should be emitted on the ItemsChanged event.
                if (newItemsCount != newItems.Count)
                {
                    _mergedMappedData.Add(new MergedMappedData(mergedImpl, new[] { mappedData }));
                    added.Add(new CollectionChangedItem<TCollectionItem>((TCollectionItem)mergedImpl, _mergedMappedData.Count - 1));
                }
            }

            return added;
        }

        private List<CollectionChangedItem<TCollectionItem>> ItemsChanged_CheckRemovedItems<T>(IReadOnlyList<CollectionChangedItem<T>> removedItems)
            where T : class, ICollectionItemBase, ICoreMember
        {
            var removed = new List<CollectionChangedItem<TCollectionItem>>();

            foreach (var item in removedItems)
            {
                var mappedData = _sortedMap.FirstOrDefault(x => x.OriginalIndex == item.Index && item.Data.SourceCore == x.SourceCollection.SourceCore);

                if (mappedData == null) continue;

                foreach (var mergedData in _mergedMappedData)
                {
                    foreach (var mergedSource in mergedData.CollectionItem.Cast<IMerged<TCoreCollectionItem>>().Sources)
                    {
                        if (mappedData.CollectionItem != mergedSource)
                            continue;

                        _sortedMap.Remove(mappedData);

                        mergedData.CollectionItem.RemoveSource(mergedSource);

                        mergedData.MergedMapData.RemoveAll(x => x.OriginalIndex == item.Index && item.Data.SourceCore == x.SourceCollection.SourceCore);

                        if (mergedData.CollectionItem.Cast<IMerged<TCoreCollectionItem>>().Sources.Count == 0)
                        {
                            _mergedMappedData.Remove(mergedData);

                            var index = _mergedMappedData.IndexOf(mergedData);
                            removed.Add(new CollectionChangedItem<TCollectionItem>((TCollectionItem)mergedData.CollectionItem, index));
                        }

                        return removed;
                    }
                }
            }

            return removed;
        }

        private void MergedCollectionMap_CountChanged(object sender, int e)
        {
            // This is sent from each core.
            // The count would be wrong if we tried to re-emit it as is.
            // We emit CountChanged (for the MergedCollectionMap) when items are changed.

            // TODO: Maybe we can use it this event verify the size of the collection is correct?
        }

        /// <summary>
        /// Gets a range of items from the collection, merged and sorted from multiple sources.
        /// </summary>
        /// <param name="limit">The max number of items to return.</param>
        /// <param name="offset">Get items starting at this index.</param>
        /// <returns>The requested range of items, sorted and merged from the sources in the input collection.</returns>
        public async Task<IReadOnlyList<TCollectionItem>> GetItemsAsync(int limit, int offset)
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

            Guard.IsNotNull(item, nameof(item));

            if (item is IInitialData createdData)
            {
                await InsertNewItem(Sources, _collection.GetSourceCores(), createdData, index);
                return;
            }

            // Handle inserting an existing item
            await InsertExistingItem(item, _sortedMap[index]);
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

                var isAvailable = await sourceCollection.IsRemoveAvailable(index);
                if (!isAvailable)
                    continue;

                switch (sourceCollection)
                {
                    case ICorePlayableCollectionGroup playableCollection:
                            await playableCollection.RemoveChildAsync(mappedData.OriginalIndex);
                        break;
                    case ICoreAlbumCollection albumCollection:
                            await albumCollection.RemoveAlbumItemAsync(mappedData.OriginalIndex);
                        break;
                    case ICoreArtistCollection artistCollection:
                            await artistCollection.RemoveArtistItemAsync(mappedData.OriginalIndex);
                        break;
                    case ICorePlaylistCollection playlistCollection:
                            await playlistCollection.RemovePlaylistItemAsync(mappedData.OriginalIndex);
                        break;
                    case ICoreTrackCollection trackCollection:
                            await trackCollection.AddTrackAsync((ICoreTrack)source, mappedData.OriginalIndex);
                        break;
                    case ICoreImageCollection imageCollection:
                            await imageCollection.RemoveImageAsync(mappedData.OriginalIndex);
                        break;
                    case ICoreGenreCollection genreCollection:
                            await genreCollection.RemoveGenreAsync(mappedData.OriginalIndex);
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
        public async Task<bool> IsAddItemAvailable(int index)
        {
            await TryInitAsync();

            var sourceResults = await _mergedMappedData[index].MergedMapData
                .InParallel(async x => await x.SourceCollection.IsAddAvailable(x.OriginalIndex));

            return sourceResults.Any();
        }

        /// <summary>
        /// Checks if removing an item from the sorted map is supported.
        /// </summary>
        /// <param name="index">The index to remove.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. Value indicates support.</returns>
        public async Task<bool> IsRemoveItemAvailable(int index)
        {
            await TryInitAsync();

            var sourceResults = await _mergedMappedData[index].MergedMapData
                .InParallel(async x => await x.SourceCollection.IsRemoveAvailable(x.OriginalIndex));

            return sourceResults.Any();
        }

        private static IMergedMutable<TCoreCollectionItem> MergeOrAdd(List<IMergedMutable<TCoreCollectionItem>> collection, TCoreCollectionItem itemToMerge)
        {
            foreach (var item in collection)
            {
                // ReSharper disable once SuspiciousTypeConversion.Global
                if (item.Equals(itemToMerge))
                {
                    item.AddSource(itemToMerge);
                    return item;
                }
            }

            IMergedMutable<TCoreCollectionItem>? returnData;

            // if the collection doesn't contain IMerged<TCollectionItem> at all, create a new Merged
            switch (itemToMerge)
            {
                case ICoreArtist artist:
                    returnData = (IMergedMutable<TCoreCollectionItem>)new MergedArtist(artist.IntoList());
                    collection.Add(returnData);
                    break;
                case ICoreAlbum album:
                    returnData = (IMergedMutable<TCoreCollectionItem>)new MergedAlbum(album.IntoList());
                    collection.Add(returnData);
                    break;
                case ICorePlaylist playlist:
                    returnData = (IMergedMutable<TCoreCollectionItem>)new MergedPlaylist(playlist.IntoList());
                    collection.Add(returnData);
                    break;
                case ICoreTrack track:
                    returnData = (IMergedMutable<TCoreCollectionItem>)new MergedTrack(track.IntoList());
                    collection.Add(returnData);
                    break;
                case ICoreDiscoverables discoverables:
                    returnData = (IMergedMutable<TCoreCollectionItem>)new MergedDiscoverables(discoverables.IntoList());
                    collection.Add(returnData);
                    break;
                case ICoreLibrary library:
                    returnData = (IMergedMutable<TCoreCollectionItem>)new MergedLibrary(library.IntoList());
                    collection.Add(returnData);
                    break;
                case ICoreRecentlyPlayed recentlyPlayed:
                    returnData = (IMergedMutable<TCoreCollectionItem>)new MergedRecentlyPlayed(recentlyPlayed.IntoList());
                    collection.Add(returnData);
                    break;
                case ICoreImage coreImage:
                    returnData = (IMergedMutable<TCoreCollectionItem>)new MergedImage(coreImage.IntoList());
                    collection.Add(returnData);
                    break;
                case ICoreGenre coreGenre:
                    returnData = (IMergedMutable<TCoreCollectionItem>)new MergedGenre(coreGenre.IntoList());
                    collection.Add(returnData);
                    break;

                // TODO: Search results post search redo (done, please do this)

                // Collections that are returned from other collections, but need their own separate ViewModels.
                // Example: an AlbumCollection can return either an Album or another AlbumCollection,
                // so we need ViewModels and Merged proxy classes for both.
                case ICorePlayableCollectionGroup playableCollection:
                    returnData = (IMergedMutable<TCoreCollectionItem>)new MergedPlayableCollectionGroup(playableCollection.IntoList());
                    collection.Add(returnData);
                    break;
                case ICoreAlbumCollection albumCollection:
                    returnData = (IMergedMutable<TCoreCollectionItem>)new MergedAlbumCollection(albumCollection.IntoList());
                    collection.Add(returnData);
                    break;
                case ICoreArtistCollection artistCollection:
                    returnData = (IMergedMutable<TCoreCollectionItem>)new MergedArtistCollection(artistCollection.IntoList());
                    collection.Add(returnData);
                    break;
                case ICorePlaylistCollection playlistCollection:
                    returnData = (IMergedMutable<TCoreCollectionItem>)new MergedPlaylistCollection(playlistCollection.IntoList());
                    collection.Add(returnData);
                    break;
                case ICoreTrackCollection trackCollection:
                    returnData = (IMergedMutable<TCoreCollectionItem>)new MergedTrackCollection(trackCollection.IntoList());
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
            Guard.IsNotNull(_coreInstanceRegistry, nameof(_coreInstanceRegistry));
            Guard.IsGreaterThan(_coreInstanceRegistry.Count, 0, nameof(_coreInstanceRegistry.Count));
            Guard.IsGreaterThan(limit, 0, nameof(limit));

            var mappedData = new List<MappedData>();
            if (_sortedMap.Count > 0)
            {
                mappedData = BuildSortedMapRanked(_sortedMap.Count);
            }
            else
            {
                mappedData = BuildSortedMapRanked();
            }

            _sortedMap.AddRange(mappedData);

            // If the offset exceeds the number of items we have, return nothing.
            if (offset >= _sortedMap.Count)
                return new List<TCollectionItem>();

            // If the total number of requested items exceeds the number of items we have, adjust the limit so it won't go out of range.
            if (offset + limit > _sortedMap.Count)
                limit = _sortedMap.Count - offset;

            // Get all requested items using the sorted map
            for (var i = 0; i < limit; i++)
            {
                var mappedIndex = offset + i;

                var currentSource = _sortedMap[mappedIndex];
                var itemsCountForSource = currentSource.SourceCollection.GetItemsCount<TCollection>();

                var itemLimitForSource = limit;

                // Get the max items from each source once per collection.
                // If the currentSource and the previous source are the same, skip this iteration.
                // Checking if mappedIndex > offset ensures that the request is made at the first mapped item for this source.
                if (mappedIndex > offset && currentSource.SourceCollection.SourceCore == _sortedMap[mappedIndex - 1].SourceCollection.SourceCore)
                    continue;

                // do we end up outside the range if we try getting all items from this source?
                if (currentSource.OriginalIndex + limit > itemsCountForSource)
                {
                    // If so, reduce limit so it only gets the remaining items in this collection.
                    itemLimitForSource = itemsCountForSource - currentSource.OriginalIndex;
                }

                var remainingItemsForSource = await OwlCore.APIs.GetAllItemsAsync<TCoreCollectionItem>(
                    itemLimitForSource, // Try to get as many items as possible for each page.
                    currentSource.OriginalIndex,
                    async currentOffset => await currentSource.SourceCollection.GetItems<TCoreCollection, TCoreCollectionItem>(itemLimitForSource, currentOffset).ToListAsync().AsTask());

                Guard.IsNotNull(remainingItemsForSource, nameof(remainingItemsForSource));

                // Blindly getting as many items as possible will probably cause it to get more than the limit
                if (remainingItemsForSource.Count > itemLimitForSource)
                {
                    remainingItemsForSource = remainingItemsForSource.Take(itemLimitForSource).ToList();
                }

                // For each item that we just retrieved, find the index in the sorted maps and assign the item.
                for (var o = 0; o < remainingItemsForSource.Count; o++)
                {
                    var item = remainingItemsForSource[o];

                    Guard.IsNotNull(item, nameof(item));

                    _sortedMap[mappedIndex + o].CollectionItem = item;
                }
            }

            lock (_sortedMap)
            {
                var allItemsWithData = MergeMappedData(_sortedMap.ToArray());

#warning TODO Re-do of merged collection item handling. 
                // Since we don't get all items from the API, we don't know which are merged until we get the data, causing the count to be off.
                // This problem may require a fundamental re-think of how we handle collection items,
                // likely getting and processing the entire collection before emitting any items count
                // or something simpler but smarter, like sparsely processing and adjusting the count as you get items.
                // Until then, supply the maximum possible count (as if no items are merged).
                ItemsCountChanged?.Invoke(this, _sortedMap.Count);

                var relevantMergedMappedData = allItemsWithData.Skip(offset).Take(limit);
                var merged = relevantMergedMappedData.Select(x => (TCollectionItem)x).ToList();

                return merged;
            }
        }

        private List<IMergedMutable<TCoreCollectionItem>> MergeMappedData(IList<MappedData> sortedData)
        {
            var returnedData = new List<IMergedMutable<TCoreCollectionItem>>();
            var mergedItemMaps = new Dictionary<IMergedMutable<TCoreCollectionItem>, List<MappedData>>();

            foreach (var item in sortedData)
            {
                if (item.CollectionItem is null)
                    continue;

                var mergedInto = MergeOrAdd(returnedData, item.CollectionItem);

                bool exists = mergedItemMaps.TryGetValue(mergedInto, out List<MappedData> mergedMapItems);

                mergedMapItems ??= new List<MappedData>();
                mergedMapItems.Add(item);

                if (!exists)
                    mergedItemMaps.Add(mergedInto, mergedMapItems);
            }
          
            foreach (var item in mergedItemMaps)
                _mergedMappedData.Add(new MergedMappedData(item.Key, item.Value));

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

        private List<MappedData> BuildSortedMapRanked(int offset = 0)
        {
            Guard.IsNotNull(_coreRanking, nameof(_coreRanking));
            Guard.IsNotNull(_coreInstanceRegistry, nameof(_coreInstanceRegistry));
            Guard.IsGreaterThan(_coreInstanceRegistry.Count, 0, nameof(_coreInstanceRegistry.Count));

            // Rank the sources by core
            var rankedSources = new List<TCoreCollection>();
            foreach (var instanceId in _coreRanking)
            {
                var coreAssemblyInfo = _coreInstanceRegistry.FirstOrDefault(x => x.Key == instanceId).Value;
                if (coreAssemblyInfo is null)
                    continue;

                var coreType = Type.GetType(coreAssemblyInfo.AttributeData.CoreTypeAssemblyQualifiedName);

                var source = Sources.FirstOrDefault(x => x.SourceCore.GetType() == coreType);

                // A core that is in the core ranking might not be part of the sources for this object
                if (source is null)
                    continue;

                rankedSources.Add(source);
            }

            // Create the map for each possible item returned from a source collection.
            var itemsMap = new List<MappedData>();

            foreach (var source in rankedSources)
            {
                var itemsCount = source.GetItemsCount<TCollection>();

                for (var i = offset; i < itemsCount; i++)
                {
                    itemsMap.Add(new MappedData(i, source));
                }
            }

            return itemsMap;
        }

        private Task<List<string>> GetCoreRankings()
        {
            return _settingsService.GetValue<List<string>>(nameof(SettingsKeys.CoreRanking));
        }

        private Task<Dictionary<string, CoreAssemblyInfo>> GetConfiguredCoreRegistry()
        {
            return _settingsService.GetValue<Dictionary<string, CoreAssemblyInfo>>(nameof(SettingsKeys.CoreInstanceRegistry));
        }

        private Task<MergedCollectionSorting> GetSortingMethod()
        {
            return Task.FromResult(MergedCollectionSorting.Ranked);

            //return _settingsService.GetValue<MergedCollectionSorting>(nameof(SettingsKeys.MergedCollectionSorting));
        }

        private void SettingsServiceOnSettingChanged(object sender, SettingChangedEventArgs e)
        {
            switch (e.Key)
            {
                case nameof(SettingsKeys.CoreRanking):
                    Guard.IsNotNull(e.Value, nameof(e.Value));
                    _coreRanking = e.Value as IReadOnlyList<string>;
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

            for (int i = 0; i < previouslySortedItems.Count; i++)
            {
                var item = previouslySortedItems[i];

                // If the currentSource and the previous source are the same, skip this iteration
                // because we get and re-emit the range of items for this source.
                if (i > 0 && item.SourceCollection.SourceCore == _sortedMap[i - 1].SourceCollection.SourceCore)
                    continue;

                // The items retrieved will exist in the sorted map.
                await GetItemsAsync(item.OriginalIndex, i);
            }

            var addedItems = new List<CollectionChangedItem<TCollectionItem>>();

            // For each item that we just retrieved, find the index in the sorted map and assign the item.
            for (var i = 0; i < _mergedMappedData.Count; i++)
            {
                var addedItem = _mergedMappedData[i];

                Guard.IsNotNull(addedItem.CollectionItem, nameof(addedItem.CollectionItem));

                var x = new CollectionChangedItem<TCollectionItem>((TCollectionItem)addedItem.CollectionItem, i);
                addedItems.Add(x);
            }

            // logic for removed was copy-pasted and tweaked from the added logic. Not checked or tested.
            var removedItems = new List<CollectionChangedItem<TCollectionItem>>();

            for (var i = 0; i < previousMergedMap.Count; i++)
            {
                var removedItem = previousMergedMap[i];

                Guard.IsNotNull(removedItem.CollectionItem, nameof(removedItem.CollectionItem));

                var x = new CollectionChangedItem<TCollectionItem>((TCollectionItem)removedItem.CollectionItem, i);
                removedItems.Add(x);
            }

            if (addedItems.Count > 0 || removedItems.Count > 0)
            {
                ItemsChanged?.Invoke(this, addedItems, removedItems);
                ItemsCountChanged?.Invoke(this, _mergedMappedData.Count);
            }
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
        void IMergedMutable<TCoreCollection>.AddSource(TCoreCollection itemToMerge)
        {
            ResetDataRanked().Forget();
        }

        /// <inheritdoc />
        void IMergedMutable<TCoreCollection>.RemoveSource(TCoreCollection itemToRemove)
        {
            ResetDataRanked().Forget();
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
            public MergedMappedData(IMergedMutable<TCoreCollectionItem> collectionItem, IEnumerable<MappedData> mergedMapData)
            {
                CollectionItem = collectionItem;
                MergedMapData = mergedMapData.ToList();
            }

            public IMergedMutable<TCoreCollectionItem> CollectionItem { get; }

            public List<MappedData> MergedMapData { get; }
        }

        /// <inheritdoc />
        public ValueTask DisposeAsync()
        {
            DetachEvents();
            _mergedMappedData.Clear();
            _sortedMap.Clear();

            return default;
        }
    }
}