using StrixMusic.Sdk.AppModels;
using StrixMusic.Sdk.ViewModels.Notifications;
using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Sdk.WinUI.Controls.Shells
{
    /// <summary>
    /// A base class for the root control that all shells implement.
    /// </summary>
    public abstract partial class Shell : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Shell"/> class.
        /// </summary>
        protected Shell()
        {
            Loaded += ShellControl_Loaded;
        }

        /// <summary>
        /// The backing dependency property for <see cref="DataRoot"/>.
        /// </summary>
        public static readonly DependencyProperty DataRootProperty =
            DependencyProperty.Register(nameof(DataRoot), typeof(IStrixDataRoot), typeof(Shell), new PropertyMetadata(null));

        /// <summary>
        /// The backing dependency property for <see cref="Notifications"/>.
        /// </summary>
        public static readonly DependencyProperty NotificationsProperty =
            DependencyProperty.Register(nameof(Notifications), typeof(NotificationsViewModel), typeof(Shell), new PropertyMetadata(null));

        /// <summary>
        /// A ViewModel wrapper for all merged core data.
        /// </summary>
        public IStrixDataRoot DataRoot
        {
            get => (IStrixDataRoot)GetValue(DataRootProperty);
            set => SetValue(DataRootProperty, value);
        }

        /// <summary>
        /// A ViewModel for notifications displayed to the user.
        /// </summary>
        public NotificationsViewModel Notifications
        {
            get => (NotificationsViewModel)GetValue(NotificationsProperty);
            set => SetValue(NotificationsProperty, value);
        }

        private void ShellControl_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= ShellControl_Loaded;
            SetupTitleBar();
        }

        /// <summary>
        /// Sets properties of the title bar for this shell.
        /// </summary>
        protected virtual void SetupTitleBar()
        {
#if NETFX_CORE
            CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = false;
            ApplicationViewTitleBar titleBar = ApplicationView.GetForCurrentView().TitleBar;
            titleBar.ButtonBackgroundColor = Resources["SystemAltHighColor"] as Color?;
            SystemNavigationManager currentView = SystemNavigationManager.GetForCurrentView();
            currentView.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
#endif
        }
    }
}
