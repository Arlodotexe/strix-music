using StrixMusic.Sdk.Data.Core;

namespace StrixMusic.Sdk.Tests.Mock.Core.Search
{
    public class MockCoreSearchHistory : MockCorePlayableCollectionGroupBase, ICoreSearchHistory
    {
        public MockCoreSearchHistory(ICore sourceCore)
            : base(sourceCore)
        {
        }
    }
}
