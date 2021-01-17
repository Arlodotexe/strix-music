using System;
using System.Threading;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using OwlCore;
using StrixMusic.Helpers;
using StrixMusic.Sdk;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.Services.Localization;
using StrixMusic.Sdk.Services.Navigation;
using StrixMusic.Sdk.Uno.Services.Localization;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shared
{
    public sealed partial class AppFrame : UserControl
    {
        /// <summary>
        /// The navigation service used exclusively by the <see cref="AppFrame"/> to display various top-level app content.
        /// </summary>
        public INavigationService<Control> NavigationService { get; }

        private LocalizationResourceLoader? _localizationService;

        /// <summary>
        /// A reference to the <see cref="ILocalizationService"/> used through out the app (except Cores).
        /// </summary>
        public LocalizationResourceLoader LocalizationService => _localizationService ??= Ioc.Default.GetRequiredService<LocalizationResourceLoader>();

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

            Threading.SetPrimarySynchronizationContext(SynchronizationContext.Current);
            Threading.SetPrimaryThreadInvokeHandler(a => Window.Current.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => a()).AsTask());

            NavigationService = new NavigationService<Control>();

            MainViewModel = new MainViewModel();

            MainPage = new MainPage();

            NavigationService.RegisterCommonPage(MainPage);

            AttachEvents();
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
                // Send the user to the shell settings if a shell is loaded.
                if (MainPage.ActiveShellModel != null)
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
                    {
                        OverlayPresenter.Show(
                            superShell,
                            LocalizationService[
                                Constants.Localization.CommonResource,
                                "Settings"]);
                    }
                    else
                    {
                        PART_ContentPresenter.Content = superShell;
                    }

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
