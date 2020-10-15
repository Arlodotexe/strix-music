using StrixMusic.Sdk.Observables;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shells.Controls
{
    /// <summary>
    /// A Templated <see cref="Control"/> for displaying an <see cref="ObservableAlbum"/> in a list.
    /// </summary>
    public sealed partial class AlbumItem : Control
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AlbumItem"/> class.
        /// </summary>
        public AlbumItem()
        {
            this.DefaultStyleKey = typeof(AlbumItem);
        }
    }
}
