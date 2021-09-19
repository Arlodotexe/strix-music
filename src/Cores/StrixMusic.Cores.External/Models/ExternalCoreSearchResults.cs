using StrixMusic.Sdk.Data.Core;

namespace StrixMusic.Core.External.Models
{
    /// <summary>
    /// An external, remotely synchronized implementation of <see cref="ICoreSearchResults"/>
    /// </summary>
    public class ExternalCoreSearchResults : ExternalCorePlayableCollectionGroupBase, ICoreSearchResults
    {
        /// <summary>
        /// Creates a new instance of <see cref="ExternalCoreSearchResults"/>.
        /// </summary>
        /// <param name="sourceCore">The core that created this object.</param>
        /// <param name="query">The query that was given to produce these results.</param>
        public ExternalCoreSearchResults(string sourceCoreInstanceId)
            : base(sourceCoreInstanceId, "Search Results")
        {
        }
    }
}
