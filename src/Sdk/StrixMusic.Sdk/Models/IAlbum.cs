using StrixMusic.Sdk.Models.Base;
using StrixMusic.Sdk.Models.Core;
using StrixMusic.Sdk.Models.Merged;

namespace StrixMusic.Sdk.Models
{
    /// <summary>
    /// A published album containing one or more tracks, discs, artist, etc.
    /// </summary>
    /// <remarks>Instances of this class may contain data merged from one or more sources.</remarks>
    public interface IAlbum : IAlbumBase, IAlbumCollectionItem, IArtistCollection, ITrackCollection, IImageCollection, IGenreCollection, IPlayable, ISdkMember, IMerged<ICoreAlbum>, IMerged<ICoreAlbumCollectionItem>
    {
        /// <summary>
        /// A <see cref="IPlayableCollectionGroupBase"/> of items related to this item.
        /// </summary>
        IPlayableCollectionGroup? RelatedItems { get; }
    }
}