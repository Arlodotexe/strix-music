namespace StrixMusic.Sdk.Core.Data
{
    /// <inheritdoc cref="ISearchResultsBase"/>
    /// <remarks>This interface should be implemented by the Sdk.</remarks>
    public interface ISearchResults : ISearchResultsBase, IPlayableCollectionGroup, ISdkMember
    {
    }
}