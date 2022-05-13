using System.Collections.Generic;
using StrixMusic.Sdk.AppModels;
using StrixMusic.Sdk.CoreModels;

namespace StrixMusic.Sdk.Tests.Mock.AppModels;

public class MockSearchHistory : MockPlayableCollectionGroup, ISearchHistory
{
    public bool Equals(ICoreSearchHistory? other) => false;

    public IReadOnlyList<ICoreSearchHistory> Sources { get; } = new List<ICoreSearchHistory>();
}
