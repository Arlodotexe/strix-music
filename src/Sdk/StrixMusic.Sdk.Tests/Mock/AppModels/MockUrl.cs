using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StrixMusic.Sdk.AppModels;
using StrixMusic.Sdk.CoreModels;

namespace StrixMusic.Sdk.Tests.Mock.AppModels;

public class MockUrl : IUrl
{
    public ValueTask DisposeAsync() => default;

    public string Label => string.Empty;

    public Uri Url => new("/");

    public UrlType Type  => default;

    public bool Equals(ICoreUrl? other) => false;

    public IReadOnlyList<ICoreUrl> Sources => new List<ICoreUrl>();

    public event EventHandler? SourcesChanged;
}
