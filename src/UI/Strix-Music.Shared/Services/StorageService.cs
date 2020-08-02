using System;
using System.Threading.Tasks;
using StrixMusic.Services.SettingsStorage;
using Windows.Storage;

namespace Strix_Music.Services
{
    /// <inheritdoc cref="IStorageService"/>
    public class StorageService : IStorageService
    {
        private readonly StorageFolder _localFolder;

        /// <summary>
        /// Initializes a new instance of this <see cref="StorageService"/>
        /// </summary>
        public StorageService()
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
            try
            {
                var file = await _localFolder.GetFileAsync(path + filename);
                return file != null;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <inheritdoc />
        public async Task<string> GetValueAsync(string filename)
        {
            try
            {
                var file = await _localFolder.GetFileAsync(filename);
                var value = await FileIO.ReadTextAsync(file);
                return value;
            }
            catch (Exception)
            {
                return null!;
            }
        }

        /// <inheritdoc />
        public async Task<string> GetValueAsync(string filename, string path)
        {
            try
            {
                var file = await _localFolder.GetFileAsync(path + filename);
                var value = await FileIO.ReadTextAsync(file);
                return value;
            }
            catch (Exception)
            {
                return null!;
            }
        }

        /// <inheritdoc />
        public async Task SetValueAsync(string filename, string value)
        {
            if (await FileExistsAsync(filename))
            {
                var fileHandle = await _localFolder.GetFileAsync(filename);
                await FileIO.WriteTextAsync(fileHandle, value);
            }
        }

        /// <inheritdoc />
        public async Task SetValueAsync(string filename, string value, string path)
        {
            if (await FileExistsAsync(filename))
            {
                var fileHandle = await _localFolder.GetFileAsync(filename);
                await FileIO.WriteTextAsync(fileHandle, value);
            }
        }
    }
}
