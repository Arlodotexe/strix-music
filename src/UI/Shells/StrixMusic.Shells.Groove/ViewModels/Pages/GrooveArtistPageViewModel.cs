using Microsoft.Toolkit.Mvvm.ComponentModel;
using StrixMusic.Sdk.ViewModels;
using StrixMusic.Shells.Groove.ViewModels.Collections;
using StrixMusic.Shells.Groove.ViewModels.Pages.Interfaces;
using System;
using Windows.UI;

namespace StrixMusic.Shells.Groove.ViewModels.Pages
{
    /// <summary>
    /// A ViewModel for an <see cref="Controls.Pages.GrooveArtistPage"/>.
    /// </summary>
    public class GrooveArtistPageViewModel : ObservableObject, IGroovePageViewModel
    {
        private ArtistViewModel? _artistViewModel;
        private GrooveAlbumCollectionViewModel? _albumCollectionViewModel;
        private Color? _backgroundColor;

        /// <summary>
        /// Initializes a new instance of the <see cref="GrooveArtistPageViewModel"/> class.
        /// </summary>
        /// <param name="viewModel">The <see cref="ArtistViewModel"/> inside this ViewModel on display.</param>
        public GrooveArtistPageViewModel(ArtistViewModel viewModel)
        {
            ViewModel = viewModel;
            AlbumCollectionViewModel = new GrooveAlbumCollectionViewModel(viewModel);
        }

        /// <inheritdoc/>
        public bool ShowLargeHeader => false;

        /// <inheritdoc/>
        public string PageTitleResource => "Artist";

        /// <summary>
        /// The <see cref="ArtistViewModel"/> inside this ViewModel on display.
        /// </summary>
        public ArtistViewModel? ViewModel
        {
            get => _artistViewModel;
            set => SetProperty(ref _artistViewModel, value);
        }

        /// <summary>
        /// The <see cref="GrooveAlbumCollectionViewModel"/> for the <see cref="Controls.Collections.GrooveAlbumCollection"/> in the <see cref="Controls.Pages.GrooveArtistPage"/>.
        /// </summary>
        public GrooveAlbumCollectionViewModel? AlbumCollectionViewModel
        {
            get => _albumCollectionViewModel;
            set => SetProperty(ref _albumCollectionViewModel, value);
        }

        /// <summary>
        /// Gets or sets the color of the background for the <see cref="Controls.Pages.GrooveArtistPage"/>.
        /// </summary>
        public Color? BackgroundColor
        {
            get => _backgroundColor;
            set => SetProperty(ref _backgroundColor, value);
        }
    }
}
