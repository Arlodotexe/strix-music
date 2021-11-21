using StrixMusic.Sdk.Data.Core;

namespace StrixMusic.Sdk.Tests.Mock.Core.Library
{
    public class MockCoreRecentlyPlayed : MockCorePlayableCollectionGroupBase, ICoreRecentlyPlayed
    {
        public MockCoreRecentlyPlayed(ICore sourceCore)
            : base(sourceCore, nameof(MockCoreRecentlyPlayed), "RecentlyPlayed")
        {

        }
    }
}
