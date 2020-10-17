using System.Collections.Generic;
using System.Linq;
using StrixMusic.Sdk.Core.Data;

namespace StrixMusic.Sdk.Core.Merged
{
    /// <summary>
    /// A concrete class that merges multiple <see cref="IRecentlyPlayed"/>.
    /// </summary>
    public class MergedRecentlyPlayed : MergedPlayableCollectionGroupBase, IRecentlyPlayed
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MergedRecentlyPlayed"/> class.
        /// </summary>
        /// <param name="recentlyPlayed">The <see cref="IRecentlyPlayed"/> objects to merge.</param>
        public MergedRecentlyPlayed(IEnumerable<IRecentlyPlayed> recentlyPlayed)
            : base(recentlyPlayed.ToArray<IPlayableCollectionGroup>())
        {
        }
    }
}