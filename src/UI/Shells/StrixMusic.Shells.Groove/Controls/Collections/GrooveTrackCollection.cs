using StrixMusic.Shells.Groove.ViewModels.Collections;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shells.Groove.Controls.Collections
{
    /// <summary>
    /// A <see cref="Control"/> for displaying <see cref="Sdk.ViewModels.TrackCollectionViewModel"/>s in the Groove Shell.
    /// </summary>
    public partial class GrooveTrackCollection : Control
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GrooveTrackCollection"/> class.
        /// </summary>
        public GrooveTrackCollection()
        {
            this.DefaultStyleKey = typeof(GrooveTrackCollection);
        }

        /// <summary>
        /// The ViewModel for a <see cref="GrooveTrackCollection"/>
        /// </summary>
        public GrooveTrackCollectionViewModel ViewModel => (GrooveTrackCollectionViewModel)DataContext;
    }
}
