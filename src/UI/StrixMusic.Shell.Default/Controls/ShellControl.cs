using Windows.ApplicationModel.Core;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shell.Default.Controls
{
    public abstract partial class ShellControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShellControl"/> class.
        /// </summary>
        public ShellControl()
        {
            Loaded += ShellControl_Loaded;
        }

        private void ShellControl_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Loaded -= ShellControl_Loaded;
            SetupTitleBar();
        }

        /// <summary>
        /// Sets properties of the taskbar for showing this shell.
        /// </summary>
        protected virtual void SetupTitleBar()
        {
#if NETFX_CORE
            CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = false;
#endif
        }
    }
}
