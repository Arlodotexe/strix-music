using System.Collections.Generic;
using StrixMusic.Sdk.AppModels;
using StrixMusic.Sdk.CoreModels;

namespace StrixMusic.Sdk.Tests.Mock.AppModels;

public class MockRecentlyPlayed : MockPlayableCollectionGroup, IRecentlyPlayed
{
    public bool Equals(ICoreRecentlyPlayed? other) => false;

    public IReadOnlyList<ICoreRecentlyPlayed> Sources { get; } = new List<ICoreRecentlyPlayed>();
}
