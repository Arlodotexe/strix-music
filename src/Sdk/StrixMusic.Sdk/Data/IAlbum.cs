using StrixMusic.Sdk.Data.Base;

namespace StrixMusic.Sdk.Data
{
    /// <inheritdoc cref="IAlbumBase"/>
    /// <remarks>This interface should be implemented by the Sdk.</remarks>
    public interface IAlbum : IAlbumBase, IAlbumCollectionItem, ITrackCollection, ISdkMember
    {
        /// <summary>
        /// An <see cref="IArtistBase"/> object that this album was created by.
        /// </summary>
        IArtist Artist { get; }

        /// <summary>
        /// A <see cref="IPlayableCollectionGroupBase"/> of items related to this item.
        /// </summary>
        IPlayableCollectionGroup? RelatedItems { get; }
    }
}