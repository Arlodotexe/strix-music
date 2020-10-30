namespace StrixMusic.Sdk.Core.Data
{
    /// <summary>
    /// A musician or creator that has published one or more <see cref="ICoreTrack"/>s and <see cref="ICoreAlbum"/>s.
    /// </summary>
    public interface IArtist : IPlayable, IArtistCollectionItem, IAlbumCollection, ITrackCollection, IGenreCollection
    {
        /// <summary>
        /// A <see cref="IPlayableCollectionGroup"/> of items related to this item.
        /// </summary>
        IPlayableCollectionGroup? RelatedItems { get; }
    }
}