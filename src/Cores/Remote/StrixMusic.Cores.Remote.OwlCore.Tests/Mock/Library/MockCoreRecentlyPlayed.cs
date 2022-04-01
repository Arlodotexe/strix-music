using StrixMusic.Sdk.Models.Core;

namespace StrixMusic.Cores.Remote.OwlCore.Tests.Mock.Library
{
    public class MockCoreRecentlyPlayed : MockCorePlayableCollectionGroupBase, ICoreRecentlyPlayed
    {
        public MockCoreRecentlyPlayed(ICore sourceCore)
            : base(sourceCore, nameof(MockCoreRecentlyPlayed), "RecentlyPlayed")
        {

        }
    }
}
