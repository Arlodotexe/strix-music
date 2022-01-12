using Newtonsoft.Json;
using StrixMusic.Sdk.Models.Core;

namespace StrixMusic.Sdk.Plugins.CoreRemote.Models
{
    /// <summary>
    /// An external, remotely synchronized implementation of <see cref="ICorePlayableCollectionGroup"/>.
    /// </summary>
    public sealed class RemoteCorePlayableCollectionGroup : RemoteCorePlayableCollectionGroupBase
    {
        /// <summary>
        /// Creates a new instance of <see cref="RemoteCorePlayableCollectionGroup"/>. Interacts with a remote core, identified by the given parameters.
        /// </summary>
        /// <param name="sourceCoreInstanceId">The ID of the core that created this instance.</param>
        /// <param name="id">Uniquely identifies the instance being remoted.</param>
        [JsonConstructor]
        internal RemoteCorePlayableCollectionGroup(string sourceCoreInstanceId, string id)
            : base(sourceCoreInstanceId, id)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="RemoteCorePlayableCollectionGroup"/>. Wraps around the given <paramref name="collection"/> for remote interaction.
        /// </summary>
        /// <param name="collection">The collection to control remotely.</param>
        internal RemoteCorePlayableCollectionGroup(ICorePlayableCollectionGroup collection)
            : base(collection)
        {
        }
    }
}
