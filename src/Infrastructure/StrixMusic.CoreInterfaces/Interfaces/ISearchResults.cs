using System;
using System.Collections.Generic;

namespace StrixMusic.CoreInterfaces.Interfaces
{
    /// <summary>
    /// Interface that represents search results.
    /// </summary>
    public interface ISearchResults
    {
        /// <summary>
        /// The tracks returned from the search results.
        /// </summary>
        ITrackCollection Tracks { get; }

        /// <summary>
        /// The artists returned from search results.
        /// </summary>
        IArtistCollection Artists { get; }

        /// <summary>
        /// The albums returned from search results.
        /// </summary>
        IAlbumCollection Albums { get; }

        /// <summary>
        /// The playlists returned from search results.
        /// </summary>
        IPlaylistCollection Playlists { get; }

        /// <summary>
        /// The total number of pages of results.
        /// </summary>
        int NumberOfPages { get; }

        /// <summary>
        /// On page changed event
        /// </summary>
        event EventHandler<int> OnPageChanged;
    }
}
