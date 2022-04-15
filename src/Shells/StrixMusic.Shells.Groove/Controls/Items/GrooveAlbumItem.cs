using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using StrixMusic.Sdk.WinUI.Controls.Items;
using StrixMusic.Sdk.ViewModels;
using StrixMusic.Shells.Groove.Messages.Navigation.Pages;
using System;

namespace StrixMusic.Shells.Groove.Controls.Items
{
    /// <inheritdoc/>
    public class GrooveAlbumItem : AlbumItem
    {
        /// <inheritdoc/>
        public GrooveAlbumItem()
        {
            this.DefaultStyleKey = typeof(GrooveAlbumItem);

            NavigateToAlbumCommand = new RelayCommand<AlbumViewModel>(new Action<AlbumViewModel?>(NavigateToAlbum));
        }
        
        public RelayCommand<AlbumViewModel> NavigateToAlbumCommand { get; private set; }

        private void NavigateToAlbum(AlbumViewModel? viewModel)
        {
            if (viewModel != null)
                WeakReferenceMessenger.Default.Send(new AlbumViewNavigationRequestMessage(viewModel));
        }
    }
}
