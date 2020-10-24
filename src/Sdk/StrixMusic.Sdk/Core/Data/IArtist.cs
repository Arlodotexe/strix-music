namespace StrixMusic.Sdk.Core.Data
{
    /// <summary>
    /// A musician or creator that has published one or more <see cref="ITrack"/>s and <see cref="IAlbum"/>s.
    /// </summary>
    public interface IArtist : IPlayable, IArtistCollectionItem, IAlbumCollection, ITrackCollection, IGenreCollection
    {
        /// <summary>
        /// A <see cref="IPlayableCollectionGroup"/> of items related to this item.
        /// </summary>
        IPlayableCollectionGroup? RelatedItems { get; }
    }
}
