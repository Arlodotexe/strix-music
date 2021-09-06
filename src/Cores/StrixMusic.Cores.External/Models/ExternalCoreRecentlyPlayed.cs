using StrixMusic.Sdk.Data.Core;

namespace StrixMusic.Core.External.Models
{
    /// <summary>
    /// An external, remotely synchronized implementation of <see cref="ICoreRecentlyPlayed"/>
    /// </summary>
    public class ExternalCoreRecentlyPlayed : ExternalCorePlayableCollectionGroupBase, ICoreRecentlyPlayed
    {
        /// <inheritdoc />
        public ExternalCoreRecentlyPlayed(string sourceCoreInstanceId)
            : base(sourceCoreInstanceId, "Recently Played")
        {
        }
    }
}
