using OwlCore.AbstractStorage;
using OwlCore.AbstractUI.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StrixMusic.Sdk.Components
{
    /// <summary>
    /// File explorer that lets user choose a folder using <see cref="IFolderData"/> and <see cref="IFileData"/>
    /// </summary>
    public interface IFileExplorer
    {
        /// <summary>
        /// Selected path of the <see cref="IFileExplorer"/>.
        /// </summary>
        public IFolderData? SelectedFolder { get; }

        /// <summary>
        /// Currently opened folder of <see cref="IFileExplorer"/>.
        /// </summary>
        public IFolderData? CurrentFolder { get; }

        /// <summary>
        /// Initializes the <see cref="IFileExplorer"/> with <see cref="IFolderData"/>.
        /// </summary>
        /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
        public Task<AbstractDataList?> SetupFileExplorerAsync(IFolderData folder);
    }
}
