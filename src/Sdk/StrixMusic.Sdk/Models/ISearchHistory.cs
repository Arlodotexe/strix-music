using System.Diagnostics.CodeAnalysis;
using StrixMusic.Sdk.Models.Base;
using StrixMusic.Sdk.Models.Core;
using StrixMusic.Sdk.Models.Merged;

namespace StrixMusic.Sdk.Models
{
    /// <inheritdoc cref="ISearchHistoryBase"/>
    /// <remarks>This interface should be implemented by the Sdk.</remarks>
    public interface ISearchHistory : ISearchHistoryBase, IPlayableCollectionGroup, ISdkMember, IMerged<ICoreSearchHistory>
    {
    }
}