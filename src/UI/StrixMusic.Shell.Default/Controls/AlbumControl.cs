using StrixMusic.ViewModels.Bindables;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shell.Default.Controls
{
    /// <summary>
    /// A Templated <see cref="Control"/> for displaying an <see cref="ObservableAlbum"/> in a list.
    /// </summary>
    public sealed partial class AlbumControl : Control
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AlbumControl"/> class.
        /// </summary>
        public AlbumControl()
        {
            this.DefaultStyleKey = typeof(AlbumControl);
        }
    }
}
