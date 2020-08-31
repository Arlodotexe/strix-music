using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using StrixMusic.Services.Navigation;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shell.Default.Controls.Internal
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Shell : UserControl
    {
        private readonly IReadOnlyDictionary<NavigationViewItemBase, Type> _pagesMapping;
        private INavigationService<Control> _navigationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="Shell"/> class.
        /// </summary>
        public Shell()
        {
            this.InitializeComponent();
            _navigationService = Ioc.Default.GetService<INavigationService<Control>>();
            RegisterPages();
            _navigationService.NavigationRequested += NavigationService_NavigationRequested;
            _pagesMapping = new Dictionary<NavigationViewItemBase, Type>
            {
                [HomeItem] = typeof(HomeControl),
            };
        }

        private void RegisterPages()
        {
            _navigationService!.RegisterCommonPage(typeof(HomeControl));
        }

        private void NavigationService_NavigationRequested(object sender, Control e)
        {
            NavView.Content = e;
        }

        private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            _navigationService!.NavigateTo(_pagesMapping[(args.SelectedItem as NavigationViewItemBase) !]);
        }

        private void AutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            _navigationService.NavigateTo(typeof(SearchViewControl), args.QueryText);
        }
    }
}
