using System.Diagnostics.CodeAnalysis;
using StrixMusic.Sdk.Models.Base;

namespace StrixMusic.Sdk.Models
{
    /// <inheritdoc cref="IUserProfileBase"/>
    /// <remarks>This interface should be implemented by the Sdk.</remarks>
    [SuppressMessage("ReSharper", "PossibleInterfaceMemberAmbiguity", Justification = "Ambiguity is handled")]
    public interface IUserProfile : IUserProfileBase, IUrlCollection, IImageCollection, ISdkMember
    {
    }
}
