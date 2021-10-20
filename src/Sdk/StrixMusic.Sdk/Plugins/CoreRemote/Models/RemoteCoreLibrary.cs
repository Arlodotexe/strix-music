using StrixMusic.Sdk.Data.Core;

namespace StrixMusic.Sdk.Plugins.CoreRemote.Models
{
    /// <summary>
    /// An external, remotely synchronized implementation of <see cref="ICoreLibrary"/>
    /// </summary>
    public class RemoteCoreLibrary : RemoteCorePlayableCollectionGroupBase, ICoreLibrary
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteCoreLibrary"/> class.
        /// </summary>
        /// <param name="sourceCoreInstanceId">The ID of the core that created this instance.</param>
        public RemoteCoreLibrary(string sourceCoreInstanceId)
            : base(sourceCoreInstanceId, "Library")
        {
        }
    }
}
