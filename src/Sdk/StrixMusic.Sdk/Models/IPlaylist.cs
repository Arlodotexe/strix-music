using StrixMusic.Sdk.Models.Base;
using StrixMusic.Sdk.Models.Core;
using StrixMusic.Sdk.Models.Merged;

namespace StrixMusic.Sdk.Models
{
    /// <summary>
    /// A collection of artibrary songs that the user can edit, rearrange and play back.
    /// </summary>
    /// <remarks>Instances of this class may contain data merged from one or more sources.</remarks>
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