using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StrixMusic.Sdk.AppModels;
using StrixMusic.Sdk.CoreModels;

namespace StrixMusic.Sdk.Tests.Mock.AppModels;

public class MockImage : IImage
{
    public ValueTask DisposeAsync()  => default;

    public Uri Uri { get; } = new("https://example.com/");
    public double Height => 0;
    public double Width => 0;

    public bool Equals(ICoreImage? other) => default;

    public IReadOnlyList<ICoreImage> Sources => new List<ICoreImage>();
    public event EventHandler? SourcesChanged;
}
