using StrixMusic.Sdk.Data.Core;

namespace StrixMusic.Sdk.Plugins.CoreRemote.Models
{
    /// <summary>
    /// An external, remotely synchronized implementation of <see cref="ICoreRecentlyPlayed"/>
    /// </summary>
    public class RemoteCoreRecentlyPlayed : RemoteCorePlayableCollectionGroupBase, ICoreRecentlyPlayed
    {
        /// <inheritdoc />
        public RemoteCoreRecentlyPlayed(string sourceCoreInstanceId)
            : base(sourceCoreInstanceId, "Recently Played")
        {
        }
    }
}
