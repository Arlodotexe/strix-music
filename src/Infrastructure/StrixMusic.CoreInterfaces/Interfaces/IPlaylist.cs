using System;

namespace StrixMusic.CoreInterfaces.Interfaces
{
    /// <summary>
    /// Interface that represents a playlist.
    /// </summary>
    public interface IPlaylist : ITrackCollection
    {
        /// <summary>
        /// Owner of the playable item.
        /// </summary>
        IUserProfile? Owner { get; }

        /// <summary>
        /// A <see cref="IPlayable"/> of items related to this item.
        /// </summary>
        IPlayableCollectionGroup RelatedItems { get; }
    }
}
