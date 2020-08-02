using System;
using System.Collections.Generic;
using System.Text;

namespace StrixMusic.CoreInterfaces.Interfaces
{
    /// <summary>
    /// Interface that represents a playlist.
    /// </summary>
    public interface IPlaylist : IPlayableCollection
    {
        /// <summary>
        /// List of <see cref="ITrack"/>s that this collection contains.
        /// </summary>
        IList<ITrack> Tracks { get; }
    }
}
