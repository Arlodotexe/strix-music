using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using StrixMusic.Sdk.Uno.Controls.Items;
using StrixMusic.Sdk.ViewModels;
using StrixMusic.Shells.Groove.Messages.Navigation.Pages;
using System;

namespace StrixMusic.Shells.Groove.Controls.Items
{
    public class GroovePlaylistItem : PlaylistItem
    {
        public GroovePlaylistItem()
        {
            this.DefaultStyleKey = typeof(GroovePlaylistItem);

            NavigateToPlaylistCommand = new RelayCommand<PlaylistViewModel>(new Action<PlaylistViewModel?>(NavigateToPlaylist));
        }

        public RelayCommand<PlaylistViewModel> NavigateToPlaylistCommand { get; private set; }

        private void NavigateToPlaylist(PlaylistViewModel? viewModel)
        {
            if (viewModel != null)
                WeakReferenceMessenger.Default.Send(PlaylistViewNavigationRequested.To(viewModel));
        }
    }
}
