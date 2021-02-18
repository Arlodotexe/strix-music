using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Diagnostics;
using StrixMusic.Sdk;
using StrixMusic.Sdk.Services.Localization;
using StrixMusic.Sdk.Services.Navigation;
using StrixMusic.Sdk.Uno.Controls.Collections.Events;
using StrixMusic.Sdk.Uno.Controls.Shells;
using StrixMusic.Sdk.Uno.Controls.Views;
using StrixMusic.Sdk.Uno.Controls.Views.Secondary;
using StrixMusic.Sdk.Uno.ViewModels;
using StrixMusic.Sdk.ViewModels;
using StrixMusic.Shells.Groove.Controls;
using System;
using System.Collections.Generic;
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
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GrooveShell : Shell
    {
        private readonly IReadOnlyDictionary<ToggleButton, Type> _pagesMapping;
        private readonly IReadOnlyDictionary<Type, string> _pageHeaderMapping;
        private readonly HashSet<Type> _pageHeaderVisibilitySet;
        private readonly Stack<Control> _history = new Stack<Control>();
        private INavigationService<Control>? _navigationService;
        private ILocalizationService? _localizationService;
        private NotificationsViewModel? _notificationsViewModel;
        private ToggleButton? _selectedPage;

        /// <summary>
        /// Initializes a new instance of the <see cref="GrooveShell"/> class.
        /// </summary>
        public GrooveShell()
        {
            InitializeComponent();

            _pagesMapping = new Dictionary<ToggleButton, Type>
            {
                { MyMusicButton, typeof(HomeView) },
                { NowPlayingButton, typeof(NowPlayingView) },
                { PlaylistsButton, typeof(PlaylistsPage) },
            };

            _pageHeaderMapping = new Dictionary<Type, string>
            {
                { typeof(HomeView), "MyMusic" },
                { typeof(AlbumView), "Album" },
                { typeof(ArtistView), "Artist" },
                { typeof(PlaylistView), "Playlist" },
                { typeof(PlaylistsPage), "Playlists" },
            };

            _pageHeaderVisibilitySet = new HashSet<Type>
            {
                typeof(HomeView),
                typeof(PlaylistsPage),
            };

            _selectedPage = MyMusicButton;
        }

        private MainViewModel ViewModel => (MainViewModel)DataContext;

        private NotificationsViewModel? NotificationsViewModel => _notificationsViewModel;

        /// <inheritdoc/>
        protected override void SetupTitleBar()
        {
            base.SetupTitleBar();

#if NETFX_CORE
            CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
            ApplicationViewTitleBar titleBar = ApplicationView.GetForCurrentView().TitleBar;
            titleBar.ButtonBackgroundColor = Colors.Transparent;
#endif

            SystemNavigationManager currentView = SystemNavigationManager.GetForCurrentView();
            currentView.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            currentView.BackRequested += (s, e) => _navigationService!.GoBack();
        }

        /// <inheritdoc/>
        protected override void PostShellSetup()
        {
            NavigationButtonClicked(MyMusicButton, new RoutedEventArgs());
            UpdatePaneState();
        }

        /// <inheritdoc />
        public override Task InitServices(IServiceCollection services)
        {
            foreach (var service in services)
            {
                if (service is null)
                    continue;

                if (service.ImplementationInstance is INavigationService<Control> navigationService)
                    _navigationService = SetupNavigationService(navigationService);

                if (service.ImplementationInstance is ILocalizationService localizationService)
                    _localizationService = localizationService;

                if (service.ImplementationInstance is NotificationsViewModel notificationsViewModel)
                    _notificationsViewModel = SetupNotificationsViewModel(notificationsViewModel);
            }

            return base.InitServices(services);
        }

        private INavigationService<Control> SetupNavigationService(INavigationService<Control> navigationService)
        {
            navigationService.NavigationRequested += NavigationService_NavigationRequested;
            navigationService.BackRequested += Shell_BackRequested;

            navigationService.RegisterCommonPage(typeof(HomeView));
            navigationService.RegisterCommonPage(typeof(NowPlayingView));

            return navigationService;
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

        private void NavigationButtonClicked(object sender, RoutedEventArgs e)
        {
            if (!(sender is ToggleButton button))
                return;

            bool isOverlay = false;

            if (button != NowPlayingButton)
            {
                // Clear history and change the selected page
                _history.Clear();
                if (_selectedPage != null)
                    _selectedPage.IsChecked = false;
                PlaylistList.ClearSelected();
                button.IsChecked = true;
                _selectedPage = button;
            }
            else
            {
                isOverlay = true;

                // Override button checked.
                //The SplitView isn't visible and the selection would need to be reveresed
                button.IsChecked = false;
            }

            if (_navigationService != null && _pagesMapping.ContainsKey(button))
                _navigationService.NavigateTo(_pagesMapping[button], isOverlay);

            if (MainSplitView.DisplayMode == SplitViewDisplayMode.CompactOverlay ||
                MainSplitView.DisplayMode == SplitViewDisplayMode.Overlay)
            {
                MainSplitView.IsPaneOpen = false;
            }
        }

        private void PlaylistClicked(object sender, SelectionChangedEventArgs<PlaylistViewModel> e)
        {
            if (_selectedPage != null)
                _selectedPage.IsChecked = false;

            _selectedPage = null;
        }

        private void Shell_BackRequested(object sender, EventArgs e)
        {
            if (OverlayContent.Visibility == Visibility.Visible)
            {
                OverlayContent.Visibility = Visibility.Collapsed;
                return;
            }

            if (_history.Count > 0)
            {
                MainContent.Content = _history.Pop();
            }
        }

        private void AutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            Guard.IsNotNull(_navigationService, nameof(_navigationService));
            _navigationService.NavigateTo(typeof(SearchView), false, args.QueryText);
        }

        private void NavigationService_NavigationRequested(object sender, NavigateEventArgs<Control> e)
        {
            if (!e.IsOverlay)
            {
                _history.Push((Control)MainContent.Content);
                MainContent.Content = e.Page;

                Type pageType = e.Page.GetType();
                if (_pageHeaderMapping.ContainsKey(pageType) && _localizationService != null)
                {
                    LargeHeaderText.Text = SmallHeaderText.Text =
                        _localizationService["Music", _pageHeaderMapping[pageType]];
                }

                LargeHeaderWrapper.Visibility = _pageHeaderVisibilitySet.Contains(pageType) ?
                    Visibility.Visible : Visibility.Collapsed;
            }
            else
            {
                OverlayContent.Content = e.Page;
                OverlayContent.Visibility = Visibility.Visible;
            }
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
    }
}
