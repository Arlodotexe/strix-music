using StrixMusic.Sdk.ViewModels;
using StrixMusic.Shells.Groove.ViewModels.Pages.Abstract;
using Windows.UI;

namespace StrixMusic.Shells.Groove.ViewModels.Pages
{
    public class AlbumViewViewModel : GroovePageViewModel<AlbumViewModel>
    {
        private Color? _backgroundColor;

        public AlbumViewViewModel(AlbumViewModel viewModel) : base(viewModel)
        {
        }

        public override string PageTitleResource => "Album";

        public Color? BackgroundColor
        {
            get => _backgroundColor;
            set => SetProperty(ref _backgroundColor, value);
        }
    }
}
