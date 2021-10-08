using StrixMusic.Sdk.ViewModels;
using StrixMusic.Shells.Groove.Messages.Navigation.Pages.Abstract;

namespace StrixMusic.Shells.Groove.Messages.Navigation.Pages
{
    public class PlaylistViewNavigationRequested : PageNavigationRequestedMessage<PlaylistViewModel>
    {
        public PlaylistViewNavigationRequested(PlaylistViewModel viewModel) : base(viewModel) { }

        public static PlaylistViewNavigationRequested To(PlaylistViewModel viewModel)
        {
            return new PlaylistViewNavigationRequested(viewModel);
        }
    }
}
