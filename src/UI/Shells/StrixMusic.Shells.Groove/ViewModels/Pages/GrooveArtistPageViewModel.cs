using StrixMusic.Sdk.ViewModels;
using StrixMusic.Shells.Groove.ViewModels.Abstract;
using StrixMusic.Shells.Groove.ViewModels.Pages.Interfaces;
using Windows.UI;

namespace StrixMusic.Shells.Groove.ViewModels.Pages
{
    public class GrooveArtistPageViewModel : GrooveViewModel<ArtistViewModel>, IGroovePageViewModel
    {
        private Color? _backgroundColor;

        public GrooveArtistPageViewModel(ArtistViewModel viewModel) : base(viewModel)
        {
        }

        public string PageTitleResource => "Artist";

        public Color? BackgroundColor
        {
            get => _backgroundColor;
            set => SetProperty(ref _backgroundColor, value);
        }
    }
}
