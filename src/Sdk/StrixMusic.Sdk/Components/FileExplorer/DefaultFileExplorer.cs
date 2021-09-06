using OwlCore.AbstractStorage;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StrixMusic.Sdk.Components
{
    /// <inheritdoc/>
    public class DefaultFileExplorer : IFileExplorer
    {
        /// <summary>
        /// The currently selected folder.
        /// </summary>
        public IFolderData SelectedFolder { get; private set; }


        /// <summary>
        /// Initializes the <see cref="IFileExplorer"/>.
        /// </summary>
        /// <param name="folder"></param>
        /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
        public async Task SetupFileExplorerAsync(IFolderData folder)
        {
            SelectedFolder = folder;
            var folders = await folder.GetFoldersAsync();

        }

        private void SetupFileExplorerUIComponents()
        {
            //TODO: create a ui for file explorer.
        }
    }
}
