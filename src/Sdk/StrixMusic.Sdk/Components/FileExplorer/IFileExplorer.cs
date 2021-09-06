using OwlCore.AbstractStorage;
using System;
using System.Collections.Generic;
using System.Text;

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
        public IFolderData SelectedFolder { get; }

        /// <summary>
        /// Initializes the <see cref="IFileExplorer"/> with <see cref="IFolderData"/>.
        /// </summary>
        /// <param name="folder">The folder.</param>
        public void InitializeFileExplorer(IFolderData folder);

        /// <summary>
        /// Setsup the UI For fileExplorer.
        /// </summary>
        public void SetupFileExplorerUIComponents();
    }
}
