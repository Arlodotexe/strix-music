using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using StrixMusic.Services.Navigation;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shell.Default.Controls.Internal
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Shell : UserControl
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
            _pagesMapping = new Dictionary<string, Type>
            {
                { nameof(HomeItem), typeof(HomeControl) },
                { "SettingsNavPaneItem", typeof(SettingsViewControl) },
            };
        }

        private void SetupIoc()
        {
            DefaultShellIoc.Initialize();
            _navigationService = DefaultShellIoc.Ioc.GetService<INavigationService<Control>>();
            _navigationService!.RegisterCommonPage(typeof(HomeControl));
        }

        private void NavigationService_NavigationRequested(object sender, NavigateEventArgs<Control> e)
        {
            if (!e.IsOverlay)
            {
                MainContent.Content = e.Page;
            }

            // This isn't great, but there should only be 4 items
            Type controlType = e.GetType();
            bool containsValue = false;
            foreach (var value in _pagesMapping.Values)
            {
                containsValue = containsValue || (value == controlType);
            }

            if (!containsValue)
            {
                NavView.SelectedItem = null;
            }
        }

        private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            NavigationViewItemBase navi = (args.SelectedItem as NavigationViewItemBase) !;
            if (navi == null || !_pagesMapping.ContainsKey(navi!.Name))
            {
                return;
            }

            _navigationService!.NavigateTo(_pagesMapping[navi!.Name]);
        }

        private void AutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            _navigationService!.NavigateTo(typeof(SearchViewControl), false, args.QueryText);
        }
    }
}
