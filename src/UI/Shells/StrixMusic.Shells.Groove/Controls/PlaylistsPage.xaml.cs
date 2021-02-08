using StrixMusic.Sdk;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shells.Groove.Controls
{
    /// <summary>
    /// A page for displaying playlists in Groove shell.
    /// </summary>
    public sealed partial class PlaylistsPage : UserControl
    {
        public PlaylistsPage()
        {
            this.InitializeComponent();
        }

        private MainViewModel ViewModel => (MainViewModel)DataContext;
    }
}
