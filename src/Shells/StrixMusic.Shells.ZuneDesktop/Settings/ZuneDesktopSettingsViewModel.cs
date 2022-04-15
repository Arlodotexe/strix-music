using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using OwlCore.Extensions;
using StrixMusic.Sdk.Services;
using StrixMusic.Sdk.WinUI.Services.Localization;
using StrixMusic.Shells.ZuneDesktop.Settings.Models;

namespace StrixMusic.Shells.ZuneDesktop.Settings
{
    /// <summary>
    /// The settings viewmodel for the Zune Desktop
    /// </summary>
    public class ZuneDesktopSettingsViewModel : ObservableObject
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
            { "Shards", new ZuneDesktopBackgroundImage("Shards", Windows.UI.Xaml.Media.AlignmentY.Top) },
            { "Smooth", new ZuneDesktopBackgroundImage("Smooth") },
            { "Wired", new ZuneDesktopBackgroundImage("Wired", stretch: Windows.UI.Xaml.Media.Stretch.Uniform) },
        };

        /// <summary>
        /// List of names for the images
        /// </summary>
        public IEnumerable<string> ImageNames => _displayNameMap.Keys;

        private readonly ZuneDesktopSettings _settings;
        private readonly ResourceLoader _localizationService;
        private string _selectedBackgroundImage = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="ZuneDesktopSettingsViewModel"/> class.
        /// </summary>
        public ZuneDesktopSettingsViewModel()
        {
            _settings = ZuneShell.Ioc.GetRequiredService<ZuneDesktopSettings>();
            _localizationService = ResourceLoader.GetForCurrentView("StrixMusic.Shells.ZuneDesktop/ZuneSettings");

            _displayNameMap = _zuneBackgroundImages.Keys
                .ToDictionary(_localizationService.GetString);

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

                _settings.BackgroundImage = image;
                _ = _settings.SaveAsync();
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
            await _settings.LoadAsync();

            string displayName = _localizationService.GetString(_settings.BackgroundImage.Name);
            SetProperty(ref _selectedBackgroundImage, displayName);
        }
    }
}
