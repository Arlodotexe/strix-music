using StrixMusic.Shells.Groove.ViewModels.Pages;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shells.Groove.Controls.Pages
{
    /// <summary>
    /// A <see cref="Control"/> to display a <see cref="Sdk.ViewModels.PlaylistViewModel"/> on a page.
    /// </summary>
    public partial class GroovePlaylistPage : Control
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GroovePlaylistPage"/> class.
        /// </summary>
        public GroovePlaylistPage()
        {
            this.DefaultStyleKey = typeof(GroovePlaylistPage);
        }

        /// <summary>
        /// The <see cref="GroovePlaylistPageViewModel"/> for the <see cref="GrooveHomePage"/> template.
        /// </summary>
        public GroovePlaylistPageViewModel ViewModel => (GroovePlaylistPageViewModel)DataContext;
    }
}
