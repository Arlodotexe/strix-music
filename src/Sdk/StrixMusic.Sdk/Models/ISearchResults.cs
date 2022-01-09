using System.Diagnostics.CodeAnalysis;
using StrixMusic.Sdk.Models.Base;
using StrixMusic.Sdk.Models.Core;
using StrixMusic.Sdk.Models.Merged;

namespace StrixMusic.Sdk.Models
{
    /// <inheritdoc cref="ISearchResultsBase"/>
    /// <remarks>This interface should be implemented by the Sdk.</remarks>
    public interface ISearchResults : ISearchResultsBase, IPlayableCollectionGroup, IPlayable, ISdkMember, IMerged<ICoreSearchResults>
    {
    }
}