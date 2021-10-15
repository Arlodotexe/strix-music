using Microsoft.Toolkit.Mvvm.ComponentModel;
using OwlCore.Extensions;
using StrixMusic.Sdk.ViewModels;
using StrixMusic.Shells.Groove.Helper;
using StrixMusic.Shells.Groove.ViewModels.Pages.Interfaces;
using System.Threading.Tasks;
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
            : this()
        {
            Album = viewModel;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GrooveAlbumPageViewModel"/> class.
        /// </summary>
        /// <param name="viewModel">The <see cref="AlbumViewModel"/> inside this ViewModel on display.</param>
        public GrooveAlbumPageViewModel()
        {
        }

        /// <inheritdoc/>
        public bool ShowLargeHeader => false;

        /// <inheritdoc/>
        public string PageTitleResource => "Album";

        /// <summary>
        /// The <see cref="AlbumViewModel"/> inside this ViewModel on display.
        /// </summary>
        public AlbumViewModel? Album
        {
            get => _albumViewModel;
            set
            {
                SetProperty(ref _albumViewModel, value);

                if (!(value is null))
                    _ = ProcessAlbumArtColorAsync(value);
            }
        }

        /// <summary>
        /// Gets or sets the color of the background for the <see cref="Controls.Pages.GrooveAlbumPage"/>.
        /// </summary>
        public Color? BackgroundColor
        {
            get => _backgroundColor;
            set => SetProperty(ref _backgroundColor, value);
        }

        private async Task ProcessAlbumArtColorAsync(AlbumViewModel album)
        {
            // Load images if there aren't images loaded.
            await album.InitImageCollectionAsync();

            if (album.Images.Count > 0)
                BackgroundColor = await Task.Run(() => DynamicColorHelper.GetImageAccentColorAsync(album.Images[0]));
            else
                BackgroundColor = Colors.Transparent;
        }
    }
}
