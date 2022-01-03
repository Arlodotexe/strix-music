using System.Collections.Generic;
using StrixMusic.Sdk.Models;
using StrixMusic.Sdk.Models.Core;
using StrixMusic.Sdk.Models.Merged;
using StrixMusic.Sdk.Extensions;

namespace StrixMusic.Sdk.ViewModels
{
    /// <summary>
    /// Used to bind recently played across multiple cores to the View model.
    /// </summary>
    public class RecentlyPlayedViewModel : PlayableCollectionGroupViewModel, IRecentlyPlayed
    {
        private readonly IRecentlyPlayed _recentlyPlayed;

        /// <summary>
        /// Initializes a new instance of the <see cref="RecentlyPlayedViewModel"/> class.
        /// </summary>
        /// <param name="recentlyPlayed">The <see cref="IRecentlyPlayed"/> to wrap.</param>
        public RecentlyPlayedViewModel(IRecentlyPlayed recentlyPlayed)
            : base(recentlyPlayed)
        {
            _recentlyPlayed = recentlyPlayed;
        }

        /// <inheritdoc />
        IReadOnlyList<ICoreRecentlyPlayed> IMerged<ICoreRecentlyPlayed>.Sources => this.GetSources<ICoreRecentlyPlayed>();

        /// <inheritdoc />
        public bool Equals(ICoreRecentlyPlayed other) => _recentlyPlayed.Equals(other);
    }
}
