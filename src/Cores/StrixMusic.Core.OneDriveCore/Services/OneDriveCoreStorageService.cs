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
        /// Creates an ew instance for <see cref="OneDriveCore"/>.
        /// </summary>
        /// <param name="graphClient">The MS graph client.</param>
        public OneDriveCoreStorageService()
        {

        }

        /// <summary>
        /// Initializes the OneDrive service with a <see cref="GraphServiceClient"/>.
        /// </summary>
        /// <param name="graphClient"></param>
        public void Init(GraphServiceClient graphClient)
        {
            _graphClient = graphClient;
        }


        /// <summary>
        /// Gets the folder details for the provide <see cref="folderId"/>.
        /// </summary>
        /// <param name="folderId">The provided folderId</param>
        public async Task<IFolderData> GetRootFolderAsync()
        {
            try
            {
                Guard.IsNotNull(typeof(GraphServiceClient), "Graph client should be provided.");

                var fileData = new List<OneDriveFolderData>();
                var clientType = new ClientType();

                var expandValue = clientType == ClientType.Consumer
                    ? "thumbnails,children($expand=thumbnails)"
                    : "thumbnails,children";

                var driveItem = await _graphClient.Drive.Root.Request().Expand(ExplandString).GetAsync();
                foreach (var item in driveItem.Children)
                {
                    if (item.Folder != null && item.Children != null && item.Children.CurrentPage != null)
                    {
                        var folderData = new OneDriveFolderData();
                    }
                }

                return null;
            }
            catch
            {
                return null;
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
