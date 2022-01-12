using StrixMusic.Sdk.Models.Base;
using StrixMusic.Sdk.Models.Core;
using StrixMusic.Sdk.Models.Merged;

namespace StrixMusic.Sdk.Models
{
    /// <inheritdoc cref="IArtistBase"/>
    /// <remarks>This interface should be implemented by the Sdk.</remarks>
    public interface IArtist : IArtistBase, IArtistCollectionItem, IAlbumCollection, ITrackCollection, IGenreCollection, IPlayable, ISdkMember, IMerged<ICoreArtist>, IMerged<ICoreArtistCollectionItem>
    {
        /// <summary>
        /// A <see cref="IPlayableCollectionGroup"/> of items related to this item.
        /// </summary>
        IPlayableCollectionGroup? RelatedItems { get; }
    }
}