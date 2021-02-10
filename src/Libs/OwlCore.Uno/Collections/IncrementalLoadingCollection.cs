using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using OwlCore.Collections;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace OwlCore.Uno.Collections
{
    /// <summary>
    /// An collection that supports incremental loading.
    /// </summary>
    /// <remarks>Code adapted from <see href="https://stackoverflow.com/a/20262636"/></remarks>
    /// <typeparam name="T">The held type in the underlying <see cref="ObservableCollection{T}"/></typeparam>
    public class IncrementalLoadingCollection<T>
        : SynchronizedObservableCollection<T>, ISupportIncrementalLoading
    {
        private readonly Func<int, Task<List<T>>> _loadMoreItems;
        private readonly Action<List<T>>? _onBatchComplete;
        private readonly Action? _onBatchStart;

        /// <summary>
        /// How many records to currently skip
        /// </summary>
        private int Skip { get; set; }

        /// <summary>
        /// The max number of items to get per batch
        /// </summary>
        private int Take { get; set; }

        /// <summary>
        /// The number of items in the last batch retrieved
        /// </summary>
        private int VirtualCount { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="IncrementalLoadingCollection{T}"/> class.
        /// </summary>
        /// <param name="take">How many items to take per batch</param>
        /// <param name="loadMoreItems">The load more items function</param>
        /// <param name="onBatchComplete">The action to fire when the items are done being retrieved.</param>
        /// <param name="onBatchStart">The action to fire when the <see cref="LoadMoreItemsAsync"/> is called.</param>
        public IncrementalLoadingCollection(int take, Func<int, Task<List<T>>> loadMoreItems, Action<List<T>>? onBatchComplete = null, Action? onBatchStart = null)
            : base(SynchronizationContext.Current)
        {
            Take = take;
            _loadMoreItems = loadMoreItems;
            _onBatchStart = onBatchStart;
            _onBatchComplete = onBatchComplete;
            VirtualCount = take;
        }

        /// <summary>
        /// Returns whether there are more items (if the current batch size is equal to the amount retrieved then YES)
        /// </summary>
        public bool HasMoreItems => VirtualCount >= Take;

        /// <summary>
        /// Loads more items into the collection.
        /// </summary>
        /// <param name="count"></param>
        /// <returns>An asynchronous operation that results in the <see cref="LoadMoreItemsResult"/>.</returns>
        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            CoreDispatcher dispatcher = Window.Current.Dispatcher;
            _onBatchStart?.Invoke(); // This is the UI thread

            return Task.Run(async () =>
                 {
                     var result = await _loadMoreItems(Skip);
                     VirtualCount = result.Count;
                     Skip += Take;

                     foreach (T item in result)
                         Add(item);

                     await dispatcher.RunAsync(
                         CoreDispatcherPriority.Normal,
                         () =>
                         {
                             _onBatchComplete?.Invoke(result); // This is the UI thread
                         });

                     return new LoadMoreItemsResult
                     {
                         Count = (uint)result.Count,
                     };
                 })
                .AsAsyncOperation();
        }
    }
}
