using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Diagnostics;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using OwlCore.AbstractStorage;
using OwlCore.Helpers;
using StrixMusic.Sdk;
using StrixMusic.Sdk.Services.Navigation;
using StrixMusic.Sdk.Services.Settings;
using StrixMusic.Sdk.Uno.Controls;
using StrixMusic.Sdk.Uno.Models;
using StrixMusic.Sdk.Uno.Services;
using StrixMusic.Sdk.Uno.Services.Localization;
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
        private IShellService? _shellService;

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
        internal ShellModel? ActiveShellModel { get; private set; }

        /// <summary>
        /// The user's preferred shell.
        /// </summary>
        internal ShellModel? PreferredShell { get; private set; }

        private static Shell CreateShellControl(Type shellType)
        {
            if (shellType.BaseType != typeof(Shell))
                throw new ArgumentException($@"Expected type {nameof(Shell)}", nameof(shellType));

            var shellControl = (Shell)Activator.CreateInstance(shellType);

            return shellControl;
        }

        private static void InjectServices(Shell shell)
        {
            var services = new ServiceCollection();

            services.AddSingleton(new NavigationService<Control>());
            services.AddSingleton(new LocalizationResourceLoader());

            shell.InitServices(services);
        }

        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= MainPage_Loaded;
            Unloaded += MainPage_Unloaded;

            // Services might not be configured when MainPage is created, but they are once MainPage is added to the visual tree.
            _shellService = Ioc.Default.GetRequiredService<IShellService>();

            LoadRegisteredMediaPlayerElements();
            await SetupInitialShell();

            AttachEvents();

            // Events must be attached before initializing if you want them to fire correctly.
            await Ioc.Default.GetRequiredService<IFileSystemService>().InitAsync();
        }

        private void MainPage_Unloaded(object sender, RoutedEventArgs e)
        {
            DetachEvents();
        }

        /// <summary>
        /// Fires when the Super buttons is clicked. Temporary until a proper trigger mechanism is found for touch devices.
        /// </summary>
        public void Button_Click(object sender, RoutedEventArgs e)
        {
            CurrentWindow.NavigationService.NavigateTo(typeof(SuperShell), true);
        }

        private void AttachEvents()
        {
            Ioc.Default.GetRequiredService<ISettingsService>().SettingChanged += SettingsService_SettingChanged;
        }

        private void DetachEvents()
        {
            Unloaded -= MainPage_Unloaded;

            Ioc.Default.GetRequiredService<ISettingsService>().SettingChanged -= SettingsService_SettingChanged;
        }

        private async void SettingsService_SettingChanged(object sender, SettingChangedEventArgs e)
        {
            if (e.Key == nameof(SettingsKeys.PreferredShell))
            {
                await SetupInitialShell();
            }
        }

        private void LoadRegisteredMediaPlayerElements()
        {
            foreach (var item in _mediaPlayerElements.Where(item => !PART_MediaPlayerElements.Children.Contains(item)))
            {
                PART_MediaPlayerElements.Children.Add(item);
            }
        }

        private async Task SetupInitialShell()
        {
            Guard.IsNotNull(_shellService, nameof(_shellService));

            // Gets the preferred shell from settings.
            var preferredShell = await Ioc.Default.GetRequiredService<ISettingsService>().GetValue<string>(nameof(SettingsKeys.PreferredShell));
            var shellModel = _shellService.DefaultShellModel;

            if (_shellService.LoadedShells.ContainsKey(preferredShell))
            {
                shellModel = _shellService.LoadedShells[preferredShell];
                PreferredShell = shellModel;
            }

            await SetupShell(shellModel);
        }

        private Task SetupShell(ShellModel shellModel)
        {
            Guard.IsNotNull(_shellService, nameof(_shellService));

            using (Threading.UIThread)
            {
                // Removes the current shell.
                ShellDisplay.Content = null;

                ActiveShellModel = shellModel;

                var shell = CreateShellControl(shellModel.ShellAttribute.ShellSubType);
                InjectServices(shell);

                shell.DataContext = MainViewModel.Singleton;

                ShellDisplay.Content = shell;
            }

            return Task.CompletedTask;
        }

        private bool CheckShellModelSupport(ShellModel shell)
        {
            bool heightIsInRange = ActualHeight < shell.ShellAttribute.MaxWindowSize.Height &&
                                   ActualHeight > shell.ShellAttribute.MinWindowSize.Height;

            bool widthIsInRange = ActualWidth > shell.ShellAttribute.MinWindowSize.Width &&
                                  ActualWidth > shell.ShellAttribute.MinWindowSize.Width;

            return widthIsInRange && heightIsInRange;
        }

        private async void MainPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_shellService is null)
                return;

            if (ActiveShellModel == null || PreferredShell == null)
            {
                // Ignore this during initialization
                return;
            }

            if (ActiveShellModel != PreferredShell)
            {
                if (CheckShellModelSupport(PreferredShell))
                {
                    await SetupShell(PreferredShell);
                }
            }
            else if (!CheckShellModelSupport(ActiveShellModel))
            {
                await SetupShell(_shellService.DefaultShellModel);
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
