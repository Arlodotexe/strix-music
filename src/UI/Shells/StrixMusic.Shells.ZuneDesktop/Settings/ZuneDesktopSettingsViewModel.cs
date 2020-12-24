using Microsoft.Toolkit.Diagnostics;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using StrixMusic.Sdk;
using StrixMusic.Sdk.Services.Localization;
using StrixMusic.Sdk.Uno.Services.Localization;
using System.Collections.Generic;
using System.Linq;

namespace StrixMusic.Shells.ZuneDesktop.Settings
{
    /// <summary>
    /// The settings viewmodel for the Zune Desktop
    /// </summary>
    public class ZuneDesktopSettingsViewModel : SettingsViewModelBase
    {
        private LocalizationLoaderService? _localizationService = null;

        private LocalizationLoaderService LocalizationService => _localizationService ?? (_localizationService = Ioc.Default.GetService<LocalizationLoaderService>())!;

        private readonly Dictionary<string, string> _displayNameMap = new Dictionary<string, string>();

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

        private readonly ZuneDesktopSettingsService _zuneDesktopSettingsService;
        private string _selectedBackgroundImage = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="ZuneDesktopSettingsViewModel"/> class.
        /// </summary>
        public ZuneDesktopSettingsViewModel()
        {
            _zuneDesktopSettingsService = ZuneDesktopShellIoc.Ioc.GetService<ZuneDesktopSettingsService>() ?? ThrowHelper.ThrowInvalidOperationException<ZuneDesktopSettingsService>();

            _displayNameMap = _zuneBackgroundImages.Keys
                .ToDictionary(x => LocalizationService["StrixMusic.Shells.ZuneDesktop/ZuneSettings", x]);
            
            LoadInitalValues();
        }

        /// <summary>
        /// Gets or sets if the ZuneShell BackgroundImage is None.
        /// </summary>
        public string SelectedBackgroundImage
        {
            get => _selectedBackgroundImage;
            set
            {
                if (SetProperty(ref _selectedBackgroundImage, value))
                {
                    ZuneDesktopBackgroundImage image = _zuneBackgroundImages[_displayNameMap[value]];
                    _ = _zuneDesktopSettingsService.SetValue<ZuneDesktopBackgroundImage>(nameof(ZuneDesktopSettingsKeys.BackgroundImage), image);
                }
            }
        }

        /// <summary>
        /// Gets initial values from settings and sets the properties
        /// </summary>
        /// <remarks>
        /// Once general settings are setup, this should be made a virtual method
        /// </remarks>
        private async void LoadInitalValues()
        {
            ZuneDesktopBackgroundImage backgroundImage = await _zuneDesktopSettingsService.GetValue<ZuneDesktopBackgroundImage>(nameof(ZuneDesktopSettingsKeys.BackgroundImage));
            var displayName = LocalizationService["StrixMusic.Shells.ZuneDesktop/ZuneSettings", backgroundImage.Name];
            SetProperty(ref _selectedBackgroundImage, displayName);
        }
    }
}
