using StrixMusic.Sdk.Data.Base;

namespace StrixMusic.Sdk.Data
{
    /// <inheritdoc cref="IArtistBase"/>
    /// <remarks>This interface should be implemented by the Sdk.</remarks>
    public interface IArtist : IArtistBase, IArtistCollectionItem, IAlbumCollection, ITrackCollection, IGenreCollection, ISdkMember
    {
        /// <summary>
        /// A <see cref="IPlayableCollectionGroupBase"/> of items related to this item.
        /// </summary>
        IPlayableCollectionGroup? RelatedItems { get; }
    }
}