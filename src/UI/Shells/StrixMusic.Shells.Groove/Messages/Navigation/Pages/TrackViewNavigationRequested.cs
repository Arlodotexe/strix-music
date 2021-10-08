using StrixMusic.Sdk.ViewModels;
using StrixMusic.Shells.Groove.Messages.Navigation.Pages.Abstract;

namespace StrixMusic.Shells.Groove.Messages.Navigation.Pages
{
    public class TrackViewNavigationRequested : PageNavigationRequestedMessage<TrackViewModel>
    {
        public TrackViewNavigationRequested(TrackViewModel viewModel) : base(viewModel) { }

        public static TrackViewNavigationRequested To(TrackViewModel viewModel)
        {
            return new TrackViewNavigationRequested(viewModel);
        }
    }
}
