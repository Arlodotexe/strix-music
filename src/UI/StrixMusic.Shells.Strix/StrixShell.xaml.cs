using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using StrixMusic.Sdk.Services.Navigation;
using StrixMusic.Sdk.Uno.Controls;
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
        private INavigationService<Control>? _navigationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="StrixShell"/> class.
        /// </summary>
        public StrixShell()
        {
            this.InitializeComponent();
            SetupIoc();
            _navigationService!.NavigationRequested += NavigationService_NavigationRequested;
            _navigationService!.BackRequested += Shell_BackRequested;
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
            Window.Current.SetTitleBar(CustomTitleBar);
#endif
        }

        private void Shell_BackRequested(object sender, EventArgs e)
        {
            // TODO: Pop the stack if the overlay is not open
            VisualStateManager.GoToState(this, nameof(OverlayClosed), true);
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
            }
            else
            {
                MainContent.Content = e.Page;
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

        private void GoBack(object sender, RoutedEventArgs e)
        {
            _navigationService!.GoBack();
        }
    }
}
