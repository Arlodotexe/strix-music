using StrixMusic.Sdk.Data.Base;

namespace StrixMusic.Sdk.Data
{
    /// <inheritdoc cref="IPlaylistBase"/>
    /// <remarks>This interface should be implemented by the Sdk.</remarks>
    public interface IPlaylist : IPlaylistBase, ITrackCollection, IPlaylistCollection, IGenreCollection, IPlaylistCollectionItem, ISdkMember
    {
        /// <summary>
        /// Owner of the playable item.
        /// </summary>
        IUserProfile? Owner { get; }

        /// <summary>
        /// A <see cref="IPlayableCollectionGroup"/> of items related to this item.
        /// </summary>
        IPlayableCollectionGroup? RelatedItems { get; }
    }
}