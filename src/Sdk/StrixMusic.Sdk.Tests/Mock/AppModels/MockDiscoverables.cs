using System.Collections.Generic;
using StrixMusic.Sdk.AppModels;
using StrixMusic.Sdk.CoreModels;

namespace StrixMusic.Sdk.Tests.Mock.AppModels;

public class MockDiscoverables : MockPlayableCollectionGroup, IDiscoverables
{
    public bool Equals(ICoreDiscoverables? other) => false;

    public IReadOnlyList<ICoreDiscoverables> Sources { get; } = new List<ICoreDiscoverables>();
}
