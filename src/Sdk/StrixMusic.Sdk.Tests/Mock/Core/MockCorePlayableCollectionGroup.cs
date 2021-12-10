using StrixMusic.Sdk.Data.Core;

namespace StrixMusic.Sdk.Tests.Mock.Core
{
    public class MockCorePlayableCollectionGroup : MockCorePlayableCollectionGroupBase
    {
        public MockCorePlayableCollectionGroup(ICore sourceCore, string id, string name)
            : base(sourceCore, id, name)
        {
        }
    }
}
