using StrixMusic.Sdk.ViewModels;
using StrixMusic.Shells.Groove.Messages.Navigation.Pages.Abstract;

namespace StrixMusic.Shells.Groove.Messages.Navigation.Pages
{
    public sealed class AlbumViewNavigationRequested : PageNavigationRequestedMessage<AlbumViewModel>
    {
        public AlbumViewNavigationRequested(AlbumViewModel viewModel, bool record = true) : base(viewModel, record)
        {
        }
    }
}
