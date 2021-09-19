using StrixMusic.Sdk.Data.Core;

namespace StrixMusic.Core.External.Models
{
    /// <summary>
    /// An external, remotely synchronized implementation of the <see cref="ICorePlayableCollectionGroup"/> behind <see cref="ICore.Pins"/>.
    /// </summary>
    public class ExternalCorePins : ExternalCorePlayableCollectionGroupBase
    {
        /// <summary>
        /// Initializes a new instance of <see cref="ExternalCorePins"/>.
        /// </summary>
        /// <param name="sourceCoreInstanceId">The instance ID of the core that created this instance.</param>
        public ExternalCorePins(string sourceCoreInstanceId)
            : base(sourceCoreInstanceId, "Pins")
        {
        }
    }
}
