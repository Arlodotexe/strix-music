using StrixMusic.Sdk.Models.Base;
using StrixMusic.Sdk.Models.Core;
using StrixMusic.Sdk.Models.Merged;

namespace StrixMusic.Sdk.Models
{
    /// <summary>
    /// A musician or creator that has published one or more <see cref="ITrack"/>s and <see cref="IAlbum"/>s.
    /// </summary>
    /// <remarks>Instances of this class may contain data merged from one or more sources.</remarks>
    public interface IArtist : IArtistBase, IArtistCollectionItem, IAlbumCollection, ITrackCollection, IGenreCollection, IPlayable, ISdkMember, IMerged<ICoreArtist>, IMerged<ICoreArtistCollectionItem>
    {
        /// <summary>
        /// A <see cref="IPlayableCollectionGroup"/> of items related to this item.
        /// </summary>
        IPlayableCollectionGroup? RelatedItems { get; }
    }
}