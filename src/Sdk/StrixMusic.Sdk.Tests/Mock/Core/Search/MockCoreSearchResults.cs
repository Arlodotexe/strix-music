using StrixMusic.Sdk.Models.Core;

namespace StrixMusic.Sdk.Tests.Mock.Core.Search
{
    public class MockCoreSearchResults : MockCorePlayableCollectionGroupBase, ICoreSearchResults
    {
        public MockCoreSearchResults(ICore sourceCore, string query)
            : base(sourceCore, nameof(MockCoreSearchResults), $"Search results for {query}")
        {
        }
    }
}
