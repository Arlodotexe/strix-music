using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CommunityToolkit.Diagnostics;
using OwlCore;
using OwlCore.WinUI.Controls;
using StrixMusic.Sdk.Services.Navigation;
using StrixMusic.Sdk.ViewModels;
using StrixMusic.Sdk.ViewModels.Notifications;
using StrixMusic.Sdk.WinUI.Services.Localization;
using StrixMusic.Sdk.WinUI.Services.NotificationService;
using StrixMusic.Services;
using StrixMusic.Services.CoreManagement;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shared
{
    /// <summary>
    /// A top-level frame that holds all other app content.
    /// </summary>
    public sealed partial class AppFrame : UserControl
    {
        private SuperShell? _superShell;

        /// <summary>
        /// The backing dependency property for <see cref="Notifications" />.
        /// </summary>
        public static readonly DependencyProperty NotificationsProperty =
            DependencyProperty.Register(nameof(Notifications), typeof(NotificationsViewModel), typeof(AppFrame), new PropertyMetadata(null));

        /// <summary>
        /// Creates a new instance of <see cref="AppFrame"/>.
        /// </summary>
        public AppFrame()
        {
            InitializeComponent();

            Guard.IsNotNull(SynchronizationContext.Current, nameof(SynchronizationContext.Current));
            Threading.SetPrimarySynchronizationContext(SynchronizationContext.Current);

            var window = Window.Current;
            Threading.SetPrimaryThreadInvokeHandler(a => window.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => a()).AsTask());

            NotificationService = new NotificationService();
            Notifications = new NotificationsViewModel(NotificationService);
            LocalizationService = new()
            {
                Startup = ResourceLoader.GetForCurrentView(nameof(LocalizationResourceLoader.Startup)),
                SuperShell = ResourceLoader.GetForCurrentView(nameof(LocalizationResourceLoader.SuperShell)),
                Common = ResourceLoader.GetForCurrentView(nameof(LocalizationResourceLoader.Common)),
                Time = ResourceLoader.GetForCurrentView(nameof(LocalizationResourceLoader.Time)),
                Music = ResourceLoader.GetForCurrentView(nameof(LocalizationResourceLoader.Music)),
                Quips = ResourceLoader.GetForCurrentView(nameof(LocalizationResourceLoader.Quips)),
            };

            AttachEvents();
            AttachEvents(NotificationService);
        }

        /// <summary>
        /// A ViewModel for notifications displayed to the user.
        /// </summary>
        public NotificationsViewModel Notifications
        {
            get => (NotificationsViewModel)GetValue(NotificationsProperty);
            set => SetValue(NotificationsProperty, value);
        }

        /// <summary>
        /// The content overlay used as a popup dialog for the entire app.
        /// </summary>
        public ContentOverlay? ContentOverlay { get; private set; }

        /// <summary>
        /// A service that facilitates raising notifications.
        /// </summary>
        public NotificationService NotificationService { get; }

        /// <summary>
        ///  A service for getting localized strings from <see cref="ResourceLoader"/> providers.
        /// </summary>
        public LocalizationResourceLoader LocalizationService { get; }

        /// <summary>
        /// Navigates top the primary app content to the given <paramref name="element" />.
        /// </summary>
        /// <param name="element"></param>
        public void Present(FrameworkElement element)
        {
            PART_ContentPresenter.Content = element;
        }

        private void AttachEvents()
        {
            Loaded += AppFrame_Loaded;
            Unloaded += AppFrame_Unloaded;
        }

        private void AppFrame_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= AppFrame_Loaded;
            ContentOverlay = OverlayPresenter;
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

        private void DetachEvents()
        {
            Unloaded -= AppFrame_Unloaded;

            if (!(NotificationService is null))
                DetachEvents(NotificationService);
        }

        /// <summary>
        /// Displays the AbstractUI panel for the given core.
        /// </summary>
        public void DisplayAbstractUIPanel(ICore core, AppSettings appSettings, IReadOnlyList<ICore> loadedCores, ICoreManagementService coreManagementService)
        {
            _ = Threading.OnPrimaryThread(() =>
            {
                _superShell ??= new SuperShell(appSettings, coreManagementService);
                _superShell.LoadedCores = loadedCores.ToList();

                _superShell.CurrentCoreConfig = new CoreViewModel(core);
                _superShell.SelectedTabIndex = 1;

                OverlayPresenter.Show(_superShell, LocalizationService.Common?.GetString("Settings") ?? string.Empty);
            });
        }

        /// <summary>
        /// Opens the SuperShell.
        /// </summary>
        public void DisplaySuperShell(AppSettings appSettings, IReadOnlyList<ICore> loadedCores, ICoreManagementService coreManagementService)
        {
            _ = Threading.OnPrimaryThread(() =>
            {
                _superShell ??= new SuperShell(appSettings, coreManagementService);
                _superShell.LoadedCores = loadedCores.ToList();

                OverlayPresenter.Show(_superShell, LocalizationService.Common?.GetString("Settings") ?? string.Empty);
            });
        }

        private void NotificationService_NotificationMarginChanged(object? sender, Thickness e)
        {
            NotificationItems.Margin = e;
        }

        private void NotificationService_NotificationAlignmentChanged(object? sender, (HorizontalAlignment Horizontal, VerticalAlignment Vertical) e)
        {
            NotificationItems.HorizontalAlignment = e.Horizontal;
            NotificationItems.VerticalAlignment = e.Vertical;
        }

        private void AppFrame_Unloaded(object? sender, RoutedEventArgs e) => DetachEvents();

        private void AppFrame_OnLoaded(object? sender, RoutedEventArgs e) => Present(new AppLoadingView());
    }
}
