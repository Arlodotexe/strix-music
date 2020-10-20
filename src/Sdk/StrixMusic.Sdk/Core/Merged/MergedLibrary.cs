using System.Collections.Generic;
using System.Linq;
using StrixMusic.Sdk.Core.Data;

namespace StrixMusic.Sdk.Core.Merged
{
    /// <summary>
    /// A concrete class that merges multiple <see cref="ILibrary"/>.
    /// </summary>
    public class MergedLibrary : MergedPlayableCollectionGroupBase, ILibrary
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MergedLibrary"/> class.
        /// </summary>
        /// <param name="source">The <see cref="ILibrary"/> objects to merge.</param>
        public MergedLibrary(IEnumerable<ILibrary> source)
            : base(source.ToArray<IPlayableCollectionGroup>())
        {
        }
    }
}