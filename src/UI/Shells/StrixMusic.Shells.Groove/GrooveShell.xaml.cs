using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Diagnostics;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using StrixMusic.Sdk;
using StrixMusic.Sdk.Services.Localization;
using StrixMusic.Sdk.Uno.Controls.Shells;
using StrixMusic.Sdk.ViewModels;
using StrixMusic.Sdk.ViewModels.Notifications;
using StrixMusic.Shells.Groove.Messages.Navigation.Pages;
using StrixMusic.Shells.Groove.ViewModels.Pages;
using StrixMusic.Shells.Groove.ViewModels.Pages.Interfaces;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace StrixMusic.Shells.Groove
{
    public sealed partial class GrooveShell : Shell
    {
        /// <summary>
        /// A backing <see cref="DependencyProperty"/> for the <see cref="Title"/> property.
        /// </summary>
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(nameof(Title), typeof(string), typeof(GrooveShell), new PropertyMetadata(null));

        /// <summary>
        /// A backing <see cref="DependencyProperty"/> for the <see cref="ShowLargeHeader"/> property.
        /// </summary>
        public static readonly DependencyProperty ShowLargeHeaderProperty =
            DependencyProperty.Register(nameof(ShowLargeHeader), typeof(bool), typeof(GrooveShell), new PropertyMetadata(true));

        private ILocalizationService? _localizationService;

        private NotificationsViewModel? _notificationsViewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="GrooveShell"/> class.
        /// </summary>
        public GrooveShell()
        {
            this.InitializeComponent();

            WeakReferenceMessenger.Default.Register<HomeViewNavigationRequested>(this,
                (s, e) => NavigatePage(new GrooveHomePageViewModel(e.PageData)));
            WeakReferenceMessenger.Default.Register<AlbumViewNavigationRequested>(this,
                (s, e) => NavigatePage(new GrooveAlbumPageViewModel(e.PageData)));
            WeakReferenceMessenger.Default.Register<ArtistViewNavigationRequested>(this,
                (s, e) => NavigatePage(new GrooveArtistPageViewModel(e.PageData)));

            HamburgerPressedCommand = new RelayCommand(HamburgerToggled);

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

        /// <summary>
        /// Gets or sets the Title text for the Groove Shell.
        /// </summary>
        public bool ShowLargeHeader
        {
            get { return (bool)GetValue(ShowLargeHeaderProperty); }
            set { SetValue(ShowLargeHeaderProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Title text for the Groove Shell.
        /// </summary>
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        /// <summary>
        /// Gets a Command that handles a Hamburger button press.
        /// </summary>
        public RelayCommand HamburgerPressedCommand { get; }

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

        private void NavigationButtonClicked(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleButton button)
            {
                switch (button.Tag)
                {
                    case LibraryViewModel library:
                        WeakReferenceMessenger.Default.Send(new HomeViewNavigationRequested(library));
                        break;
                }
            }
        }

        private void HamburgerToggled()
        {
            MainSplitView.IsPaneOpen = !MainSplitView.IsPaneOpen;
        }

        private void OnPaneStateChanged(SplitView sender, object e) => UpdatePaneState();

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
            where T : IGroovePageViewModel
        {
            MainContent.Content = viewModel;

            Guard.IsNotNull(_localizationService, nameof(_localizationService));
            Title = _localizationService["Music", viewModel.PageTitleResource];
            ShowLargeHeader = viewModel.ShowLargeHeader;

            UpdateSelectedNavigationButton(viewModel);
        }

        private void UpdateSelectedNavigationButton<T>(T viewModel)
        {
            ToggleButton? button = viewModel switch
            {
                GrooveHomePageViewModel _ => MyMusicButton,
                _ => null,
            };

            // Check if a new navigation button will be set
            if (button != null)
            {
                // Reset all toggled 
                MyMusicButton.IsChecked = false;
                RecentButton.IsChecked = false;
                NowPlayingButton.IsChecked = false;
                PlaylistsButton.IsChecked = false;

                // Set the active navigation button
                button.IsChecked = true;
            }
        }
    }
}
