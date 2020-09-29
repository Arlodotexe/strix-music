using System.Collections.Generic;
using System.Threading.Tasks;

namespace StrixMusic.Sdk.Interfaces
{
    /// <summary>
    /// Represents multiple playable collections that are grouped together under a single context.
    /// </summary>
    public interface IPlayableCollectionGroup : IPlayable, IPlaylistCollection, ITrackCollection, IAlbumCollection, IArtistCollection, IPlayableCollectionGroupChildren
    {
    }
}
