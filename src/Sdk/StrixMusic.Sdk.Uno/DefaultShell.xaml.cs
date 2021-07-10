using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Diagnostics;
using StrixMusic.Sdk.Services.Navigation;
using StrixMusic.Sdk.Uno.Controls.Shells;
using StrixMusic.Sdk.Uno.Controls.Views;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shells.Default
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DefaultShell : Shell
    {
        private readonly IReadOnlyDictionary<NavigationViewItemBase, Type> _pagesMapping;
        private readonly Stack<Control> _history = new Stack<Control>();
        private INavigationService<Control>? _navigationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultShell"/> class.
        /// </summary>
        public DefaultShell()
        {
            InitializeComponent();

            // Initialize map between NavigationItems and page types
            _pagesMapping = new Dictionary<NavigationViewItemBase, Type>
            {
                { HomeItem, typeof(HomeView) },
                { NowPlayingItem, typeof(NowPlayingView) },
            };
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

            base.InitServices(services);

            if (_navigationService is INavigationService<object> navService)
            {
                SubPageContainer.AttachNavigationService(navService);
            }

            return Task.CompletedTask;
        }

        private INavigationService<Control> SetupNavigationService(INavigationService<Control> navigationService)
        {
            navigationService.NavigationRequested += NavigationService_NavigationRequested;
            navigationService.BackRequested += Shell_BackRequested;

            navigationService.RegisterCommonPage(typeof(HomeView));
            navigationService.RegisterCommonPage(typeof(NowPlayingView));

            return navigationService;
        }

        /// <inheritdoc/>
        protected override void SetupTitleBar()
        {
            Guard.IsNotNull(_navigationService, nameof(_navigationService));
            base.SetupTitleBar();

            SystemNavigationManager currentView = SystemNavigationManager.GetForCurrentView();
            currentView.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            currentView.BackRequested += (s, e) => _navigationService.GoBack();
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
                
                // No sense checking the NavigationItems if the type is an overlay
                return;
            }

            // Set the selected NavigationItem to the new page, if it's on the list
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
                // If it's a top level item, clear history
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

            NavigationViewItemBase invokedItem = (args.InvokedItemContainer as NavigationViewItemBase)!;
            if (invokedItem == null || !_pagesMapping.ContainsKey(invokedItem))
            {
                return;
            }

            _navigationService.NavigateTo(_pagesMapping[invokedItem], invokedItem == NowPlayingItem);
        }
    }
}
