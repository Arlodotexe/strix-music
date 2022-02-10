using StrixMusic.Sdk.Models.Base;
using StrixMusic.Sdk.Models.Core;
using StrixMusic.Sdk.Models.Merged;

namespace StrixMusic.Sdk.Models
{
    /// <summary>
    /// Relevant items requested with a query from a core.
    /// </summary>
    /// <remarks>Instances of this class may contain data merged from one or more sources.</remarks>
    public interface ISearchResults : ISearchResultsBase, IPlayableCollectionGroup, IPlayable, ISdkMember, IMerged<ICoreSearchResults>
    {
    }
}