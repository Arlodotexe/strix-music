using Newtonsoft.Json;
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
        /// <param name="id">Uniquely identifies the instance being remoted.</param>
        [JsonConstructor]
        internal RemoteCoreDiscoverables(string sourceCoreInstanceId, string id)
            : base(sourceCoreInstanceId, id)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="RemoteCoreDiscoverables"/>. Wraps around the given <paramref name="discoverables"/> for remote interaction.
        /// </summary>
        /// <param name="discoverables">The discoverables to control remotely.</param>
        internal RemoteCoreDiscoverables(ICoreDiscoverables discoverables)
            : base(discoverables)
        {
        }
    }
}
