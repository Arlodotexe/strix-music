using System.Threading.Tasks;
using Microsoft.Toolkit.Diagnostics;
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
        }

        /// <summary>
        /// The root <see cref="MainViewModel" /> used by the shell.
        /// </summary>
        public MainViewModel? DataRoot
        {
            get { return (MainViewModel)GetValue(DataRootProperty); }
            set { SetValue(DataRootProperty, value); }
        }

        /// <summary>
        /// The backing dependency property for <see cref="DataRoot"/>.
        /// </summary>
        public static readonly DependencyProperty DataRootProperty =
            DependencyProperty.Register(nameof(DataRoot), typeof(MainViewModel), typeof(CollectionContent), new PropertyMetadata(null));

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
            if (e.SelectedItem == null)
                return;

            ZuneAlbumCollection.DetachItemEvents();

            e.SelectedItem.PopulateMoreAlbumsCommand.Execute(e.SelectedItem.TotalAlbumItemsCount);
            ZuneAlbumCollection.Collection = e.SelectedItem;

            e.SelectedItem.PopulateMoreTracksCommand.Execute(e.SelectedItem.TotalTrackCount);
            TrackCollection.Collection = e.SelectedItem;

            ZuneAlbumCollection.ArtistSelected(e.SelectedItem);
        }

        private void AlbumSelected(object sender, SelectionChangedEventArgs<AlbumViewModel> e)
        {
            if (e.SelectedItem == null)
                return;

            e.SelectedItem.PopulateMoreTracksCommand.Execute(e.SelectedItem.TotalTrackCount);
            TrackCollection.Collection = e.SelectedItem;
        }

        private void PlaylistSelected(object sender, SelectionChangedEventArgs<PlaylistViewModel> e)
        {
            if (e.SelectedItem == null)
                return;

            e.SelectedItem.PopulateMoreTracksCommand.Execute(e.SelectedItem.TotalTrackCount);
            TrackTable.Collection = e.SelectedItem;
            DetailsPane.DataContext = e.SelectedItem;
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

            ArtistCollection.Collection = DataRoot.Library;
            ZuneAlbumCollection.Collection = DataRoot.Library;
            TrackCollection.Collection = DataRoot.Library;
            TrackTable.Collection = DataRoot.Library;
            PlaylistCollection.Collection = DataRoot.Library;

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
