using System;
using System.Threading;
using OwlCore.Helpers;
using StrixMusic.Sdk;
using StrixMusic.Sdk.Services.Navigation;
using StrixMusic.Shells.Strix;
using StrixMusic.Shells.ZuneDesktop;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using StrixMusic.Sdk.Data.Core;

namespace StrixMusic.Shared
{
    public sealed partial class AppFrame : UserControl
    {
        /// <summary>
        /// The navigation service used exclusively by the <see cref="AppFrame"/> to display various top-level app content.
        /// </summary>
        public INavigationService<Control> NavigationService { get; }

        /// <summary>
        /// The <see cref="MainViewModel"/> for the app.
        /// </summary>
        public MainViewModel MainViewModel { get; private set; }

        /// <summary>
        /// The <see cref="Shared.MainPage"/> displayed in the app.
        /// </summary>
        public MainPage MainPage { get; private set; }

        /// <summary>
        /// Creates a new instance of <see cref="AppFrame"/>.
        /// </summary>
        public AppFrame()
        {
            this.InitializeComponent();

            var zune = typeof(ZuneShell);
            var strix = typeof(StrixShell);

            Threading.SetUISynchronizationContext(SynchronizationContext.Current);

            NavigationService = new NavigationService<Control>();

            MainViewModel = new MainViewModel();

            MainPage = new MainPage();

            NavigationService.RegisterCommonPage(MainPage);
        }

        private void AttachEvents()
        {
            Unloaded += AppFrame_Unloaded;
            MainViewModel.AppNavigationRequested += MainViewModel_AppNavigationRequested;
        }

        private void MainViewModel_AppNavigationRequested(object sender, AppNavigationTarget e)
        {
            if (e == AppNavigationTarget.Settings && sender is ICore core)
            {
                if (MainPage.ActiveShell != null)
                {
                    // TODO post shell service refactor (need one common, injected ioc where we have access to the navigation service.
                    throw new NotImplementedException();
                }
                else
                {
                    NavigationService.NavigateTo(typeof(SuperShell), false, core);
                }
            }
        }

        private void DetachEvents()
        {
            Unloaded -= AppFrame_Unloaded;
        }

        private void AppFrame_Unloaded(object sender, RoutedEventArgs e) => DetachEvents();

        private void AppFrame_OnLoaded(object sender, RoutedEventArgs e)
        {
            NavigationService.NavigationRequested += NavServiceOnNavigationRequested;

            PART_ContentPresenter.Content = new AppLoadingView();
        }

        private void NavServiceOnNavigationRequested(object sender, NavigateEventArgs<Control> e)
        {
            switch (e.Page)
            {
                case SuperShell superShell:
                    if (e.IsOverlay)
                        OverlayPresenter.Show(superShell, "Settings");
                    else
                        PART_ContentPresenter.Content = superShell;

                    break;
                case MainPage mainPage:
                    PART_ContentPresenter.Content = mainPage;
                    break;
                default:
                    return;
            }
        }
    }
}
