using CommunityToolkit.Mvvm.ComponentModel;
using StrixMusic.Sdk.ViewModels;
using StrixMusic.Shells.Groove.ViewModels.Collections;
using Windows.UI;

namespace StrixMusic.Shells.Groove.ViewModels.Pages
{
    /// <summary>
    /// A ViewModel for an <see cref="Controls.Pages.GroovePlaylistPage"/>.
    /// </summary>
    public class GroovePlaylistPageViewModel : ObservableObject
    {
        private PlaylistViewModel? _playlistViewModel;
        private Color? _backgroundColor;

        /// <summary>
        /// Initializes a new instance of the <see cref="GroovePlaylistPageViewModel"/> class.
        /// </summary>
        public GroovePlaylistPageViewModel()
        {
        }

        /// <summary>
        /// The <see cref="PlaylistViewModel"/> inside this ViewModel on display.
        /// </summary>
        public PlaylistViewModel? Playlist
        {
            get => _playlistViewModel;
            set => SetProperty(ref _playlistViewModel, value);
        }

        /// <summary>
        /// Gets or sets the color of the background for the <see cref="Controls.Pages.GroovePlaylistPage"/>.
        /// </summary>
        public Color? BackgroundColor
        {
            get => _backgroundColor;
            set => SetProperty(ref _backgroundColor, value);
        }
    }
}
