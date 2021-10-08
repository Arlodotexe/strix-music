using StrixMusic.Sdk.ViewModels;
using StrixMusic.Shells.Groove.ViewModels.Pages.Abstract;
using Windows.UI;

namespace StrixMusic.Shells.Groove.ViewModels.Pages
{
    public class PlaylistViewViewModel : GroovePageViewModel<PlaylistViewModel>
    {
        private Color? _backgroundColor;

        public PlaylistViewViewModel(PlaylistViewModel viewModel) : base(viewModel)
        {
        }

        public override string PageTitleResource => "Playlist";

        public Color? BackgroundColor
        {
            get => _backgroundColor;
            set => SetProperty(ref _backgroundColor, value);
        }
    }
}
