﻿using StrixMusic.Sdk;
using StrixMusic.Sdk.Core.Data;
using StrixMusic.Sdk.Core.ViewModels;
using StrixMusic.Sdk.Uno.Controls.Events;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shells.ZuneDesktop.Controls
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
        }

        private MainViewModel? ViewModel => DataContext as MainViewModel;

        private void ArtistsPageSelected(object sender, RoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "Artists", true);
        }

        private void ArtistSelected(object sender, SelectionChangedEventArgs<ArtistViewModel> e)
        {
            e.SelectedItem.PopulateMoreAlbumsCommand.Execute(20);
            AlbumCollection.DataContext = e.SelectedItem;
        }

        private void AlbumsPageSelected(object sender, RoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "Albums", true);
        }

        private void SongsPageSelected(object sender, RoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "Songs", true);
        }
    }
}
