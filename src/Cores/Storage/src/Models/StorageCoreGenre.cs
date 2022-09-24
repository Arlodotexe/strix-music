using StrixMusic.Sdk.CoreModels;

namespace StrixMusic.Cores.Storage.Models;

/// <inheritdoc/>
public sealed class StorageCoreGenre : ICoreGenre
{
    /// <summary>
    /// Creates a new instance of <see cref="StorageCoreGenre"/>.
    /// </summary>
    /// <param name="sourceCore">The source core that this instance belongs to.</param>
    /// <param name="genre">The name of the genre.</param>
    public StorageCoreGenre(ICore sourceCore, string genre)
    {
        SourceCore = sourceCore;
        Name = genre;
    }

    /// <inheritdoc/>
    public string Name { get; }

    /// <inheritdoc/>
    public ICore SourceCore { get; }
}