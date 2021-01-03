using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LaunchPad.Extensions.Windows.UI.Xaml;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Diagnostics;
using StrixMusic.Sdk.Services.Navigation;
using StrixMusic.Sdk.Uno.Controls;
using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shells.Groove
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GrooveShell : Shell
    {
        private readonly IReadOnlyDictionary<NavigationViewItemBase, Type> _pagesMapping;
        private readonly Stack<Control> _history = new Stack<Control>();
        private INavigationService<Control>? _navigationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="GrooveShell"/> class.
        /// </summary>
        public GrooveShell()
        {
            InitializeComponent();

            _pagesMapping = new Dictionary<NavigationViewItemBase, Type>
            {
                { HomeItem, typeof(HomeView) },
                { NowPlayingItem, typeof(NowPlayingView) },
            };
        }

        /// <inheritdoc/>
        protected override void SetupTitleBar()
        {
            base.SetupTitleBar();

#if NETFX_CORE
            CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
            ApplicationViewTitleBar titleBar = ApplicationView.GetForCurrentView().TitleBar;
            titleBar.ButtonBackgroundColor = Resources["SystemAltHighColor"] as Color?;
#endif

            SystemNavigationManager currentView = SystemNavigationManager.GetForCurrentView();
            currentView.BackRequested += (s, e) => _navigationService!.GoBack();
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

        private void NavigationService_NavigationRequested(object sender, NavigateEventArgs<Control> e)
        {
            if (!e.IsOverlay)
            {
                _history.Push((Control)MainContent.Content);
                MainContent.Content = e.Page;
            }
            else
            {
                OverlayContent.Content = e.Page;
                OverlayContent.Visibility = Visibility.Visible;
            }

            Type controlType = e.Page.GetType();
            bool containsValue = controlType == typeof(SettingsView);
            // This isn't great, but there should only be 4 items
            foreach (var value in _pagesMapping.Values)
            {
                containsValue = containsValue || (value == controlType);
            }

            if (!containsValue)
            {
                NavView.SelectedItem = null;
            }
            else
            {
                _history.Clear();
            }
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

        private void NavView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            Guard.IsNotNull(_navigationService, nameof(_navigationService));

            if (args.IsSettingsInvoked)
            {
                _navigationService.NavigateTo(typeof(SettingsView));
                return;
            }

            var invokedItem = args.InvokedItemContainer;
            if (invokedItem == null || !_pagesMapping.ContainsKey(invokedItem))
                return;

            _navigationService.NavigateTo(_pagesMapping[invokedItem], invokedItem == NowPlayingItem);
        }
    }
}
