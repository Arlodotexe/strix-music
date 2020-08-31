using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using StrixMusic.Services.Navigation;
using StrixMusic.Shell.Default.Controls;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shell.Strix.Controls
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Shell : UserControl
    {
        private readonly IReadOnlyDictionary<Button, Type> _pagesMapping;
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
            _pagesMapping = new Dictionary<Button, Type>
            {
                [HomeButton] = typeof(HomeControl),
            };
        }

        private void RegisterPages()
        {
            _navigationService!.RegisterCommonPage(typeof(HomeControl));
        }

        private void NavigationService_NavigationRequested(object sender, Control e)
        {
            MainContent.Child = e;
        }

        private void NavButtonClicked(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            _navigationService.NavigateTo(_pagesMapping[(sender as Button) !]);
        }
    }
}
