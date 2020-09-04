using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using StrixMusic.Services.Navigation;
using StrixMusic.Shell.Default.Controls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shell.Strix.Controls
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Shell : ShellControl
    {
        private readonly IReadOnlyDictionary<Button, Type> _pagesMapping;
        private readonly IReadOnlyDictionary<Type, string> _overlayTypeMapping;
        private INavigationService<Control>? _navigationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="Shell"/> class.
        /// </summary>
        public Shell()
        {
            this.InitializeComponent();
            SetupIoc();
            _navigationService!.NavigationRequested += NavigationService_NavigationRequested;
            _navigationService!.BackRequested += Shell_BackRequested;
            _pagesMapping = new Dictionary<Button, Type>
            {
                [HomeTopButton] = typeof(HomeControl),
                [HomeBottomButton] = typeof(HomeControl),
                [SettingsButton] = typeof(SettingsViewControl),
            };

            _overlayTypeMapping = new Dictionary<Type, string>
            {
                { typeof(SettingsViewControl), nameof(OverlayOpenedPadded) },
            };
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
            _navigationService!.RegisterCommonPage(typeof(HomeControl));
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
            _navigationService!.NavigateTo(typeof(SearchViewControl), false, SearchTextBox.Text);
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
