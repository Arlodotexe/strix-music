using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LaunchPad.Extensions.Windows.UI.Xaml;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Diagnostics;
using StrixMusic.Sdk.Services.Localization;
using StrixMusic.Sdk.Services.Navigation;
using StrixMusic.Sdk.Uno.Controls;
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
        private ToggleButton _selectedPage;

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
            };

            _pageHeaderMapping = new Dictionary<Type, string>
            {
                { typeof(HomeView), "MyMusic" },
                { typeof(AlbumView), "Album" },
                { typeof(ArtistView), "Artist" },
            };

            _pageHeaderVisibilitySet = new HashSet<Type>
            {
                typeof(HomeView),
            };

            _selectedPage = MyMusicButton;
        }

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
            currentView.BackRequested += (s, e) => _navigationService!.GoBack();
        }

        /// <inheritdoc/>
        protected override void PostShellSetup()
        {
            NavigationButtonClicked(MyMusicButton, new RoutedEventArgs());
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
                _selectedPage.IsChecked = false;
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
        }

        private void Shell_BackRequested(object sender, EventArgs e)
        {
            if (OverlayContent.Visibility == Visibility.Visible)
            {
                OverlayContent.Visibility = Visibility.Collapsed;
                SystemNavigationManager currentView = SystemNavigationManager.GetForCurrentView();
                currentView.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
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

                LargeHeaderText.Visibility = _pageHeaderVisibilitySet.Contains(pageType) ?
                    Visibility.Visible : Visibility.Collapsed;
            }
            else
            {
                OverlayContent.Content = e.Page;
                OverlayContent.Visibility = Visibility.Visible;
                SystemNavigationManager currentView = SystemNavigationManager.GetForCurrentView();
                currentView.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            }
        }
    }
}
