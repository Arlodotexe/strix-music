using StrixMusic.Sdk.ViewModels;
using StrixMusic.Shells.Groove.Messages.Navigation.Pages.Abstract;

namespace StrixMusic.Shells.Groove.Messages.Navigation.Pages
{
    public class PlaylistsViewNavigationRequestMessage : PageNavigationRequestMessage<IPlaylistCollectionViewModel>
    {
        public PlaylistsViewNavigationRequestMessage(IPlaylistCollectionViewModel viewModel, bool record = true) : base(viewModel, record)
        {
        }

        /// <inheritdoc/>
        public override bool ShowLargeHeader => true;

        /// <inheritdoc/>
        public override string PageTitleResource => "Playlists";
    }
}
