using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using StrixMusic.Sdk.WinUI.Controls.Items;
using StrixMusic.Sdk.ViewModels;
using StrixMusic.Shells.Groove.Messages.Navigation.Pages;
using System;

namespace StrixMusic.Shells.Groove.Controls.Items
{
    public class GrooveArtistItem : ArtistItem
    {
        public GrooveArtistItem()
        {
            this.DefaultStyleKey = typeof(GrooveArtistItem);

            NavigateToArtistCommand = new RelayCommand<ArtistViewModel>(new Action<ArtistViewModel?>(NavigateToArtist));
        }

        public RelayCommand<ArtistViewModel> NavigateToArtistCommand { get; private set; }

        private void NavigateToArtist(ArtistViewModel? viewModel)
        {
            if (viewModel != null)
                WeakReferenceMessenger.Default.Send(new ArtistViewNavigationRequestMessage(viewModel));
        }
    }
}
