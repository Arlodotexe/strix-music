using Microsoft.Toolkit.Mvvm.ComponentModel;
using StrixMusic.Sdk.ViewModels;

namespace StrixMusic.Shells.Groove.ViewModels.Pages
{
    /// <summary>
    /// A ViewModel for an <see cref="Controls.Pages.GroovePlaylistsPage"/>.
    /// </summary>
    public class GroovePlaylistsPageViewModel : ObservableObject
    {
        private IPlaylistCollectionViewModel? _playlistCollectionViewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="GroovePlaylistPageViewModel"/> class.
        /// </summary>
        /// <param name="viewModel">The <see cref="PlaylistViewModel"/> inside this ViewModel on display.</param>
        public GroovePlaylistsPageViewModel()
        {
        }

        /// <summary>
        /// The <see cref="PlaylistViewModel"/> inside this ViewModel on display.
        /// </summary>
        public IPlaylistCollectionViewModel? PlaylistCollection
        {
            get => _playlistCollectionViewModel;
            set => SetProperty(ref _playlistCollectionViewModel, value);
        }
    }
}
