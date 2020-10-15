using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using StrixMusic.Sdk;

namespace StrixMusic.Shells.ZuneDesktop.Settings
{
    /// <summary>
    /// The settings viewmodel for the Zune Desktop
    /// </summary>
    public class ZuneDesktopSettingsViewModel : SettingsViewModelBase
    {
        private readonly Dictionary<string, ZuneDesktopBackgroundImage> _zuneBackgroundImages = new Dictionary<string, ZuneDesktopBackgroundImage>()
        {
            { "None", new ZuneDesktopBackgroundImage() },
            { "Bubbles", new ZuneDesktopBackgroundImage("Bubbles") },
            { "Cells", new ZuneDesktopBackgroundImage("Cells") },
            { "Meadow", new ZuneDesktopBackgroundImage("Meadow") },
            { "RobotOwl", new ZuneDesktopBackgroundImage("RobotOwl", Windows.UI.Xaml.Media.AlignmentY.Center) },
            { "Shards", new ZuneDesktopBackgroundImage("Shards", Windows.UI.Xaml.Media.AlignmentY.Top) },
            { "Wired", new ZuneDesktopBackgroundImage("Wired") },
        };

        /// <summary>
        /// List of names for the images
        /// </summary>
        public IEnumerable<string> ImageNames => _zuneBackgroundImages.Keys;

        private readonly ZuneDesktopSettingsService _zuneDesktopSettingsService;
        private string _selectedBackgroundImage = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="ZuneDesktopSettingsViewModel"/> class.
        /// </summary>
        public ZuneDesktopSettingsViewModel()
        {
            _zuneDesktopSettingsService = ZuneDesktopShellIoc.Ioc.GetService<ZuneDesktopSettingsService>();
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
                    ZuneDesktopBackgroundImage image = _zuneBackgroundImages[value];
                    _zuneDesktopSettingsService.SetValue<ZuneDesktopBackgroundImage>(nameof(ZuneDesktopSettingsKeys.BackgroundImage), image);
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
            SetProperty(ref _selectedBackgroundImage, backgroundImage.Name);
        }
    }
}
