using System.Collections.Generic;
using StrixMusic.Sdk.Extensions;
using StrixMusic.Sdk.Models;
using StrixMusic.Sdk.Models.Core;
using StrixMusic.Sdk.Models.Merged;

namespace StrixMusic.Sdk.ViewModels
{
    /// <summary>
    /// Used to bind to recently played items across multiple cores.
    /// </summary>
    public sealed class RecentlyPlayedViewModel : PlayableCollectionGroupViewModel, ISdkViewModel, IRecentlyPlayed
    {
        private readonly IRecentlyPlayed _recentlyPlayed;

        /// <summary>
        /// Initializes a new instance of the <see cref="RecentlyPlayedViewModel"/> class.
        /// </summary>
        /// <param name="root">The <see cref="MainViewModel"/> that this or the object that created this originated from.</param>
        /// <param name="recentlyPlayed">The <see cref="IRecentlyPlayed"/> to wrap.</param>
        internal RecentlyPlayedViewModel(MainViewModel root, IRecentlyPlayed recentlyPlayed)
            : base(root, recentlyPlayed)
        {
            _recentlyPlayed = recentlyPlayed;
        }

        /// <inheritdoc />
        IReadOnlyList<ICoreRecentlyPlayed> IMerged<ICoreRecentlyPlayed>.Sources => this.GetSources<ICoreRecentlyPlayed>();

        /// <inheritdoc />
        public bool Equals(ICoreRecentlyPlayed other) => _recentlyPlayed.Equals(other);
    }
}
