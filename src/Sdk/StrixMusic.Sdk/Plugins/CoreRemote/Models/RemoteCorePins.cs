using StrixMusic.Sdk.Data.Core;

namespace StrixMusic.Sdk.Plugins.CoreRemote.Models
{
    /// <summary>
    /// An external, remotely synchronized implementation of the <see cref="ICorePlayableCollectionGroup"/> behind <see cref="ICore.Pins"/>.
    /// </summary>
    public class RemoteCorePins : RemoteCorePlayableCollectionGroupBase
    {
        /// <summary>
        /// Creates a new instance of <see cref="RemoteCorePins"/>. Interacts with a remote core, identified by the given parameters.
        /// </summary>
        /// <param name="sourceCoreInstanceId">The ID of the core that created this instance.</param>
        /// <param name="remotingId">Uniquely identifies the instance being remoted.</param>
        internal RemoteCorePins(string sourceCoreInstanceId, string remotingId)
            : base(sourceCoreInstanceId, remotingId)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="RemoteCorePins"/>. Wraps around the given <paramref name="pins"/> for remote interaction.
        /// </summary>
        /// <param name="pins">The pins to control remotely.</param>
        /// <param name="remotingId">A unique identifier for this pins instance.</param>
        internal RemoteCorePins(ICorePlayableCollectionGroup pins, string remotingId)
            : base(pins, remotingId)
        {
        }
    }
}
