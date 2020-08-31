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
        private INavigationService<Control>? _navigationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="Shell"/> class.
        /// </summary>
        public Shell()
        {
            this.InitializeComponent();
            SetupIoc();
            _navigationService!.NavigationRequested += NavigationService_NavigationRequested;
            _pagesMapping = new Dictionary<NavigationViewItemBase, Type>
            {
                [HomeItem] = typeof(HomeControl),
            };
        }

        private void SetupIoc()
        {
            DefaultShellIoc.Initialize();
            _navigationService = DefaultShellIoc.Ioc.GetService<INavigationService<Control>>();
            _navigationService!.RegisterCommonPage(typeof(HomeControl));
        }

        private void NavigationService_NavigationRequested(object sender, Control e)
        {
            MainContent.Content = e;
        }

        private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            NavigationViewItemBase navi = (args.SelectedItem as NavigationViewItemBase)!;
            if (!_pagesMapping.ContainsKey(navi))
            {
                return;
            }

            _navigationService!.NavigateTo(_pagesMapping[navi]);
        }

        private void AutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            _navigationService!.NavigateTo(typeof(SearchViewControl), args.QueryText);
        }
    }
}
