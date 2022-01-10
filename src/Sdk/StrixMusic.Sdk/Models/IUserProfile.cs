using System.Diagnostics.CodeAnalysis;
using StrixMusic.Sdk.Models.Base;

namespace StrixMusic.Sdk.Models
{
    /// <inheritdoc cref="IUserProfileBase"/>
    /// <remarks>This interface should be implemented by the Sdk.</remarks>
    public interface IUserProfile : IUserProfileBase, IUrlCollection, IImageCollection, ISdkMember
    {
    }
}
