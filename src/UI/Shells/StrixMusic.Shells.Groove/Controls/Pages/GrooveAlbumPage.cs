using StrixMusic.Shells.Groove.ViewModels.Pages;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shells.Groove.Controls.Pages
{
    /// <summary>
    /// A <see cref="Control"/> to display a <see cref="Sdk.ViewModels.AlbumViewModel"/> on a page.
    /// </summary>
    public partial class GrooveAlbumPage : Control
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GrooveAlbumPage"/> class.
        /// </summary>
        public GrooveAlbumPage()
        {
            this.DefaultStyleKey = typeof(GrooveAlbumPage);
        }

        /// <summary>
        /// The <see cref="GrooveAlbumPageViewModel"/> for the <see cref="GrooveAlbumPage"/> template.
        /// </summary>
        public GrooveAlbumPageViewModel ViewModel => (GrooveAlbumPageViewModel)DataContext;
    }
}
