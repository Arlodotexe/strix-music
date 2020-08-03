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
        IReadOnlyList<IPlayableCollectionBase> Items { get; }
    }
}
