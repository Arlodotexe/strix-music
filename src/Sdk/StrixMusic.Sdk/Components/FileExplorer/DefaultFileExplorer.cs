using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Diagnostics;
using OwlCore.AbstractStorage;
using OwlCore.AbstractUI.Models;

namespace StrixMusic.Sdk.Components
{
    /// <inheritdoc/>
    public class DefaultFileExplorer : IFileExplorer
    {
        private IServiceProvider _services;

        /// <summary>
        /// Back button id.
        /// </summary>
        public string BackBtnId => "BackBtn";

        ///<inheritdoc />
        public Stack<IFolderData> FolderStack { get; private set; }

        ///<inheritdoc />
        public IFolderData? PreviousFolder { get; private set; }

        ///<inheritdoc />
        public IFolderData? SelectedFolder { get; private set; }

        ///<inheritdoc />
        public IFolderData? CurrentFolder { get; private set; }

        ///<inheritdoc />
        public bool IsRootDirectory { get; private set; }

        ///<inheritdoc />
        public NavigationState NavigationState { get; private set; }


        ///<inheritdoc />
        public event EventHandler<AbstractUIMetadata>? DirectoryChanged;

        /// <summary>
        /// Creates a new instance of <see cref="DefaultFileExplorer"/>.
        /// </summary>
        /// <param name="services"></param>
        public DefaultFileExplorer(IServiceProvider services)
        {
            _services = services;

            NavigationState = NavigationState.None;
            FolderStack = new Stack<IFolderData>();
        }

        ///<inheritdoc />
        public async Task<AbstractUIElementGroup?> SetupFileExplorerAsync(IFolderData folder, bool isRoot = false)
        {
            IsRootDirectory = isRoot;

            if (NavigationState != NavigationState.Back)
                FolderStack.Push(folder);

            CurrentFolder = folder;

            var folders = await folder.GetFoldersAsync();

            return SetupFileExplorerUIComponents(folders);
        }

        private AbstractUIElementGroup? SetupFileExplorerUIComponents(IEnumerable<IFolderData>? folderDatas)
        {
            if (folderDatas == null)
                return null;

            var abstractMetadatas = new List<AbstractUIMetadata>();
            if (!IsRootDirectory)
            {
                abstractMetadatas.Add(new AbstractUIMetadata(BackBtnId)
                {
                    Title = "Go back",
                    IconCode = "\uE7EA",
                });
            }

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

            var selectFolder = new AbstractButton("SelectBtn", "Select Current Button")
            {
                Title = "Select Current Folder",
            };

            var abstractUIGroup = new AbstractUIElementGroup("FileExplorer")
            {
                Title = "File Explorer",
                Items = new List<AbstractUIElement>()
                {
                    selectFolder,
                    abstractDataList
                }
            };

            abstractDataList.ItemTapped += AbstractDataList_ItemTapped;

            return abstractUIGroup;
        }

        private void AbstractDataList_ItemTapped(object sender, AbstractUIMetadata e)
        {
            //TODO: Move the directory switching logic in the explorer.

            Guard.IsNotNull(e.Id, nameof(e.Id));

            if (e.Id.Equals(BackBtnId))
            {
                FolderStack.Pop();

                PreviousFolder = FolderStack.FirstOrDefault();
                NavigationState = NavigationState.Back;
            }
            else
            {
                NavigationState = NavigationState.Forward;
            }

            DirectoryChanged?.Invoke(this, e);
        }
    }
}