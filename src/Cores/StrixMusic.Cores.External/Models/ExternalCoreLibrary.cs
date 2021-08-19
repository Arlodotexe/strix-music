using StrixMusic.Sdk.Data.Core;

namespace StrixMusic.Core.External.Models
{
    /// <summary>
    /// An external, remotely synchronized implementation of <see cref="ICoreLibrary"/>
    /// </summary>
    public class ExternalCoreLibrary : ExternalCorePlayableCollectionGroupBase, ICoreLibrary
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExternalCoreLibrary"/> class.
        /// </summary>
        /// <param name="sourceCoreInstanceId">The ID of the core that created this instance.</param>
        public ExternalCoreLibrary(string sourceCoreInstanceId)
            : base(sourceCoreInstanceId, "Library")
        {
        }
    }
}
