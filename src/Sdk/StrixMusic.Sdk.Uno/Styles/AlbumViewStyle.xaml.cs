﻿using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using StrixMusic.Sdk.Services.Navigation;
using StrixMusic.Sdk.Uno.Controls;
using Microsoft.Extensions.DependencyInjection;
using StrixMusic.Sdk.ViewModels;

namespace StrixMusic.Sdk.Uno.Styles
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
            if ((sender as Control)?.DataContext is AlbumViewModel viewModel)
            {
                INavigationService<Control> navigationService = DefaultShellIoc.Ioc.GetService<INavigationService<Control>>();
                navigationService.NavigateTo(typeof(ArtistView), false, viewModel.Artist);
            }
        }
    }
}
