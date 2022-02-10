using StrixMusic.Sdk.Models.Base;
using StrixMusic.Sdk.Models.Core;
using StrixMusic.Sdk.Models.Merged;

namespace StrixMusic.Sdk.Models
{
    /// <summary>
    /// Contains details about an image.
    /// </summary>
    /// <remarks>Instances of this class may contain data merged from one or more sources.</remarks>
    public interface IImage : IImageBase, ISdkMember, IMerged<ICoreImage>
    {
    }
}