﻿using Microsoft.Toolkit.Diagnostics;
using StrixMusic.Sdk.Services.Navigation;
using StrixMusic.Sdk.Uno.Controls;
using StrixMusic.Sdk.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shells.Strix.Styles
{
    /// <summary>
    /// A <see cref="ResourceDictionary"/> containing the style and template for the <see cref="AlbumView"/> in the Strix Shell.
    /// </summary>
    public sealed partial class AlbumViewStyle : ResourceDictionary
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AlbumViewStyle"/> class.
        /// </summary>
        public AlbumViewStyle()
        {
            this.InitializeComponent();
        }

        private void GoToArtist(object sender, RoutedEventArgs e)
        {
            // TODO: Navigate to ArtistView
            if ((sender as Control)?.DataContext is AlbumViewModel viewModel)
            {
                INavigationService<Control> navigationService = StrixShellIoc.Ioc.GetService<INavigationService<Control>>() ?? ThrowHelper.ThrowInvalidOperationException<INavigationService<Control>>();
                navigationService.NavigateTo(typeof(ArtistView), false, viewModel.Artist);
            }
        }
    }
}
