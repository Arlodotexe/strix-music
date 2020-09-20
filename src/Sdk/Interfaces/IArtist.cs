namespace StrixMusic.Sdk.Interfaces
{
    /// <summary>
    /// Interface representing an artist.
    /// </summary>
    public interface IArtist : IAlbumCollection, ITrackCollection, IGenreCollection
    {
        /// <summary>
        /// A <see cref="IPlayableCollectionGroup"/> of items related to this item.
        /// </summary>
        IPlayableCollectionGroup? RelatedItems { get; }
    }
}
