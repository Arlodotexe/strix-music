using Windows.UI.Xaml.Controls;
using StrixMusic.Sdk.ViewModels;

namespace StrixMusic.Sdk.Uno.Controls
{
    /// <summary>
    /// A Templated <see cref="Control"/> for displaying an <see cref="AlbumViewModel"/> in a list.
    /// </summary>
    public sealed partial class AlbumItem : ItemControl
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
