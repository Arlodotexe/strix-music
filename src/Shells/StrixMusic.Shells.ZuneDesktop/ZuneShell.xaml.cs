using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Diagnostics;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using OwlCore.AbstractStorage;
using OwlCore.Extensions;
using StrixMusic.Sdk.Services;
using StrixMusic.Sdk.Services.Navigation;
using StrixMusic.Sdk.Uno.Controls.Shells;
using StrixMusic.Sdk.Uno.Controls.Views;
using StrixMusic.Sdk.Uno.Services.NotificationService;
using StrixMusic.Sdk.Uno.Services.ShellManagement;
using StrixMusic.Shells.ZuneDesktop.Settings;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
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
        private readonly IFolderData _settingStorage;
        private INavigationService<Control>? _navigationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ZuneShell"/> class.
        /// </summary>
        public ZuneShell(IFolderData settingStorage)
        {
            this.InitializeComponent();

            Loaded += ZuneShell_Loaded;
            _settingStorage = settingStorage;
        }

        /// <summary>
        /// Metadata used to identify this shell before instantiation.
        /// </summary>
        public static ShellMetadata Metadata { get; } = new ShellMetadata(id: "Zune.Desktop.4.8",
                                                                          displayName: "Zune Desktop",
                                                                          description: "A faithful recreation of the iconic Zune client for Windows",
                                                                          inputMethods: InputMethods.Mouse,
                                                                          minWindowSize: new Size(width: 700, height: 600));

        /// <inheritdoc/>
        public override Task InitServices(IServiceCollection services)
        {
            _navigationService = Ioc.Default.GetRequiredService<INavigationService<Control>>();
            SetupNavigationService(_navigationService);
            SetupNotificationService();

            var settings = new ZuneDesktopSettings(_settingStorage);
            settings.PropertyChanged += SettingsService_SettingChanged;

            services.AddSingleton(settings);

            return base.InitServices(services);
        }

        private INavigationService<Control> SetupNavigationService(INavigationService<Control> navigationService)
        {
            navigationService.NavigationRequested += ZuneShell_NavigationRequested;
            navigationService.BackRequested += ZuneShell_BackRequested;

            return navigationService;
        }

        private void SetupNotificationService()
        {
            var notificationService = Ioc.Default.GetRequiredService<INotificationService>();

            notificationService.Cast<NotificationService>().ChangeNotificationAlignment(HorizontalAlignment.Right, VerticalAlignment.Bottom);
            notificationService.Cast<NotificationService>().ChangeNotificationMargins(new Thickness(25, 100, 25, 100));
        }

        private void ZuneShell_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= ZuneShell_Loaded;

            SettingsOverlay.DataContext = new ZuneDesktopSettingsViewModel();
            _ = SetupBackgroundImage();
        }

        private async Task SetupBackgroundImage()
        {
            var settingsService = Ioc.GetRequiredService<ZuneDesktopSettings>();
            await settingsService.LoadAsync();

            var backgroundImage = settingsService.BackgroundImage;

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
