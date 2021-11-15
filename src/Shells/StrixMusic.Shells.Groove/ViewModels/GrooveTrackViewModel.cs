using Microsoft.Toolkit.Mvvm.ComponentModel;
using StrixMusic.Sdk.Data;
using StrixMusic.Sdk.ViewModels;

namespace StrixMusic.Shells.Groove.Controls.Collections
{
    /// <summary>
    /// A single track with additional properties needed for using inside an items template.
    /// </summary>
    /// <remarks>
    /// This class was needed to access the context inside of a data template.
    /// <para/>
    /// The context is not available on the SDK view models yet because each track is treated as the same instance,
    /// and so it can't yet track the context of a single track.
    /// </remarks>
    public class GrooveTrackViewModel : ObservableObject
    {
        /// <summary>
        /// Creates a new instance of <see cref="GrooveTrackCollection"/>.
        /// </summary>
        public GrooveTrackViewModel(ITrackCollectionViewModel context, ITrack track)
        {
            Context = context;
            Track = track;
        }

        /// <summary>
        /// The playback context of the <see cref="Track"/>.
        /// </summary>
        public ITrackCollectionViewModel Context { get; }

        /// <summary>
        /// The relevant track.
        /// </summary>
        public ITrack Track { get; }
    }
}
