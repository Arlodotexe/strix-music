using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using OwlCore.AbstractStorage;
using OwlCore.Helpers;
using StrixMusic.Sdk;
using StrixMusic.Sdk.Services.Settings;
using StrixMusic.Sdk.Uno.Controls;
using StrixMusic.Sdk.Uno.Models;
using StrixMusic.Sdk.Uno.Services;
using Windows.Media.Playback;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.Toolkit.Diagnostics;

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
        internal ShellModel? ActiveShell { get; private set; }

        /// <summary>
        /// The user's preferred shell.
        /// </summary>
        internal ShellModel? PreferredShell { get; private set; }

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

        private Task SetupShell(ShellModel shell)
        {
            Guard.IsNotNull(_shellService, nameof(_shellService));

            using (Threading.UIThread)
            {
                // Removes the current shell.
                ShellDisplay.Content = null;

                // Removes old resource(s).
                foreach (var dict in Application.Current.Resources.MergedDictionaries)
                {
                    Match shellMatch = Regex.Match(dict.Source.AbsoluteUri, _shellService.ShellResourceDictionaryRegex);

                    if (shellMatch.Success)
                    {
                        // Skips removing the default ResourceDictionary.
                        if (shellMatch.Groups[1].Value == _shellService.DefaultShellAssemblyName)
                            continue;

                        Application.Current.Resources.MergedDictionaries.Remove(dict);
                        break;
                    }
                }

                if (shell.AssemblyName != _shellService.DefaultShellAssemblyName)
                {
                    // Loads the preferred shell
                    var resourcePath = $"{_shellService.ResourcesPrefix}{_shellService.ShellNamespacePrefix}.{shell.AssemblyName}/{_shellService.ShellResourcesSuffix}";
                    var resourceDictionary = new ResourceDictionary() { Source = new Uri(resourcePath) };
                    Application.Current.Resources.MergedDictionaries.Add(resourceDictionary);
                }

                ActiveShell = shell;
                ShellDisplay.Content = CreateShellControl(shell.ShellAttribute.ShellBaseSubType);
            }

            return Task.CompletedTask;
        }

        private Control CreateShellControl(Type shellType)
        {
            if (shellType.BaseType != typeof(ShellBase))
                throw new ArgumentException($@"Expected type {nameof(ShellBase)}", nameof(shellType));

            Control shellControl = (Control)Activator.CreateInstance(shellType);
            shellControl.DataContext = MainViewModel.Singleton;

            return shellControl;
        }

        private bool CheckShellModelSupport(ShellModel shell)
        {
            return

                // Check height is within range
                ActualHeight < shell.ShellAttribute.MaxWindowSize.Height &&
                ActualHeight > shell.ShellAttribute.MinWindowSize.Height &&

                // Check width is within range
                ActualWidth > shell.ShellAttribute.MinWindowSize.Width &&
                ActualWidth > shell.ShellAttribute.MinWindowSize.Width;
        }

        private async void MainPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_shellService is null)
                return;

            if (ActiveShell == null || PreferredShell == null)
            {
                // Ignore this during initialization
                return;
            }

            if (ActiveShell != PreferredShell)
            {
                if (CheckShellModelSupport(PreferredShell))
                {
                    await SetupShell(PreferredShell);
                }
            }
            else if (!CheckShellModelSupport(ActiveShell))
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
