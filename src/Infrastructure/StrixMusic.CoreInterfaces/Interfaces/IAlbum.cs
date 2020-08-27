using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StrixMusic.CoreInterfaces.Interfaces
{
    /// <summary>
    /// Interface representing an album.
    /// </summary>
    public interface IAlbum : ITrackCollection, IRelatedCollectionGroups
    {
        /// <summary>
        /// An <see cref="IArtist"/> object that this album was created by.
        /// </summary>
        IArtist Artist { get; }
    }
}
