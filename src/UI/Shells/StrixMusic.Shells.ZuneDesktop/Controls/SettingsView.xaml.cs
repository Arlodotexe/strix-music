using Microsoft.Toolkit.Mvvm.DependencyInjection;
using StrixMusic.Sdk.Services.Navigation;
using StrixMusic.Sdk.Uno.Services.Localization;
using StrixMusic.Shells.ZuneDesktop.Settings;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shells.ZuneDesktop.Controls
{
    /// <summary>
    /// The Settings page in the ZuneDesktop shell.
    /// </summary>
    public sealed partial class SettingsView : UserControl
    {
        private LocalizationLoaderService? _localizationService = null;

        private LocalizationLoaderService LocalizationService => _localizationService ?? (_localizationService = Ioc.Default.GetService<LocalizationLoaderService>())!;

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsView"/> class.
        /// </summary>
        public SettingsView()
        {
            this.InitializeComponent();
            _displayPages = _displayPages.Select(x => LocalizationService["StrixMusic.Shells.ZuneDesktop/ZuneSettings", x].ToUpper()).ToList();
        }

        private ZuneDesktopSettingsViewModel? ViewModel => DataContext as ZuneDesktopSettingsViewModel;

        /// <remarks>
        /// Translated in constructor.
        /// </remarks>
        private readonly IEnumerable<string> _displayPages = new string[]
        {
            "Background",
            "Scale",
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
            //TODO: Revert unsaved changes
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
