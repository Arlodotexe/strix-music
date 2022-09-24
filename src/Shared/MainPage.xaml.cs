using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using OwlCore;
using StrixMusic.Sdk.Services;
using StrixMusic.Sdk.ViewModels;
using StrixMusic.Sdk.WinUI.Services.NotificationService;
using StrixMusic.Sdk.WinUI.Services.ShellManagement;
using StrixMusic.Services;
using StrixMusic.Services.CoreManagement;
using Windows.Media.Playback;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic
{
    /// <summary>
    /// Displays the main content of the app (the user's preferred shell).
    /// </summary>
    public sealed partial class MainPage : UserControl
    {
        private readonly AppSettings _settings;
        private readonly ICoreManagementService _coreManagementService;
        private readonly List<MediaPlayerElement> _mediaPlayerElements = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="MainPage"/> class.
        /// </summary>
        public MainPage(StrixDataRootViewModel dataRoot, AppSettings settings, ICoreManagementService coreManagementService)
        {
            _settings = settings;
            _coreManagementService = coreManagementService;
            DataRoot = dataRoot;
            InitializeComponent();
            Loaded += MainPage_Loaded;
        }

        /// <summary>
        /// The currently loaded shell, if any.
        /// </summary>
        internal ShellMetadata? ActiveShell { get; private set; }

        /// <summary>
        /// The user's preferred shell.
        /// </summary>
        internal ShellMetadata? PreferredShell { get; private set; }

        /// <summary>
        /// The user's preferred shell.
        /// </summary>
        internal ShellMetadata? FallbackShell { get; private set; }

        /// <summary>
        /// The data root the page was initialized with.
        /// </summary>
        public StrixDataRootViewModel DataRoot { get; }

        private async void MainPage_Loaded(object? sender, RoutedEventArgs e)
        {
            Loaded -= MainPage_Loaded;
            Unloaded += MainPage_Unloaded;

            LoadRegisteredMediaPlayerElements();
            SetupShellsFromSettings();
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
        public void Button_Click(object? sender, RoutedEventArgs e) => Window.Current.GetAppFrame().DisplaySuperShell(_settings, DataRoot.Sources, _coreManagementService);

        private void AttachEvents()
        {
            Ioc.Default.GetRequiredService<AppSettings>().PropertyChanged += OnSettingChanged;

#warning Remove me when live core editing is stable.
            var coreManagementService = Ioc.Default.GetRequiredService<ICoreManagementService>();

            coreManagementService.CoreInstanceRegistered += ShowEditCoresWarning;
            coreManagementService.CoreInstanceUnregistered += ShowEditCoresWarning;
        }

        private void DetachEvents()
        {
            Unloaded -= MainPage_Unloaded;

            Ioc.Default.GetRequiredService<AppSettings>().PropertyChanged -= OnSettingChanged;

            var coreManagementService = Ioc.Default.GetRequiredService<ICoreManagementService>();
            coreManagementService.CoreInstanceRegistered -= ShowEditCoresWarning;
            coreManagementService.CoreInstanceUnregistered -= ShowEditCoresWarning;
        }

        private void ShowEditCoresWarning(object? sender, CoreInstanceEventArgs args)
        {
            Ioc.Default.GetRequiredService<INotificationService>().RaiseNotification("Restart recommended",
                "Editing cores while the app is running is not stable yet");
        }

        private async void OnSettingChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(AppSettings.FallbackShell) || e.PropertyName == nameof(AppSettings.PreferredShell))
            {
                SetupShellsFromSettings();

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

        private void SetupShellsFromSettings()
        {
            // Gets the preferred shell from settings.
            var preferredShellId = _settings.PreferredShell;

            PreferredShell = ShellRegistry.MetadataRegistry.FirstOrDefault(x => x.Id == preferredShellId);
            if (PreferredShell == default)
            {
                AppContext.NavigationService.NavigateTo(typeof(SuperShell), true);
                return;
            }

            // Gets the preferred shell from settings.
            var fallbackShellId = _settings.FallbackShell;
            FallbackShell = ShellRegistry.MetadataRegistry.FirstOrDefault(x => x.Id == fallbackShellId);
        }

        private Task SetupShell(ShellMetadata shellMetadata)
        {
            using (Threading.PrimaryContext)
            {
                if (ActiveShell is not null && ActiveShell.Id == shellMetadata.Id)
                    return Task.CompletedTask;

                var notificationService = Ioc.Default.GetRequiredService<INotificationService>().Cast<NotificationService>();

                notificationService.ChangeNotificationMargins(new Thickness(25, 35, 25, 35));
                notificationService.ChangeNotificationAlignment(HorizontalAlignment.Right, VerticalAlignment.Top);

                // Removes the current shell.
                ShellDisplay.Content = null;

                var shell = ShellRegistry.CreateShell(shellMetadata, DataRoot);

                shell.DataRoot = DataRoot;
                shell.Notifications = Window.Current.GetAppFrame().Notifications;
                shell.Notifications.IsHandled = false;
                shell.InitServices(new ServiceCollection());
                ShellDisplay.Content = shell;
                ActiveShell = shellMetadata;

                return Task.CompletedTask;
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
            if (ActiveShell is null || PreferredShell is null || FallbackShell is null)
            {
                // Ignore this during initialization
                return;
            }

            if (ActiveShell != PreferredShell)
            {
                if (CheckShellModelSupport(PreferredShell))
                {
                    if (FallbackShell != PreferredShell)
                        await SetupShell(PreferredShell);
                }
            }
            else if (!CheckShellModelSupport(ActiveShell))
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
