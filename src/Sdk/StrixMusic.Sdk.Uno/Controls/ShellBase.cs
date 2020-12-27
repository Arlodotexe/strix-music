using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Sdk.Uno.Controls
{
    /// <summary>
    /// The base type for the root element in all Shells.
    /// </summary>
    public abstract partial class ShellBase : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShellBase"/> class.
        /// </summary>
        protected ShellBase()
        {
            Loaded += ShellControl_Loaded;
        }

        private void ShellControl_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
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
