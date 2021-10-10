using StrixMusic.Sdk.ViewModels;
using StrixMusic.Shells.Groove.Messages.Navigation.Pages.Abstract;

namespace StrixMusic.Shells.Groove.Messages.Navigation.Pages
{
    public sealed class ArtistViewNavigationRequested : PageNavigationRequestedMessage<ArtistViewModel>
    {
        public ArtistViewNavigationRequested(ArtistViewModel viewModel) : base(viewModel) { }
    }
}
