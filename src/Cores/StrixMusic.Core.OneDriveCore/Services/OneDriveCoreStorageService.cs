using Microsoft.Graph;
using Microsoft.Toolkit.Diagnostics;
using OwlCore.AbstractStorage;
using StrixMusic.Core.OneDriveCore.Storage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StrixMusic.Core.OneDriveCore.Services
{
    /// <summary>
    /// Handles are requests for OneDrive.
    /// </summary>
    public class OneDriveCoreStorageService
    {
        private GraphServiceClient _graphClient;

        /// <summary>
        /// The expand string for consumers.
        /// </summary>
        /// <returns></returns>
        public string ExplandString => "children";

        /// <summary>
        /// Initializes the OneDrive service with a <see cref="GraphServiceClient"/>.
        /// </summary>
        /// <param name="graphClient"></param>
        public void Init(GraphServiceClient graphClient)
        {
            _graphClient = graphClient;
        }

        public async Task<IFolderData> GetRootFolderAsync()
        {
            var driveItem = await _graphClient.Drive.Root.Request().Expand(ExplandString).GetAsync();

            return new OneDriveFolderData(this, driveItem.Name, driveItem.WebUrl, driveItem.Id, true);
        }

        /// <summary>
        /// Gets the folder details.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="isRoot"></param>
        public async Task<IEnumerable<IFolderData>> GetFolderChildrenAsync(string id, bool isRoot = false)
        {
            try
            {
                Guard.IsNotNull(typeof(GraphServiceClient), "Graph client should be provided.");
                var fileData = new List<IFolderData>();

                DriveItem driveItem;

                if (isRoot)
                {
                    driveItem = await _graphClient.Drive.Root.Request().Expand(ExplandString).GetAsync();
                }
                else
                {
                    driveItem = await _graphClient.Drive.Items[id].Request().Expand(ExplandString).GetAsync();
                }

                fileData = new List<IFolderData>(FillChildren(driveItem, fileData));

                return fileData;
            }
            catch
            {
                return null;
            }
        }

        private IEnumerable<IFolderData> FillChildren(DriveItem driveItem, List<IFolderData> fileData)
        {
            foreach (var item in driveItem.Children)
            {
                if (item.Folder != null)
                {
                    yield return new OneDriveFolderData(this, item.Name, item.WebUrl, item.Id);
                }
            }
        }

        /// <summary>
        /// Gets the folder details for the provide <see cref="folderId"/>.
        /// </summary>
        /// <param name="folderId">The provided folderId</param>
        public async Task<DriveItem> GetOneDriveCoreFolderIdAsync(string folderId)
        {
            try
            {
                Guard.IsNotNull(typeof(GraphServiceClient), "Graph client should be provided.");

                return await _graphClient.Drive.Items[folderId].Request().Expand(ExplandString).GetAsync();
            }
            catch
            {
                return null;
            }
        }
    }
}
