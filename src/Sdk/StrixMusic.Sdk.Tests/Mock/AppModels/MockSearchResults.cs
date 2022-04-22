using System.Collections.Generic;
using StrixMusic.Sdk.AppModels;
using StrixMusic.Sdk.CoreModels;

namespace StrixMusic.Sdk.Tests.Mock.AppModels;

public class MockSearchResults : MockPlayableCollectionGroup, ISearchResults
{
    public bool Equals(ICoreSearchResults? other) => false;

    public IReadOnlyList<ICoreSearchResults> Sources { get; } = new List<ICoreSearchResults>();
}
