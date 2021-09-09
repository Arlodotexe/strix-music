using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Toolkit.Diagnostics;
using OwlCore.AbstractStorage;
using OwlCore.AbstractUI.Models;

namespace StrixMusic.Sdk.Components.Explorers
{
    /// <summary>
    /// Creates and handles events for <see cref="IFolderExplorer"/>
    /// </summary>
    public class FolderExplorerUIHandler
    {
        private IEnumerable<IFolderData>? _folderDatas;

        /// <summary>
        /// It's raised whenever the folder explorer ui is updated.
        /// </summary>
        public event EventHandler<AbstractUIElementGroup>? FolderExplorerUIUpdated;

        /// <summary>
        /// Occurs on every directory navigation.
        /// </summary>
        public event EventHandler<NavigationEventArgs>? FolderItemTapped;

        /// <summary>
        /// Occurs when folder selection.
        /// </summary>
        public event EventHandler? FolderSelectedTapped;

        /// <summary>
        /// Back button id.
        /// </summary>
        public const string BACK_BTN_ID = "BackBtn";

        /// <summary>
        /// Creates the <see cref="AbstractUIElementGroup"/> for <see cref="IFolderExplorer"/>.
        /// </summary>
        /// <param name="folderDatas">The folder listing.</param>
        /// <param name="isRoot">Determines whether current directory is a root directory or not.</param>
        public void SetupFileExplorerUIComponents(IEnumerable<IFolderData>? folderDatas, bool isRoot = false)
        {
            Guard.IsNotNull(folderDatas, nameof(folderDatas));

            _folderDatas = folderDatas;

            var abstractMetadatas = new List<AbstractUIMetadata>();
            if (!isRoot)
            {
                abstractMetadatas.Add(new AbstractUIMetadata(BACK_BTN_ID)
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

            var selectFolder = new AbstractButton("SelectBtn", "Select Current Folder");

            var abstractUIGroup = new AbstractUIElementGroup("Folder")
            {
                Title = "Folder Explorer",
                Items = new List<AbstractUIElement>()
                {
                    selectFolder,
                    abstractDataList
                }
            };

            abstractDataList.ItemTapped += AbstractDataList_ItemTapped;
            selectFolder.Clicked += SelectFolder_Clicked;

            FolderExplorerUIUpdated?.Invoke(this, abstractUIGroup);
        }

        private void SelectFolder_Clicked(object sender, EventArgs e)
        {
            if (sender is AbstractButton button)
            {
                Guard.IsNotNull(_folderDatas, nameof(_folderDatas));

                button.Clicked -= SelectFolder_Clicked;

                FolderSelectedTapped?.Invoke(this, e);
            }
        }

        private void AbstractDataList_ItemTapped(object sender, AbstractUIMetadata e)
        {
            Guard.IsNotNull(_folderDatas, nameof(_folderDatas));

            ((AbstractDataList)sender).ItemTapped -= AbstractDataList_ItemTapped;

            NavigationEventArgs navEventsArgs;

            if (e.Id.Equals(BACK_BTN_ID))
            {
                navEventsArgs = new NavigationEventArgs()
                {
                    TappedFolder = null,
                    BackNavigationOccurred = true
                };
            }
            else
            {
                var folder = _folderDatas.FirstOrDefault(c => c.Name.Equals(e.Id));
                navEventsArgs = new NavigationEventArgs()
                {
                    TappedFolder = folder,
                    BackNavigationOccurred = false
                };
            }

            FolderItemTapped?.Invoke(this, navEventsArgs);
        }
    }
}
