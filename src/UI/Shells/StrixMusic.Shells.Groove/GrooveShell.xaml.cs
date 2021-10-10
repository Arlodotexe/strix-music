using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Diagnostics;
using Microsoft.Toolkit.Mvvm.Messaging;
using StrixMusic.Sdk;
using StrixMusic.Sdk.Services.Localization;
using StrixMusic.Sdk.Uno.Controls.Shells;
using StrixMusic.Sdk.ViewModels;
using StrixMusic.Sdk.ViewModels.Notifications;
using StrixMusic.Shells.Groove.Messages.Navigation.Pages;
using StrixMusic.Shells.Groove.ViewModels.Pages.Interfaces;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shells.Groove
{
    public sealed partial class GrooveShell : Shell
    {
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(nameof(Title), typeof(string), typeof(GrooveShell), new PropertyMetadata(null));

        private ILocalizationService? _localizationService;

        private NotificationsViewModel? _notificationsViewModel;

        public GrooveShell()
        {
            this.InitializeComponent();

            WeakReferenceMessenger.Default.Register<HomeViewNavigationRequested>(this,
                (s, e) => NavigatePage(e.PageData));
            WeakReferenceMessenger.Default.Register<AlbumViewNavigationRequested>(this,
                (s, e) => NavigatePage(e.PageData));

            DataContextChanged += GrooveShell_DataContextChanged;
        }

        private void GrooveShell_DataContextChanged(DependencyObject sender, DataContextChangedEventArgs args)
        {
            DataContextChanged -= GrooveShell_DataContextChanged;

            if (ViewModel?.Library != null)
                _ = WeakReferenceMessenger.Default.Send(new HomeViewNavigationRequested(ViewModel.Library));
        }

        private MainViewModel ViewModel => (MainViewModel)DataContext;

        private NotificationsViewModel? NotificationsViewModel => _notificationsViewModel;

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        /// <inheritdoc/>
        protected override void SetupTitleBar()
        {
            base.SetupTitleBar();

#if NETFX_CORE
            CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
            ApplicationViewTitleBar titleBar = ApplicationView.GetForCurrentView().TitleBar;
            titleBar.ButtonForegroundColor = Colors.White;
            titleBar.ButtonBackgroundColor = Colors.Transparent;
#endif

            SystemNavigationManager currentView = SystemNavigationManager.GetForCurrentView();
            currentView.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
        }

        /// <inheritdoc />
        public override Task InitServices(IServiceCollection services)
        {
            foreach (var service in services)
            {
                if (service is null)
                    continue;

                if (service.ImplementationInstance is ILocalizationService localizationService)
                    _localizationService = localizationService;

                if (service.ImplementationInstance is NotificationsViewModel notificationsViewModel)
                    _notificationsViewModel = SetupNotificationsViewModel(notificationsViewModel);
            }

            return base.InitServices(services);
        }

        private NotificationsViewModel SetupNotificationsViewModel(NotificationsViewModel notificationsViewModel)
        {
            notificationsViewModel.IsHandled = true;
            return notificationsViewModel;
        }

        private void HamburgerToggled(object sender, RoutedEventArgs e)
        {
            MainSplitView.IsPaneOpen = !MainSplitView.IsPaneOpen;
        }

        private void OnPaneOpening(SplitView sender, object e)
        {
            UpdatePaneState();
        }

        private void OnPaneClosing(SplitView sender, object e)
        {
            UpdatePaneState();
        }

        private void OnPaneClosed(SplitView sender, object e)
        {
            UpdatePaneState();
        }

        private void UpdatePaneState()
        {
            if (MainSplitView.IsPaneOpen)
            {
                VisualStateManager.GoToState(this, "Full", true);
            }
            else
            {
                VisualStateManager.GoToState(this, "Compact", true);
            }
        }

        private void NavigatePage<T>(T viewModel)
            where T : class
        {
            string titleResource = viewModel switch
            {
                AlbumViewModel _ => "Album",
                ArtistViewModel _ => "Artist",
                LibraryViewModel _ => "MyMusic",
                PlaylistViewModel _ => "Playlist",
                _ => ThrowHelper.ThrowArgumentOutOfRangeException<string>(),
            };

            MainContent.Content = viewModel;

            Guard.IsNotNull(_localizationService, nameof(_localizationService));
            Title = _localizationService["Music", titleResource];
        }
    }
}
