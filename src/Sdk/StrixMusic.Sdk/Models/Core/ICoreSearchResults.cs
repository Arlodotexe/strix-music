using StrixMusic.Sdk.Models.Base;

namespace StrixMusic.Sdk.Models.Core
{
    /// <inheritdoc cref="ISearchResultsBase"/>
    /// <remarks>This interface should be implemented by a core.</remarks>
    public interface ICoreSearchResults : ISearchResultsBase, ICorePlayableCollectionGroup, ICoreMember
    {
    }
}
