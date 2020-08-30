using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StrixMusic.CoreInterfaces.Interfaces
{
    /// <summary>
    /// Interface representing an artist.
    /// </summary>
    public interface IArtist : IAlbumCollection, ITrackCollection
    {
        /// <summary>
        /// A <see cref="IPlayableCollectionGroup"/> of items related to this item.
        /// </summary>
        IPlayableCollectionGroup RelatedItems { get; }
    }
}
