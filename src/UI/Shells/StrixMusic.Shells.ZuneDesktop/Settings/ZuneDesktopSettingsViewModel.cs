using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OwlCore.Extensions;
using StrixMusic.Sdk;
using StrixMusic.Sdk.Services.Settings;
using StrixMusic.Sdk.Uno.Controls.Shells;
using StrixMusic.Sdk.Uno.Services.Localization;
using StrixMusic.Shells.ZuneDesktop.Settings.Models;

namespace StrixMusic.Shells.ZuneDesktop.Settings
{
    /// <summary>
    /// The settings viewmodel for the Zune Desktop
    /// </summary>
    public class ZuneDesktopSettingsViewModel : SettingsViewModelBase
    {
        private readonly Dictionary<string, string> _displayNameMap;

        private readonly Dictionary<string, ZuneDesktopBackgroundImage> _zuneBackgroundImages = new Dictionary<string, ZuneDesktopBackgroundImage>()
        {
            { "None", new ZuneDesktopBackgroundImage() },
            { "AuroraBorealis", new ZuneDesktopBackgroundImage("AuroraBorealis", Windows.UI.Xaml.Media.AlignmentY.Top) },
            { "Bubbles", new ZuneDesktopBackgroundImage("Bubbles") },
            { "Cells", new ZuneDesktopBackgroundImage("Cells", Windows.UI.Xaml.Media.AlignmentY.Center) },
            { "Hero", new ZuneDesktopBackgroundImage("Hero", Windows.UI.Xaml.Media.AlignmentY.Center) },
            { "Meadow", new ZuneDesktopBackgroundImage("Meadow", stretch: Windows.UI.Xaml.Media.Stretch.None) },
            { "RobotOwl", new ZuneDesktopBackgroundImage("RobotOwl", Windows.UI.Xaml.Media.AlignmentY.Center) },
            { "Shards", new ZuneDesktopBackgroundImage("Shards", Windows.UI.Xaml.Media.AlignmentY.Top) },
            { "Smooth", new ZuneDesktopBackgroundImage("Smooth") },
            { "Wired", new ZuneDesktopBackgroundImage("Wired", stretch: Windows.UI.Xaml.Media.Stretch.Uniform) },
        };

        /// <summary>
        /// List of names for the images
        /// </summary>
        public IEnumerable<string> ImageNames => _displayNameMap.Keys;

        private readonly ISettingsService _settingsService;
        private readonly LocalizationResourceLoader _localizationService;
        private string _selectedBackgroundImage = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="ZuneDesktopSettingsViewModel"/> class.
        /// </summary>
        public ZuneDesktopSettingsViewModel()
        {
            _settingsService = Shell.Ioc.GetRequiredService<ISettingsService>();
            _localizationService = Shell.Ioc.GetRequiredService<LocalizationResourceLoader>();

            _displayNameMap = _zuneBackgroundImages.Keys
                .ToDictionary(x => _localizationService["StrixMusic.Shells.ZuneDesktop/ZuneSettings", x]);

            LoadInitialValues().Forget();
        }

        /// <summary>
        /// Gets or sets if the ZuneShell BackgroundImage is None.
        /// </summary>
        public string SelectedBackgroundImage
        {
            get => _selectedBackgroundImage;
            set
            {
                if (!SetProperty(ref _selectedBackgroundImage, value))
                    return;

                var image = _zuneBackgroundImages[_displayNameMap[value]];

                _settingsService.SetValue<ZuneDesktopBackgroundImage>(nameof(ZuneDesktopSettingsKeys.BackgroundImage), image).Forget();
            }
        }

        /// <summary>
        /// Gets initial values from settings and sets the properties
        /// </summary>
        /// <remarks>
        /// Once general settings are setup, this should be made a virtual method
        /// </remarks>
        private async Task LoadInitialValues()
        {
            var backgroundImage = await _settingsService.GetValue<ZuneDesktopBackgroundImage>(nameof(ZuneDesktopSettingsKeys.BackgroundImage));

            string displayName = _localizationService["StrixMusic.Shells.ZuneDesktop/ZuneSettings", backgroundImage.Name];
            SetProperty(ref _selectedBackgroundImage, displayName);
        }
    }
}
