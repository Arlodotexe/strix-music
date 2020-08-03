using System.Collections.Generic;

namespace StrixMusic.CoreInterfaces.Interfaces
{
    /// <summary>
    /// Interface representing an artist.
    /// </summary>
    public interface IArtist : IPlayableCollectionGroup
    {
        /// <summary>
        /// The Discography for the Artist.
        /// </summary>
        IList<IAlbum> Albums { get; }
    }
}
