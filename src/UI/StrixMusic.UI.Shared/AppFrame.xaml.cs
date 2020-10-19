using LaunchPad.Controls;
using StrixMusic.Sdk;
using StrixMusic.Sdk.Services.Navigation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

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
        public MainViewModel MainViewModel { get; }

        /// <summary>
        /// Creates a new instance of <see cref="AppFrame"/>.
        /// </summary>
        public AppFrame()
        {
            this.InitializeComponent();
            NavigationService = new NavigationService<Control>();
            MainViewModel = new MainViewModel();
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
                case ShellLoader shellLoader:
                    PART_ContentPresenter.Content = shellLoader;
                    break;
                default:
                    return;
            }
        }
    }
}
