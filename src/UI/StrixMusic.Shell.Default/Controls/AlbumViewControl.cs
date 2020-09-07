using StrixMusic.Sdk.Observables;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shell.Default.Controls
{
    /// <summary>
    /// A Templated <see cref="Control"/> for showing an <see cref="ObservableAlbum"/> as a page.
    /// </summary>
    public sealed partial class AlbumViewControl : Control
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AlbumViewControl"/> class.
        /// </summary>
        public AlbumViewControl()
        {
            this.DefaultStyleKey = typeof(AlbumViewControl);
        }
    }
}
