using System.Threading;
using OwlCore.Helpers;
using StrixMusic.Sdk;
using StrixMusic.Sdk.Services.Navigation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using StrixMusic.Shells.Strix;
using StrixMusic.Shells.ZuneDesktop;

namespace StrixMusic.Shared
{
    public sealed partial class AppFrame : UserControl
    {
        /// <summary>
        /// The navigation service used exclusively by the <see cref="AppFrame"/> to display various top-level app content.
        /// </summary>
        public INavigationService<Control> NavigationService { get; }

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

            var zune = typeof(ZuneShell);
            var strix = typeof(StrixShell);

            Threading.SetUISynchronizationContext(SynchronizationContext.Current);

            NavigationService = new NavigationService<Control>();

            MainViewModel = new MainViewModel();

            MainPage = new MainPage();

            NavigationService.RegisterCommonPage(MainPage);
        }

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
                    OverlayPresenter.Show(superShell, "Settings");
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
