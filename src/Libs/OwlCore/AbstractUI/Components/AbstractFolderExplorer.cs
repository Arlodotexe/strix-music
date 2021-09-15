using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Toolkit.Diagnostics;
using OwlCore.AbstractStorage;
using OwlCore.AbstractUI.Models;
using OwlCore.Extensions;
using OwlCore.Provisos;

namespace OwlCore.AbstractUI.Components
{
    /// <summary>
    /// An <see cref="AbstractUICollection"/> that acts as a standalone, inbox component. A Folder explorer that interops with <see cref="OwlCore.AbstractStorage"/> to browse and select subfolders from an <see cref="IFolderData"/>.
    /// </summary>
    public class AbstractFolderExplorer : AbstractUICollection, IAsyncInit, IDisposable
    {
        private readonly AbstractUIMetadata _backUIMetadata = new("BackBtn")
        {
            Title = "Go back",
            IconCode = "\uE7EA",
        };

        private readonly AbstractUICollection _actionButtons = new("ActionButtons", PreferredOrientation.Horizontal);

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

            _actionButtons.Add(_cancelButton);
            _actionButtons.Add(_selectButton);

            FolderStack = new Stack<IFolderData>();

            Title = "Pick a folder";

            AttachEvents();
        }

        /// <inheritdoc />
        public async Task InitAsync()
        {
            FolderStack.Push(_rootFolder);
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
        /// <returns>Created datalist for the UI to display.</returns>
        private async Task SetupFolderAsync(IFolderData folder)
        {
            CurrentFolder = folder;
            _isRootFolder = ReferenceEquals(folder, _rootFolder);

            var folders = await folder.GetFoldersAsync();
            var folderData = folders.ToArray();

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

            _currentDataList = new AbstractDataList(uniqueIdForFolders, folderListMetadata)
            {
                Title = CurrentFolder?.Name ?? string.Empty,
            };

            Clear();

            Add(_currentDataList);
            Add(_actionButtons);

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

            IFolderData targetFolder;

            if (ReferenceEquals(e, _backUIMetadata))
            {
                FolderStack.Pop();
                targetFolder = FolderStack.Peek();
            }
            else
            {
                targetFolder = _currentDisplayedFolders.First(x => x.Name == e.Id);
                FolderStack.Push(targetFolder);
            }

            await SetupFolderAsync(targetFolder);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            DetachEvents();
        }
    }

}