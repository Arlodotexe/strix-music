using StrixMusic.Sdk.Data.Core;

namespace StrixMusic.Sdk.Plugins.CoreRemote.Models
{
    /// <summary>
    /// An external, remotely synchronized implementation of <see cref="ICoreDiscoverables"/>
    /// </summary>
    public class RemoteCoreDiscoverables : RemoteCorePlayableCollectionGroupBase, ICoreDiscoverables
    {
        /// <summary>
        /// Creates a new instance of <see cref="RemoteCoreDiscoverables"/>. Interacts with a remote core, identified by the given parameters.
        /// </summary>
        /// <param name="sourceCoreInstanceId">The ID of the core that created this instance.</param>
        /// <param name="remotingId">Uniquely identifies the instance being remoted.</param>
        internal RemoteCoreDiscoverables(string sourceCoreInstanceId, string remotingId)
            : base(sourceCoreInstanceId, remotingId)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="RemoteCoreDiscoverables"/>. Wraps around the given <paramref name="discoverables"/> for remote interaction.
        /// </summary>
        /// <param name="discoverables">The discoverables to control remotely.</param>
        /// <param name="remotingId">Uniquely identifies the instance being remoted.</param>
        internal RemoteCoreDiscoverables(ICoreDiscoverables discoverables, string remotingId)
            : base(discoverables, remotingId)
        {
        }
    }
}
