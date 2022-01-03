using StrixMusic.Sdk.Models.Core;

namespace StrixMusic.Sdk.Tests.Mock.Core.Search
{
    public class MockCoreSearchHistory : MockCorePlayableCollectionGroupBase, ICoreSearchHistory
    {
        public MockCoreSearchHistory(ICore sourceCore)
            : base(sourceCore, nameof(MockCoreSearchHistory), "Search History")
        {
        }
    }
}
