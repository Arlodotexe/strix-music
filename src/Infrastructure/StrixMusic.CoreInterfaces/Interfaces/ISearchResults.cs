using System.Collections.Generic;

namespace StrixMusic.CoreInterfaces.Interfaces
{
    /// <summary>
    /// Interface that represents search results.
    /// </summary>
    public interface ISearchResults
    {
        /// <summary>
        /// The original search query
        /// </summary>
        string SearchQuery { get; }

        /// <summary>
        /// List of <see cref="ITrack"/>s that this collection contains.
        /// </summary>
        IList<ITrack> Tracks { get; }

        /// <summary>
        /// List of <see cref="IAlbum"/>s that this collection contains.
        /// </summary>
        IList<IAlbum> Albums { get; }

        /// <summary>
        /// List of <see cref="IArtist"/>s that this collection contains.
        /// </summary>
        IList<IArtist> Artists { get; }

        /// <summary>
        /// List of <see cref="IPlaylist"/>s that this collection contains.
        /// </summary>
        IList<IPlaylist> Playlists { get; }
    }
}
