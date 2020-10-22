namespace StrixMusic.Sdk.Core.Data
{
    /// <summary>
    /// Interface that represents a playlist.
    /// </summary>
    public interface IPlaylist : IPlayable, ITrackCollection, IGenreCollection, IPlaylistCollectionItem
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
