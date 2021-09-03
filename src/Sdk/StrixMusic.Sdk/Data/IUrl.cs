using StrixMusic.Sdk.Data.Base;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.Data.Merged;

namespace StrixMusic.Sdk.Data
{
    /// <inheritdoc cref="IUrlBase"/>
    /// <remarks>This interface should be implemented in the Sdk.</remarks>
    public interface IUrl : IUrlBase, ISdkMember, IMerged<ICoreUrl>
    {
    }
}