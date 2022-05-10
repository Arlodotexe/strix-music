using System;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using StrixMusic.Sdk.ViewModels;
using StrixMusic.Sdk.WinUI.Controls.Items;
using StrixMusic.Shells.Groove.Messages.Navigation.Pages;

namespace StrixMusic.Shells.Groove.Controls.Items
{
    /// <inheritdoc/>
    public class GrooveArtistItem : ArtistItem
    {
        /// <inheritdoc/>
        public GrooveArtistItem()
        {
            this.DefaultStyleKey = typeof(GrooveArtistItem);

            NavigateToArtistCommand = new RelayCommand<ArtistViewModel>(new Action<ArtistViewModel?>(NavigateToArtist));
        }

        /// <summary>
        /// A command that triggers navigation to the provided artist.
        /// </summary>
        public RelayCommand<ArtistViewModel> NavigateToArtistCommand { get; private set; }

        private void NavigateToArtist(ArtistViewModel? viewModel)
        {
            if (viewModel != null)
                WeakReferenceMessenger.Default.Send(new ArtistViewNavigationRequestMessage(viewModel));
        }
    }
}
