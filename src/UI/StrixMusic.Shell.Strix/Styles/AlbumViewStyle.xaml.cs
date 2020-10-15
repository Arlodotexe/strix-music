﻿using Microsoft.Extensions.DependencyInjection;
using StrixMusic.Sdk.Observables;
using StrixMusic.Sdk.Services.Navigation;
using StrixMusic.Shells.Controls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

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
            if ((sender as Control)?.DataContext is ObservableAlbum viewModel)
            {
                INavigationService<Control> navigationService = StrixShellIoc.Ioc.GetService<INavigationService<Control>>();
                navigationService.NavigateTo(typeof(ArtistView), false, viewModel.Artist);
            }
        }
    }
}
