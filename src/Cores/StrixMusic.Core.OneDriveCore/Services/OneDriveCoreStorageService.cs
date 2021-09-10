using Microsoft.Graph;
using Microsoft.Toolkit.Diagnostics;
using OwlCore.AbstractStorage;
using StrixMusic.Cores.OneDrive.Storage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StrixMusic.Cores.OneDrive.Services
{
    /// <summary>
    /// Handles are requests for OneDrive.
    /// </summary>
    public class OneDriveCoreStorageService
    {
        private GraphServiceClient _graphClient;

        private string _expandString => "children";

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
            var driveItem = await _graphClient.Drive.Root.Request().Expand(_expandString).GetAsync();

            return new OneDriveFolderData(this, driveItem.Name, driveItem.WebUrl, driveItem.Id, true);
        }

        /// <summary>
        /// Gets the folder details.
        /// </summary>
        /// <param name="folderData"></param>
        /// <param name="isRoot"></param>
        public async Task<IEnumerable<IFolderData>> GetItemsAsync(OneDriveFolderData folderData, bool isRoot = false)
        {
            try
            {
                Guard.IsNotNull(typeof(GraphServiceClient), "Graph client should be provided.");
                var fileData = new List<IFolderData>();

                DriveItem driveItem;

                if (isRoot)
                {
                    driveItem = await _graphClient.Drive.Root.Request().Expand(_expandString).GetAsync();
                }
                else
                {
                    driveItem = await _graphClient.Drive.Items[folderData.OneDriveFolderId].Request().Expand(_expandString).GetAsync();
                }

                fileData = new List<IFolderData>(FillChildren(driveItem, fileData));

                return fileData;
            }
            catch
            {
                return new List<IFolderData>();
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
    }
}
