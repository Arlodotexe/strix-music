using StrixMusic.Sdk.CoreModels;

namespace StrixMusic.Cores.OwlCoreRpc.Tests.Mock.Library
{
    public class MockCoreRecentlyPlayed : MockCorePlayableCollectionGroupBase, ICoreRecentlyPlayed
    {
        public MockCoreRecentlyPlayed(ICore sourceCore)
            : base(sourceCore, nameof(MockCoreRecentlyPlayed), "RecentlyPlayed")
        {

        }
    }
}
