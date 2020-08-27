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
    }
}
