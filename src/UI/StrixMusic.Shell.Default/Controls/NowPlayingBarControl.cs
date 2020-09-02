using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shell.Default.Controls
{
    /// <summary>
    /// A Templated <see cref="Control"/> for the NowPlaying bar in a Shell.
    /// </summary>
    public sealed partial class NowPlayingBarControl : Control
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NowPlayingBarControl"/> class.
        /// </summary>
        public NowPlayingBarControl()
        {
            this.DefaultStyleKey = typeof(NowPlayingBarControl);
        }
    }
}
