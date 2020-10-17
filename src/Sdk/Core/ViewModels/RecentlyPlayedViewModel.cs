using StrixMusic.Sdk.Core.Data;

namespace StrixMusic.Sdk.Core.ViewModels
{
    /// <summary>
    /// Used to bind recently played across multiple cores to the View model.
    /// </summary>
    public class RecentlyPlayedViewModel : PlayableCollectionGroupViewModel, IRecentlyPlayed
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RecentlyPlayedViewModel"/> class.
        /// </summary>
        /// <param name="recentlyPlayed">The <see cref="IRecentlyPlayed"/> to wrap.</param>
        public RecentlyPlayedViewModel(IRecentlyPlayed recentlyPlayed)
            : base(recentlyPlayed)
        {
        }
    }
}
