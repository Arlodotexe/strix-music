using System.Collections.Generic;

namespace StrixMusic.CoreInterfaces.Interfaces
{
    /// <summary>
    /// The common playable collection groups that are displayed in the UI
    /// </summary>
    public interface ICommonPlayableCollections
    {
        /// <summary>
        /// The consolidated music library across all cores.
        /// </summary>
        public IPlayableCollectionGroup Library { get; set; }

        /// <summary>
        /// The consolidated recently played items across all cores.
        /// </summary>
        public IPlayableCollectionGroup RecentlyPlayed { get; set; }

        /// <summary>
        /// Used to browse and discovered new music.
        /// </summary>
        public IList<IPlayableCollectionGroup>? Discoverables { get; }

        // TODO: Search
    }
}
