using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Uwp.UI.Extensions;
using StrixMusic.Sdk.Services.Navigation;
using StrixMusic.Shell.Strix;
using StrixMusic.Shell.ZuneDesktop.Settings;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shell.ZuneDesktop.Controls
{
    /// <summary>
    /// A thing. TODO: Comment better
    /// </summary>
    public sealed partial class SettingsView : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsView"/> class.
        /// </summary>
        public SettingsView()
        {
            this.InitializeComponent();
        }

        private ZuneDesktopSettingsViewModel? ViewModel => DataContext as ZuneDesktopSettingsViewModel;

        private readonly List<string> _displayPages = new List<string>()
        {
            "BACKGROUNDS",
            "SCALING",
            "TEST1",
            "TEST2",
            "TEST3",
            "TEST4",
            "TEST5",
            "TEST6",
            "TEST7",
            "TEST8",
        };

        private readonly List<string> _behaviorPages = new List<string>()
        {
            "MERGE BY",
        };

        private void SaveClicked(object sender, RoutedEventArgs e)
        {
            // TODO: Save settings changes
            ZuneDesktopShellIoc.Ioc.GetService<INavigationService<Control>>()!.GoBack();
        }

        private void CancelClicked(object sender, RoutedEventArgs e)
        {
            ZuneDesktopShellIoc.Ioc.GetService<INavigationService<Control>>()!.GoBack();
        }

        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (PagesPivot.SelectedIndex)
            {
                case 0:
                    PagesList.ItemsSource = _displayPages;
                    break;
                case 1:
                    PagesList.ItemsSource = _behaviorPages;
                    break;
                case 2:
                    // TODO:
                    break;
                default:
                    break;
            }
        }
    }
}
