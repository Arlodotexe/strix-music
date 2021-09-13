using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Graph;
using OwlCore.AbstractStorage;

namespace StrixMusic.Cores.OneDrive.Storage
{
    /// <summary>
    /// Implementation of <see cref="IFolderData"/> for OneDrive.
    /// </summary>
    public class OneDriveFolderData : IFolderData
    {
        private const string EXPAND_STRING = "children";
        private readonly GraphServiceClient _graphClient;
        private readonly DriveItem _driveItem;

        /// <summary>
        /// Creates a new instance of <see cref="OneDriveFolderData"/>.
        /// </summary>
        /// <param name="graphClient">The service that handles API requests to Microsoft Graph.</param>
        /// <param name="driveItem">An instance of <see cref="DriveItem"/> that represents the underlying OneDrive folder.</param>
        public OneDriveFolderData(GraphServiceClient graphClient, DriveItem driveItem)
        {
            _graphClient = graphClient;
            _driveItem = driveItem;
        }

        /// <summary>
        /// A unique identifier returned from OneDrive api, a folder can be uniquely identified by this id. Helpful during record reads.
        /// </summary>
        public string OneDriveFolderId => _driveItem.Id;

        ///<inheritdoc />
        public string Name => _driveItem.Name;

        ///<inheritdoc />
        public string Path => _driveItem.WebUrl;

        ///<inheritdoc />
        public Task<IFileData> CreateFileAsync(string desiredName)
        {
            throw new NotSupportedException();
        }

        ///<inheritdoc />
        public Task<IFileData> CreateFileAsync(string desiredName, CreationCollisionOption options)
        {
            throw new NotSupportedException();
        }

        ///<inheritdoc />
        public Task<IFolderData> CreateFolderAsync(string desiredName)
        {
            throw new NotSupportedException();
        }

        ///<inheritdoc />
        public Task<IFolderData> CreateFolderAsync(string desiredName, CreationCollisionOption options)
        {
            throw new NotSupportedException();
        }

        ///<inheritdoc />
        public Task DeleteAsync()
        {
            throw new NotSupportedException();
        }

        ///<inheritdoc />
        public Task EnsureExists()
        {
            throw new NotSupportedException();
        }

        ///<inheritdoc />
        public Task<IFileData?> GetFileAsync(string name)
        {
            throw new NotImplementedException();
        }

        ///<inheritdoc />
        public Task<IEnumerable<IFileData>> GetFilesAsync()
        {
            throw new NotImplementedException();
        }

        ///<inheritdoc />
        public async Task<IFolderData?> GetFolderAsync(string name)
        {
            var driveItem = await _graphClient.Drive.Items[OneDriveFolderId].Request().Expand(EXPAND_STRING).GetAsync();

            foreach (var item in driveItem.Children)
            {
                if (item.Folder is null)
                    continue;

                if (item.Name == name)
                    return new OneDriveFolderData(_graphClient, item);
            }

            return null;
        }

        ///<inheritdoc />
        public async Task<IEnumerable<IFolderData>> GetFoldersAsync()
        {
            var driveItem = await _graphClient.Drive.Items[OneDriveFolderId].Request().Expand(EXPAND_STRING).GetAsync();

            var results = new List<IFolderData>();

            foreach (var item in driveItem.Children)
            {
                if (item.Folder is null)
                    continue;

                results.Add(new OneDriveFolderData(_graphClient, item));
            }

            return results;
        }

        ///<inheritdoc />
        public async Task<IFolderData?> GetParentAsync()
        {
            if (_driveItem.ParentReference is null)
                return null;

            var parent = await _graphClient.Drive.Items[_driveItem.ParentReference.DriveId].Request().GetAsync();

            return new OneDriveFolderData(_graphClient, parent);
        }
    }
}
