using Microsoft.Toolkit.Mvvm.ComponentModel;
using StrixMusic.Sdk.ViewModels;
using StrixMusic.Shells.Groove.ViewModels.Pages.Interfaces;
using Windows.UI;

namespace StrixMusic.Shells.Groove.ViewModels.Pages
{
    /// <summary>
    /// A ViewModel for a <see cref="Controls.Pages.GrooveAlbumPage"/>.
    /// </summary>
    public class GrooveAlbumPageViewModel : ObservableObject, IGroovePageViewModel
    {
        private AlbumViewModel? _albumViewModel;
        private Color? _backgroundColor;

        /// <summary>
        /// Initializes a new instance of the <see cref="GrooveAlbumPageViewModel"/> class.
        /// </summary>
        /// <param name="viewModel">The <see cref="AlbumViewModel"/> inside this ViewModel on display.</param>
        public GrooveAlbumPageViewModel(AlbumViewModel viewModel)
        {
            ViewModel = viewModel;
        }

        /// <inheritdoc/>
        public bool ShowLargeHeader => false;

        /// <inheritdoc/>
        public string PageTitleResource => "Album";

        /// <summary>
        /// The <see cref="AlbumViewModel"/> inside this ViewModel on display.
        /// </summary>
        public AlbumViewModel? ViewModel
        {
            get => _albumViewModel;
            set => SetProperty(ref _albumViewModel, value);
        }

        /// <summary>
        /// Gets or sets the color of the background for the <see cref="Controls.Pages.GrooveAlbumPage"/>.
        /// </summary>
        public Color? BackgroundColor
        {
            get => _backgroundColor;
            set => SetProperty(ref _backgroundColor, value);
        }
    }
}
