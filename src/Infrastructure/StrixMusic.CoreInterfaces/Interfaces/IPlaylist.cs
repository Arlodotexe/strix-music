using System;

namespace StrixMusic.CoreInterfaces.Interfaces
{
    /// <summary>
    /// Interface that represents a playlist.
    /// </summary>
    public interface IPlaylist : ITrackCollection, IRelatedCollectionGroups
    {
        /// <summary>
        /// Owner of the playable item.
        /// </summary>
        IUserProfile? Owner { get; }

        /// <summary>
        /// Fires when <see cref="Owner"/> changes.
        /// </summary>
        event EventHandler<IUserProfile>? OwnerChanged;
    }
}
