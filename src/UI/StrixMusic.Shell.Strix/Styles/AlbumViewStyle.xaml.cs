using Windows.UI.Xaml;

namespace StrixMusic.Shell.Strix.Styles
{
    public sealed partial class AlbumViewStyle : ResourceDictionary
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AlbumStyle"/> class.
        /// </summary>
        public AlbumViewStyle()
        {
            this.InitializeComponent();
        }

        private void GoToArtist(object sender, RoutedEventArgs e)
        {
            // TODO: Navigate to ArtistView
            //if ((sender as Control)?.DataContext is ObservableAlbum viewModel)
            //{
            //    // TODO: Investigate exposed ObservableArtist in ObservableAlbum
            //    if (viewModel.Artist is ObservableArtist observableArtist)
            //    {
            //        INavigationService<Control> navigationService = StrixShellIoc.Ioc.GetService<INavigationService<Control>>();
            //        navigationService.NavigateTo(typeof(ArtistView), false, observableArtist);
            //    }
            //}
        }
    }
}
