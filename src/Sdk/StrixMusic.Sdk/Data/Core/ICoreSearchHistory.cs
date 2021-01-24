using StrixMusic.Sdk.Data.Base;

namespace StrixMusic.Sdk.Data.Core
{
    /// <summary>
    /// Contains items that the user has recently selected from the search results.
    /// </summary>
    /// <remarks>This interface should be implemented by a core.</remarks>
    public interface ICoreSearchHistory: ISearchHistoryBase, ICorePlayableCollectionGroup, ICoreMember
    {
    }
}