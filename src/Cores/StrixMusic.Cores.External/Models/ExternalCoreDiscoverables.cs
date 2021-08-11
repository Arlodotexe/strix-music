using StrixMusic.Sdk.Data.Core;

namespace StrixMusic.Core.External.Models
{
    /// <summary>
    /// Discoverable music for the <see cref="ExternalCore"/>.
    /// </summary>
    public class ExternalCoreDiscoverables : ExternalCorePlayableCollectionGroupBase, ICoreDiscoverables
    {
        /// <inheritdoc />
        public ExternalCoreDiscoverables(ICore sourceCore)
            : base(sourceCore, "Discoverables")
        {
        }
    }
}
