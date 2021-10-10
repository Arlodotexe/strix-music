using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using StrixMusic.Sdk.ViewModels;
using StrixMusic.Shells.Groove.Messages.Navigation.Pages;

namespace StrixMusic.Shells.Groove.ViewModels.Collections
{
    /// <summary>
    /// A ViewModel for a <see cref="Controls.Collections.GroovePlaylistCollection"/>.
    /// </summary>
    public class GroovePlaylistCollectionViewModel : ObservableObject
    {
        private IPlaylistCollectionViewModel? _playlistCollectionViewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="GrooveArtistCollectionViewModel"/> class.
        /// </summary>
        public GroovePlaylistCollectionViewModel(IPlaylistCollectionViewModel? viewModel)
        {
            ViewModel = viewModel;
            NavigateToPlaylistCommand = new RelayCommand<PlaylistViewModel>(NavigateToPlaylist);
        }

        /// <summary>
        /// The <see cref="IPlaylistCollectionViewModel"/> inside this ViewModel on display.
        /// </summary>
        public IPlaylistCollectionViewModel? ViewModel
        {
            get => _playlistCollectionViewModel;
            set => SetProperty(ref _playlistCollectionViewModel, value);
        }

        /// <summary>
        /// A Command that requests a navigation to a playlist page.
        /// </summary>
        public RelayCommand<PlaylistViewModel> NavigateToPlaylistCommand { get; private set; }

        private void NavigateToPlaylist(PlaylistViewModel? viewModel)
        {
            if (viewModel != null)
                WeakReferenceMessenger.Default.Send(new PlaylistViewNavigationRequested(viewModel));
        }
    }
}
