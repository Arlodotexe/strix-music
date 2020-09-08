using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using StrixMusic.Sdk.Services.Navigation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shell.Default.Controls.Internal
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Shell : ShellControl
    {
        private readonly IReadOnlyDictionary<string, Type> _pagesMapping;
        private INavigationService<Control>? _navigationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="Shell"/> class.
        /// </summary>
        public Shell()
        {
            this.InitializeComponent();
            SetupIoc();
            _navigationService!.NavigationRequested += NavigationService_NavigationRequested;
            _navigationService!.BackRequested += Shell_BackRequested; ;
            _pagesMapping = new Dictionary<string, Type>
            {
                { "Home", typeof(HomeControl) },
                { "SettingsViewControl", typeof(SettingsViewControl) },
                { "Now Playing", typeof(NowPlayingViewControl) },
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
            _navigationService!.RegisterCommonPage(typeof(HomeControl));
            _navigationService!.RegisterCommonPage(typeof(NowPlayingViewControl));
        }

        private void NavigationService_NavigationRequested(object sender, NavigateEventArgs<Control> e)
        {
            if (!e.IsOverlay)
            {
                MainContent.Content = e.Page;
            }
            else
            {
                OverlayContent.Content = e.Page;
                OverlayContent.Visibility = Visibility.Visible;
            }

            // This isn't great, but there should only be 4 items
            Type controlType = e.Page.GetType();
            bool containsValue = controlType == typeof(SettingsViewControl);
            foreach (var value in _pagesMapping.Values)
            {
                containsValue = containsValue || (value == controlType);
            }

            if (!containsValue)
            {
                NavView.SelectedItem = null;
            }
        }

        private void Shell_BackRequested(object sender, EventArgs e)
        {
            OverlayContent.Visibility = Visibility.Collapsed;
        }

        private void AutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            _navigationService!.NavigateTo(typeof(SearchViewControl), false, args.QueryText);
        }

        private void NavView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked)
            {
                _navigationService!.NavigateTo(_pagesMapping[nameof(SettingsViewControl)]);
                return;
            }

            string invokedItemString = (args.InvokedItem as string)!;
            if (invokedItemString == null || !_pagesMapping.ContainsKey(invokedItemString))
            {
                return;
            }

            _navigationService!.NavigateTo(_pagesMapping[invokedItemString], invokedItemString == "Now Playing");
        }
    }
}
