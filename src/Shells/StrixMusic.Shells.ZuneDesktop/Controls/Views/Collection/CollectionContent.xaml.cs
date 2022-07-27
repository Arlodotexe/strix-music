using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using StrixMusic.Sdk;
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

            ZuneAlbumCollection.ZuneCollectionType = CollectionContentType.Artist;
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
            if (ZuneAlbumCollection.Visibility == Visibility.Visible && ZuneAlbumCollection.AlbumsLoaded)
            {
                ZuneAlbumCollection.AnimateCollection();
            }
        }

        private void SwapPage(string pageVisualStateName)
        {
            AnimateAlbumCollection();

            VisualStateManager.GoToState(this, pageVisualStateName, true);
            PageTransition.Begin();
            ClearSelections();
        }

        private void ArtistsPageSelected(object sender, RoutedEventArgs e)
        {
            ZuneAlbumCollection.ZuneCollectionType = CollectionContentType.Artist;

            SwapPage("Artists");
        }

        private void AlbumsPageSelected(object sender, RoutedEventArgs e)
        {
            ZuneAlbumCollection.ZuneCollectionType = CollectionContentType.Albums;

            SwapPage("Albums");
        }

        private void SongsPageSelected(object sender, RoutedEventArgs e)
        {
            ZuneAlbumCollection.ZuneCollectionType = CollectionContentType.Tracks;

            SwapPage("Songs");
        }

        private void PlaylistPageSelected(object sender, RoutedEventArgs e)
        {
            SwapPage("Playlists");
        }

        private void ArtistSelected(object sender, SelectionChangedEventArgs<ArtistViewModel> e)
        {
            if (e.AddedItems.Count == 1)
            {
                var selectedItem = e.AddedItems.FirstOrDefault();
                if (selectedItem == null)
                    return;

                selectedItem.PopulateMoreAlbumsCommand.Execute(selectedItem.TotalAlbumItemsCount);
                ZuneAlbumCollection.Collection = selectedItem;

                selectedItem.PopulateMoreTracksCommand.Execute(selectedItem.TotalTrackCount);
                TrackCollection.Collection = selectedItem;
            }
            else
            {
                // TODO
            }
        }

        private void AlbumSelected(object sender, SelectionChangedEventArgs<ZuneAlbumCollectionItem> e)
        {
            if (e.AddedItems.Count == 1)
            {
                var selectedItem = e.AddedItems.FirstOrDefault();

                if (selectedItem == null)
                    return;

                if (selectedItem.Album == null)
                    return;

                selectedItem.Album.PopulateMoreTracksCommand.Execute(selectedItem.Album.TotalTrackCount);
                TrackCollection.Collection = selectedItem.Album;
            }
            else
            {
                // TODO
            }
        }

        private void PlaylistSelected(object sender, SelectionChangedEventArgs<PlaylistViewModel> e)
        {
            if (e.AddedItems.Count == 1)
            {
                var selectedItem = e.AddedItems.FirstOrDefault();

                if (selectedItem == null)
                    return;

                selectedItem.PopulateMoreTracksCommand.Execute(selectedItem.TotalTrackCount);
                TrackTable.Collection = selectedItem;
                DetailsPane.DataContext = selectedItem;
            }
            else
            {
                // TODO
            }
        }

        private void ClearSelections()
        {
            if (DataRoot == null)
                return;

            ArtistCollection.ClearSelected();
            ZuneAlbumCollection.ClearSelected();
            TrackTable.ClearSelected();
            TrackCollection.ClearSelected();
            PlaylistCollection.ClearSelected();

            // Clears by rebinding
            TrackTable.DataContext = null;

            Guard.IsNotNull(DataRoot?.Library, nameof(DataRoot.Library));

            ArtistCollection.Collection = (LibraryViewModel)DataRoot.Library;
            ZuneAlbumCollection.Collection = (LibraryViewModel)DataRoot.Library;
            TrackCollection.Collection = (LibraryViewModel)DataRoot.Library;
            TrackTable.Collection = (LibraryViewModel)DataRoot.Library;
            PlaylistCollection.Collection = (LibraryViewModel)DataRoot.Library;

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
