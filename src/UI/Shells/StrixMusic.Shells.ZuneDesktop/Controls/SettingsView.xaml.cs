using Microsoft.Extensions.DependencyInjection;
using StrixMusic.Sdk.Services.Navigation;
using StrixMusic.Shells.ZuneDesktop.Settings;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shells.ZuneDesktop.Controls
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

        private void NavigateToDisplay(object sender, RoutedEventArgs e)
        {
            PagesList.ItemsSource = _displayPages;
        }

        private void NavigateToBehavior(object sender, RoutedEventArgs e)
        {
            PagesList.ItemsSource = _behaviorPages;
        }

        private void NavigateToCores(object sender, RoutedEventArgs e)
        {
        }
    }
}
