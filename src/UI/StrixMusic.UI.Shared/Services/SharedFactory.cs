using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using LaunchPad.Collections;
using OwlCore;
using OwlCore.AbstractStorage;
using OwlCore.Collections;
using Windows.Storage;
using StrixMusic.Sdk.Services;

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
            StorageFolder rootStorageFolder;

            try
            {
                rootStorageFolder = await ApplicationData.Current.LocalCacheFolder.GetFolderAsync(folderName);
            }
            catch (FileNotFoundException)
            {
                rootStorageFolder = await ApplicationData.Current.LocalCacheFolder.CreateFolderAsync(folderName);
            }

            return new FileSystemService(rootStorageFolder);
        }
    }
}
