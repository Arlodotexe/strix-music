using StrixMusic.Sdk.Models.Base;

namespace StrixMusic.Sdk.Models.Core
{
    /// <inheritdoc cref="IAlbumBase"/>
    /// <remarks>This interface should be implemented by a core.</remarks>
    public interface ICoreAlbum : IAlbumBase, ICoreAlbumCollectionItem, ICoreArtistCollection, ICoreTrackCollection, ICoreGenreCollection, ICoreMember
    {
        /// <summary>
        /// A <see cref="IPlayableCollectionGroupBase"/> of items related to this item.
        /// </summary>
        ICorePlayableCollectionGroup? RelatedItems { get; }
    }
}
