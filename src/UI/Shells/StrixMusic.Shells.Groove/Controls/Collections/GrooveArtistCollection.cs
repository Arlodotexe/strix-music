using StrixMusic.Shells.Groove.ViewModels.Collections;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shells.Groove.Controls.Collections
{
    /// <summary>
    /// A <see cref="Control"/> for displaying <see cref="Sdk.ViewModels.ArtistCollectionViewModel"/>s in the Groove Shell.
    /// </summary>
    public partial class GrooveArtistCollection : Control
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GrooveTrackCollection"/> class.
        /// </summary>
        public GrooveArtistCollection()
        {
            this.DefaultStyleKey = typeof(GrooveArtistCollection);
        }

        /// <summary>
        /// The ViewModel for a <see cref="GrooveArtistCollection"/>
        /// </summary>
        public GrooveArtistCollectionViewModel ViewModel => (GrooveArtistCollectionViewModel)DataContext;
    }
}
