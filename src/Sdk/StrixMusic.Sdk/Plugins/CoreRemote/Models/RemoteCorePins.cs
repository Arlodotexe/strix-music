using StrixMusic.Sdk.Data.Core;

namespace StrixMusic.Sdk.Plugins.CoreRemote.Models
{
    /// <summary>
    /// An external, remotely synchronized implementation of the <see cref="ICorePlayableCollectionGroup"/> behind <see cref="ICore.Pins"/>.
    /// </summary>
    public class RemoteCorePins : RemoteCorePlayableCollectionGroupBase
    {
        /// <summary>
        /// Initializes a new instance of <see cref="RemoteCorePins"/>.
        /// </summary>
        /// <param name="sourceCoreInstanceId">The instance ID of the core that created this instance.</param>
        public RemoteCorePins(string sourceCoreInstanceId)
            : base(sourceCoreInstanceId, "Pins")
        {
        }
    }
}
