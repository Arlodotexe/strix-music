using StrixMusic.Sdk.Observables;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shell.Default.Controls
{
    /// <summary>
    /// A Templated <see cref="Control"/> for showing an <see cref="ObservableTrack"/> in a list.
    /// </summary>
    public sealed partial class TrackItem : Control
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TrackItem"/> class.
        /// </summary>
        public TrackItem()
        {
            this.DefaultStyleKey = typeof(TrackItem);
        }
    }
}
