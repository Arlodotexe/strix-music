using StrixMusic.Sdk.Models.Core;

namespace StrixMusic.Sdk.Tests.Mock.Core.Library
{
    public class MockCoreLibrary : MockCorePlayableCollectionGroupBase, ICoreLibrary
    {
        public MockCoreLibrary(ICore sourceCore)
            : base(sourceCore, nameof(MockCoreLibrary), "Library")
        {
        }
    }
}
