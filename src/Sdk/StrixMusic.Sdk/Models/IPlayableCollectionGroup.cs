using StrixMusic.Sdk.Models.Base;
using StrixMusic.Sdk.Models.Core;
using StrixMusic.Sdk.Models.Merged;

namespace StrixMusic.Sdk.Models
{
    /// <summary>
    /// Multiple playable collections that are grouped together under a single context.
    /// </summary>
    /// <remarks>Instances of this class may contain data merged from one or more sources.</remarks>
    public interface IPlayableCollectionGroup : IPlayableCollectionGroupBase, IPlaylistCollection, ITrackCollection, IAlbumCollection, IArtistCollection, IPlayableCollectionGroupChildren, IPlayable, ISdkMember, IMerged<ICorePlayableCollectionGroup>
    {
    }
}