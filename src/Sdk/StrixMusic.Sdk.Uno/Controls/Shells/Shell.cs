using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Sdk.Uno.Controls.Shells
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

            // Creating a new instance here so the old Ioc is wiped even if they don't call base.InitServices();
            Ioc = new Ioc();
        }

        /// <summary>
        /// The ioc provider used by this shell.
        /// </summary>
        public static Ioc Ioc { get; private set; } = new Ioc();

        /// <summary>
        /// Initializes the services for this shell's IoC.
        /// </summary>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        public virtual Task InitServices(IServiceCollection serviceCollection)
        {
            Ioc.ConfigureServices(serviceCollection.BuildServiceProvider());

            PostShellSetup();

            return Task.CompletedTask;
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

        /// <summary>
        /// Runs after all Shell services and window setup are finished.
        /// </summary>
        protected virtual void PostShellSetup()
        {
        }
    }
}
