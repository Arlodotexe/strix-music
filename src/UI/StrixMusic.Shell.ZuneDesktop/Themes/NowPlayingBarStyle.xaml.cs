using Microsoft.Extensions.DependencyInjection;
using StrixMusic.Sdk.Services.Navigation;
using StrixMusic.Shell.Default.Controls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shell.ZuneDesktop.Themes
{
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
            ZuneDesktopShellIoc.Ioc.GetService<INavigationService<Control>>()!.NavigateTo(typeof(NowPlayingViewControl));
        }
    }
}
