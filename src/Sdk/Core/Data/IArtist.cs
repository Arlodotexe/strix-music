namespace StrixMusic.Sdk.Core.Data
{
    /// <summary>
    /// Interface representing an artistViewModel.
    /// </summary>
    public interface IArtist : IPlayable, IAlbumCollection, ITrackCollection, IGenreCollection
    {
        /// <summary>
        /// A <see cref="IPlayableCollectionGroup"/> of items related to this item.
        /// </summary>
        IPlayableCollectionGroup? RelatedItems { get; }
    }
}
