using StrixMusic.Sdk.ViewModels;
using StrixMusic.Shells.Groove.ViewModels.Abstract;
using StrixMusic.Shells.Groove.ViewModels.Pages.Interfaces;
using Windows.UI;

namespace StrixMusic.Shells.Groove.ViewModels.Pages
{
    public class GrooveAlbumPageViewModel : GrooveViewModel<AlbumViewModel>, IGroovePageViewModel
    {
        private Color? _backgroundColor;

        public GrooveAlbumPageViewModel(AlbumViewModel viewModel) : base(viewModel)
        {
        }

        public string PageTitleResource => "Album";

        public Color? BackgroundColor
        {
            get => _backgroundColor;
            set => SetProperty(ref _backgroundColor, value);
        }
    }
}
