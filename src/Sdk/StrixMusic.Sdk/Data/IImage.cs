using StrixMusic.Sdk.Data.Base;
using StrixMusic.Sdk.Data.Core;

namespace StrixMusic.Sdk.Data
{
    /// <inheritdoc cref="IImageBase"/>
    /// <remarks>This interface should be implemented in the Sdk.</remarks>
    public interface IImage : IImageBase, ISdkMember<ICoreImage>
    {
    }
}