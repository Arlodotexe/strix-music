using System;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using StrixMusic.Sdk.ViewModels;
using StrixMusic.Sdk.WinUI.Controls.Items;
using StrixMusic.Shells.Groove.Messages.Navigation.Pages;

namespace StrixMusic.Shells.Groove.Controls.Items
{
    /// <inheritdoc/>
    public partial class GrooveAlbumItem : AlbumItem
    {
        /// <inheritdoc/>
        public GrooveAlbumItem()
        {
            this.DefaultStyleKey = typeof(GrooveAlbumItem);

            NavigateToAlbumCommand = new RelayCommand<AlbumViewModel>(new Action<AlbumViewModel?>(NavigateToAlbum));
        }
        
        /// <summary>
        /// A command that triggers navigation to the provided album.
        /// </summary>
        public RelayCommand<AlbumViewModel> NavigateToAlbumCommand { get; private set; }

        private void NavigateToAlbum(AlbumViewModel? viewModel)
        {
            if (viewModel != null)
                WeakReferenceMessenger.Default.Send(new AlbumViewNavigationRequestMessage(viewModel));
        }
    }
}
