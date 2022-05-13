using System.Collections.Generic;
using StrixMusic.Sdk.AppModels;
using StrixMusic.Sdk.CoreModels;

namespace StrixMusic.Sdk.Tests.Mock.AppModels;

public class MockLibrary : MockPlayableCollectionGroup, ILibrary
{
    public bool Equals(ICoreLibrary? other) => false;

    public IReadOnlyList<ICoreLibrary> Sources { get; } = new List<ICoreLibrary>();
}
