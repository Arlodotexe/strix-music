using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using StrixMusic.Sdk.ViewModels;

namespace StrixMusic.Sdk.WinUI.Controls
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
        /// The backing dependency property for <see cref="Root"/>.
        /// </summary>
        public static readonly DependencyProperty RootProperty =
            DependencyProperty.Register(nameof(Root), typeof(StrixDataRootViewModel), typeof(Shell), new PropertyMetadata(null));

        /// <summary>
        /// A ViewModel wrapper for all merged core data.
        /// </summary>
        public StrixDataRootViewModel? Root
        {
            get => (StrixDataRootViewModel?)GetValue(RootProperty);
            set => SetValue(RootProperty, value);
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
