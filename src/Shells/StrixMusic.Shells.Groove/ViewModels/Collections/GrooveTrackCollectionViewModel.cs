using CommunityToolkit.Mvvm.ComponentModel;
using StrixMusic.Sdk.ViewModels;

namespace StrixMusic.Shells.Groove.ViewModels.Collections
{
    /// <summary>
    /// A ViewModel for a <see cref="Controls.Collections.GrooveTrackCollectionViewModel"/>.
    /// </summary>
    public class GrooveTrackCollectionViewModel : ObservableObject
    {
        private ITrackCollectionViewModel? _trackCollectionViewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="GrooveTrackCollectionViewModel"/> class.
        /// </summary>
        public GrooveTrackCollectionViewModel()
        {
        }

        /// <summary>
        /// The <see cref="ITrackCollectionViewModel"/> inside this ViewModel on display.
        /// </summary>
        public ITrackCollectionViewModel? TrackCollection
        {
            get => _trackCollectionViewModel;
            set => SetProperty(ref _trackCollectionViewModel, value);
        }
    }
}
