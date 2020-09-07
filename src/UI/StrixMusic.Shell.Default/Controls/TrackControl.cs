using StrixMusic.Sdk.Observables;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shell.Default.Controls
{
    /// <summary>
    /// A Templated <see cref="Control"/> for showing an <see cref="ObservableTrack"/> in a list.
    /// </summary>
    public sealed partial class TrackControl : Control
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TrackControl"/> class.
        /// </summary>
        public TrackControl()
        {
            this.DefaultStyleKey = typeof(TrackControl);
        }
    }
}
