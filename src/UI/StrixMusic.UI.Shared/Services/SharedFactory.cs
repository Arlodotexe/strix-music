using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LaunchPad.Collections;
using OwlCore.Collections;
using OwlCore.Helpers;
using StrixMusic.Sdk.Services;

namespace StrixMusic.Shared.Services
{
    /// <inheritdoc cref="ISharedFactory" />
    public class SharedFactory : ISharedFactory
    {
        /// <inheritdoc />
        public SynchronizedObservableCollection<T> GetIncrementalCollection<T>(int take, Func<int, Task<List<T>>> loadMoreItems, Action onBatchStart, Action<List<T>> onBatchComplete)
        {
            return Threading.InvokeOnUI(() => new IncrementalLoadingCollection<T>(take, loadMoreItems, onBatchComplete, onBatchStart));
        }

        /// <inheritdoc />
        public SynchronizedObservableCollection<T> GetIncrementalCollection<T>(int take, Func<int, Task<List<T>>> loadMoreItems, Action<List<T>> onBatchComplete)
        {
            return Threading.InvokeOnUI(() => new IncrementalLoadingCollection<T>(take, loadMoreItems, onBatchComplete));
        }

        /// <inheritdoc />
        public SynchronizedObservableCollection<T> GetIncrementalCollection<T>(int take, Func<int, Task<List<T>>> loadMoreItems)
        {
            return Threading.InvokeOnUI(() => new IncrementalLoadingCollection<T>(take, loadMoreItems));
        }
    }
}
