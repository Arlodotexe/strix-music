using Microsoft.Extensions.DependencyInjection;
using StrixMusic.Services.Navigation;
using StrixMusic.Shell.Strix;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shell.ZuneDesktop.Themes
{
    public sealed partial class SettingsViewStyle : ResourceDictionary
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsViewStyle"/> class.
        /// </summary>
        public SettingsViewStyle()
        {
            this.InitializeComponent();
        }

        private void SaveClicked(object sender, RoutedEventArgs e)
        {
            // TODO: Save settings changes
            ZuneDesktopShellIoc.Ioc.GetService<INavigationService<Control>>()!.GoBack();
        }

        private void CancelClicked(object sender, RoutedEventArgs e)
        {
            ZuneDesktopShellIoc.Ioc.GetService<INavigationService<Control>>()!.GoBack();
        }
    }
}
