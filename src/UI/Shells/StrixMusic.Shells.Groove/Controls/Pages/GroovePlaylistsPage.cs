using StrixMusic.Shells.Groove.ViewModels.Pages;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shells.Groove.Controls.Pages
{
    /// <summary>
    /// A <see cref="Control"/> to display an <see cref="Sdk.ViewModels.IPlaylistCollectionViewModel"/> on a page.
    /// </summary>
    public partial class GroovePlaylistsPage : Control
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GroovePlaylistPage"/> class.
        /// </summary>
        public GroovePlaylistsPage()
        {
            this.DefaultStyleKey = typeof(GroovePlaylistsPage);
        }

        /// <summary>
        /// The <see cref="GroovePlaylistsPageViewModel"/> for the <see cref="GroovePlaylistsPage"/> template.
        /// </summary>
        public GroovePlaylistsPageViewModel ViewModel => (GroovePlaylistsPageViewModel)DataContext;
    }
}
