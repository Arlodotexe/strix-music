using System;
using System.Collections.Generic;
using StrixMusic.Sdk.WinUI.Controls;
using StrixMusic.Sdk.WinUI.Controls.Views;
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
    public sealed partial class StrixShell : Shell
    {
        private readonly IReadOnlyDictionary<Button, Type> _pagesMapping;
        private readonly IReadOnlyDictionary<Type, string> _overlayTypeMapping;
        private readonly Stack<Control> _history = new Stack<Control>();
        private bool _isOverlayOpen;

        /// <summary>
        /// Initializes a new instance of the <see cref="StrixShell"/> class.
        /// </summary>
        public StrixShell()
        {
            this.InitializeComponent();

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
            //Guard.IsNotNull(_navigationService, nameof(_navigationService));
            //_navigationService.NavigateTo(_pagesMapping[(sender as Button) !]);
        }

        private void SettingsButtonClick(object sender, RoutedEventArgs e)
        {
            //Guard.IsNotNull(_navigationService, nameof(_navigationService));
            //_navigationService.NavigateTo(_pagesMapping[(sender as Button) !], true);
        }

        private void SearchButtonClicked(object sender, RoutedEventArgs e)
        {
            //Guard.IsNotNull(_navigationService, nameof(_navigationService));
            //_navigationService.NavigateTo(typeof(SearchView), false, SearchTextBox.Text);
        }

        private void EnterOverlayView(Control page)
        {
            OverlayContent.Content = page;

            VisualStateManager.GoToState(this, _overlayTypeMapping[page.GetType()], true);
        }

        private void BackButtonClicked(object sender, RoutedEventArgs e)
        {
            //Guard.IsNotNull(_navigationService, nameof(_navigationService));
            //_navigationService.GoBack();
        }
    }
}
