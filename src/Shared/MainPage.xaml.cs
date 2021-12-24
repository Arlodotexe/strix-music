using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Diagnostics;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using OwlCore;
using OwlCore.AbstractStorage;
using OwlCore.Extensions;
using StrixMusic.Sdk;
using StrixMusic.Sdk.Services;
using StrixMusic.Sdk.Services.Navigation;
using StrixMusic.Sdk.Services.Notifications;
using StrixMusic.Sdk.Services.Settings;
using StrixMusic.Sdk.Uno.Services;
using StrixMusic.Sdk.Uno.Services.Localization;
using StrixMusic.Sdk.Uno.Services.NotificationService;
using StrixMusic.Sdk.Uno.Services.ShellManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Media;
using Windows.Media.Playback;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shared
{
    /// <summary>
    /// Displays the main content of the app (the user's preferred shell).
    /// </summary>
    public sealed partial class MainPage : UserControl
    {
        private readonly List<MediaPlayerElement> _mediaPlayerElements = new List<MediaPlayerElement>();
        private INavigationService<Control>? _navigationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainPage"/> class.
        /// </summary>
        public MainPage()
        {
            InitializeComponent();
            Loaded += MainPage_Loaded;
        }

        /// <summary>
        /// The currently loaded shell, if any.
        /// </summary>
        internal ShellMetadata? ActiveShellModel { get; private set; }

        /// <summary>
        /// The user's preferred shell.
        /// </summary>
        internal ShellMetadata? PreferredShell { get; private set; }

        /// <summary>
        /// The user's preferred shell.
        /// </summary>
        internal ShellMetadata? FallbackShell { get; private set; }

        private static void InjectServices(Sdk.Uno.Controls.Shells.Shell shell)
        {
            var services = new ServiceCollection();

            var mainViewModel = Ioc.Default.GetRequiredService<MainViewModel>();
            var notificationService = Ioc.Default.GetRequiredService<INotificationService>();
            var localizationService = new LocalizationResourceLoader();
            localizationService.RegisterProvider(Sdk.Helpers.Constants.Localization.CommonResource);
            localizationService.RegisterProvider(Sdk.Helpers.Constants.Localization.MusicResource);

            services.AddSingleton<INavigationService<Control>>(new NavigationService<Control>());
            services.AddSingleton(localizationService);
            services.AddSingleton(notificationService);
            services.AddSingleton(mainViewModel);

            shell.InitServices(services);
        }

        private async void MainPage_Loaded(object? sender, RoutedEventArgs e)
        {
            Loaded -= MainPage_Loaded;
            Unloaded += MainPage_Unloaded;

            var settingsService = Ioc.Default.GetRequiredService<ISettingsService>();
            _navigationService = CurrentWindow.NavigationService;

            LoadRegisteredMediaPlayerElements();
            await SetupShellsFromSettings();
            Guard.IsNotNull(PreferredShell, nameof(PreferredShell));

            await SetupShell(PreferredShell);

            AttachEvents();

            // Events must be attached before initializing if you want them to fire correctly.
            await Ioc.Default.GetRequiredService<IFileSystemService>().InitAsync();
        }

        private void MainPage_Unloaded(object? sender, RoutedEventArgs e)
        {
            DetachEvents();
        }

        /// <summary>
        /// Fires when the Super buttons is clicked. Temporary until a proper trigger mechanism is found for touch devices.
        /// </summary>
        public void Button_Click(object? sender, RoutedEventArgs e)
        {
            CurrentWindow.NavigationService.NavigateTo(typeof(SuperShell), true);
        }

        private void AttachEvents()
        {
            Ioc.Default.GetRequiredService<ISettingsService>().SettingChanged += SettingsService_SettingChanged;

#warning Remove me when live core editing is stable.
            Ioc.Default.GetRequiredService<ICoreManagementService>().CoreInstanceRegistered += ShowEditCoresWarning;
            Ioc.Default.GetRequiredService<ICoreManagementService>().CoreInstanceUnregistered += ShowEditCoresWarning;

            void ShowEditCoresWarning(object? sender, CoreInstanceEventArgs args)
            {
                Ioc.Default.GetRequiredService<INotificationService>().RaiseNotification("Restart recommended",
                    "Editing cores while the app is running is not stable yet");
            };
        }

        private void DetachEvents()
        {
            Unloaded -= MainPage_Unloaded;

            Ioc.Default.GetRequiredService<ISettingsService>().SettingChanged -= SettingsService_SettingChanged;
        }

        private async void SettingsService_SettingChanged(object? sender, SettingChangedEventArgs e)
        {
            if (e.Key == nameof(SettingsKeysUI.FallbackShell) || e.Key == nameof(SettingsKeysUI.PreferredShell))
            {
                await SetupShellsFromSettings();

                if (PreferredShell is null)
                    return;

                if (FallbackShell is null)
                    return;

                if (CheckShellModelSupport(PreferredShell))
                    await SetupShell(PreferredShell);
                else
                    await SetupShell(FallbackShell);
            }
        }

        private void LoadRegisteredMediaPlayerElements()
        {
            foreach (var item in _mediaPlayerElements.Where(item => !PART_MediaPlayerElements.Children.Contains(item)))
            {
                PART_MediaPlayerElements.Children.Add(item);
            }
        }

        private async Task SetupShellsFromSettings()
        {
            Guard.IsNotNull(_navigationService, nameof(_navigationService));

            // Gets the preferred shell from settings.
            var preferredShellId = await Ioc.Default.GetRequiredService<ISettingsService>().GetValue<string>(nameof(SettingsKeysUI.PreferredShell));

            PreferredShell = ShellRegistry.MetadataRegistry.FirstOrDefault(x => x.Id == preferredShellId);

            if (PreferredShell == default)
            {
                _navigationService.NavigateTo(typeof(SuperShell), true);
                return;
            }

            // Gets the preferred shell from settings.
            var fallbackShellId = await Ioc.Default.GetRequiredService<ISettingsService>().GetValue<string>(nameof(SettingsKeysUI.FallbackShell));

            FallbackShell = ShellRegistry.MetadataRegistry.FirstOrDefault(x => x.Id == fallbackShellId);
        }

        private Task SetupShell(ShellMetadata shellMetadata)
        {
            using (Threading.PrimaryContext)
            {
                var notificationService = Ioc.Default.GetRequiredService<INotificationService>().Cast<NotificationService>();

                notificationService.ChangeNotificationMargins(new Thickness(25, 35, 25, 35));
                notificationService.ChangeNotificationAlignment(HorizontalAlignment.Right, VerticalAlignment.Top);

                // Removes the current shell.
                ShellDisplay.Content = null;

                ActiveShellModel = shellMetadata;

                var shell = ShellRegistry.CreateShell(shellMetadata);

                var mainViewModel = Ioc.Default.GetRequiredService<MainViewModel>();

                mainViewModel.Notifications.IsHandled = false;
                InjectServices(shell);

                shell.DataContext = MainViewModel.Singleton;

                ShellDisplay.Content = shell;
            }

            return Task.CompletedTask;
        }

        private bool CheckShellModelSupport(ShellMetadata shell)
        {
            bool heightIsInRange = ActualHeight < shell.MaxWindowSize.Height &&
                                   ActualHeight > shell.MinWindowSize.Height;

            bool widthIsInRange = ActualWidth > shell.MinWindowSize.Width &&
                                  ActualWidth > shell.MinWindowSize.Width;

            return widthIsInRange && heightIsInRange;
        }

        private async void MainPage_SizeChanged(object? sender, SizeChangedEventArgs e)
        {
            if (ActiveShellModel is null || PreferredShell is null || FallbackShell is null)
            {
                // Ignore this during initialization
                return;
            }

            if (ActiveShellModel != PreferredShell)
            {
                if (CheckShellModelSupport(PreferredShell))
                    await SetupShell(PreferredShell);
            }
            else if (!CheckShellModelSupport(ActiveShellModel))
                await SetupShell(FallbackShell);
        }

        /// <summary>
        /// Creates a <see cref="MediaPlayerElement"/> and inserts it into the UI.
        /// </summary>
        /// <returns>The created <see cref="MediaPlayerElement"/></returns>
        public MediaPlayerElement CreateMediaPlayerElement()
        {
            var mediaSource = new MediaPlayer();
            var mediaPlayerElement = new MediaPlayerElement();
            mediaPlayerElement.SetMediaPlayer(mediaSource);

#if !__WASM__
            mediaSource.CommandManager.IsEnabled = false;
#endif

            /* If we're disabling CommandManager and not using automatic integration of background audio with SMTC, 
             * then we need to manually integrate background audio with SMTC by at-least enabling play/pause and 
             * handling ButtonPressed event.Its the minimum requirement for background audio to work.
             * References:
             ** https://docs.microsoft.com/en-us/windows/uwp/audio-video-camera/system-media-transport-controls#use-the-system-media-transport-controls-for-background-audio 
             ** https://docs.microsoft.com/en-us/windows/uwp/audio-video-camera/background-audio#requirements-for-background-audio
            */
            var mediaTransportControls = mediaSource.SystemMediaTransportControls;
            mediaTransportControls.IsPlayEnabled = true;
            mediaTransportControls.IsPauseEnabled = true;
            mediaTransportControls.ButtonPressed += SystemControls_ButtonPressed;

            async void SystemControls_ButtonPressed(SystemMediaTransportControls sender, SystemMediaTransportControlsButtonPressedEventArgs args)
            {
                switch (args.Button)
                {
                    case SystemMediaTransportControlsButton.Play:
                        await Threading.OnPrimaryThread(() =>
                         {
                             mediaSource.Play();
                         });
                        break;
                    case SystemMediaTransportControlsButton.Pause:
                        await Threading.OnPrimaryThread(() =>
                        {
                            mediaSource.Pause();
                        });
                        break;
                    default:
                        break;
                }
            }

            _mediaPlayerElements.Add(mediaPlayerElement);

            // If loaded, add it to the visual tree.
            // If not loaded, we'll handle it in MainPage's Loaded event handler.
            if (IsLoaded)
            {
                PART_MediaPlayerElements.Children.Add(mediaPlayerElement);
            }

            return mediaPlayerElement;
        }
    }
}
