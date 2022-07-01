using System;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using StrixMusic.Sdk.ViewModels;
using StrixMusic.Sdk.WinUI.Controls.Items;
using StrixMusic.Shells.Groove.Messages.Navigation.Pages;

namespace StrixMusic.Shells.Groove.Controls.Items
{
    /// <inheritdoc/>
    public partial class GrooveTrackItem : TrackItem
    {
        /// <inheritdoc/>
        public GrooveTrackItem()
        {
            this.DefaultStyleKey = typeof(GrooveTrackItem);

            NavigateToAlbumCommand = new RelayCommand<TrackViewModel>(new Action<TrackViewModel?>(NavigateToAlbum));
        }
        
        /// <summary>
        /// A command that triggers navigation to the provided track's album.
        /// </summary>
        public RelayCommand<TrackViewModel> NavigateToAlbumCommand { get; private set; }

        private void NavigateToAlbum(TrackViewModel? viewModel)
        {
            if (viewModel != null && viewModel.Album != null)
                WeakReferenceMessenger.Default.Send(new AlbumViewNavigationRequestMessage(viewModel.Album));
        }
    }
}
