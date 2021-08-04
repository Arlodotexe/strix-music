using StrixMusic.Sdk.Data.Core;

namespace StrixMusic.Core.External.Models
{
    /// <inheritdoc cref="ICoreLibrary"/>
    public class ExternalCoreLibrary : ExternalCorePlayableCollectionGroupBase, ICoreLibrary
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExternalCoreLibrary"/> class.
        /// </summary>
        /// <param name="sourceCore">The core that created this instance.</param>
        public ExternalCoreLibrary(ICore sourceCore)
            : base(sourceCore)
        {
        }
    }
}
