using StrixMusic.Sdk.Services.Navigation;
using StrixMusic.Sdk.WinUI.Controls.Shells;
using StrixMusic.Sdk.WinUI.Controls.Views;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shells.ZuneDesktop.Styles.NowPlaying
{
    public partial class ZuneMediaTransportStyle : ResourceDictionary
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ZuneMediaTransportStyle"/> class.
        /// </summary>
        public ZuneMediaTransportStyle()
        {
            this.InitializeComponent();
        }

        private void NavigateToNowPlaying(object sender, RoutedEventArgs e)
        {
            Shell.Ioc.GetRequiredService<INavigationService<Control>>().NavigateTo(typeof(NowPlayingView));
        }
    }
}
