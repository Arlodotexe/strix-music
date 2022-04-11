using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Identity.Client;
using OwlCore;
using OwlCore.AbstractStorage;
using OwlCore.Collections.ObjectModel;
using OwlCore.Uno.Collections;
using StrixMusic.Sdk.Services;
using StrixMusic.Services;
using Uno.UI.MSAL;
using Windows.Storage;
using CreationCollisionOption = Windows.Storage.CreationCollisionOption;

namespace StrixMusic.Shared.Services
{
    /// <inheritdoc cref="ISharedFactory" />
    public class SharedFactory : ISharedFactory
    {
        /// <inheritdoc />
        public SynchronizedObservableCollection<T> GetIncrementalCollection<T>(int take, Func<int, Task<List<T>>> loadMoreItems, Action onBatchStart, Action<List<T>> onBatchComplete)
        {
            using (Threading.PrimaryContext)
            {
                return new IncrementalLoadingCollection<T>(take, loadMoreItems, onBatchComplete, onBatchStart);
            }
        }

        /// <inheritdoc />
        public SynchronizedObservableCollection<T> GetIncrementalCollection<T>(int take, Func<int, Task<List<T>>> loadMoreItems, Action<List<T>> onBatchComplete)
        {
            using (Threading.PrimaryContext)
            {
                return new IncrementalLoadingCollection<T>(take, loadMoreItems, onBatchComplete);
            }
        }

        /// <inheritdoc />
        public SynchronizedObservableCollection<T> GetIncrementalCollection<T>(int take, Func<int, Task<List<T>>> loadMoreItems)
        {
            using (Threading.PrimaryContext)
            {
                return new IncrementalLoadingCollection<T>(take, loadMoreItems);
            }
        }

        /// <summary>
        /// Creates a <see cref="IFileSystemService"/> using the default settings.
        /// </summary>
        /// <returns>The requested <see cref="IFileSystemService"/>.</returns>
        public IFileSystemService CreateFileSystemService()
        {
            return new FileSystemService();
        }

        /// <inheritdoc/>
        public async Task<IFileSystemService> CreateFileSystemServiceAsync(string folderPath)
        {
            var folder = await StorageFolder.GetFolderFromPathAsync(folderPath);
            return new FileSystemService(folder);
        }

        /// <summary>
        /// Creates a <see cref="IFileSystemService"/> for caching.
        /// </summary>
        /// <returns>The requested <see cref="IFileSystemService"/>.</returns>
        public IFileSystemService CreateFileSystemServiceForCache()
        {
            return new FileSystemService(ApplicationData.Current.LocalCacheFolder);
        }

        /// <summary>
        /// Creates a <see cref="IFileSystemService"/> for caching, using the given <paramref name="folderName"/> as the root directory.
        /// </summary>
        /// <param name="folderName">The folder to use as the root directory.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. Value is the requested <see cref="FileSystemService"/>.</returns>
        public async Task<IFileSystemService> CreateFileSystemServiceForCache(string folderName)
        {
            var rootStorageFolder = await ApplicationData.Current.LocalCacheFolder.CreateFolderAsync(folderName, CreationCollisionOption.OpenIfExists);

            return new FileSystemService(rootStorageFolder);
        }
    }
}
