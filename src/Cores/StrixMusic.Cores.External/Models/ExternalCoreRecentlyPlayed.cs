using StrixMusic.Sdk.Data.Core;

namespace StrixMusic.Core.External.Models
{
    /// <summary>
    /// The recently played items for the <see cref="ExternalCore"/>.
    /// </summary>
    public class ExternalCoreRecentlyPlayed : ExternalCorePlayableCollectionGroupBase, ICoreRecentlyPlayed
    {
        /// <inheritdoc />
        public ExternalCoreRecentlyPlayed(ICore sourceCore)
            : base(sourceCore, "Recently Played")
        {
        }
    }
}
