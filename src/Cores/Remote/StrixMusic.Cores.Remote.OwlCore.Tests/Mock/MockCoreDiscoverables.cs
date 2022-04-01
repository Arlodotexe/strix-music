using StrixMusic.Sdk.Models.Core;

namespace StrixMusic.Cores.Remote.OwlCore.Tests.Mock
{
    public class MockCoreDiscoverables : MockCorePlayableCollectionGroupBase, ICoreDiscoverables
    {
        public MockCoreDiscoverables(ICore sourceCore)
            : base(sourceCore, nameof(MockCoreDiscoverables), "Discoverables")
        {

        }
    }
}
