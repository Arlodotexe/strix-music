using StrixMusic.Sdk.Data.Core;

namespace StrixMusic.Sdk.Plugins.CoreRemote.Models
{
    /// <summary>
    /// An external, remotely synchronized implementation of <see cref="ICorePlayableCollectionGroup"/>.
    /// </summary>
    public class RemoteCorePlayableCollectionGroup : RemoteCorePlayableCollectionGroupBase
    {
        /// <summary>
        /// Creates a new instance of <see cref="RemoteCorePlayableCollectionGroup"/>. Interacts with a remote core, identified by the given parameters.
        /// </summary>
        /// <param name="sourceCoreInstanceId">The ID of the core that created this instance.</param>
        /// <param name="remotingId">Uniquely identifies the instance being remoted.</param>
        internal RemoteCorePlayableCollectionGroup(string sourceCoreInstanceId, string remotingId)
            : base(sourceCoreInstanceId, remotingId)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="RemoteCorePlayableCollectionGroup"/>. Wraps around the given <paramref name="collection"/> for remote interaction.
        /// </summary>
        /// <param name="collection">The collection to control remotely.</param>
        /// <param name="remotingId">Uniquely identifies the instance being remoted.</param>
        internal RemoteCorePlayableCollectionGroup(ICorePlayableCollectionGroup collection, string remotingId)
            : base(collection, remotingId)
        {
        }
    }
}
