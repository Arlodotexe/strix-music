using StrixMusic.Sdk.Observables;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shell.Default.Controls
{
    /// <summary>
    /// A Templated <see cref="Control"/> for showing an <see cref="ObservableAlbum"/> as a page.
    /// </summary>
    public sealed partial class AlbumView : Control
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AlbumView"/> class.
        /// </summary>
        public AlbumView()
        {
            this.DefaultStyleKey = typeof(AlbumView);
        }
    }
}
