using StrixMusic.ViewModels.Bindables;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shell.Default.Controls
{
    /// <summary>
    /// A Templated <see cref="Control"/> for displaying any Object containing a list of <see cref="ObservableTrack"/>.
    /// </summary>
    public sealed partial class TrackListControl : Control
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TrackListControl"/> class.
        /// </summary>
        public TrackListControl()
        {
            this.DefaultStyleKey = typeof(TrackListControl);
        }
    }
}
