using System.Collections.Generic;
using System.Linq;
using StrixMusic.Sdk.Interfaces;

namespace StrixMusic.Sdk.MergedWrappers
{
    /// <summary>
    /// A concrete class that merges multiple <see cref="ISearchResults"/>.
    /// </summary>
    public class MergedSearchResults : MergedPlayableCollectionGroupBase, ISearchResults
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MergedSearchResults"/> class.
        /// </summary>
        /// <param name="searchResults">The search results to merge.</param>
        public MergedSearchResults(IEnumerable<ISearchResults> searchResults)
            : base(searchResults.ToArray<IPlayableCollectionGroup>())
        {
        }
    }
}