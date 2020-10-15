using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shells.Controls
{
    /// <summary>
    /// A Templated <see cref="Control"/> for displaying the NowPlaying view.
    /// </summary>
    public partial class NowPlayingView : Control
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NowPlayingView"/> class.
        /// </summary>
        public NowPlayingView()
        {
            this.DefaultStyleKey = typeof(NowPlayingView);
        }
    }
}
