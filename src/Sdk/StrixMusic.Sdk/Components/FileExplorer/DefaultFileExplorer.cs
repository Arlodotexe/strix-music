using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using OwlCore.AbstractStorage;
using OwlCore.AbstractUI.Models;

namespace StrixMusic.Sdk.Components
{
    /// <inheritdoc/>
    public class DefaultFileExplorer : IFileExplorer
    {
        ///<inheritdoc />
        public IFolderData? SelectedFolder { get; private set; }

        ///<inheritdoc />
        public IFolderData? CurrentFolder { get; private set; }

        /// <summary>
        /// Initializes the <see cref="IFileExplorer"/>.
        /// </summary>
        /// <param name="folder"></param>
        /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
        public async Task<AbstractDataList?> SetupFileExplorerAsync(IFolderData folder)
        {
            CurrentFolder = folder;
            var folders = await folder.GetFoldersAsync();

            return SetupFileExplorerUIComponents(folders);
        }

        private AbstractDataList? SetupFileExplorerUIComponents(IEnumerable<IFolderData>? folderDatas)
        {
            if (folderDatas == null)
                return null;

            var abstractMetadatas = new List<AbstractUIMetadata>();

            foreach (var item in folderDatas)
            {
                var abstractMetadata = new AbstractUIMetadata(item.Name)
                {
                    Title = item.Name,
                    ImagePath = "ms-appx:///Assets/FileExplorer/Folder.png",
                };

                abstractMetadatas.Add(abstractMetadata);
            }

            var abstractDataList = new AbstractDataList("Directory", abstractMetadatas);

            return abstractDataList;
        }
    }
}