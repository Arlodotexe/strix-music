using StrixMusic.Sdk.Models.Base;
using StrixMusic.Sdk.Models.Core;
using StrixMusic.Sdk.Models.Merged;

namespace StrixMusic.Sdk.Models
{
    /// <summary>
    /// Contains a history of playable items which were selected from search results.
    /// </summary>
    /// <remarks>Instances of this class may contain data merged from one or more sources.</remarks>
    public interface ISearchHistory : ISearchHistoryBase, IPlayableCollectionGroup, IPlayable, ISdkMember, IMerged<ICoreSearchHistory>
    {
    }
}