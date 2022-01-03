using StrixMusic.Sdk.Models.Base;
using StrixMusic.Sdk.Models.Core;
using StrixMusic.Sdk.Models.Merged;

namespace StrixMusic.Sdk.Models
{
    /// <inheritdoc cref="IUrlBase"/>
    /// <remarks>This interface should be implemented in the Sdk.</remarks>
    public interface IUrl : IUrlBase, ISdkMember, IMerged<ICoreUrl>
    {
    }
}