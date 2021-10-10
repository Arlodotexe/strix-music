using StrixMusic.Sdk.ViewModels;
using StrixMusic.Shells.Groove.ViewModels.Abstract;
using StrixMusic.Shells.Groove.ViewModels.Pages.Interfaces;

namespace StrixMusic.Shells.Groove.ViewModels.Pages
{
    public class GrooveHomePageViewModel : GrooveViewModel<LibraryViewModel>, IGroovePageViewModel
    {
        public GrooveHomePageViewModel(LibraryViewModel viewModel) : base(viewModel)
        {
        }

        public string PageTitleResource => "MyMusic";
    }
}
