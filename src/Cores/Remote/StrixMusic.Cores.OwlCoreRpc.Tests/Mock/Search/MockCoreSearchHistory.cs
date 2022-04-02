using StrixMusic.Sdk.Models.Core;

namespace StrixMusic.Cores.OwlCoreRpc.Tests.Mock.Search
{
    public class MockCoreSearchHistory : MockCorePlayableCollectionGroupBase, ICoreSearchHistory
    {
        public MockCoreSearchHistory(ICore sourceCore)
            : base(sourceCore, nameof(MockCoreSearchHistory), "Search History")
        {
        }
    }
}
