using Microsoft.Toolkit.Mvvm.ComponentModel;
using StrixMusic.Sdk.ViewModels;
using StrixMusic.Shells.Groove.ViewModels.Collections;
using StrixMusic.Shells.Groove.ViewModels.Pages.Interfaces;

namespace StrixMusic.Shells.Groove.ViewModels.Pages
{
    /// <summary>
    /// A ViewModel for an <see cref="Controls.Pages.GrooveHomePage"/>.
    /// </summary>
    public class GrooveHomePageViewModel : ObservableObject, IGroovePageViewModel
    {
        private LibraryViewModel? _libraryViewModel;
        private GrooveAlbumCollectionViewModel? _albumCollectionViewModel;
        private GrooveArtistCollectionViewModel? _artistCollectionViewModel;
        private GrooveTrackCollectionViewModel? _trackCollectionViewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="GrooveHomePageViewModel"/> class.
        /// </summary>
        /// <param name="viewModel">The <see cref="LibraryViewModel"/> inside this ViewModel on display.</param>
        public GrooveHomePageViewModel(LibraryViewModel viewModel)
        {
            ViewModel = viewModel;
            AlbumCollectionViewModel = new GrooveAlbumCollectionViewModel(viewModel);
            ArtistCollectionViewModel = new GrooveArtistCollectionViewModel(viewModel);
            TrackCollectionViewModel = new GrooveTrackCollectionViewModel(viewModel);
        }

        /// <inheritdoc/>
        public bool ShowLargeHeader => true;

        /// <inheritdoc/>
        public string PageTitleResource => "MyMusic";

        /// <summary>
        /// The <see cref="LibraryViewModel"/> inside this ViewModel on display.
        /// </summary>
        public LibraryViewModel? ViewModel
        {
            get => _libraryViewModel;
            set => SetProperty(ref _libraryViewModel, value);
        }

        /// <summary>
        /// The <see cref="GrooveAlbumCollectionViewModel"/> for the <see cref="Controls.Collections.GrooveAlbumCollection"/> on display in the home page.
        /// </summary>
        public GrooveAlbumCollectionViewModel? AlbumCollectionViewModel
        {
            get => _albumCollectionViewModel;
            set => SetProperty(ref _albumCollectionViewModel, value);
        }

        /// <summary>
        /// The <see cref="GrooveArtistCollectionViewModel"/> for the <see cref="Controls.Collections.GrooveArtistCollection"/> on display in the home page.
        /// </summary>
        public GrooveArtistCollectionViewModel? ArtistCollectionViewModel
        {
            get => _artistCollectionViewModel;
            set => SetProperty(ref _artistCollectionViewModel, value);
        }

        /// <summary>
        /// The <see cref="GrooveTrackCollectionViewModel"/> for the <see cref="Controls.Collections.GrooveTrackCollection"/> on display in the home page.
        /// </summary>
        public GrooveTrackCollectionViewModel? TrackCollectionViewModel
        {
            get => _trackCollectionViewModel;
            set => SetProperty(ref _trackCollectionViewModel, value);
        }
    }
}
