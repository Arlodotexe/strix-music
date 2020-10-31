using System.Collections.Generic;

namespace StrixMusic.Sdk.Core.Data
{
    /// <inheritdoc cref="IAlbumBase"/>
    /// <remarks>This interface should be implemented by a core.</remarks>
    public interface ICoreAlbum : IAlbumBase, ICoreAlbumCollectionItem, ICoreTrackCollection, ICoreMember
    {
        /// <summary>
        /// An <see cref="IArtistBase"/> object that this album was created by.
        /// </summary>
        ICoreArtist Artist { get; }

        /// <summary>
        /// A <see cref="IPlayableCollectionGroupBase"/> of items related to this item.
        /// </summary>
        ICorePlayableCollectionGroup? RelatedItems { get; }
    }
}
