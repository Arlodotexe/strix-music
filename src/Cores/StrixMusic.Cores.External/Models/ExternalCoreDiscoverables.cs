using StrixMusic.Sdk.Data.Core;

namespace StrixMusic.Core.External.Models
{
    /// <summary>
    /// An external, remotely synchronized implementation of <see cref="ICoreDiscoverables"/>
    /// </summary>
    public class ExternalCoreDiscoverables : ExternalCorePlayableCollectionGroupBase, ICoreDiscoverables
    {
        /// <inheritdoc />
        public ExternalCoreDiscoverables(string sourceCoreInstanceId)
            : base(sourceCoreInstanceId, "Discoverables")
        {
        }
    }
}
