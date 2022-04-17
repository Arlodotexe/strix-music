using StrixMusic.Sdk.CoreModels;

namespace StrixMusic.Cores.OwlCoreRpc.Tests.Mock.Library
{
    public class MockCorePins : MockCorePlayableCollectionGroupBase
    {
        public MockCorePins(ICore sourceCore)
            : base(sourceCore, nameof(MockCoreLibrary), "Pins")
        {
        }
    }
}
