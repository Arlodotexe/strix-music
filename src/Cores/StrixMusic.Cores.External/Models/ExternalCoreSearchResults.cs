using StrixMusic.Sdk.Data.Core;

namespace StrixMusic.Core.External.Models
{
    /// <summary>
    /// A LocalFileCore implementation of <see cref="ICoreSearchResults"/>.
    /// </summary>
    public class ExternalCoreSearchResults : ExternalCorePlayableCollectionGroupBase, ICoreSearchResults
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExternalCoreSearchResults"/> class.
        /// </summary>
        /// <param name="sourceCore">The core that created this object.</param>
        /// <param name="query">The query that was given to produce these results.</param>
        public ExternalCoreSearchResults(ICore sourceCore, string query)
            : base(sourceCore, "Search Results")
        {
        }
    }
}
