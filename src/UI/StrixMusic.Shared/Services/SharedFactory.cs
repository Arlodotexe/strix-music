using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OwlCore.Collections;
using StrixMusic.CustomClasses.Collections;
using StrixMusic.Sdk.Services;

namespace StrixMusic.Services
{
    /// <inheritdoc cref="ISharedFactory" />
    public class SharedFactory : ISharedFactory
    {
        /// <inheritdoc />
        public SynchronizedObservableCollection<T> GetIncrementalCollection<T>(int take, Func<int, Task<List<T>>> loadMoreItems, Action onBatchStart, Action<List<T>> onBatchComplete)
        {
            return new IncrementalLoadingCollection<T>(take, loadMoreItems, onBatchComplete, onBatchStart);
        }

        /// <inheritdoc />
        public SynchronizedObservableCollection<T> GetIncrementalCollection<T>(int take, Func<int, Task<List<T>>> loadMoreItems, Action<List<T>> onBatchComplete)
        {
            return new IncrementalLoadingCollection<T>(take, loadMoreItems, onBatchComplete);
        }
    }
}
