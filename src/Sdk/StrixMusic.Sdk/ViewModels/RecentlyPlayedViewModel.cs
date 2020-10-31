using StrixMusic.Sdk.Data.Core;

namespace StrixMusic.Sdk.ViewModels
{
    /// <summary>
    /// Used to bind recently played across multiple cores to the View model.
    /// </summary>
    public class RecentlyPlayedViewModel : PlayableCollectionGroupViewModel, ICoreRecentlyPlayed
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RecentlyPlayedViewModel"/> class.
        /// </summary>
        /// <param name="recentlyPlayed">The <see cref="ICoreRecentlyPlayed"/> to wrap.</param>
        public RecentlyPlayedViewModel(ICoreRecentlyPlayed recentlyPlayed)
            : base(recentlyPlayed)
        {
        }
    }
}
