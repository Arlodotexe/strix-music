using StrixMusic.Shells.Groove.ViewModels.Collections;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shells.Groove.Controls.Collections
{
    /// <summary>
    /// A <see cref="Control"/> for displaying <see cref="Sdk.ViewModels.PlaylistCollectionViewModel"/>s in the Groove Shell.
    /// </summary>
    public partial class GroovePlaylistCollection : Control
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GroovePlaylistCollection"/> class.
        /// </summary>
        public GroovePlaylistCollection()
        {
            this.DefaultStyleKey = typeof(GroovePlaylistCollection);
        }

        /// <summary>
        /// The ViewModel for a <see cref="GroovePlaylistCollection"/>
        /// </summary>
        public GroovePlaylistCollectionViewModel ViewModel => (GroovePlaylistCollectionViewModel)DataContext;
    }
}
