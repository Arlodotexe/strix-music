using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using StrixMusic.Sdk.Services.Navigation;
using StrixMusic.Sdk.Uno.Controls;
using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shells.Strix
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class StrixShell : ShellBase
    {
        private readonly IReadOnlyDictionary<Button, Type> _pagesMapping;
        private readonly IReadOnlyDictionary<Type, string> _overlayTypeMapping;
        private readonly Stack<Control> _history = new Stack<Control>();
        private INavigationService<Control>? _navigationService;
        private bool _isOverlayOpen;

        /// <summary>
        /// Initializes a new instance of the <see cref="StrixShell"/> class.
        /// </summary>
        public StrixShell()
        {
            this.InitializeComponent();
            SetupIoc();
            _navigationService!.NavigationRequested += NavigationService_NavigationRequested;
            _navigationService!.BackRequested += Shell_BackRequested;
            _isOverlayOpen = false;
            _pagesMapping = new Dictionary<Button, Type>
            {
                [HomeTopButton] = typeof(HomeView),
                [HomeBottomButton] = typeof(HomeView),
                [SettingsButton] = typeof(SettingsView),
            };

            _overlayTypeMapping = new Dictionary<Type, string>
            {
                { typeof(SettingsView), nameof(OverlayOpenedPadded) },
            };
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

        private void SetupIoc()
        {
            StrixShellIoc.Initialize();
            _navigationService = StrixShellIoc.Ioc.GetService<INavigationService<Control>>();
            _navigationService!.RegisterCommonPage(typeof(HomeView));
        }

        private void NavigationService_NavigationRequested(object sender, NavigateEventArgs<Control> e)
        {
            if (e.IsOverlay)
            {
                EnterOverlayView(e.Page);
                _isOverlayOpen = true;
            }
            else
            {
                _history.Push((Control)MainContent.Content);
                MainContent.Content = e.Page;

                // Clear history if root page type
                if (_pagesMapping.Values.Any(x => x == e.Page.GetType()))
                {
                    _history.Clear();
                }
            }
        }

        private void Shell_BackRequested(object sender, EventArgs e)
        {
            if (_isOverlayOpen)
            {
                VisualStateManager.GoToState(this, nameof(OverlayClosed), true);
            }
            else if (_history.Count > 0)
            {
                MainContent.Content = _history.Pop();
            }
        }

        private void NavButtonClicked(object sender, RoutedEventArgs e)
        {
            _navigationService!.NavigateTo(_pagesMapping[(sender as Button) !]);
        }

        private void SettingsButtonClick(object sender, RoutedEventArgs e)
        {
            _navigationService!.NavigateTo(_pagesMapping[(sender as Button) !], true);
        }

        private void SearchButtonClicked(object sender, RoutedEventArgs e)
        {
            _navigationService!.NavigateTo(typeof(SearchView), false, SearchTextBox.Text);
        }

        private void EnterOverlayView(Control page)
        {
            OverlayContent.Content = page;

            // TODO: Different overlay VisualStates dependant on overlay types
            VisualStateManager.GoToState(this, _overlayTypeMapping[page.GetType()], true);
        }

        private void BackButtonClicked(object sender, RoutedEventArgs e)
        {
            _navigationService!.GoBack();
        }
    }
}
