using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StrixMusic.Sdk.AppModels;
using StrixMusic.Sdk.CoreModels;

namespace StrixMusic.Sdk.Tests.Mock.AppModels;

public class MockGenre : IGenre
{
    public ValueTask DisposeAsync() => default;

    public string Name { get; } = Guid.NewGuid().ToString();

    public bool Equals(ICoreGenre? other) => false;

    public IReadOnlyList<ICoreGenre> Sources { get; } = new List<ICoreGenre>();
    public event EventHandler? SourcesChanged;
}
