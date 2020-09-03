using Microsoft.Extensions.DependencyInjection;
using StrixMusic.Services.Navigation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shell.Strix.Themes
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            INavigationService<Control> navigationService = StrixShellIoc.Ioc.GetService<INavigationService<Control>>();
            navigationService.GoBack();
        }
    }
}
