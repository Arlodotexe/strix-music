using StrixMusic.Shells.Groove.ViewModels.Pages;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shells.Groove.Controls.Pages
{
    /// <summary>
    /// A <see cref="Control"/> to display a <see cref="Sdk.ViewModels.ArtistViewModel"/> on a page.
    /// </summary>
    public partial class GrooveArtistPage : Control
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GrooveArtistPage"/> class.
        /// </summary>
        public GrooveArtistPage()
        {
            this.DefaultStyleKey = typeof(GrooveArtistPage);
        }

        /// <summary>
        /// The <see cref="GrooveArtistPageViewModel"/> for the <see cref="GrooveArtistPage"/> template.
        /// </summary>
        public GrooveArtistPageViewModel ViewModel => (GrooveArtistPageViewModel)DataContext;
    }
}
