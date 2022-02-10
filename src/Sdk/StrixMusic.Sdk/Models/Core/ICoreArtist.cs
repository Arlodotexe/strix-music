using StrixMusic.Sdk.Models.Base;

namespace StrixMusic.Sdk.Models.Core
{
    /// <summary>
    /// A musician or creator that has published one or more <see cref="ICoreTrack"/>s and <see cref="ICoreAlbum"/>s.
    /// </summary>
    /// <remarks>This interface should be implemented by a core.</remarks>
    public interface ICoreArtist : IArtistBase, ICoreArtistCollectionItem, ICoreAlbumCollection, ICoreTrackCollection, ICoreGenreCollection, ICoreMember
    {
        /// <summary>
        /// A <see cref="IPlayableCollectionGroupBase"/> of items related to this item.
        /// </summary>
        ICorePlayableCollectionGroup? RelatedItems { get; }
    }
}
