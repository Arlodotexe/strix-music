using StrixMusic.Sdk.Observables;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shell.Default.Controls
{
    /// <summary>
    /// A Templated <see cref="Control"/> for displaying an <see cref="ObservableArtist"/> as a page.
    /// </summary>
    public sealed partial class ArtistViewControl : Control
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ArtistViewControl"/> class.
        /// </summary>
        public ArtistViewControl()
        {
            this.DefaultStyleKey = typeof(ArtistViewControl);
        }
    }
}
