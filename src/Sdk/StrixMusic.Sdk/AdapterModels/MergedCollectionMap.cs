// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using OwlCore.Events;
using OwlCore.Extensions;
using OwlCore.Provisos;
using StrixMusic.Sdk.BaseModels;
using StrixMusic.Sdk.CoreModels;
using StrixMusic.Sdk.Extensions;
using StrixMusic.Sdk.Models;

namespace StrixMusic.Sdk.AdapterModels
{
    /// <summary>
    /// Manages the merging of multiple <typeparamref name="TCoreCollection"/>s and presents them as single <typeparamref name="TCollection"/>.
    /// </summary>
    /// <typeparam name="TCollection">The collection type that this is part of.</typeparam>
    /// <typeparam name="TCoreCollection">The types of items that were merged to form <typeparamref name="TCollection"/>.</typeparam>
    /// <typeparam name="TCollectionItem">The type of the item returned from the merged collection.</typeparam>
    /// <typeparam name="TCoreCollectionItem">The type of the items returned from the original source collections.</typeparam>
    internal sealed class MergedCollectionMap<TCollection, TCoreCollection, TCollectionItem, TCoreCollectionItem>
        : IMerged<TCoreCollection>, IMergedMutable<TCoreCollection>, IAsyncInit, IAsyncDisposable
            where TCollection : class, ICollectionBase, IMerged<TCoreCollection>
            where TCoreCollection : class, ICoreCollection
            where TCollectionItem : class, ICollectionItemBase, IMerged<TCoreCollectionItem>
            where TCoreCollectionItem : class, ICollectionItemBase, ICoreMember
    {
        // ReSharper disable StaticMemberInGenericType
        private static bool _isInitialized;
        private static TaskCompletionSource<bool>? _initCompletionSource;

        private readonly SemaphoreSlim _disposeSemaphore = new(1, 1);

        private readonly TCollection _collection;
        private readonly MergedCollectionConfig _config;

        /// <summary>
        /// A map where each index contains the representation of an item returned from a source collection, where the value is that source collection.
        /// </summary>
        private readonly List<MappedData> _sortedMap = new();
        private readonly List<MergedMappedData> _mergedMappedData = new();

        private bool _isDisposed;

        /// <inheritdoc />
        public IReadOnlyList<TCoreCollection> Sources => _collection.Sources;

        /// <inheritdoc />
        public IReadOnlyList<ICore> SourceCores => _collection.SourceCores;

        /// <summary>
        /// Initializes a new instance of <see cref="MergedCollectionMap{TCollection, TCoreCollection, TCollectionItem, TCoreCollectionItem}"/>.
        /// </summary>
        /// <param name="collection">The collection that contains the items </param>
        /// <param name="config">Configurable options for this merged collection map.</param>
        public MergedCollectionMap(TCollection collection, MergedCollectionConfig? config = null)
        {
            _collection = collection;
            _config = config ?? new MergedCollectionConfig();
            AttachEvents();
        }

        private static async Task InsertItemIntoCollectionAsync<T>(TCoreCollection sourceCollection, T itemToAdd, int originalIndex, CancellationToken cancellationToken)
            where T : class, ICollectionItemBase, ICoreMember // https://twitter.com/Arlodottxt/status/1351317100959326213?s=20
        {
            switch (sourceCollection)
            {
                case ICorePlayableCollectionGroup playableCollection:
                    if (await playableCollection.IsAddChildAvailableAsync(originalIndex, cancellationToken))
                        await playableCollection.AddChildAsync((ICorePlayableCollectionGroup)itemToAdd, originalIndex, cancellationToken);
                    break;
                case ICoreAlbumCollection albumCollection:
                    if (await albumCollection.IsAddAlbumItemAvailableAsync(originalIndex, cancellationToken))
                        await albumCollection.AddAlbumItemAsync((ICoreAlbumCollectionItem)itemToAdd, originalIndex, cancellationToken);
                    break;
                case ICoreArtistCollection artistCollection:
                    if (await artistCollection.IsAddArtistItemAvailableAsync(originalIndex, cancellationToken))
                        await artistCollection.AddArtistItemAsync((ICoreArtistCollectionItem)itemToAdd, originalIndex, cancellationToken);
                    break;
                case ICorePlaylistCollection playlistCollection:
                    if (await playlistCollection.IsAddPlaylistItemAvailableAsync(originalIndex, cancellationToken))
                        await playlistCollection.AddPlaylistItemAsync((ICorePlaylistCollectionItem)itemToAdd, originalIndex, cancellationToken);
                    break;
                case ICoreTrackCollection trackCollection:
                    if (await trackCollection.IsAddTrackAvailableAsync(originalIndex, cancellationToken))
                        await trackCollection.AddTrackAsync((ICoreTrack)itemToAdd, originalIndex, cancellationToken);
                    break;
                case ICoreImageCollection imageCollection:
                    if (await imageCollection.IsAddImageAvailableAsync(originalIndex, cancellationToken))
                        await imageCollection.AddImageAsync((ICoreImage)itemToAdd, originalIndex, cancellationToken);
                    break;
                case ICoreGenreCollection genreCollection:
                    if (await genreCollection.IsAddGenreAvailableAsync(originalIndex, cancellationToken))
                        await genreCollection.AddGenreAsync((ICoreGenre)itemToAdd, originalIndex, cancellationToken);
                    break;
                case ICoreUrlCollection urlCollection:
                    if (await urlCollection.IsAddUrlAvailableAsync(originalIndex, cancellationToken))
                        await urlCollection.AddUrlAsync((ICoreUrl)itemToAdd, originalIndex, cancellationToken);
                    break;
                default:
                    ThrowHelper.ThrowNotSupportedException<IMergedMutable<TCoreCollection>>($"Couldn't add item to collection. Type {sourceCollection.GetType()} not supported.");
                    break;
            }
        }

        private static async Task InsertExistingItemAsync(TCollectionItem itemToInsert, MappedData mappedData, CancellationToken cancellationToken)
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

                await InsertItemIntoCollectionAsync(sourceCollection, source, originalIndex, cancellationToken);
            }
        }

        private static async Task InsertNewItemAsync(IEnumerable<TCoreCollection> sourceCollections, IReadOnlyList<ICore> sourceCores, IInitialData dataToInsert, int index, CancellationToken cancellationToken = default)
        {
            // TODO create setting to let user decide default
            foreach (var source in sourceCollections)
            {
                IEnumerable<ICore> targetSources = sourceCores;

                if (dataToInsert.TargetSourceCores is { Count: > 0 })
                {
                    targetSources = dataToInsert.TargetSourceCores;
                }

                // Try adding to all by default
                foreach (var targetCore in targetSources)
                {
                    if (dataToInsert is InitialPlaylistData playlistData)
                    {
                        var coreData = new InitialCorePlaylistData(playlistData, targetCore);

                        await InsertItemIntoCollectionAsync(source, coreData, index, cancellationToken);
                    }
                }
            }
        }

        /// <inheritdoc />
        public async Task InitAsync(CancellationToken cancellationToken = default)
        {
            if (IsInitialized)
                return;

            if (_initCompletionSource?.Task.Status is TaskStatus.Running or TaskStatus.WaitingForActivation or TaskStatus.RanToCompletion)
            {
                await _initCompletionSource.Task;
                return;
            }

            _initCompletionSource = new TaskCompletionSource<bool>();

            _config.CoreRankingChanged += ConfigOnCoreRankingChanged;
            _config.MergedCollectionSortingChanged += ConfigOnMergedCollectionSortingChanged;

            Guard.HasSizeGreaterThan(_config.CoreRanking, 0, nameof(_config.CoreRanking));

            _initCompletionSource.SetResult(true);
            IsInitialized = true;
        }

        /// <inheritdoc />
        public bool IsInitialized
        {
            get => _isInitialized;
            set => _isInitialized = value;
        }

        private Task TryInitAsync(CancellationToken cancellationToken) => InitAsync(cancellationToken);

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
                ((ICoreTrackCollection)item).TracksCountChanged += MergedCollectionMap_CountChanged;
                ((ICoreTrackCollection)item).TracksChanged += MergedCollectionMap_TrackItemsChanged;
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
            else if (typeof(TCoreCollection) == typeof(ICoreUrlCollection))
            {
                ((ICoreUrlCollection)item).UrlsCountChanged += MergedCollectionMap_CountChanged;
                ((ICoreUrlCollection)item).UrlsChanged += MergedCollectionMap_UrlsChanged;
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
                ((ICoreTrackCollection)item).TracksCountChanged -= MergedCollectionMap_CountChanged;
                ((ICoreTrackCollection)item).TracksChanged -= MergedCollectionMap_TrackItemsChanged;
            }
            else if (typeof(TCoreCollection) == typeof(ICoreImageCollection))
            {
                ((ICoreImageCollection)item).ImagesCountChanged -= MergedCollectionMap_CountChanged;
                ((ICoreImageCollection)item).ImagesChanged -= MergedCollectionMap_ImagesChanged;
            }
            else if (typeof(TCoreCollection) == typeof(ICoreGenreCollection))
            {
                ((ICoreGenreCollection)item).GenresCountChanged -= MergedCollectionMap_CountChanged;
                ((ICoreGenreCollection)item).GenresChanged -= MergedCollectionMap_GenresChanged;
            }
            else if (typeof(TCoreCollection) == typeof(ICoreUrlCollection))
            {
                ((ICoreUrlCollection)item).UrlsCountChanged -= MergedCollectionMap_CountChanged;
                ((ICoreUrlCollection)item).UrlsChanged -= MergedCollectionMap_UrlsChanged;
            }
            else
            {
                ThrowHelper.ThrowNotSupportedException<IMerged<TCoreCollection>>(
                    "Couldn't detach events. Type not supported.");
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

        private void MergedCollectionMap_UrlsChanged(object sender, IReadOnlyList<CollectionChangedItem<ICoreUrl>> addedItems, IReadOnlyList<CollectionChangedItem<ICoreUrl>> removedItems)
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
                var mergedImpl = MergeOrAdd(newItems, collectionItemData, _config);

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

            if (_sortedMap.Count == 0)
                return removed;

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
            // The count would be wrong if we tried to re-emit it as is due to merging.
            // We emit CountChanged (for the MergedCollectionMap) when items are changed.

            // TODO: Maybe we can use it this event verify the size of the collection is correct?
        }

        /// <summary>
        /// Gets a range of items from the collection, merged and sorted from multiple sources.
        /// </summary>
        /// <param name="limit">The max number of items to return.</param>
        /// <param name="offset">Get items starting at this index.</param>
        /// <param name="cancellationToken">A cancellation token that may be used to cancel the ongoing task.</param>
        /// <returns>The requested range of items, sorted and merged from the sources in the input collection.</returns>
        public async Task<IReadOnlyList<TCollectionItem>> GetItemsAsync(int limit, int offset, CancellationToken cancellationToken = default)
        {
            await TryInitAsync(cancellationToken);

            return _config.MergedCollectionSorting switch
            {
                MergedCollectionSorting.Ranked => await GetItemsByRank(limit, offset, cancellationToken),
                _ => ThrowHelper.ThrowNotSupportedException<IReadOnlyList<TCollectionItem>>($"Merged collection sorting by \"{_config.MergedCollectionSorting}\" not supported.")
            };
        }

        /// <summary>
        /// Inserts an item into the compatible source collections on the backend.
        /// </summary>
        /// <param name="item">The item to insert.</param>
        /// <param name="index">The index to place this item at.</param>
        /// <param name="cancellationToken">A cancellation token that may be used to cancel the ongoing task.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task InsertItemAsync(TCollectionItem item, int index, CancellationToken cancellationToken = default)
        {
            await TryInitAsync(cancellationToken);

            Guard.IsNotNull(item, nameof(item));

            if (item is IInitialData createdData)
            {
                await InsertNewItemAsync(Sources, _collection.GetSourceCores(), createdData, index, cancellationToken);
                return;
            }

            // Handle inserting an existing item
            await InsertExistingItemAsync(item, _sortedMap[index], cancellationToken);
        }

        /// <summary>
        /// Inserts an item into the compatible source collections on the backend.
        /// </summary>
        /// <param name="index">The index to place this item at.</param>
        /// <param name="cancellationToken">A cancellation token that may be used to cancel the ongoing task.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task RemoveAtAsync(int index, CancellationToken cancellationToken = default)
        {
            await TryInitAsync(cancellationToken);

            // Externally, the app sees non-core items as this internal collection of merged and sorted items and data.
            // When they ask for an item at an index, they're asking for an item at that index that could be merged.
            // So we go through each of the mapped sources for the item at this index and handle removing from the core side.
            var targetItem = _mergedMappedData[index];

            foreach (var mappedData in targetItem.MergedMapData)
            {
                Guard.IsNotNull(mappedData.CollectionItem, nameof(mappedData.CollectionItem));

                var sourceCollection = mappedData.SourceCollection;
                var source = mappedData.CollectionItem;

                var isAvailable = await sourceCollection.IsRemoveAvailable(index, cancellationToken);
                if (!isAvailable)
                    continue;

                switch (sourceCollection)
                {
                    case ICorePlayableCollectionGroup playableCollection:
                        await playableCollection.RemoveChildAsync(mappedData.OriginalIndex, cancellationToken);
                        break;
                    case ICoreAlbumCollection albumCollection:
                        await albumCollection.RemoveAlbumItemAsync(mappedData.OriginalIndex, cancellationToken);
                        break;
                    case ICoreArtistCollection artistCollection:
                        await artistCollection.RemoveArtistItemAsync(mappedData.OriginalIndex, cancellationToken);
                        break;
                    case ICorePlaylistCollection playlistCollection:
                        await playlistCollection.RemovePlaylistItemAsync(mappedData.OriginalIndex, cancellationToken);
                        break;
                    case ICoreTrackCollection trackCollection:
                        await trackCollection.RemoveTrackAsync(mappedData.OriginalIndex, cancellationToken);
                        break;
                    case ICoreImageCollection imageCollection:
                        await imageCollection.RemoveImageAsync(mappedData.OriginalIndex, cancellationToken);
                        break;
                    case ICoreGenreCollection genreCollection:
                        await genreCollection.RemoveGenreAsync(mappedData.OriginalIndex, cancellationToken);
                        break;
                    case ICoreUrlCollection urlCollection:
                        await urlCollection.RemoveUrlAsync(mappedData.OriginalIndex, cancellationToken);
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
        /// <param name="cancellationToken">A cancellation token that may be used to cancel the ongoing task.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. Value indicates support.</returns>
        public async Task<bool> IsAddItemAvailableAsync(int index, CancellationToken cancellationToken = default)
        {
            await TryInitAsync(cancellationToken);

            var sourceResults = await _mergedMappedData[index].MergedMapData
                .InParallel(async x => await x.SourceCollection.IsAddAvailable(x.OriginalIndex, cancellationToken));

            return sourceResults.Any();
        }

        /// <summary>
        /// Checks if removing an item from the sorted map is supported.
        /// </summary>
        /// <param name="index">The index to remove.</param>
        /// <param name="cancellationToken">A cancellation token that may be used to cancel the ongoing task.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. Value indicates support.</returns>
        public async Task<bool> IsRemoveItemAvailableAsync(int index, CancellationToken cancellationToken = default)
        {
            await TryInitAsync(cancellationToken);

            var sourceResults = await _mergedMappedData[index].MergedMapData
                .InParallel(async x => await x.SourceCollection.IsRemoveAvailable(x.OriginalIndex, cancellationToken));

            return sourceResults.Any();
        }

        private static IMergedMutable<TCoreCollectionItem> MergeOrAdd(List<IMergedMutable<TCoreCollectionItem>> collection, TCoreCollectionItem itemToMerge, MergedCollectionConfig config)
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
                    returnData = (IMergedMutable<TCoreCollectionItem>)new MergedArtist(artist.IntoList(), config);
                    collection.Add(returnData);
                    break;
                case ICoreAlbum album:
                    returnData = (IMergedMutable<TCoreCollectionItem>)new MergedAlbum(album.IntoList(), config);
                    collection.Add(returnData);
                    break;
                case ICorePlaylist playlist:
                    returnData = (IMergedMutable<TCoreCollectionItem>)new MergedPlaylist(playlist.IntoList(), config);
                    collection.Add(returnData);
                    break;
                case ICoreTrack track:
                    returnData = (IMergedMutable<TCoreCollectionItem>)new MergedTrack(track.IntoList(), config);
                    collection.Add(returnData);
                    break;
                case ICoreDiscoverables discoverables:
                    returnData = (IMergedMutable<TCoreCollectionItem>)new MergedDiscoverables(discoverables.IntoList(), config);
                    collection.Add(returnData);
                    break;
                case ICoreLibrary library:
                    returnData = (IMergedMutable<TCoreCollectionItem>)new MergedLibrary(library.IntoList(), config);
                    collection.Add(returnData);
                    break;
                case ICoreRecentlyPlayed recentlyPlayed:
                    returnData = (IMergedMutable<TCoreCollectionItem>)new MergedRecentlyPlayed(recentlyPlayed.IntoList(), config);
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
                case ICoreUrl coreUrl:
                    returnData = (IMergedMutable<TCoreCollectionItem>)new MergedUrl(coreUrl.IntoList());
                    collection.Add(returnData);
                    break;

                // TODO: Search results post search redo (done, please do this)

                // Collections that are returned from other collections, but need their own separate ViewModels.
                // Example: an AlbumCollection can return either an Album or another AlbumCollection,
                // so we need ViewModels and Merged proxy classes for both.
                case ICorePlayableCollectionGroup playableCollection:
                    returnData = (IMergedMutable<TCoreCollectionItem>)new MergedPlayableCollectionGroup(playableCollection.IntoList(), config);
                    collection.Add(returnData);
                    break;
                case ICoreAlbumCollection albumCollection:
                    returnData = (IMergedMutable<TCoreCollectionItem>)new MergedAlbumCollection(albumCollection.IntoList(), config);
                    collection.Add(returnData);
                    break;
                case ICoreArtistCollection artistCollection:
                    returnData = (IMergedMutable<TCoreCollectionItem>)new MergedArtistCollection(artistCollection.IntoList(), config);
                    collection.Add(returnData);
                    break;
                case ICorePlaylistCollection playlistCollection:
                    returnData = (IMergedMutable<TCoreCollectionItem>)new MergedPlaylistCollection(playlistCollection.IntoList(), config);
                    collection.Add(returnData);
                    break;
                case ICoreTrackCollection trackCollection:
                    returnData = (IMergedMutable<TCoreCollectionItem>)new MergedTrackCollection(trackCollection.IntoList(), config);
                    collection.Add(returnData);
                    break;
                default:
                    // Replace throw with this when verified that this is fully finished.
                    // ThrowHelper.ThrowNotSupportedException<IMerged<TCoreCollection>>("Couldn't create merged item. Type not supported.");
                    throw new NotImplementedException();
            }

            return returnData;
        }

        private async Task<IReadOnlyList<TCollectionItem>> GetItemsByRank(int limit, int offset, CancellationToken cancellationToken = default)
        {
            Guard.IsGreaterThan(_config.CoreRanking.Count, 0, nameof(_config.CoreRanking.Count));
            Guard.IsGreaterThan(limit, 0, nameof(limit));

            var mappedData = BuildSortedMapRanked(_sortedMap.Count);

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
                    async currentOffset => await currentSource.SourceCollection.GetItems<TCoreCollection, TCoreCollectionItem>(itemLimitForSource, currentOffset).ToListAsync(cancellationToken).AsTask());

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
                // Initial item count == the item count for all sources combined
                // Interacting with _sortedMap is treating as though all items are included but nothing is merged.
                var allItemsWithData = MergeMappedData(_sortedMap.Skip(offset).Take(limit).ToArray());

                // TODO Re-do of merged collection item handling.

                // Since we don't get all items from the API, we don't know which are merged until we get the data, causing the count to be off.
                // This problem may require a fundamental re-think of how we handle collection items,
                // likely getting and processing the entire collection before emitting any items count
                // or something simpler but smarter, like sparsely processing and adjusting the count as you get items.
                // Until then, supply the maximum possible count (as if no items are merged).
                ItemsCountChanged?.Invoke(this, _sortedMap.Count);

                var merged = allItemsWithData.Select(x => (TCollectionItem)x).ToList();

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

                var mergedInto = MergeOrAdd(returnedData, item.CollectionItem, _config);

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

        private List<MappedData> BuildSortedMap() => _config.MergedCollectionSorting switch
        {
            MergedCollectionSorting.Ranked => BuildSortedMapRanked(),
            _ => throw new NotSupportedException($"Merged collection sorting by \"{_config.MergedCollectionSorting}\" not supported.")
        };

        private List<MappedData> BuildSortedMapRanked(int offset = 0)
        {
            // Rank the sources by core
            var rankedSources = new List<TCoreCollection>();
            foreach (var instanceId in _config.CoreRanking)
            {
                // Find source by instance id.
                var source = Sources.FirstOrDefault(x => x.SourceCore.InstanceId == instanceId);

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

        private Task<MergedCollectionSorting> GetSortingMethod()
        {
            return Task.FromResult(MergedCollectionSorting.Ranked);

            //return _settingsService.GetValue<MergedCollectionSorting>(nameof(SettingsKeys.MergedCollectionSorting));
        }

        private void ConfigOnMergedCollectionSortingChanged(object sender, MergedCollectionSorting e)
        {
            throw new NotImplementedException();
        }

        private void ConfigOnCoreRankingChanged(object sender, IReadOnlyList<string> e)
        {
            throw new NotImplementedException();
        }

        private async Task ResetDataRanked()
        {
            await TryInitAsync(CancellationToken.None);

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
                await GetItemsAsync(item.OriginalIndex, i, CancellationToken.None);
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
            #warning TODO: AddSource and RemoveSource needs to be async.
            OwlCore.Flow.Catch(() => ResetDataRanked());
        }

        /// <inheritdoc />
        void IMergedMutable<TCoreCollection>.RemoveSource(TCoreCollection itemToRemove)
        {
            OwlCore.Flow.Catch(() => ResetDataRanked());
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
        public async ValueTask DisposeAsync()
        {
            if (_isDisposed)
                return;

            using (await OwlCore.Flow.EasySemaphore(_disposeSemaphore))
            {
                if (_isDisposed)
                    return;

                DetachEvents();
                _mergedMappedData.Clear();
                _sortedMap.Clear();

                _isDisposed = true;

                return;
            }
        }
    }
}
