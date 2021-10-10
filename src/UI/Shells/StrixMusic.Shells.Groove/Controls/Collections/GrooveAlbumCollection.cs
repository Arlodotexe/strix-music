using StrixMusic.Shells.Groove.ViewModels.Collections;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shells.Groove.Controls.Collections
{
    /// <summary>
    /// A <see cref="Control"/> for displaying <see cref="Sdk.ViewModels.AlbumCollectionViewModel"/>s in the Groove Shell.
    /// </summary>
    public partial class GrooveAlbumCollection : Control
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GrooveAlbumCollection"/> class.
        /// </summary>
        public GrooveAlbumCollection()
        {
            this.DefaultStyleKey = typeof(GrooveAlbumCollection);
        }

        /// <summary>
        /// The ViewModel for a <see cref="GrooveAlbumCollection"/>
        /// </summary>
        public GrooveAlbumCollectionViewModel ViewModel => (GrooveAlbumCollectionViewModel)DataContext;
    }
}
