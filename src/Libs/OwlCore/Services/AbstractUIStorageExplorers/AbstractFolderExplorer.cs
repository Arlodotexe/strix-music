using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Diagnostics;
using OwlCore.AbstractStorage;
using OwlCore.Services.AbstractUIStorageExplorers.Handlers;

namespace OwlCore.Services.AbstractUIStorageExplorers
{
    /// <summary>
    /// File explorer that lets user choose a folder using <see cref="IFolderData"/> and <see cref="IFileData"/>
    /// </summary>
    public class AbstractFolderExplorer
    {
        private IServiceProvider _services;

        private FolderExplorerUIHandler? _folderExplorerUIHandler;

        /// <summary>
        /// Occurs won every folder selection.
        /// </summary>
        public event EventHandler<IFolderData>? FolderSelected;

        /// <summary>
        /// Occurs on every directory navigation.
        /// </summary>
        public event EventHandler<IFolderData>? DirectoryChanged;

        /// <summary>
        /// The stack that holds all navigated directories, the top of the stack has the recently opened folder, the last item in the stack has the root folder.
        /// </summary>
        public Stack<IFolderData> FolderStack { get; private set; }

        /// <summary>
        /// Holds the previous folder when navigating back.
        /// </summary>
        public IFolderData? PreviousFolder { get; private set; }

        /// <summary>
        /// Selected path of the <see cref="AbstractFolderExplorer"/>.
        /// </summary>
        public IFolderData? SelectedFolder { get; private set; }

        /// <summary>
        /// Currently opened folder.
        /// </summary>
        public IFolderData? CurrentFolder { get; private set; }

        /// <summary>
        /// Determines whether the current directory is root or not.
        /// </summary>
        public bool IsRootDirectory { get; private set; }

        /// <summary>
        /// The navigation state of the <see cref="AbstractFolderExplorer"/>.
        /// </summary>
        public NavigationAction NavigationAction { get; private set; }

        /// <summary>
        /// Creates a new instance of <see cref="AbstractFolderExplorer"/>.
        /// </summary>
        /// <param name="services"></param>
        public AbstractFolderExplorer(IServiceProvider services)
        {
            _services = services;

            NavigationAction = NavigationAction.None;
            FolderStack = new Stack<IFolderData>();
        }

        /// <summary>
        /// Setups the <see cref="AbstractFolderExplorer"/>.
        /// </summary>
        /// <param name="folder">The current directory to open.</param>
        /// <param name="isRoot">Root folder indicator.</param>
        /// <returns>Created datalist for the UI to display.</returns>
        public async Task SetupFolderExplorerAsync(IFolderData folder, bool isRoot = false)
        {
            IsRootDirectory = isRoot;

            _folderExplorerUIHandler = _services.GetService<FolderExplorerUIHandler>();

            Guard.IsNotNull(_folderExplorerUIHandler, nameof(_folderExplorerUIHandler));

            if (NavigationAction != NavigationAction.Back)
                FolderStack.Push(folder);

            CurrentFolder = folder;

            var folders = await folder.GetFoldersAsync();

            _folderExplorerUIHandler.SetupFileExplorerUIComponents(folders, IsRootDirectory);

            _folderExplorerUIHandler.FolderItemTapped += FolderUIHandler_ItemTapped;
            _folderExplorerUIHandler.FolderSelectedTapped += _folderExplorerUIHandler_FolderSelectedTapped;
        }

        private void _folderExplorerUIHandler_FolderSelectedTapped(object sender, EventArgs e)
        {
            Guard.IsNotNull(_folderExplorerUIHandler, nameof(_folderExplorerUIHandler));

            _folderExplorerUIHandler.FolderSelectedTapped -= _folderExplorerUIHandler_FolderSelectedTapped;

            SelectedFolder = CurrentFolder;

            Guard.IsNotNull(SelectedFolder, nameof(SelectedFolder));
            FolderSelected?.Invoke(this, SelectedFolder);
        }

        private async void FolderUIHandler_ItemTapped(object sender, NavigationEventArgs e)
        {
            Guard.IsNotNull(_folderExplorerUIHandler, nameof(_folderExplorerUIHandler));
            _folderExplorerUIHandler.FolderItemTapped -= FolderUIHandler_ItemTapped;

            if (e.BackNavigationOccurred)
            {
                FolderStack.Pop();
                PreviousFolder = FolderStack.FirstOrDefault();
                NavigationAction = NavigationAction.Back;

                Guard.IsNotNull(PreviousFolder, nameof(PreviousFolder));


                await SetupFolderExplorerAsync(PreviousFolder, PreviousFolder.Name.Equals("root", StringComparison.OrdinalIgnoreCase));

                DirectoryChanged?.Invoke(this, PreviousFolder);
            }
            else
            {
                Guard.IsNotNull(e.TappedFolder, nameof(e.TappedFolder));

                NavigationAction = NavigationAction.Forward;

                await SetupFolderExplorerAsync(e.TappedFolder, e.TappedFolder.Name.Equals("root", StringComparison.OrdinalIgnoreCase));

                DirectoryChanged?.Invoke(this, e.TappedFolder);
            }
        }
    }

}