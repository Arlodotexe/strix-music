using System;
using System.Collections.Generic;
using System.Text;

namespace StrixMusic.CoreInterfaces.Interfaces
{
    /// <summary>
    /// Represents multiple playable collections that are grouped together under a single context.
    /// </summary>
    public interface IPlayableCollectionGroup : IPlayableCollectionBase
    {
        /// <summary>
        /// The <see cref="IPlayableCollectionBase"/>s in this Group
        /// </summary>
        IList<IPlayableCollectionBase> Items { get; }

        /// <summary>
        /// The name of the collection group.
        /// </summary>
        /// <remarks>
        /// Example: <example>Library, Recently Played, Playlist folder name, Daily Mix, etc</example>
        /// </remarks>
        new string Name { get; }
    }
}
