using StrixMusic.Sdk.WinUI.Controls;

namespace StrixMusic.Shells.ZuneDesktop.Controls.Shells
{
    /// <summary>
    /// A <see cref="NowPlayingBar"/> subclass for the Zune Desktop shell.
    /// </summary>
    public sealed partial class ZuneNowPlayingBar : NowPlayingBar
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ZuneNowPlayingBar"/> class.
        /// </summary>
        public ZuneNowPlayingBar()
        {
            DefaultStyleKey = typeof(ZuneNowPlayingBar);
        }
    }
}
