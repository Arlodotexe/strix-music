using StrixMusic.Sdk.ViewModels;
using StrixMusic.Shells.Groove.ViewModels.Pages.Abstract;

namespace StrixMusic.Shells.Groove.ViewModels.Pages
{
    public class HomeViewViewModel : GroovePageViewModel<LibraryViewModel>
    {
        public HomeViewViewModel(LibraryViewModel viewModel) : base(viewModel)
        {
        }

        public override string PageTitleResource => "MyMusic";
    }
}
