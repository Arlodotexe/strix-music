using StrixMusic.Sdk.Models.Core;

namespace StrixMusic.Sdk.Plugins.CoreRemote
{
    /// <summary>
    /// An external, remotely synchronized implementation of <see cref="ICoreSearchResults"/>
    /// </summary>
    public sealed class RemoteCoreSearchResults : RemoteCorePlayableCollectionGroupBase, ICoreSearchResults
    {
        /// <summary>
        /// Creates a new instance of <see cref="RemoteCoreSearchResults"/>.
        /// </summary>
        public RemoteCoreSearchResults(string sourceCoreInstanceId)
            : base(sourceCoreInstanceId, "Search Results")
        {
        }
    }
}
