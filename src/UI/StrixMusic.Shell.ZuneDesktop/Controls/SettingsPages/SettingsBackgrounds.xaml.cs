using StrixMusic.Shell.ZuneDesktop.Settings;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shell.ZuneDesktop.Controls.SettingsPages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsBackgrounds : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsBackgrounds"/> class.
        /// </summary>
        public SettingsBackgrounds()
        {
            this.InitializeComponent();
        }

        private ZuneDesktopSettingsViewModel? ViewModel => DataContext as ZuneDesktopSettingsViewModel;
    }
}
