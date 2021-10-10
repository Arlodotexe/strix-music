using StrixMusic.Shells.Groove.ViewModels.Pages;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shells.Groove.Controls.Pages
{
    /// <summary>
    /// A <see cref="Control"/> to display a <see cref="Sdk.ViewModels.LibraryViewModel"/> on a page.
    /// </summary>
    public partial class GrooveHomePage : Control
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GrooveHomePage"/> class.
        /// </summary>
        public GrooveHomePage()
        {
            this.DefaultStyleKey = typeof(GrooveHomePage);
        }

        /// <summary>
        /// The <see cref="GrooveHomePageViewModel"/> for the <see cref="GrooveHomePage"/> template.
        /// </summary>
        public GrooveHomePageViewModel ViewModel => (GrooveHomePageViewModel)DataContext;
    }
}
