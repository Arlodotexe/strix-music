﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Diagnostics;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using OwlCore;
using OwlCore.AbstractStorage;
using OwlCore.Extensions;
using StrixMusic.Sdk;
using StrixMusic.Sdk.Services.Navigation;
using StrixMusic.Sdk.Services.Notifications;
using StrixMusic.Sdk.Services.Settings;
using StrixMusic.Sdk.Uno.Controls.Shells;
using StrixMusic.Sdk.Uno.Models;
using StrixMusic.Sdk.Uno.Services;
using StrixMusic.Sdk.Uno.Services.Localization;
using StrixMusic.Sdk.Uno.Services.NotificationService;
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
        private IReadOnlyList<ShellAssemblyInfo>? _shellRegistry;

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
        internal ShellAssemblyInfo? ActiveShellModel { get; private set; }

        /// <summary>
        /// The user's preferred shell.
        /// </summary>
        internal ShellAssemblyInfo? PreferredShell { get; private set; }

        /// <summary>
        /// The user's preferred shell.
        /// </summary>
        internal ShellAssemblyInfo? FallbackShell { get; private set; }

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

            var mainViewModel = Ioc.Default.GetRequiredService<MainViewModel>();
            var notificationService = Ioc.Default.GetRequiredService<INotificationService>();
            var localizationService = new LocalizationResourceLoader();
            localizationService.RegisterProvider(Helpers.Constants.Localization.CommonResource);
            localizationService.RegisterProvider(Helpers.Constants.Localization.MusicResource);

            services.AddSingleton<INavigationService<Control>>(new NavigationService<Control>());
            services.AddSingleton(localizationService);
            services.AddSingleton(notificationService);
            services.AddSingleton(mainViewModel);

            shell.InitServices(services);
        }

        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= MainPage_Loaded;
            Unloaded += MainPage_Unloaded;

            var settingsService = Ioc.Default.GetRequiredService<ISettingsService>();
            _navigationService = CurrentWindow.NavigationService;
            _shellRegistry = await settingsService.GetValue<IReadOnlyList<ShellAssemblyInfo>>(nameof(SettingsKeysUI.ShellRegistry));

            LoadRegisteredMediaPlayerElements();
            await SetupShellsFromSettings();
            Guard.IsNotNull(PreferredShell, nameof(PreferredShell));

            await SetupShell(PreferredShell);

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
            Guard.IsNotNull(_shellRegistry, nameof(_shellRegistry));
            Guard.IsNotNull(_navigationService, nameof(_navigationService));

            // Gets the preferred shell from settings.
            var preferredShellAssemblyName = await Ioc.Default.GetRequiredService<ISettingsService>().GetValue<string>(nameof(SettingsKeysUI.PreferredShell));

            PreferredShell = _shellRegistry.FirstOrDefault(x => x.AssemblyName == preferredShellAssemblyName);

            if (PreferredShell == default)
            {
                _navigationService.NavigateTo(typeof(SuperShell), true);
                return;
            }

            // Gets the preferred shell from settings.
            var fallbackShellAssemblyName = await Ioc.Default.GetRequiredService<ISettingsService>().GetValue<string>(nameof(SettingsKeysUI.FallbackShell));

            FallbackShell = _shellRegistry.FirstOrDefault(x => x.AssemblyName == fallbackShellAssemblyName);
        }

        private Task SetupShell(ShellAssemblyInfo shellAssemblyInfo)
        {
            using (Threading.PrimaryContext)
            {
                var notificationService = Ioc.Default.GetRequiredService<INotificationService>().Cast<NotificationService>();

                notificationService.ChangeNotificationMargins(new Thickness(25, 35, 25, 35));
                notificationService.ChangeNotificationAlignment(HorizontalAlignment.Right, VerticalAlignment.Top);

                // Removes the current shell.
                ShellDisplay.Content = null;

                ActiveShellModel = shellAssemblyInfo;

                var shellDataType = Type.GetType(shellAssemblyInfo.AttributeData.ShellTypeAssemblyQualifiedName);
                Guard.IsNotNull(shellDataType, nameof(shellDataType));

                var shell = CreateShellControl(shellDataType);

                var mainViewModel = Ioc.Default.GetRequiredService<MainViewModel>();

                mainViewModel.Notifications.IsHandled = false;
                InjectServices(shell);

                shell.DataContext = MainViewModel.Singleton;

                ShellDisplay.Content = shell;
            }

            return Task.CompletedTask;
        }

        private bool CheckShellModelSupport(ShellAssemblyInfo shell)
        {
            bool heightIsInRange = ActualHeight < shell.AttributeData.MaxWindowSize.Height &&
                                   ActualHeight > shell.AttributeData.MinWindowSize.Height;

            bool widthIsInRange = ActualWidth > shell.AttributeData.MinWindowSize.Width &&
                                  ActualWidth > shell.AttributeData.MinWindowSize.Width;

            return widthIsInRange && heightIsInRange;
        }

        private async void MainPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_shellRegistry is null)
                return;

            if (ActiveShellModel is null || PreferredShell is null || FallbackShell is null)
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
