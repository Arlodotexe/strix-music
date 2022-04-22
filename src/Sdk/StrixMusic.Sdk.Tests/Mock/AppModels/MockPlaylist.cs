using System.Collections.Generic;
using StrixMusic.Sdk.AppModels;
using StrixMusic.Sdk.CoreModels;

namespace StrixMusic.Sdk.Tests.Mock.AppModels;

public class MockPlaylist : MockPlayableCollectionGroup, IPlaylist
{
    public bool Equals(ICorePlaylist? other) => false;
    public IReadOnlyList<ICorePlaylist> Sources { get; } = new List<ICorePlaylist>();
    public IUserProfile? Owner { get; }
    public IPlayableCollectionGroup? RelatedItems { get; }
}
