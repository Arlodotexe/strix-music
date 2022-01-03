using StrixMusic.Sdk.Models.Base;

namespace StrixMusic.Sdk.Models.Core
{
    /// <inheritdoc cref="IArtistBase"/>
    /// <remarks>This interface should be implemented by a core.</remarks>
    public interface ICoreArtist : IArtistBase, ICoreArtistCollectionItem, ICoreAlbumCollection, ICoreTrackCollection, ICoreGenreCollection, ICoreMember
    {
        /// <summary>
        /// A <see cref="IPlayableCollectionGroupBase"/> of items related to this item.
        /// </summary>
        ICorePlayableCollectionGroup? RelatedItems { get; }
    }
}
