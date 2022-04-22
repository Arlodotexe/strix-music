using System;
using System.Collections.Generic;
using StrixMusic.Sdk.AppModels;
using StrixMusic.Sdk.CoreModels;

namespace StrixMusic.Sdk.Tests.Mock.AppModels;

public class MockLyrics : ILyrics
{
    public Dictionary<TimeSpan, string>? TimedLyrics { get; } = new();
    public string? TextLyrics { get; } = string.Empty;
    public bool Equals(ICoreLyrics? other) => false;
    public IReadOnlyList<ICoreLyrics> Sources { get; } = new List<ICoreLyrics>();
    public IReadOnlyList<ICore> SourceCores { get; } = new List<ICore>();
    public ITrack Track { get; } = new MockTrack();
}
