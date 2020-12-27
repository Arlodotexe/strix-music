using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using StrixMusic.Sdk.Services.Navigation;
using StrixMusic.Sdk.Uno;
using StrixMusic.Sdk.Uno.Controls;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shells.Default
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DefaultShell : ShellBase
    {
        private readonly IReadOnlyDictionary<NavigationViewItemBase, Type> _pagesMapping;
        private INavigationService<Control>? _navigationService;
        private Stack<Control> _history = new Stack<Control>();

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultShell"/> class.
        /// </summary>
        public DefaultShell()
        {
            InitializeComponent();
            SetupIoc();

            _navigationService!.NavigationRequested += NavigationService_NavigationRequested;
            _navigationService!.BackRequested += Shell_BackRequested;

            // Initialize map between NavigationItems and page types
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
            SystemNavigationManager currentView = SystemNavigationManager.GetForCurrentView();
            currentView.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            currentView.BackRequested += (s, e) => _navigationService!.GoBack();
        }

        private void SetupIoc()
        {
            DefaultShellIoc.Initialize();
            _navigationService = DefaultShellIoc.Ioc.GetService<INavigationService<Control>>();
            _navigationService!.RegisterCommonPage(typeof(HomeView));
            _navigationService!.RegisterCommonPage(typeof(NowPlayingView));
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
            _navigationService!.NavigateTo(typeof(SearchView), false, args.QueryText);
        }

        private void NavView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked)
            {
                _navigationService!.NavigateTo(typeof(SettingsView));
                return;
            }

            NavigationViewItemBase invokedItem = (args.InvokedItemContainer as NavigationViewItemBase) !;
            if (invokedItem == null || !_pagesMapping.ContainsKey(invokedItem))
            {
                return;
            }

            _navigationService!.NavigateTo(_pagesMapping[invokedItem], invokedItem == NowPlayingItem);
        }
    }
}
