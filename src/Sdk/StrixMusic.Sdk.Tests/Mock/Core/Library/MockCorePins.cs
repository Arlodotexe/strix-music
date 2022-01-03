using StrixMusic.Sdk.Models.Core;

namespace StrixMusic.Sdk.Tests.Mock.Core.Library
{
    public class MockCorePins : MockCorePlayableCollectionGroupBase
    {
        public MockCorePins(ICore sourceCore)
            : base(sourceCore, nameof(MockCoreLibrary), "Pins")
        {
        }
    }
}
