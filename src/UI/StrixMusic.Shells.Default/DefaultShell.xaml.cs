﻿using System;
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
        private readonly IReadOnlyDictionary<string, Type> _pagesMapping;
        private INavigationService<Control>? _navigationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultShell"/> class.
        /// </summary>
        public DefaultShell()
        {
            InitializeComponent();
            SetupIoc();

            _navigationService!.NavigationRequested += NavigationService_NavigationRequested;
            _navigationService!.BackRequested += Shell_BackRequested;

            _pagesMapping = new Dictionary<string, Type>
            {
                { "Home", typeof(HomeView) },
                { "SettingsView", typeof(SettingsView) },
                { "Now Playing", typeof(NowPlayingView) },
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
                MainContent.Content = e.Page;
            }
            else
            {
                OverlayContent.Content = e.Page;
                OverlayContent.Visibility = Visibility.Visible;
            }

            // This isn't great, but there should only be 4 items
            Type controlType = e.Page.GetType();
            bool containsValue = controlType == typeof(SettingsView);
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
            _navigationService!.NavigateTo(typeof(SearchView), false, args.QueryText);
        }

        private void NavView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked)
            {
                _navigationService!.NavigateTo(_pagesMapping[nameof(SettingsView)]);
                return;
            }

            string invokedItemString = (args.InvokedItem as string) !;
            if (invokedItemString == null || !_pagesMapping.ContainsKey(invokedItemString))
            {
                return;
            }

            _navigationService!.NavigateTo(_pagesMapping[invokedItemString], invokedItemString == "Now Playing");
        }
    }
}
