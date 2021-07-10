using Microsoft.Toolkit.Mvvm.DependencyInjection;
using OwlCore;
using OwlCore.Uno.Controls;
using StrixMusic.Sdk;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.Services.Navigation;
using StrixMusic.Sdk.Uno.Controls.SubPages;
using StrixMusic.Sdk.Uno.Helpers;
using StrixMusic.Sdk.Uno.Services.Localization;
using StrixMusic.Sdk.Uno.Services.NotificationService;
using System;
using System.Linq;
using System.Threading;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shared
{
    /// <summary>
    /// A top-level frame that holds all other app content.
    /// </summary>
    public sealed partial class AppFrame : UserControl
    {
        private NotificationService? _notificationService;
        private SuperShell? _superShell;

        /// <summary>
        /// The Window handle this AppFrame was created on.
        /// </summary>
        public Window Window { get; } = Window.Current;

        /// <summary>
        /// The root view model used throughout the app.
        /// </summary>
        public MainViewModel? ViewModel => DataContext as MainViewModel;

        /// <summary>
        /// The content overlay used as a popup dialog for the entire app.
        /// </summary>
        public ControlSubPageContainer? PageContainer { get; private set; }

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
            DataContext = mainViewModel;
            this.Bindings.Update();

            AttachEvents(mainViewModel);
        }

        /// <summary>
        /// Setup handling of any app-level content that requires an instance of a <see cref="NotificationService"/>.
        /// </summary>
        /// <param name="notificationService"></param>
        public void SetupNotificationService(NotificationService notificationService)
        {
            _notificationService = notificationService;
            AttachEvents(notificationService);
        }

        private void AttachEvents()
        {
            subPageContainer.AttachNavigationService(CurrentWindow.NavigationService);
            CurrentWindow.NavigationService.NavigationRequested += NavServiceOnNavigationRequested;
            Loaded += AppFrame_Loaded;
            Unloaded += AppFrame_Unloaded;
        }

        private void AppFrame_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= AppFrame_Loaded;
            PageContainer = subPageContainer;
        }

        private void AttachEvents(NotificationService notificationService)
        {
            notificationService.NotificationMarginChanged += NotificationService_NotificationMarginChanged;
            notificationService.NotificationAlignmentChanged += NotificationService_NotificationAlignmentChanged;
        }

        private void DetachEvents(NotificationService notificationService)
        {
            notificationService.NotificationMarginChanged -= NotificationService_NotificationMarginChanged;
            notificationService.NotificationAlignmentChanged -= NotificationService_NotificationAlignmentChanged;
        }

        private void AttachEvents(MainViewModel mainViewModel)
        {
            mainViewModel.Cores.CollectionChanged += Cores_CollectionChanged;
        }

        private void DetachEvents(MainViewModel mainViewModel)
        {
            mainViewModel.Cores.CollectionChanged -= Cores_CollectionChanged;
        }

        private void DetachEvents()
        {
            Unloaded -= AppFrame_Unloaded;
            CurrentWindow.NavigationService.NavigationRequested -= NavServiceOnNavigationRequested;

            if (DataContext is MainViewModel mainViewModel)
                DetachEvents(mainViewModel);

            if (!(_notificationService is null))
                DetachEvents(_notificationService);
        }

        /// <summary>
        /// For displaying the UI for a <see cref="Sdk.Data.CoreState.NeedsSetup"/> core state.
        /// </summary>
        private void Cores_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (!(e.NewItems is null))
            {
                foreach (var item in e.NewItems)
                {
                    if (!(item is ICore core))
                        continue;

                    core.CoreStateChanged += Core_CoreStateChanged;
                }
            }

            if (!(e.OldItems is null))
            {
                foreach (var item in e.OldItems)
                {
                    if (!(item is ICore core))
                        continue;

                    core.CoreStateChanged -= Core_CoreStateChanged;
                }
            }
        }

        private async void Core_CoreStateChanged(object sender, Sdk.Data.CoreState e)
        {
            var localizationService = Ioc.Default.GetRequiredService<LocalizationResourceLoader>();

            if (!(sender is ICore core))
                return;

            if (e == Sdk.Data.CoreState.NeedsSetup)
            {
                await Threading.OnPrimaryThread(() =>
                {
                    _superShell ??= new SuperShell();

                    var relevantVm = Ioc.Default.GetRequiredService<MainViewModel>().Cores.First(x => x.InstanceId == core.InstanceId);
                    _superShell.ViewModel.CurrentCoreConfig = relevantVm;
                    _superShell.ViewModel.SelectedTabIndex = 1;

                    CurrentWindow.NavigationService.NavigateTo(_superShell, true);
                });
            }
        }

        private void NotificationService_NotificationMarginChanged(object sender, Thickness e)
        {
            NotificationItems.Margin = e;
        }

        private void NotificationService_NotificationAlignmentChanged(object sender, (HorizontalAlignment Horizontal, VerticalAlignment Vertical) e)
        {
            NotificationItems.HorizontalAlignment = e.Horizontal;
            NotificationItems.VerticalAlignment = e.Vertical;
        }

        private void AppFrame_Unloaded(object sender, RoutedEventArgs e) => DetachEvents();

        private void AppFrame_OnLoaded(object sender, RoutedEventArgs e)
        {
            PART_ContentPresenter.Content = new AppLoadingView();
        }

        private void NavServiceOnNavigationRequested(object sender, NavigateEventArgs<Control> e)
        {
            switch (e.Page)
            {
                case MainPage mainPage:
                    {
                        PART_ContentPresenter.Content = mainPage;
                        break;
                    }

                default:
                    return;
            }
        }
    }
}
