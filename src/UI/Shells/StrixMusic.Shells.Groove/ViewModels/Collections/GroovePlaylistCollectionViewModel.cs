using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using StrixMusic.Sdk.ViewModels;
using StrixMusic.Shells.Groove.Messages.Navigation.Pages;
using StrixMusic.Shells.Groove.ViewModels.Abstract;

namespace StrixMusic.Shells.Groove.ViewModels.Collections
{
    public class GroovePlaylistCollectionViewModel : GrooveViewModel<IPlaylistCollectionViewModel>
    {
        public GroovePlaylistCollectionViewModel(IPlaylistCollectionViewModel viewModel) : base(viewModel)
        {
            NavigateToPlaylistCommand = new RelayCommand<PlaylistViewModel>(NavigateToPlaylist);
        }

        public RelayCommand<PlaylistViewModel> NavigateToPlaylistCommand { get; private set; }

        private void NavigateToPlaylist(PlaylistViewModel? viewModel)
        {
            if (viewModel != null)
                WeakReferenceMessenger.Default.Send(new PlaylistViewNavigationRequested(viewModel));
        }
    }
}
