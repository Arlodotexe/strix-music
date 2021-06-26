using StrixMusic.Sdk.ViewModels;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shells.ZuneDesktop.Controls.Views.Collection
{
    /// <summary>
    /// A control that shows the properties of a Playlist.
    /// </summary>
    public sealed partial class PlaylistDetailsPane : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlaylistDetailsPane"/> class.
        /// </summary>
        public PlaylistDetailsPane()
        {
            this.InitializeComponent();
        }

        private PlaylistViewModel? ViewModel => DataContext as PlaylistViewModel;
    }
}
