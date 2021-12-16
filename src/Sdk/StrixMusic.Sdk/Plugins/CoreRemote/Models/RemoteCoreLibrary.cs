using StrixMusic.Sdk.Data.Core;

namespace StrixMusic.Sdk.Plugins.CoreRemote.Models
{
    /// <summary>
    /// An external, remotely synchronized implementation of <see cref="ICoreLibrary"/>
    /// </summary>
    public class RemoteCoreLibrary : RemoteCorePlayableCollectionGroupBase, ICoreLibrary
    {
        /// <summary>
        /// Creates a new instance of <see cref="RemoteCoreLibrary"/>. Interacts with a remote core, identified by the given parameters.
        /// </summary>
        /// <param name="sourceCoreInstanceId">The ID of the core that created this instance.</param>
        /// <param name="id">Uniquely identifies the instance being remoted.</param>
        internal RemoteCoreLibrary(string sourceCoreInstanceId, string id)
            : base(sourceCoreInstanceId, id)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="RemoteCoreLibrary"/>. Wraps around the given <paramref name="library"/> for remote interaction.
        /// </summary>
        /// <param name="library">The library to control remotely.</param>
        internal RemoteCoreLibrary(ICoreLibrary library)
            : base(library)
        {
        }
    }
}
