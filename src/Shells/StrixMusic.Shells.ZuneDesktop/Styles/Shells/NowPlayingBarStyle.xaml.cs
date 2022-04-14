using StrixMusic.Sdk.Services.Navigation;
using StrixMusic.Sdk.WinUI.Controls.Shells;
using StrixMusic.Sdk.WinUI.Controls.Views;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shells.ZuneDesktop.Styles.Shells
{
    /// <summary>
    /// A <see cref="ResourceDictionary"/> containing the style and template for the <see cref="NowPlayingBar"/> in the ZuneDesktop Shell.
    /// </summary>
    public sealed partial class NowPlayingBarStyle : ResourceDictionary
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NowPlayingBarStyle"/> class.
        /// </summary>
        public NowPlayingBarStyle()
        {
            this.InitializeComponent();
        }
    }
}
