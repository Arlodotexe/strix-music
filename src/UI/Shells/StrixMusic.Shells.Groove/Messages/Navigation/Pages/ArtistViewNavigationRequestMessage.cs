using StrixMusic.Sdk.ViewModels;
using StrixMusic.Shells.Groove.Messages.Navigation.Pages.Abstract;

namespace StrixMusic.Shells.Groove.Messages.Navigation.Pages
{
    public sealed class ArtistViewNavigationRequestMessage : PageNavigationRequestMessage<ArtistViewModel>
    {
        public ArtistViewNavigationRequestMessage(ArtistViewModel viewModel, bool record = true) : base(viewModel, record)
        {
        }

        /// <inheritdoc/>
        public override bool ShowLargeHeader => false;

        /// <inheritdoc/>
        public override string PageTitleResource => "Artist";

    }
}
