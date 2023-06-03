using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using StrixMusic.Sdk.ViewModels;
using StrixMusic.Sdk.WinUI.Controls.Collections.Events;
using StrixMusic.Shells.ZuneDesktop.Controls.Views.Collection;
using StrixMusic.Shells.ZuneDesktop.Controls.Views.Items;
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
        private string _currentSelectedPage = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionContent"/> class.
        /// </summary>
        public CollectionContent()
        {
            this.InitializeComponent();

            Loaded += CollectionContent_Loaded;
        }

        private void CollectionContent_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= CollectionContent_Loaded;

            Artists_ZuneAlbumCollection.ZuneCollectionType = CollectionContentType.Artist;
            SwapPage("Artists");
        }

        /// <summary>
        /// The root <see cref="StrixDataRootViewModel" /> used by the shell.
        /// </summary>
        public StrixDataRootViewModel? DataRoot
        {
            get { return (StrixDataRootViewModel)GetValue(DataRootProperty); }
            set { SetValue(DataRootProperty, value); }
        }

        /// <summary>
        /// The backing dependency property for <see cref="DataRoot"/>.
        /// </summary>
        public static readonly DependencyProperty DataRootProperty =
            DependencyProperty.Register(nameof(DataRoot), typeof(StrixDataRootViewModel), typeof(CollectionContent), new PropertyMetadata(null));

        /// <summary>
        /// Trigger animation on the <see cref="ZuneAlbumCollection"/> if its visible.
        /// </summary>
        public void AnimateAlbumCollection()
        {
            if (_currentSelectedPage == "Artists")
            {
                if (Artists_ZuneAlbumCollection.Visibility == Visibility.Visible && Artists_ZuneAlbumCollection.AlbumsLoaded)
                {
                    Artists_ZuneAlbumCollection.AnimateCollection();
                }
            }
            else if (_currentSelectedPage == "Albums")
            {
                if (Albums_ZuneAlbumCollection.Visibility == Visibility.Visible && Albums_ZuneAlbumCollection.AlbumsLoaded)
                {
                    Albums_ZuneAlbumCollection.AnimateCollection();
                }
            }
        }

        private async void SwapPage(string pageName)
        {
            _currentSelectedPage = pageName;
            AnimateAlbumCollection();

            CollectionSwitch.Value = pageName;
            ClearSelections();
        }

        private void ArtistsPageSelected(object sender, RoutedEventArgs e)
        {
            Artists_ZuneAlbumCollection.ZuneCollectionType = CollectionContentType.Artist;

            if (DataRoot?.Library.InitArtistCollectionAsyncCommand.CanExecute(null) ?? false)
                DataRoot.Library.InitArtistCollectionAsyncCommand.Execute(null);

            SwapPage("Artists");
        }

        private void AlbumsPageSelected(object sender, RoutedEventArgs e)
        {
            Albums_ZuneAlbumCollection.ZuneCollectionType = CollectionContentType.Albums;

            if (DataRoot?.Library.InitAlbumCollectionAsyncCommand.CanExecute(null) ?? false)
                DataRoot.Library.InitAlbumCollectionAsyncCommand.Execute(null);

            SwapPage("Albums");
        }

        private void SongsPageSelected(object sender, RoutedEventArgs e)
        {
            // Songs_ZuneAlbumCollection.ZuneCollectionType = CollectionContentType.Tracks;

            if (DataRoot?.Library.InitTrackCollectionAsyncCommand.CanExecute(null) ?? false)
                DataRoot.Library.InitTrackCollectionAsyncCommand.Execute(null);

            SwapPage("Songs");
        }

        private void PlaylistPageSelected(object sender, RoutedEventArgs e)
        {
            SwapPage("Playlists");
        }

        private void ArtistSelected(object sender, SelectionChangedEventArgs<ArtistViewModel> e)
        {
            if (e.SelectedItem == null)
                return;

            e.SelectedItem.PopulateMoreAlbumsCommand.Execute(e.SelectedItem.TotalAlbumItemsCount);

            Artists_ZuneAlbumCollection.Collection = e.SelectedItem;

            e.SelectedItem.PopulateMoreTracksCommand.Execute(e.SelectedItem.TotalTrackCount);
            Artists_TrackCollection.Collection = e.SelectedItem;
        }

        private void AlbumSelected(object sender, SelectionChangedEventArgs<ZuneAlbumCollectionItem> e)
        {
            if (e.SelectedItem == null)
                return;

            if (e.SelectedItem.Album == null)
                return;

            e.SelectedItem.Album.PopulateMoreTracksCommand.Execute(e.SelectedItem.Album.TotalTrackCount);
            Albums_TrackCollection.Collection = e.SelectedItem.Album;
        }

        private void PlaylistSelected(object sender, SelectionChangedEventArgs<PlaylistViewModel> e)
        {
            if (e.SelectedItem == null)
                return;

            e.SelectedItem.PopulateMoreTracksCommand.Execute(e.SelectedItem.TotalTrackCount);
            Playlists_TrackTable.Collection = e.SelectedItem;
            Playlists_DetailsPane.DataContext = e.SelectedItem;
        }

        private void ClearSelections()
        {
            if (DataRoot == null)
                return;

            Artists_ArtistCollection.ClearSelected();
            Artists_ZuneAlbumCollection.ClearSelected();
            Albums_ZuneAlbumCollection.ClearSelected();
            Songs_TrackTable.ClearSelected();
            Artists_TrackCollection.ClearSelected();
            Albums_TrackCollection.ClearSelected();
            Playlists_PlaylistCollection.ClearSelected();

            // Clears by rebinding
            Songs_TrackTable.DataContext = null;

            Guard.IsNotNull(DataRoot?.Library, nameof(DataRoot.Library));

            Artists_ArtistCollection.Collection = (LibraryViewModel)DataRoot.Library;
            Artists_ZuneAlbumCollection.Collection = (LibraryViewModel)DataRoot.Library;
            Albums_ZuneAlbumCollection.Collection = (LibraryViewModel)DataRoot.Library;
            Artists_TrackCollection.Collection = (LibraryViewModel)DataRoot.Library;
            Albums_TrackCollection.Collection = (LibraryViewModel)DataRoot.Library;
            Songs_TrackTable.Collection = (LibraryViewModel)DataRoot.Library;
            Playlists_PlaylistCollection.Collection = (LibraryViewModel)DataRoot.Library;

            Playlists_DetailsPane.DataContext = null;
            Playlists_DetailsPane.DataContext = null;
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
