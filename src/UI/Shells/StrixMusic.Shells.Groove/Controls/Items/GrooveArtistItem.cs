using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using StrixMusic.Sdk.Uno.Controls.Items;
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

#warning Move this to a ViewModel
        public RelayCommand<ArtistViewModel> NavigateToArtistCommand { get; private set; }

        private void NavigateToArtist(ArtistViewModel? viewModel)
        {
            if (viewModel != null)
                WeakReferenceMessenger.Default.Send(new ArtistViewNavigationRequestMessage(viewModel));
        }
    }
}
