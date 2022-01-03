using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Diagnostics;
using StrixMusic.Sdk;
using StrixMusic.Sdk.Services.Navigation;
using StrixMusic.Sdk.Services.Settings;
using StrixMusic.Sdk.Uno.Controls;
using StrixMusic.Sdk.Uno.Controls.Shells;
using StrixMusic.Sdk.Uno.Controls.Views;
using StrixMusic.Sdk.Uno.Services;
using StrixMusic.Sdk.Uno.Services.Localization;
using StrixMusic.Sdk.Uno.Services.NotificationService;
using StrixMusic.Shells.ZuneDesktop.Settings;
using StrixMusic.Shells.ZuneDesktop.Settings.Models;
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
        private INavigationService<Control>? _navigationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ZuneShell"/> class.
        /// </summary>
        public ZuneShell()
        {
            this.InitializeComponent();

            Loaded += ZuneShell_Loaded;
        }

        /// <inheritdoc />
        public override Task InitServices(IServiceCollection services)
        {
            var textStorageService = new TextStorageService();
            var settingsService = new ZuneDesktopSettingsService(textStorageService);
            settingsService.SettingChanged += SettingsService_SettingChanged;

            services.AddSingleton<ISettingsService>(settingsService);

            foreach (var service in services)
            {
                if (service is null)
                    continue;

                if (service.ImplementationInstance is INavigationService<Control> navigationService)
                    _navigationService = SetupNavigationService(navigationService);

                if (service.ImplementationInstance is LocalizationResourceLoader localizationLoaderService)
                    SetupLocalizationService(localizationLoaderService);

                if (service.ImplementationInstance is NotificationService notificationService)
                    SetupNotificationService(notificationService);
            }

            return base.InitServices(services);
        }

        private INavigationService<Control> SetupNavigationService(INavigationService<Control> navigationService)
        {
            navigationService.NavigationRequested += ZuneShell_NavigationRequested;
            navigationService.BackRequested += ZuneShell_BackRequested;

            return navigationService;
        }

        private void SetupLocalizationService(LocalizationResourceLoader localizationLoaderService)
        {
            localizationLoaderService.RegisterProvider("StrixMusic.Shells.ZuneDesktop/ZuneSettings");
        }

        private void SetupNotificationService(NotificationService notificationService)
        {
            notificationService.ChangeNotificationAlignment(HorizontalAlignment.Right, VerticalAlignment.Bottom);
            notificationService.ChangeNotificationMargins(new Thickness(25, 100, 25, 100));
        }

        private void ZuneShell_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= ZuneShell_Loaded;
            SettingsOverlay.DataContext = new ZuneDesktopSettingsViewModel();
            SetupBackgroundImage();
        }

        private async void SetupBackgroundImage()
        {
            var settingsService = Ioc.GetRequiredService<ISettingsService>();

            var backgroundImage = await settingsService.GetValue<ZuneDesktopBackgroundImage>(nameof(ZuneDesktopSettingsKeys.BackgroundImage));

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

        private void SettingsService_SettingChanged(object sender, SettingChangedEventArgs e)
        {
            if (e.Key == nameof(ZuneDesktopSettingsKeys.BackgroundImage))
            {
                ChangeBackgroundImage();
            }
        }

        private void SettingsLinkClicked(object sender, RoutedEventArgs e)
        {
            Guard.IsNotNull(_navigationService, nameof(_navigationService));

            _navigationService.NavigateTo(typeof(SettingsView));
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
