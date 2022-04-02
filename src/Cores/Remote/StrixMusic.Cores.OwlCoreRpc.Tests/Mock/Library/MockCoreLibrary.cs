using StrixMusic.Sdk.Models.Core;

namespace StrixMusic.Cores.OwlCoreRpc.Tests.Mock.Library
{
    public class MockCoreLibrary : MockCorePlayableCollectionGroupBase, ICoreLibrary
    {
        public MockCoreLibrary(ICore sourceCore)
            : base(sourceCore, nameof(MockCoreLibrary), "Library")
        {
        }
    }
}
