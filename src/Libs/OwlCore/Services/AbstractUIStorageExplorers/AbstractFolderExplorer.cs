using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Toolkit.Diagnostics;
using OwlCore.AbstractStorage;
using OwlCore.AbstractUI.Models;
using OwlCore.Extensions;
using OwlCore.Provisos;

namespace OwlCore.Services.AbstractUIStorageExplorers
{
    /// <summary>
    /// File explorer that lets user choose a folder using <see cref="IFolderData"/> and <see cref="IFileData"/>
    /// </summary>
    public class AbstractFolderExplorer : AbstractUICollection, IAsyncInit, IDisposable
    {
        private readonly AbstractUIMetadata _backUIMetadata = new("BackBtn")
        {
            Title = "Go back",
            IconCode = "\uE7EA",
        };

        private readonly IFolderData _rootFolder;
        private readonly AbstractButton _selectButton;
        private readonly AbstractButton _cancelButton;

        private IFolderData[]? _currentDisplayedFolders;
        private AbstractDataList? _currentDataList;
        private bool _isRootFolder;

        /// <summary>
        /// Creates a new instance of <see cref="AbstractFolderExplorer"/>.
        /// </summary>
        public AbstractFolderExplorer(IFolderData rootFolder)
            : base($"{rootFolder.Path}.{nameof(AbstractFolderExplorer)}")
        {
            _rootFolder = rootFolder;
            _cancelButton = new AbstractButton("cancelFolderExplorerButton", "Cancel", type: AbstractButtonType.Cancel);
            _selectButton = new AbstractButton("selectFolderButton", "Select folder", type: AbstractButtonType.Confirm);

            FolderStack = new Stack<IFolderData>();

            AttachEvents();
        }

        /// <inheritdoc />
        public async Task InitAsync()
        {
            await SetupFolderAsync(_rootFolder);
            IsInitialized = true;
        }

        private void AttachEvents()
        {
            _cancelButton.Clicked += OnCancelButtonClicked;
            _selectButton.Clicked += OnSelectFolderButtonClicked;
        }

        private void DetachEvents()
        {
            _cancelButton.Clicked -= OnCancelButtonClicked;
            _selectButton.Clicked -= OnSelectFolderButtonClicked;

            if (_currentDataList is not null)
                _currentDataList.ItemTapped -= AbstractDataListOnItemTapped;
        }

        /// <inheritdoc />
        public bool IsInitialized { get; private set; }

        /// <summary>
        /// Holds all navigated directories. The top of the stack has the current folder. The last item in the stack has the root folder.
        /// </summary>
        public Stack<IFolderData> FolderStack { get; }

        /// <summary>
        /// The folder that the user has selected, if any.
        /// </summary>
        public IFolderData? SelectedFolder { get; private set; }

        /// <summary>
        /// Currently opened folder.
        /// </summary>
        public IFolderData? CurrentFolder { get; private set; }

        /// <summary>
        /// Raised when the user has selected a folder.
        /// </summary>
        public event EventHandler<IFolderData>? FolderSelected;

        /// <summary>
        /// Raised when the user has canceled folder picking.
        /// </summary>
        public event EventHandler? Canceled;

        /// <summary>
        /// Raised on directory navigation.
        /// </summary>
        public event EventHandler<IFolderData>? DirectoryChanged;

        /// <summary>
        /// Setups the <see cref="AbstractFolderExplorer"/>.
        /// </summary>
        /// <param name="folder">The current directory to open.</param>
        /// <param name="lastNavigationAction">The navigation action that was performed by the user.</param>
        /// <returns>Created datalist for the UI to display.</returns>
        private async Task SetupFolderAsync(IFolderData folder, NavigationAction lastNavigationAction = NavigationAction.None)
        {
            CurrentFolder = folder;
            _isRootFolder = ReferenceEquals(folder, _rootFolder);

            if (lastNavigationAction != NavigationAction.Back)
                FolderStack.Push(folder);

            var folders = await folder.GetFoldersAsync();
            var folderData = folders.ToArray();

            if (!folderData.Any())
                return;

            _currentDisplayedFolders = folderData;

            CreateAndSetupAbstractUIForFolders(folderData);
            DirectoryChanged?.Invoke(this, folder);
        }

        private void CreateAndSetupAbstractUIForFolders(IFolderData[] folderData)
        {
            var folderListMetadata = new List<AbstractUIMetadata>();

            if (!_isRootFolder)
                folderListMetadata.Add(_backUIMetadata);

            var folderUIMetadata = folderData.Select(item => new AbstractUIMetadata(item.Name)
            {
                Title = item.Name,
                IconCode = "\uE8B7",
            }).ToArray();

            var uniqueIdForFolders = string.Join(".", folderUIMetadata.Select(x => x.Id)).HashMD5Fast();

            folderListMetadata.AddRange(folderUIMetadata);

            if (_currentDataList is not null)
                _currentDataList.ItemTapped -= AbstractDataListOnItemTapped;

            _currentDataList = new AbstractDataList(uniqueIdForFolders, folderListMetadata);

            Clear();

            Add(_currentDataList);
            Add(new AbstractUICollection("ActionButtons", PreferredOrientation.Horizontal)
            {
                _cancelButton,
                _selectButton,
            });

            _currentDataList.ItemTapped += AbstractDataListOnItemTapped;
        }

        private void OnSelectFolderButtonClicked(object sender, EventArgs e)
        {
            Guard.IsNotNull(CurrentFolder, nameof(CurrentFolder));
            SelectedFolder = CurrentFolder;
            FolderSelected?.Invoke(this, CurrentFolder);
        }

        private void OnCancelButtonClicked(object sender, EventArgs e) => Canceled?.Invoke(sender, e);

        private async void AbstractDataListOnItemTapped(object sender, AbstractUIMetadata e)
        {
            Guard.IsNotNull(_currentDisplayedFolders, nameof(_currentDisplayedFolders));
            var lastNavigationAction = ReferenceEquals(e, _backUIMetadata) ? NavigationAction.Back : NavigationAction.Forward;

            IFolderData targetFolder;

            if (lastNavigationAction == NavigationAction.Back)
            {
                FolderStack.Pop();
                targetFolder = FolderStack.Peek();
            }
            else
            {
                targetFolder = _currentDisplayedFolders.First(x => x.Name == e.Id);
                FolderStack.Push(targetFolder);
            }

            await SetupFolderAsync(targetFolder, lastNavigationAction);

            DirectoryChanged?.Invoke(this, targetFolder);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            DetachEvents();
        }
    }

}