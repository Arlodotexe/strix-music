using StrixMusic.Sdk.Services.Navigation;
using StrixMusic.Sdk.Uno.Controls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shells.ZuneDesktop.Styles
{
    /// <summary>
    /// A <see cref="ResourceDictionary"/> containing the style and template for the <see cref="NowPlayingBar"/> in the ZuneDesktop Shell.
    /// </summary>
    public sealed partial class NowPlayingBarStyle : ResourceDictionary
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsViewStyle"/> class.
        /// </summary>
        public NowPlayingBarStyle()
        {
            this.InitializeComponent();
        }

        private void NavigateToNowPlaying(object sender, RoutedEventArgs e)
        {
            // TODO: Save settings changes
            Shell.Ioc.GetService<INavigationService<Control>>()!.NavigateTo(typeof(NowPlayingView));
        }
    }
}
