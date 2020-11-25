using System.Collections.Generic;
using StrixMusic.Sdk.Data;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.Extensions;

namespace StrixMusic.Sdk.ViewModels
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

        /// <inheritdoc />
        IReadOnlyList<ICoreRecentlyPlayed> ISdkMember<ICoreRecentlyPlayed>.Sources => this.GetSources<ICoreRecentlyPlayed>();
    }
}
