using StrixMusic.Sdk.Models.Core;

namespace StrixMusic.Cores.Remote.OwlCore.Tests.Mock.Search
{
    public class MockCoreSearchResults : MockCorePlayableCollectionGroupBase, ICoreSearchResults
    {
        public MockCoreSearchResults(ICore sourceCore, string query)
            : base(sourceCore, nameof(MockCoreSearchResults), $"Search results for {query}")
        {
        }
    }
}
