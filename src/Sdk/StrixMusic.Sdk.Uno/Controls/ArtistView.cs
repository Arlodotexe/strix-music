using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using OwlCore.Extensions;
using StrixMusic.Sdk.ViewModels;

namespace StrixMusic.Sdk.Uno.Controls
{
    /// <summary>
    /// A Templated <see cref="Control"/> for displaying an <see cref="ArtistViewModel"/> as a page.
    /// </summary>
    public sealed partial class ArtistView : Control
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ArtistView"/> class.
        /// </summary>
        /// <param name="artistViewModel">The Artist in view.</param>
        public ArtistView(ArtistViewModel artistViewModel)
        {
            this.DefaultStyleKey = typeof(ArtistView);
            DataContext = artistViewModel;
        }

        /// <summary>
        /// The <see cref="ArtistViewModel"/> for the control.
        /// </summary>
        public ArtistViewModel ViewModel => (ArtistViewModel)DataContext;
    }
}
