using StrixMusic.Sdk.CoreModels;

namespace StrixMusic.Sdk.Tests.Mock.Core
{
    public class MockCoreDiscoverables : MockCorePlayableCollectionGroupBase, ICoreDiscoverables
    {
        public MockCoreDiscoverables(ICore sourceCore)
            : base(sourceCore, nameof(MockCoreDiscoverables), "Discoverables")
        {

        }
    }
}
