using StrixMusic.Sdk.ViewModels;
using StrixMusic.Shells.Groove.ViewModels.Abstract;

namespace StrixMusic.Shells.Groove.ViewModels.Collections
{
    public class GrooveTrackCollectionViewModel : GrooveViewModel<ITrackCollectionViewModel>
    {
        public GrooveTrackCollectionViewModel(ITrackCollectionViewModel viewModel) : base(viewModel)
        {
        }
    }
}
