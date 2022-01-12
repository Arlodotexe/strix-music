using StrixMusic.Sdk.Models.Base;
using StrixMusic.Sdk.Models.Core;
using StrixMusic.Sdk.Models.Merged;

namespace StrixMusic.Sdk.Models
{
    /// <inheritdoc cref="IPlaylistBase"/>
    /// <remarks>This interface should be implemented by the Sdk.</remarks>
    public interface IPlaylist : IPlaylistBase, ITrackCollection, IImageCollection, IUrlCollection, IPlaylistCollectionItem, IPlayable, ISdkMember, IMerged<ICorePlaylist>, IMerged<ICorePlaylistCollectionItem>
    {
        /// <summary>
        /// Owner of the playable item.
        /// </summary>
        IUserProfile? Owner { get; }

        /// <summary>
        /// A <see cref="IPlayableCollectionGroup"/> of items related to this item.
        /// </summary>
        IPlayableCollectionGroup? RelatedItems { get; }
    }
}