using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
using Nito.AsyncEx;
using OwlCore.Extensions;
using StrixMusic.Sdk.Services.StorageService;

namespace StrixMusic.Sdk.Uno.Services
{
    /// <inheritdoc cref="ITextStorageService"/>
    public class TextStorageService : ITextStorageService
    {
        private readonly Dictionary<string, AsyncLock> _keyedMutex = new Dictionary<string, AsyncLock>();
        private readonly StorageFolder _localFolder;

        /// <summary>
        /// Initializes a new instance of this <see cref="TextStorageService"/>
        /// </summary>
        public TextStorageService()
        {
            _localFolder = ApplicationData.Current.LocalFolder;
        }

        /// <inheritdoc />
        public async Task<bool> FileExistsAsync(string filename)
        {
            try
            {
                var file = await _localFolder.GetFileAsync(filename);
                return file != null;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <inheritdoc />
        public async Task<bool> FileExistsAsync(string filename, string path)
        {
            StorageFolder pathHandle;
            try
            {
                pathHandle = await _localFolder.GetFolderAsync(path);
            }
            catch (Exception)
            {
                pathHandle = await _localFolder.CreateFolderAsync(path);
            }

            StorageFile fileHandle;
            try
            {
                fileHandle = await pathHandle.GetFileAsync(filename);
            }
            catch (Exception)
            {
                return false;
            }

            return fileHandle != null;
        }

        /// <inheritdoc />
        public async Task<string?> GetValueAsync(string filename)
        {
            try
            {
                var file = await _localFolder.GetFileAsync(filename);
                var value = await FileIO.ReadTextAsync(file);
                return value;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <inheritdoc />
        public async Task<string?> GetValueAsync(string filename, string path)
        {
            StorageFolder pathHandle;
            try
            {
                pathHandle = await _localFolder.GetFolderAsync(path);
            }
            catch (Exception)
            {
                pathHandle = await _localFolder.CreateFolderAsync(path);
            }

            StorageFile fileHandle;
            try
            {
                fileHandle = await pathHandle.GetFileAsync(filename);
            }
            catch (Exception)
            {
                fileHandle = await pathHandle.CreateFileAsync(filename);
            }

            using (await _keyedMutex.GetOrAdd(filename + path, new AsyncLock()).LockAsync())
                return await FileIO.ReadTextAsync(fileHandle);
        }

        /// <inheritdoc />
        public async Task RemoveAll()
        {
            // TODO: https://dev.azure.com/arloappx/Strix-Music/_sprints/taskboard/Strix-Music%20Team/Strix-Music/Sprint%202?workitem=232.
            var items = await _localFolder.GetItemsAsync();

            foreach (var item in items)
            {
                await item.DeleteAsync();
            }
        }

        /// <inheritdoc />
        public async Task RemoveByPathAsync(string path)
        {
            StorageFolder pathHandle;
            try
            {
                pathHandle = await _localFolder.GetFolderAsync(path);
            }
            catch (Exception)
            {
                pathHandle = await _localFolder.CreateFolderAsync(path);
            }

            await pathHandle.DeleteAsync();
        }

        /// <inheritdoc />
        public async Task SetValueAsync(string filename, string value)
        {
            StorageFile fileHandle;
            try
            {
                fileHandle = await _localFolder.GetFileAsync(filename);
            }
            catch (Exception)
            {
                fileHandle = await _localFolder.CreateFileAsync(filename);
            }

            using (await _keyedMutex.GetOrAdd(filename, new AsyncLock()).LockAsync())
                await FileIO.WriteTextAsync(fileHandle, value);
        }

        /// <inheritdoc />
        public async Task SetValueAsync(string filename, string value, string path)
        {
            StorageFolder pathHandle;
            try
            {
                pathHandle = await _localFolder.GetFolderAsync(path);
            }
            catch (Exception)
            {
                pathHandle = await _localFolder.CreateFolderAsync(path);
            }

            StorageFile fileHandle;
            try
            {
                fileHandle = await pathHandle.GetFileAsync(filename);
            }
            catch (Exception)
            {
                fileHandle = await pathHandle.CreateFileAsync(filename);
            }

            using (await _keyedMutex.GetOrAdd(filename + path, new AsyncLock()).LockAsync())
                await FileIO.WriteTextAsync(fileHandle, value);
        }
    }
}
