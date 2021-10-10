using StrixMusic.Sdk.ViewModels;
using StrixMusic.Shells.Groove.ViewModels.Abstract;
using StrixMusic.Shells.Groove.ViewModels.Pages.Interfaces;
using Windows.UI;

namespace StrixMusic.Shells.Groove.ViewModels.Pages
{
    public class GroovePlaylistViewModel : GrooveViewModel<PlaylistViewModel>, IGroovePageViewModel
    {
        private Color? _backgroundColor;

        public GroovePlaylistViewModel(PlaylistViewModel viewModel) : base(viewModel)
        {
        }

        public string PageTitleResource => "Playlist";

        public Color? BackgroundColor
        {
            get => _backgroundColor;
            set => SetProperty(ref _backgroundColor, value);
        }
    }
}
