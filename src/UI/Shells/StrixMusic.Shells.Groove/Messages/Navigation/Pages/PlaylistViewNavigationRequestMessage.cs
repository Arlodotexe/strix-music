using StrixMusic.Sdk.ViewModels;
using StrixMusic.Shells.Groove.Messages.Navigation.Pages.Abstract;

namespace StrixMusic.Shells.Groove.Messages.Navigation.Pages
{
    public class PlaylistViewNavigationRequestMessage : PageNavigationRequestMessage<PlaylistViewModel>
    {
        public PlaylistViewNavigationRequestMessage(PlaylistViewModel viewModel, bool record = true) : base(viewModel, record)
        {
        }

        /// <inheritdoc/>
        public override bool ShowLargeHeader => false;

        /// <inheritdoc/>
        public override string PageTitleResource => "Playlist";
    }
}
