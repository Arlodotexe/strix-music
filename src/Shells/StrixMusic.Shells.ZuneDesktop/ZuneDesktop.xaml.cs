using System;
using CommunityToolkit.Diagnostics;
using CommunityToolkit.Mvvm.Messaging;
using OwlCore.Storage;
using StrixMusic.Sdk.ViewModels;
using StrixMusic.Sdk.WinUI.Controls;
using StrixMusic.Shells.ZuneDesktop.Messages;
using StrixMusic.Shells.ZuneDesktop.Messages.Pages;
using StrixMusic.Shells.ZuneDesktop.Settings;
using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;

namespace StrixMusic.Shells.ZuneDesktop
{
    /// <summary>
    /// A faithful recreation of Zune Desktop 4.8.
    /// </summary>
    public sealed partial class ZuneDesktop : Shell
    {
        private ZuneDesktopSettings? _settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="ZuneDesktop"/> class.
        /// </summary>
        public ZuneDesktop()
        {
            Loaded += ZuneShell_Loaded;
            Unloaded += OnUnloaded;

            this.InitializeComponent();

            WindowHostOptions.ButtonForegroundColor = Colors.White;
            WindowHostOptions.ExtendViewIntoTitleBar = true;
            WindowHostOptions.CustomTitleBar = CustomTitleBar;

            // Register settings view navigation
            WeakReferenceMessenger.Default.Register<SettingsViewNavigationRequestMessage>(this, (s, e) => NavigatePage(e));

            // Register now playing view navigation.
            WeakReferenceMessenger.Default.Register<NowPlayingViewNavigationRequestMessage>(this, (s, e) => NavigatePage(e));
            WeakReferenceMessenger.Default.Register<BackNavigationRequested>(this, (s, e) => NavigatePage(e));
        }

        /// <summary>
        /// The backing dependency property for <see cref="SettingsStorage"/>.
        /// </summary>
        public static readonly DependencyProperty SettingsStorageProperty =
            DependencyProperty.Register(nameof(SettingsStorage), typeof(IModifiableFolder), typeof(ZuneDesktop), new PropertyMetadata(null, (d, e) => ((ZuneDesktop)d).OnSettingsStorageChanged(e.OldValue as IModifiableFolder, e.NewValue as IModifiableFolder)));

        /// <summary>
        /// The folder where settings should be stored.
        /// </summary>
        public IModifiableFolder? SettingsStorage
        {
            get => (IModifiableFolder?)GetValue(SettingsStorageProperty);
            set => SetValue(SettingsStorageProperty, value);
        }

        private void ZuneShell_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= ZuneShell_Loaded;

            if (_settings is not null)
            {
                SettingsOverlay.DataContext = new ZuneDesktopSettingsViewModel(_settings);
            }

            SetupBackgroundImage();
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            Unloaded -= OnUnloaded;

            if (_settings is not null)
                _settings.PropertyChanged -= SettingsService_SettingChanged;
        }

        private void SetupBackgroundImage()
        {
            if (_settings is null)
                return;

            var backgroundImage = _settings.BackgroundImage;
            if (backgroundImage.IsNone)
            {
                BackgroundImage.Visibility = Visibility.Collapsed;
                return;
            }
            else
            {
                BackgroundImage.Visibility = Visibility.Visible;
            }

            var bitmapImage = new BitmapImage(backgroundImage.Path);
            BackgroundImageBrush.ImageSource = bitmapImage;
            BackgroundImageBrush.AlignmentY = backgroundImage.YAlignment;
            BackgroundImageBrush.Stretch = backgroundImage.Stretch;
        }

        private void ChangeBackgroundImage()
        {
            _ = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                HideBackground.Begin();
            });
        }

        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RequestTheme(Pivot.SelectedIndex == 0 ? ElementTheme.Dark : ElementTheme.Light);

            if (Pivot.SelectedIndex == 0)
            {
                QuickplayPage.RunEnterViewAnimation();
            }

            // Collection index.
            if (Pivot.SelectedIndex == 1)
            {
                PART_CollectionContent.AnimateAlbumCollection();
            }
        }

        private void RequestTheme(ElementTheme theme = ElementTheme.Default)
        {
            if (theme == ElementTheme.Default)
            {
                theme = Pivot.SelectedIndex == 0 ? ElementTheme.Dark : ElementTheme.Light;
            }

            RootControl.RequestedTheme = theme;
            var transition = theme == ElementTheme.Dark ? EnterDarkTheme : LeaveDarkTheme;
            transition.Begin();

            WindowHostOptions.ButtonForegroundColor = theme == ElementTheme.Dark ? Colors.White : Colors.Black;
        }

        private void SettingsService_SettingChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ZuneDesktopSettings.BackgroundImage))
            {
                ChangeBackgroundImage();
            }
        }

        private void SettingsLinkClicked(object sender, RoutedEventArgs e)
        {
            WeakReferenceMessenger.Default.Send<SettingsViewNavigationRequestMessage>();
        }

        private void RequestBack(object sender, RoutedEventArgs e)
        {
            WeakReferenceMessenger.Default.Send<BackNavigationRequested>();
        }

        private void NavigatePage(SettingsViewNavigationRequestMessage e)
        {
            SettingsOverlay.Visibility = Visibility.Visible;
            NowPlayingOverlay.Visibility = Visibility.Collapsed;
            MainContent.Visibility = Visibility.Collapsed;
            MainContent.Visibility = Visibility.Collapsed;
            NowPlayingBar.Visibility = Visibility.Collapsed;
            RequestTheme(ElementTheme.Light);
        }

        private void NavigatePage(NowPlayingViewNavigationRequestMessage e)
        {
            SettingsOverlay.Visibility = Visibility.Collapsed;
            NowPlayingOverlay.Visibility = Visibility.Visible;
            MainContent.Visibility = Visibility.Collapsed;
            NowPlayingBar.Visibility = Visibility.Collapsed;
            RequestTheme(ElementTheme.Dark);
        }

        private void NavigatePage(BackNavigationRequested e)
        {
            SettingsOverlay.Visibility = Visibility.Collapsed;
            NowPlayingOverlay.Visibility = Visibility.Collapsed;
            MainContent.Visibility = Visibility.Visible;
            NowPlayingBar.Visibility = Visibility.Visible;
            RequestTheme();
        }

        private void BackgroundHideCompleted(object sender, object e)
        {
            SetupBackgroundImage();
            ShowBackground.Begin();
        }

        private void OnSettingsStorageChanged(IModifiableFolder? oldValue, IModifiableFolder? newValue)
        {
            if (oldValue is not null)
            {
                // Settings folder is always paired with a dedicated settings object.
                Guard.IsNotNull(_settings);
                _settings.PropertyChanged -= SettingsService_SettingChanged;
            }

            if (newValue is not null)
            {
                _settings = new ZuneDesktopSettings(newValue);
                _settings.PropertyChanged += SettingsService_SettingChanged;

                SettingsOverlay.DataContext = new ZuneDesktopSettingsViewModel(_settings);
            }
        }
    }
}
