using System.Diagnostics.CodeAnalysis;
using StrixMusic.Sdk.Models.Base;
using StrixMusic.Sdk.Models.Core;
using StrixMusic.Sdk.Models.Merged;

namespace StrixMusic.Sdk.Models
{
    /// <inheritdoc cref="IAlbumBase"/>
    /// <remarks>This interface should be implemented by the Sdk.</remarks>
    [SuppressMessage("ReSharper", "PossibleInterfaceMemberAmbiguity", Justification = "Ambiguity is handled")]
    public interface IAlbum : IAlbumBase, IAlbumCollectionItem, IArtistCollection, ITrackCollection, IImageCollection, IGenreCollection, ISdkMember, IMerged<ICoreAlbum>, IMerged<ICoreAlbumCollectionItem>
    {
        /// <summary>
        /// A <see cref="IPlayableCollectionGroupBase"/> of items related to this item.
        /// </summary>
        IPlayableCollectionGroup? RelatedItems { get; }
    }
}