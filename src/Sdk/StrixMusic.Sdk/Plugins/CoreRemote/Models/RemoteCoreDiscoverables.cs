using StrixMusic.Sdk.Data.Core;

namespace StrixMusic.Sdk.Plugins.CoreRemote.Models
{
    /// <summary>
    /// An external, remotely synchronized implementation of <see cref="ICoreDiscoverables"/>
    /// </summary>
    public class RemoteCoreDiscoverables : RemoteCorePlayableCollectionGroupBase, ICoreDiscoverables
    {
        /// <inheritdoc />
        public RemoteCoreDiscoverables(string sourceCoreInstanceId, string remotingId)
            : base(sourceCoreInstanceId, remotingId)
        {
        }
    }
}
