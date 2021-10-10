using StrixMusic.Sdk.ViewModels;
using StrixMusic.Shells.Groove.Messages.Navigation.Pages.Abstract;

namespace StrixMusic.Shells.Groove.Messages.Navigation.Pages
{
    public class HomeViewNavigationRequested : PageNavigationRequestedMessage<LibraryViewModel>
    {
        public HomeViewNavigationRequested(LibraryViewModel viewModel) : base(viewModel) { }
    }
}
