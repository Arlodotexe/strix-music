using StrixMusic.Sdk.Data.Base;

namespace StrixMusic.Sdk.Data.Core
{
    /// <inheritdoc cref="ISearchResultsBase"/>
    /// <remarks>This interface should be implemented by a core.</remarks>
    public interface ICoreSearchResults : ISearchResultsBase, ICorePlayableCollectionGroup, ICoreMember
    {
    }
}
