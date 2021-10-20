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
        private PlaylistViewModel? _selectedPlaylist;

        /// <summary>
        /// Initializes a new instance of the <see cref="GrooveArtistCollectionViewModel"/> class.
        /// </summary>
        public GroovePlaylistCollectionViewModel()
        {
            NavigateToPlaylistCommand = new RelayCommand<PlaylistViewModel>(NavigateToPlaylist);
        }

        /// <summary>
        /// The <see cref="IPlaylistCollectionViewModel"/> to display.
        /// </summary>
        public IPlaylistCollectionViewModel? PlaylistCollection
        {
            get => _playlistCollectionViewModel;
            set => SetProperty(ref _playlistCollectionViewModel, value);
        }

        /// <summary>
        /// Gets or sets the selected playlist
        /// </summary>
        public PlaylistViewModel SelectedPlaylist
        {
            get => _selectedPlaylist!;
            set => SetProperty(ref _selectedPlaylist, value);
        }

        /// <summary>
        /// A Command that requests a navigation to a playlist page.
        /// </summary>
        public RelayCommand<PlaylistViewModel> NavigateToPlaylistCommand { get; private set; }

        private void NavigateToPlaylist(PlaylistViewModel? viewModel)
        {
            if (viewModel != null)
                WeakReferenceMessenger.Default.Send(new PlaylistViewNavigationRequestMessage(viewModel));
        }
    }
}
