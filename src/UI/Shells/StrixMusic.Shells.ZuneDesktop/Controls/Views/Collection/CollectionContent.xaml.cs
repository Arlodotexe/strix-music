using StrixMusic.Sdk;
using StrixMusic.Sdk.Uno.Controls.Collections.Events;
using StrixMusic.Sdk.ViewModels;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace StrixMusic.Shells.ZuneDesktop.Controls.Views.Collections
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CollectionContent : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionContent"/> class.
        /// </summary>
        public CollectionContent()
        {
            this.InitializeComponent();
            _ = MainViewModel.Singleton?.Library?.PopulateMoreTracksCommand.ExecuteAsync(20);
            _ = MainViewModel.Singleton?.Library?.PopulateMoreAlbumsCommand.ExecuteAsync(20);
            _ = MainViewModel.Singleton?.Library?.PopulateMoreArtistsCommand.ExecuteAsync(20);
            _ = MainViewModel.Singleton?.Library?.PopulateMorePlaylistsCommand.ExecuteAsync(20);
        }

        private MainViewModel? ViewModel => DataContext as MainViewModel;

        private void SwapPage(string pageVisualStateName)
        {
            VisualStateManager.GoToState(this, pageVisualStateName, true);
            PageTransition.Begin();
            ClearSelections();
        }

        private void ArtistsPageSelected(object sender, RoutedEventArgs e)
        {
            SwapPage("Artists");
        }

        private void AlbumsPageSelected(object sender, RoutedEventArgs e)
        {
            SwapPage("Albums");
        }

        private void SongsPageSelected(object sender, RoutedEventArgs e)
        {
            SwapPage("Songs");
        }

        private void PlaylistPageSelected(object sender, RoutedEventArgs e)
        {
            SwapPage("Playlists");
        }

        private void ArtistSelected(object sender, SelectionChangedEventArgs<ArtistViewModel> e)
        {
            e.SelectedItem?.PopulateMoreAlbumsCommand.Execute(20);
            AlbumCollection.DataContext = e.SelectedItem;

            e.SelectedItem?.PopulateMoreTracksCommand.Execute(20);
            TrackCollection.DataContext = e.SelectedItem;
        }

        private void AlbumSelected(object sender, SelectionChangedEventArgs<AlbumViewModel> e)
        {
            e.SelectedItem?.PopulateMoreTracksCommand.Execute(20);
            TrackCollection.DataContext = e.SelectedItem;
        }

        private void PlaylistSelected(object sender, SelectionChangedEventArgs<PlaylistViewModel> e)
        {
            e.SelectedItem?.PopulateMoreTracksCommand.Execute(20);
            TrackCollection.DataContext =
                DetailsPane.DataContext = e.SelectedItem;
        }

        private void ClearSelections()
        {
            if (ViewModel == null)
                return;

            ArtistCollection.ClearSelected();
            AlbumCollection.ClearSelected();
            TrackCollection.ClearSelected();
            PlaylistCollection.ClearSelected();

            ArtistCollection.DataContext =
            AlbumCollection.DataContext =
            TrackCollection.DataContext =
            PlaylistCollection.DataContext= ViewModel.Library;

            DetailsPane.DataContext = null;
        }

        private void Grid_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Escape)
            {
                ClearSelections();
            }
        }
    }
}
