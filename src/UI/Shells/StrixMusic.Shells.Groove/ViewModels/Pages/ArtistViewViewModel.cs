using StrixMusic.Sdk.ViewModels;
using StrixMusic.Shells.Groove.ViewModels.Pages.Abstract;
using Windows.UI;

namespace StrixMusic.Shells.Groove.ViewModels.Pages
{
    public class ArtistViewViewModel : GroovePageViewModel<ArtistViewModel>
    {
        private Color? _backgroundColor;

        public ArtistViewViewModel(ArtistViewModel viewModel) : base(viewModel)
        {
        }

        public override string PageTitleResource => "Artist";

        public Color? BackgroundColor
        {
            get => _backgroundColor;
            set => SetProperty(ref _backgroundColor, value);
        }
    }
}
