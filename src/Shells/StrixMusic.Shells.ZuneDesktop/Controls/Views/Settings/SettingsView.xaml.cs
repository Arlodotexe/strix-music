using System.Collections.Generic;
using System.Linq;
using StrixMusic.Shells.ZuneDesktop.Settings;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shells.ZuneDesktop.Controls.Views.Settings
{
    /// <summary>
    /// The Settings page in the ZuneDesktop shell.
    /// </summary>
    public sealed partial class SettingsView : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsView"/> class.
        /// </summary>
        public SettingsView()
        {
            this.InitializeComponent();

            var localizationService = ResourceLoader.GetForCurrentView("StrixMusic.Shells.ZuneDesktop/ZuneSettings");

            _displayPages = _displayPages.Select(x => localizationService.GetString(x).ToUpper()).ToList();
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
        }

        private void CancelClicked(object sender, RoutedEventArgs e)
        {
            //TODO: Revert unsaved changes
        }

        private void NavigateToDisplay(object sender, RoutedEventArgs e)
        {
            PagesList.ItemsSource = _displayPages;
        }

        private void NavigateToBehavior(object sender, RoutedEventArgs e)
        {
            PagesList.ItemsSource = _behaviorPages;
        }
    }
}
