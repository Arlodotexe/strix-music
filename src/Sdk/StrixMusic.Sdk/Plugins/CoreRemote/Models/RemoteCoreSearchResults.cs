using StrixMusic.Sdk.Models.Core;

namespace StrixMusic.Sdk.Plugins.CoreRemote.Models
{
    /// <summary>
    /// An external, remotely synchronized implementation of <see cref="ICoreSearchResults"/>
    /// </summary>
    public class RemoteCoreSearchResults : RemoteCorePlayableCollectionGroupBase, ICoreSearchResults
    {
        /// <summary>
        /// Creates a new instance of <see cref="RemoteCoreSearchResults"/>.
        /// </summary>
        /// <param name="sourceCore">The core that created this object.</param>
        /// <param name="query">The query that was given to produce these results.</param>
        public RemoteCoreSearchResults(string sourceCoreInstanceId)
            : base(sourceCoreInstanceId, "Search Results")
        {
        }
    }
}
