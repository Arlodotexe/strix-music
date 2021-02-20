using StrixMusic.Shells.ZuneDesktop.Settings;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shells.ZuneDesktop.Controls.Views.Settings
{
    /// <summary>
    /// The content for the Background page of settings.
    /// </summary>
    public sealed partial class BackgroundSettings : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BackgroundSettings"/> class.
        /// </summary>
        public BackgroundSettings()
        {
            this.InitializeComponent();
        }

        private ZuneDesktopSettingsViewModel? ViewModel => DataContext as ZuneDesktopSettingsViewModel;
    }
}
