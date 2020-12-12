using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OwlCore.AbstractStorage;
using OwlCore.Collections;

namespace StrixMusic.Sdk.Services
{
    /// <summary>
    /// A service that handles building various objects across projects.
    /// </summary>
    public interface ISharedFactory
    {
        /// <summary>
        /// Constructs a new IncrementalLoadingCollection given the parameters.
        /// </summary>
        /// <typeparam name="T">The type of the items held in the collection.</typeparam>
        /// <param name="take">The number of items to load at a time.</param>
        /// <param name="loadMoreItems">
        /// This is a delegate reference to a function inside the ViewModel that will retrieve the next batch of items.
        /// Importantly, this function will not run inside the UI thread.</param>
        /// <param name="onBatchStart">
        /// This will be invoked just before the loadMoreItems method is called.
        /// This allows us to make changes to properties on the ViewModel that may impact the View. e.g.,
        /// have an observable IsProcessing property which is bound to the Visibility property of a progress bar.</param>
        /// <param name="onBatchComplete">
        /// This will be invoked just after the retrieval of the latest batch and pass the items in.
        /// Crucially, this function will invoke on the UI thread.
        /// </param>
        /// <returns>A class that implements <see cref="SynchronizedObservableCollection{T}"/> and <see href="https://docs.microsoft.com/en-us/uwp/api/windows.ui.xaml.data.isupportincrementalloading?view=winrt-19041">ISupportIncrementalLoading</see>.</returns>
        SynchronizedObservableCollection<T> GetIncrementalCollection<T>(int take,
            Func<int, Task<List<T>>> loadMoreItems, Action onBatchStart, Action<List<T>> onBatchComplete);

        /// <summary>
        /// Constructs a new IncrementalLoadingCollection given the parameters.
        /// </summary>
        /// <typeparam name="T">The type of the items held in the collection.</typeparam>
        /// <param name="take">The number of items to load at a time.</param>
        /// <param name="loadMoreItems">
        /// This is a delegate reference to a function inside the ViewModel that will retrieve the next batch of items.
        /// Importantly, this function will not run inside the UI thread.</param>
        /// <param name="onBatchComplete">
        /// This will be invoked just after the retrieval of the latest batch and pass the items in.
        /// Crucially, this function will invoke on the UI thread.
        /// </param>
        /// <returns>A class that implements <see cref="SynchronizedObservableCollection{T}"/> and <see href="https://docs.microsoft.com/en-us/uwp/api/windows.ui.xaml.data.isupportincrementalloading?view=winrt-19041">ISupportIncrementalLoading</see>.</returns>
        SynchronizedObservableCollection<T> GetIncrementalCollection<T>(int take,
            Func<int, Task<List<T>>> loadMoreItems, Action<List<T>> onBatchComplete);

        /// <summary>
        /// Constructs a new IncrementalLoadingCollection given the parameters.
        /// </summary>
        /// <typeparam name="T">The type of the items held in the collection.</typeparam>
        /// <param name="take">The number of items to load at a time.</param>
        /// <param name="loadMoreItems">
        /// This is a delegate reference to a function inside the ViewModel that will retrieve the next batch of items.
        /// Importantly, this function will not run inside the UI thread.</param>
        /// <returns>A class that implements <see cref="SynchronizedObservableCollection{T}"/> and <see href="https://docs.microsoft.com/en-us/uwp/api/windows.ui.xaml.data.isupportincrementalloading?view=winrt-19041">ISupportIncrementalLoading</see>.</returns>
        SynchronizedObservableCollection<T> GetIncrementalCollection<T>(int take,
            Func<int, Task<List<T>>> loadMoreItems);

        /// <summary>
        /// Creates a <see cref="IFileSystemService"/> using the default settings.
        /// </summary>
        /// <returns>The requested <see cref="IFileSystemService"/>.</returns>
        public IFileSystemService CreateFileSystemService();

        /// <summary>
        /// Creates a <see cref="IFileSystemService"/> for caching.
        /// </summary>
        /// <returns>The requested <see cref="IFileSystemService"/>.</returns>
        public IFileSystemService CreateFileSystemServiceForCache();

        /// <summary>
        /// Creates a <see cref="IFileSystemService"/> for caching, using the given <paramref name="folderName"/> as the root directory.
        /// </summary>
        /// <param name="folderName">The folder to use as the root directory.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. Value is the requested <see cref="IFileSystemService"/>.</returns>
        public Task<IFileSystemService> CreateFileSystemServiceForCache(string folderName);
    }
}
