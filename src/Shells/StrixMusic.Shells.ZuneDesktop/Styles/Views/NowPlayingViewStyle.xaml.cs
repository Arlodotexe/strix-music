using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using StrixMusic.Sdk.WinUI.Controls;
using StrixMusic.Sdk.WinUI.Controls.Views;

namespace StrixMusic.Shells.ZuneDesktop.Styles.Views
{
    /// <summary>
    /// A <see cref="ResourceDictionary"/> containing the style and template for the <see cref="NowPlayingView"/> in the ZuneDesktop Shell.
    /// </summary>
    public sealed partial class NowPlayingViewStyle : ResourceDictionary
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NowPlayingViewStyle"/> class.
        /// </summary>
        public NowPlayingViewStyle()
        {
            this.InitializeComponent();
        }
    }
}
