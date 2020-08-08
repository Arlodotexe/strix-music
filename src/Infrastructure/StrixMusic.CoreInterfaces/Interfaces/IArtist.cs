using System.Collections.Generic;
using System.Threading.Tasks;

namespace StrixMusic.CoreInterfaces.Interfaces
{
    /// <summary>
    /// Interface representing an artist.
    /// </summary>
    public interface IArtist : IAlbumCollection, IArtistCollection, ITrackCollection
    {
    }
}
