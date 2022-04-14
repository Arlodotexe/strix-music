using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Diagnostics;
using StrixMusic.Sdk.Services.Navigation;
using StrixMusic.Sdk.WinUI.Controls;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace StrixMusic.Shells.ZuneHD
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ZuneHDShell : Shell
    {
        private INavigationService<Control>? _navigationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ZuneHDShell"/> class.
        /// </summary>
        public ZuneHDShell()
        {
            InitializeComponent();
        }

        /// <inheritdoc />
        public override Task InitServices(IServiceCollection services)
        {
            foreach (var service in services)
            {
                if (service is null)
                    continue;

                if (service.ImplementationInstance is INavigationService<Control> navigationService)
                    _navigationService = SetupNavigationService(navigationService);
            }

            return base.InitServices(services);
        }

        private INavigationService<Control> SetupNavigationService(INavigationService<Control> navigationService)
        {
            return navigationService;
        }

        /// <inheritdoc/>
        protected override void SetupTitleBar()
        {
            Guard.IsNotNull(_navigationService, nameof(_navigationService));
            base.SetupTitleBar();

            SystemNavigationManager currentView = SystemNavigationManager.GetForCurrentView();
            currentView.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            currentView.BackRequested += (s, e) => _navigationService.GoBack();
        }

        private void EnterPrimaryView(object sender, TappedRoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "Primary", true);
        }

        private void EnterSecondaryView(object sender, TappedRoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "Secondary", true);
        }
    }
}
