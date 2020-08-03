using System.Collections.Generic;

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
        /// An <see cref="IArtist"/> object that this album was created by.
        /// </summary>
        IArtist Artist { get; }
    }
}
