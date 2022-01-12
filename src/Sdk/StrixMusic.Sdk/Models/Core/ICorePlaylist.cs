using StrixMusic.Sdk.Models.Base;

namespace StrixMusic.Sdk.Models.Core
{
    /// <summary>
    /// A collection of artibrary songs that the user can edit, rearrange and play back.
    /// </summary>
    /// <remarks>This interface should be implemented by a core.</remarks>
    public interface ICorePlaylist : IPlaylistBase, ICoreTrackCollection, ICorePlaylistCollectionItem, ICoreMember
    {
        /// <summary>
        /// Owner of the playable item.
        /// </summary>
        ICoreUserProfile? Owner { get; }

        /// <summary>
        /// A <see cref="ICorePlayableCollectionGroup"/> of items related to this item.
        /// </summary>
        ICorePlayableCollectionGroup? RelatedItems { get; }
    }
}
