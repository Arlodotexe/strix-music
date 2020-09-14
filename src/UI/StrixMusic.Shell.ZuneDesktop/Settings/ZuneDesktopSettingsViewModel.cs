using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using OwlCore.Extensions.AsyncExtensions;
using StrixMusic.Sdk;
using StrixMusic.Shell.Strix;

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
            { "Bloom", new ZuneDesktopBackgroundImage("Bloom", Windows.UI.Xaml.Media.AlignmentY.Top) },
            { "Bubbles", new ZuneDesktopBackgroundImage("Bubbles") },
            { "Cells", new ZuneDesktopBackgroundImage("Cells") },
            { "Meadow", new ZuneDesktopBackgroundImage("Meadow") },
            { "RobotOwl", new ZuneDesktopBackgroundImage("RobotOwl", Windows.UI.Xaml.Media.AlignmentY.Center) },
            { "Shards", new ZuneDesktopBackgroundImage("Shards", Windows.UI.Xaml.Media.AlignmentY.Top) },
            { "Wired", new ZuneDesktopBackgroundImage("Wired") },
        };

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
        public bool IsNoneBackgroundSelected
        {
            get => AsyncExtensions.RunSync(async () =>
            {
                var backgroundImage = await _zuneDesktopSettingsService.GetValue<ZuneDesktopBackgroundImage>(nameof(ZuneDesktopSettingsKeys.BackgroundImage));
                return backgroundImage.IsNone;
            });
            set
            {
                if (value == false)
                {
                    return;
                }

                ZuneDesktopBackgroundImage image = _zuneBackgroundImages["None"];
                _zuneDesktopSettingsService.SetValue<ZuneDesktopBackgroundImage>(nameof(ZuneDesktopSettingsKeys.BackgroundImage), image);
            }
        }

        /// <summary>
        /// Gets or sets if the ZuneShell BackgroundImage is Bloom.
        /// </summary>
        public bool IsBloomBackgroundSelected
        {
            get => AsyncExtensions.RunSync(async () =>
            {
                var backgroundImage = await _zuneDesktopSettingsService.GetValue<ZuneDesktopBackgroundImage>(nameof(ZuneDesktopSettingsKeys.BackgroundImage));
                return backgroundImage.Name == "Bloom";
            });
            set
            {
                if (value == false)
                {
                    return;
                }

                ZuneDesktopBackgroundImage image = _zuneBackgroundImages["Bloom"];
                _zuneDesktopSettingsService.SetValue<ZuneDesktopBackgroundImage>(nameof(ZuneDesktopSettingsKeys.BackgroundImage), image);
            }
        }

        /// <summary>
        /// Gets or sets if the ZuneShell BackgroundImage is Bubbles.
        /// </summary>
        public bool IsBubblesBackgroundSelected
        {
            get => AsyncExtensions.RunSync(async () =>
            {
                var backgroundImage = await _zuneDesktopSettingsService.GetValue<ZuneDesktopBackgroundImage>(nameof(ZuneDesktopSettingsKeys.BackgroundImage));
                return backgroundImage.Name == "Bubbles";
            });
            set
            {
                if (value == false)
                {
                    return;
                }

                ZuneDesktopBackgroundImage image = _zuneBackgroundImages["Bubbles"];
                _zuneDesktopSettingsService.SetValue<ZuneDesktopBackgroundImage>(nameof(ZuneDesktopSettingsKeys.BackgroundImage), image);
            }
        }

        /// <summary>
        /// Gets or sets if the ZuneShell BackgroundImage is Cells.
        /// </summary>
        public bool IsCellsBackgroundSelected
        {
            get => AsyncExtensions.RunSync(async () =>
            {
                var backgroundImage = await _zuneDesktopSettingsService.GetValue<ZuneDesktopBackgroundImage>(nameof(ZuneDesktopSettingsKeys.BackgroundImage));
                return backgroundImage.Name == "Cells";
            });
            set
            {
                if (value == false)
                {
                    return;
                }

                ZuneDesktopBackgroundImage image = _zuneBackgroundImages["Cells"];
                _zuneDesktopSettingsService.SetValue<ZuneDesktopBackgroundImage>(nameof(ZuneDesktopSettingsKeys.BackgroundImage), image);
            }
        }

        /// <summary>
        /// Gets or sets if the ZuneShell BackgroundImage is Meadow.
        /// </summary>
        public bool IsMeadowBackgroundSelected
        {
            get => AsyncExtensions.RunSync(async () =>
            {
                var backgroundImage = await _zuneDesktopSettingsService.GetValue<ZuneDesktopBackgroundImage>(nameof(ZuneDesktopSettingsKeys.BackgroundImage));
                return backgroundImage.Name == "Meadow";
            });
            set
            {
                if (value == false)
                {
                    return;
                }

                ZuneDesktopBackgroundImage image = _zuneBackgroundImages["Meadow"];
                _zuneDesktopSettingsService.SetValue<ZuneDesktopBackgroundImage>(nameof(ZuneDesktopSettingsKeys.BackgroundImage), image);
            }
        }

        /// <summary>
        /// Gets or sets if the ZuneShell BackgroundImage is RobotOwl.
        /// </summary>
        public bool IsRobotOwlBackgroundSelected
        {
            get => AsyncExtensions.RunSync(async () =>
            {
                var backgroundImage = await _zuneDesktopSettingsService.GetValue<ZuneDesktopBackgroundImage>(nameof(ZuneDesktopSettingsKeys.BackgroundImage));
                return backgroundImage.Name == "RobotOwl";
            });
            set
            {
                if (value == false)
                {
                    return;
                }

                ZuneDesktopBackgroundImage image = _zuneBackgroundImages["RobotOwl"];
                _zuneDesktopSettingsService.SetValue<ZuneDesktopBackgroundImage>(nameof(ZuneDesktopSettingsKeys.BackgroundImage), image);
            }
        }

        /// <summary>
        /// Gets or sets if the ZuneShell BackgroundImage is Shards.
        /// </summary>
        public bool IsShardsBackgroundSelected
        {
            get => AsyncExtensions.RunSync(async () =>
            {
                var backgroundImage = await _zuneDesktopSettingsService.GetValue<ZuneDesktopBackgroundImage>(nameof(ZuneDesktopSettingsKeys.BackgroundImage));
                return backgroundImage.Name == "Shards";
            });
            set
            {
                if (value == false)
                {
                    return;
                }

                ZuneDesktopBackgroundImage image = _zuneBackgroundImages["Shards"];
                _zuneDesktopSettingsService.SetValue<ZuneDesktopBackgroundImage>(nameof(ZuneDesktopSettingsKeys.BackgroundImage), image);
            }
        }

        /// <summary>
        /// Gets or sets if the ZuneShell BackgroundImage is Wired.
        /// </summary>
        public bool IsWiredBackgroundSelected
        {
            get => AsyncExtensions.RunSync(async () =>
            {
                var backgroundImage = await _zuneDesktopSettingsService.GetValue<ZuneDesktopBackgroundImage>(nameof(ZuneDesktopSettingsKeys.BackgroundImage));
                return backgroundImage.Name == "Wired";
            });
            set
            {
                if (value == false)
                {
                    return;
                }

                ZuneDesktopBackgroundImage image = _zuneBackgroundImages["Wired"];
                _zuneDesktopSettingsService.SetValue<ZuneDesktopBackgroundImage>(nameof(ZuneDesktopSettingsKeys.BackgroundImage), image);
            }
        }
    }
}
