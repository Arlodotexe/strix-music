using StrixMusic.Sdk.Data.Base;

namespace StrixMusic.Sdk.Data.Core
{
    /// <inheritdoc cref="IPlaylistBase"/>
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
