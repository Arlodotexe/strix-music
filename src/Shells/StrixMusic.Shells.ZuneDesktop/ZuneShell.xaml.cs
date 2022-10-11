using System;
using CommunityToolkit.Diagnostics;
using OwlCore.Storage;
using StrixMusic.Sdk.WinUI.Controls;
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
    /// The <see cref="Shell"/> implementation for ZuneDesktop.
    /// </summary>
    public sealed partial class ZuneShell : Shell
    {
        private IModifiableFolder? _settingStorage;
        private ZuneDesktopSettings? _settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="ZuneShell"/> class.
        /// </summary>
        public ZuneShell()
        {
            Loaded += ZuneShell_Loaded;
            Unloaded += OnUnloaded;

            this.InitializeComponent();
        }

        /// <summary>
        /// The backing dependency property for <see cref="SettingsStorage"/>.
        /// </summary>
        public static readonly DependencyProperty SettingsStorageProperty =
            DependencyProperty.Register(nameof(SettingsStorage), typeof(IModifiableFolder), typeof(ZuneShell), new PropertyMetadata(null, (d, e) => ((ZuneShell)d).OnSettingsStorageChanged(e.OldValue as IModifiableFolder, e.NewValue as IModifiableFolder)));

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

            BitmapImage bitmapImage = new BitmapImage(backgroundImage.Path);
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

        /// <inheritdoc/>
        protected override void SetupTitleBar()
        {
            base.SetupTitleBar();
#if NETFX_CORE
            CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
            Window.Current.SetTitleBar(CustomTitleBar);
            ApplicationViewTitleBar titleBar = ApplicationView.GetForCurrentView().TitleBar;
            titleBar.ButtonBackgroundColor = Colors.Transparent;
            titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
#endif
        }

        private void ZuneShell_NavigationRequested(object sender, EventArgs e)
        {
            // TODO navigate
            // if (e.Page is SettingsView)
            {
                SettingsOverlay.Visibility = Visibility.Visible;
                NowPlayingOverlay.Visibility = Visibility.Collapsed;
                MainContent.Visibility = Visibility.Collapsed;
                MainContent.Visibility = Visibility.Collapsed;
                NowPlayingBar.Visibility = Visibility.Collapsed;
                RequestTheme(ElementTheme.Light);
            }
            //   else if (e.Page is NowPlayingView)
            {
                SettingsOverlay.Visibility = Visibility.Collapsed;
                NowPlayingOverlay.Visibility = Visibility.Visible;
                MainContent.Visibility = Visibility.Collapsed;
                NowPlayingBar.Visibility = Visibility.Collapsed;
                RequestTheme(ElementTheme.Dark);
            }
        }

        private void ZuneShell_BackRequested(object sender, EventArgs e)
        {
            // TODO: Dynamic back navigation
            // Instead of just closing settings
            SettingsOverlay.Visibility = Visibility.Collapsed;
            NowPlayingOverlay.Visibility = Visibility.Collapsed;
            MainContent.Visibility = Visibility.Visible;
            NowPlayingBar.Visibility = Visibility.Visible;
            RequestTheme();
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
            Storyboard transition = theme == ElementTheme.Dark ? EnterDarkTheme : LeaveDarkTheme;
            transition.Begin();

            if (theme == ElementTheme.Dark)
            {
                ApplicationViewTitleBar titleBar = ApplicationView.GetForCurrentView().TitleBar;
                titleBar.ButtonForegroundColor = Colors.White;
            }
            else
            {
                ApplicationViewTitleBar titleBar = ApplicationView.GetForCurrentView().TitleBar;
                titleBar.ButtonForegroundColor = Colors.Black;
            }
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
            // TODO navigate
        }

        private void RequestBack(object sender, RoutedEventArgs e)
        {
            // TODO navigate
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
                _settingStorage = newValue;
                _settings = new ZuneDesktopSettings(newValue);
                _settings.PropertyChanged += SettingsService_SettingChanged;
                
                SettingsOverlay.DataContext = new ZuneDesktopSettingsViewModel(_settings);
            }
        }
    }
}
