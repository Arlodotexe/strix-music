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
    public interface IFolderExplorer
    {

        /// <summary>
        /// Occurs won every folder selection.
        /// </summary>
        public event EventHandler<IFolderData>? FolderSelected;

        /// <summary>
        /// Occurs on every directory navigation.
        /// </summary>
        public event EventHandler<IFolderData>? DirectoryChanged;

        /// <summary>
        /// The navigation state of the <see cref="IFolderExplorer"/>.
        /// </summary>
        public NavigationState NavigationState { get; }

        /// <summary>
        /// The folder queue used for navigation.
        /// </summary>
        public Stack<IFolderData> FolderStack { get; }

        /// <summary>
        /// Determines whether the current directory is root or not.
        /// </summary>
        public bool IsRootDirectory { get; }

        /// <summary>
        /// The previous folder in queue.
        /// </summary>
        public IFolderData? PreviousFolder { get; }

        /// <summary>
        /// Selected path of the <see cref="IFolderExplorer"/>.
        /// </summary>
        public IFolderData? SelectedFolder { get; }

        /// <summary>
        /// Currently opened folder of <see cref="IFolderExplorer"/>.
        /// </summary>
        public IFolderData? CurrentFolder { get; }

        /// <summary>
        /// Setups the <see cref="IFolderExplorer"/>.
        /// </summary>
        /// <param name="folder">The current directory to open.</param>
        /// <param name="isRoot">Root folder indicator.</param>
        /// <returns>Created datalist for the UI to display.</returns>
        public Task SetupFolderExplorerAsync(IFolderData folder, bool isRoot = false);
    }
}