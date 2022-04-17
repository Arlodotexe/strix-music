using StrixMusic.Sdk.CoreModels;

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
