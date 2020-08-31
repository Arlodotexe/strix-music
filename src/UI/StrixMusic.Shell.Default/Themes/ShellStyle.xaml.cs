using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using StrixMusic.Services.Navigation;
using StrixMusic.Shell.Default.Controls;
using StrixMusic.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shell.Default.Themes
{
    public sealed partial class ShellStyle : ResourceDictionary
    {
        private readonly IReadOnlyDictionary<string, Type> _pagesMapping;
        private INavigationService<Control> _navigationService;
        private NavigationView? _navigationView;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShellStyle"/> class.
        /// </summary>
        public ShellStyle()
        {
            this.InitializeComponent();
            _navigationService = Ioc.Default.GetService<INavigationService<Control>>();
            _pagesMapping = new Dictionary<string, Type>
            {
                { "HomeItem", typeof(HomeControl) },
            };
            RegisterPages();

            _navigationService.NavigationRequested += NavigationService_NavigationRequested;
        }

        private void NavigationService_NavigationRequested(object sender, Control e)
        {
            _navigationView!.Content = e;
        }

        private void RegisterPages()
        {
            _navigationService!.RegisterCommonPage(typeof(HomeControl));
        }

        private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            string pageName = (args.SelectedItem as Control) !.Tag.ToString();
            if (!_pagesMapping.ContainsKey(pageName))
            {
                return;
            }

            _navigationService!.NavigateTo(_pagesMapping[pageName]);
        }

        private void NavigationView_Loaded(object sender, RoutedEventArgs e)
        {
            _navigationView = (sender as NavigationView) !;
        }

        private void AutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            _navigationService.NavigateTo(typeof(SearchViewControl), Ioc.Default.GetService<MainViewModel>());
        }
    }
}
