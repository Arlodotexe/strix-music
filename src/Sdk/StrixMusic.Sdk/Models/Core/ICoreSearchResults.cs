using StrixMusic.Sdk.Models.Base;

namespace StrixMusic.Sdk.Models.Core
{
    /// <summary>
    /// The results of a search.
    /// </summary>
    /// <remarks>This interface should be implemented by a core.</remarks>
    public interface ICoreSearchResults : ISearchResultsBase, ICorePlayableCollectionGroup, ICoreMember
    {
    }
}
