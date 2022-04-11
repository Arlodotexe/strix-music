using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using StrixMusic.Sdk.Uno.Controls.Items;
using StrixMusic.Sdk.ViewModels;
using StrixMusic.Shells.Groove.Messages.Navigation.Pages;
using System;

namespace StrixMusic.Shells.Groove.Controls.Items
{
    public class GrooveTrackItem : TrackItem
    {
        public GrooveTrackItem()
        {
            this.DefaultStyleKey = typeof(GrooveTrackItem);

            NavigateToAlbumCommand = new RelayCommand<TrackViewModel>(new Action<TrackViewModel?>(NavigateToAlbum));
        }

        public RelayCommand<TrackViewModel> NavigateToAlbumCommand { get; private set; }

        private void NavigateToAlbum(TrackViewModel? viewModel)
        {
            if (viewModel != null && viewModel.Album != null)
                WeakReferenceMessenger.Default.Send(new AlbumViewNavigationRequestMessage(viewModel.Album));
        }
    }
}
