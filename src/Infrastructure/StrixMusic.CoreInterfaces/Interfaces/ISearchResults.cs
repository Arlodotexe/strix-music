using System.Collections.Generic;

namespace StrixMusic.CoreInterfaces.Interfaces
{
    /// <summary>
    /// Interface that represents search results.
    /// </summary>
    public interface ISearchResults : ITrackCollection, IArtistCollection, IAlbumCollection, IPlaylistCollection
    {
    }
}
