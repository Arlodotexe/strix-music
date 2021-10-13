using StrixMusic.Sdk.ViewModels;
using StrixMusic.Shells.Groove.Messages.Navigation.Pages.Abstract;

namespace StrixMusic.Shells.Groove.Messages.Navigation.Pages
{
    public class PlaylistsViewNavigationRequested : PageNavigationRequestedMessage<IPlaylistCollectionViewModel>
    {
        public PlaylistsViewNavigationRequested(IPlaylistCollectionViewModel viewModel, bool record = true) : base(viewModel, record)
        {
        }
    }
}
