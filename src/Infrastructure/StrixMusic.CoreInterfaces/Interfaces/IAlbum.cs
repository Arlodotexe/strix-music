using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StrixMusic.CoreInterfaces.Interfaces
{
    /// <summary>
    /// Interface representing an album.
    /// </summary>
    public interface IAlbum : IPlayableCollectionBase
    {
        /// <summary>
        /// List of <see cref="ITrack"/>s that this collection contains.
        /// </summary>
        IReadOnlyList<ITrack> Tracks { get; }

        /// <summary>
        /// The total number of songs in this collection.
        /// </summary>
        int TotalTrackCount { get; }

        /// <summary>
        /// An <see cref="IArtist"/> object that this album was created by.
        /// </summary>
        IArtist Artist { get; }

        /// <summary>
        /// The <see cref="Uri"/> of the AlbumCover's image.
        /// </summary>
        Uri? CoverImageUri { get; }

        /// <summary>
        /// Populates a set of <see cref="IAlbum.Tracks"/> into the collection.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task PopulateTracks(int limit, int offset = 0);
    }
}
