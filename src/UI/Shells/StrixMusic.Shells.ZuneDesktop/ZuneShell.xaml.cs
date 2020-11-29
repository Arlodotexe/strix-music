using System;
using Microsoft.Extensions.DependencyInjection;
using StrixMusic.Sdk;
using StrixMusic.Sdk.Services.Navigation;
using StrixMusic.Sdk.Uno.Controls;
using StrixMusic.Shells.ZuneDesktop.Settings;
using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Microsoft.Toolkit.Diagnostics;

namespace StrixMusic.Shells.ZuneDesktop
{
    /// <summary>
    /// The <see cref="ShellBase"/> implementation for ZuneDesktop.
    /// </summary>
    public sealed partial class ZuneShell : ShellBase
    {
        private INavigationService<Control>? _navigationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ZuneShell"/> class.
        /// </summary>
        public ZuneShell()
        {
            this.InitializeComponent();
            SetupIoc();
            _navigationService!.NavigationRequested += ZuneShell_NavigationRequested;
            _navigationService!.BackRequested += ZuneShell_BackRequested;
            Loaded += ZuneShell_Loaded;
        }

        private void ZuneShell_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= ZuneShell_Loaded;
            SettingsOverlay.DataContext = new ZuneDesktopSettingsViewModel();
            SetupBackgroundImage();
        }

        private async void SetupBackgroundImage()
        {
            ZuneDesktopSettingsService settingsService = ZuneDesktopShellIoc.Ioc.GetService<ZuneDesktopSettingsService>() ?? ThrowHelper.ThrowInvalidOperationException<ZuneDesktopSettingsService>();

            ZuneDesktopBackgroundImage backgroundImage = await settingsService.GetValue<ZuneDesktopBackgroundImage>(nameof(ZuneDesktopSettingsKeys.BackgroundImage));

            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
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
            });
        }

        private async void ChangeBackgroundImage()
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
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

        private MainViewModel? ViewModel => DataContext as MainViewModel;

        private void ZuneShell_NavigationRequested(object sender, NavigateEventArgs<Control> e)
        {
            if (e.Page is SettingsView)
            {
                SettingsOverlay.Visibility = Visibility.Visible;
                NowPlayingOverlay.Visibility = Visibility.Collapsed;
                MainContent.Visibility = Visibility.Collapsed;
                MainContent.Visibility = Visibility.Collapsed;
                NowPlayingBar.Visibility = Visibility.Collapsed;
                RequestTheme(ElementTheme.Light);
            }
            else if (e.Page is NowPlayingView)
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
            // TODO: Dyanmic back navigation
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

        private void SetupIoc()
        {
            ZuneDesktopShellIoc.Initialize();
            _navigationService = ZuneDesktopShellIoc.Ioc.GetService<INavigationService<Control>>();

            ZuneDesktopSettingsService settingsService = ZuneDesktopShellIoc.Ioc.GetService<ZuneDesktopSettingsService>() ?? ThrowHelper.ThrowInvalidOperationException<ZuneDesktopSettingsService>();
            settingsService.SettingChanged += SettingsService_SettingChanged;
        }

        private void SettingsService_SettingChanged(object sender, Sdk.Services.Settings.SettingChangedEventArgs e)
        {
            if (e.Key == nameof(ZuneDesktopSettingsKeys.BackgroundImage))
            {
                ChangeBackgroundImage();
            }
        }

        private void SettingsLinkClicked(object sender, RoutedEventArgs e)
        {
            _navigationService!.NavigateTo(typeof(SettingsView));
        }

        private void RequestBack(object sender, RoutedEventArgs e)
        {
            _navigationService!.GoBack();
        }

        private void BackgroundHideCompleted(object sender, object e)
        {
            SetupBackgroundImage();
            ShowBackground.Begin();
        }
    }
}
