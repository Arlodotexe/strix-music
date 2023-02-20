using StrixMusic.Sdk.CoreModels;

namespace StrixMusic.Cores.SoundCloud.Models;

public sealed class SoundCloudGenre : ICoreGenre
{
    public SoundCloudGenre(ICore sourceCore, string name)
    {
        SourceCore = sourceCore;
        Name = name;
    }

    public string Name { get; }

    public ICore SourceCore { get; }
}
