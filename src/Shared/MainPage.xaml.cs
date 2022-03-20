using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Diagnostics;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using OwlCore;
using OwlCore.AbstractStorage;
using OwlCore.Extensions;
using StrixMusic.Sdk;
using StrixMusic.Sdk.CoreManagement;
using StrixMusic.Sdk.Services;
using StrixMusic.Sdk.Services.Navigation;
using StrixMusic.Sdk.Uno.Services.NotificationService;
using StrixMusic.Sdk.Uno.Services.ShellManagement;
using StrixMusic.Services;
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

        private async void MainPage_Loaded(object? sender, RoutedEventArgs e)
        {
            Loaded -= MainPage_Loaded;
            Unloaded += MainPage_Unloaded;

            var settingsService = Ioc.Default.GetRequiredService<AppSettings>();
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
            Ioc.Default.GetRequiredService<AppSettings>().PropertyChanged += OnSettingChanged;

#warning Remove me when live core editing is stable.
            Ioc.Default.GetRequiredService<ICoreManagementService>().CoreInstanceRegistered += ShowEditCoresWarning;
            Ioc.Default.GetRequiredService<ICoreManagementService>().CoreInstanceUnregistered += ShowEditCoresWarning;

            void ShowEditCoresWarning(object? sender, CoreInstanceEventArgs args)
            {
                Ioc.Default.GetRequiredService<INotificationService>().RaiseNotification("Restart recommended",
                    "Editing cores while the app is running is not stable yet");
            }
        }

        private void DetachEvents()
        {
            Unloaded -= MainPage_Unloaded;

            Ioc.Default.GetRequiredService<AppSettings>().PropertyChanged -= OnSettingChanged;
        }

        private async void OnSettingChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(AppSettings.FallbackShell) || e.PropertyName == nameof(AppSettings.PreferredShell))
            {
                await SetupShellsFromSettings();

                if (PreferredShell is null)
                    return;

                if (FallbackShell is null)
                    return;

                // Store the shell that is actually going to be created according to the conditions.
                ShellMetadata? shellToCreate;

                if (CheckShellModelSupport(PreferredShell))
                    shellToCreate = PreferredShell;
                else
                    shellToCreate = FallbackShell;

                // Don't setup the fallback shell if its same as the preferred shell.
                if (e.PropertyName == nameof(AppSettings.FallbackShell) && shellToCreate != PreferredShell)
                {
                    await SetupShell(shellToCreate);
                }

                if (e.PropertyName == nameof(AppSettings.PreferredShell))
                {
                    await SetupShell(shellToCreate);
                }
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
            var preferredShellId = Ioc.Default.GetRequiredService<AppSettings>().PreferredShell;

            PreferredShell = ShellRegistry.MetadataRegistry.FirstOrDefault(x => x.Id == preferredShellId);

            if (PreferredShell == default)
            {
                _navigationService.NavigateTo(typeof(SuperShell), true);
                return;
            }

            // Gets the preferred shell from settings.
            var fallbackShellId = Ioc.Default.GetRequiredService<AppSettings>().FallbackShell;

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

                var mainViewModel = Ioc.Default.GetRequiredService<MainViewModel>();

                mainViewModel.Notifications.IsHandled = false;

                var shell = ShellRegistry.CreateShell(shellMetadata);

                shell.DataRoot = mainViewModel;
                shell.InitServices(new ServiceCollection());
                ShellDisplay.Content = shell;

                ActiveShellModel = shellMetadata;
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
                {
                    if (FallbackShell != PreferredShell)
                        await SetupShell(PreferredShell);
                }
            }
            else if (!CheckShellModelSupport(ActiveShellModel))
            {
                if (FallbackShell != PreferredShell)
                    await SetupShell(FallbackShell);
            }
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
