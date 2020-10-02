using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using OwlCore.Extensions.AsyncExtensions;
using StrixMusic.Sdk;

namespace StrixMusic.Shell.ZuneDesktop.Settings
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

        /// <summary>
        /// Initializes a new instance of the <see cref="ZuneDesktopSettingsViewModel"/> class.
        /// </summary>
        public ZuneDesktopSettingsViewModel()
        {
            _zuneDesktopSettingsService = ZuneDesktopShellIoc.Ioc.GetService<ZuneDesktopSettingsService>();
        }

        /// <summary>
        /// Gets or sets if the ZuneShell BackgroundImage is None.
        /// </summary>
        public string SelectedBackgroundImage
        {
            get
            {
                // TODO: Initialize this value from code behind and keep track in a backing field.
                var backgroundImage = _zuneDesktopSettingsService.GetValue<ZuneDesktopBackgroundImage>(nameof(ZuneDesktopSettingsKeys.BackgroundImage)).Result;
                return backgroundImage.Name;
            }

            set
            {
                ZuneDesktopBackgroundImage image = _zuneBackgroundImages[value];
                _zuneDesktopSettingsService.SetValue<ZuneDesktopBackgroundImage>(nameof(ZuneDesktopSettingsKeys.BackgroundImage), image);
            }
        }
    }
}
