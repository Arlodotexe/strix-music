using System.Collections.Generic;
using System.Linq;
using StrixMusic.Sdk.Data.Base;

namespace StrixMusic.Sdk.Data.Merged
{
    /// <summary>
    /// A concrete class that merges multiple <see cref="ILibraryBase"/>.
    /// </summary>
    public class MergedLibrary : MergedPlayableCollectionGroupBase, ILibraryBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MergedLibrary"/> class.
        /// </summary>
        /// <param name="source">The <see cref="ILibraryBase"/> objects to merge.</param>
        public MergedLibrary(IEnumerable<ILibraryBase> source)
            : base(source.ToArray<IPlayableCollectionGroupBase>())
        {
        }
    }
}