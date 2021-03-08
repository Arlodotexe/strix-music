using System;
using System.Threading;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using OwlCore;
using StrixMusic.Helpers;
using StrixMusic.Sdk;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.Services.Navigation;
using StrixMusic.Sdk.Uno.Services.Localization;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shared
{
    /// <summary>
    /// A top-level frame that holds all other app content.
    /// </summary>
    public sealed partial class AppFrame : UserControl
    {
        /// <summary>
        /// The Window handle this AppFrame was created on.
        /// </summary>
        public Window Window { get; } = Window.Current;

        /// <summary>
        /// The root view model used throughout the app.
        /// </summary>
        public MainViewModel? ViewModel => DataContext as MainViewModel;

        /// <summary>
        /// Creates a new instance of <see cref="AppFrame"/>.
        /// </summary>
        public AppFrame()
        {
            this.InitializeComponent();

            Threading.SetPrimarySynchronizationContext(SynchronizationContext.Current);
            Threading.SetPrimaryThreadInvokeHandler(a => Window.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => a()).AsTask());

            AttachEvents();
        }

        /// <summary>
        /// Setup handling of any app-level content that requires an instance of a <see cref="MainViewModel"/>..
        /// </summary>
        public void SetupMainViewModel(MainViewModel mainViewModel)
        {
            mainViewModel.AppNavigationRequested += MainViewModel_AppNavigationRequested;

            DataContext = mainViewModel;
            this.Bindings.Update();
        }

        private void AttachEvents()
        {
            CurrentWindow.NavigationService.NavigationRequested += NavServiceOnNavigationRequested;
            Unloaded += AppFrame_Unloaded;
        }

        private void DetachEvents()
        {
            Unloaded -= AppFrame_Unloaded;
            CurrentWindow.NavigationService.NavigationRequested -= NavServiceOnNavigationRequested;
        }

        private void MainViewModel_AppNavigationRequested(object sender, AppNavigationTarget e)
        {
            if (e == AppNavigationTarget.Settings && sender is ICore core)
            {
                var navService = Ioc.Default.GetRequiredService<INavigationService<Control>>();
                var mainPage = Ioc.Default.GetRequiredService<MainPage>();

                // Send the user to the shell settings if a shell is loaded.
                if (mainPage.ActiveShellModel != null)
                {
                    // TODO post shell service refactor (need one common, injected ioc where we have access to the navigation service.
                    throw new NotImplementedException();
                }
                else
                {
                    navService.NavigateTo(typeof(SuperShell), false, core);
                }
            }
        }

        private void AppFrame_Unloaded(object sender, RoutedEventArgs e) => DetachEvents();

        private void AppFrame_OnLoaded(object sender, RoutedEventArgs e)
        {
            PART_ContentPresenter.Content = new AppLoadingView();
        }

        private void NavServiceOnNavigationRequested(object sender, NavigateEventArgs<Control> e)
        {
            var localizationService = Ioc.Default.GetRequiredService<LocalizationResourceLoader>();

            switch (e.Page)
            {
                case SuperShell superShell:
                    if (e.IsOverlay)
                        OverlayPresenter.Show(superShell, localizationService[Constants.Localization.CommonResource, "Settings"]);
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
