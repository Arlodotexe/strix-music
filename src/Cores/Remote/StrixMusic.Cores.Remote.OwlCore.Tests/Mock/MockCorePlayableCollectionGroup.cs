using StrixMusic.Sdk.Models.Core;

namespace StrixMusic.Cores.Remote.OwlCore.Tests.Mock
{
    public class MockCorePlayableCollectionGroup : MockCorePlayableCollectionGroupBase
    {
        public MockCorePlayableCollectionGroup(ICore sourceCore, string id, string name)
            : base(sourceCore, id, name)
        {
        }
    }
}
