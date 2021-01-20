using System.Diagnostics.CodeAnalysis;
using StrixMusic.Sdk.Data.Base;

namespace StrixMusic.Sdk.Data
{
    /// <inheritdoc cref="IUserProfileBase"/>
    /// <remarks>This interface should be implemented by the Sdk.</remarks>
    [SuppressMessage("ReSharper", "PossibleInterfaceMemberAmbiguity", Justification = "Ambiguity is handled")]
    public interface IUserProfile : IUserProfileBase, IImageCollection, ISdkMember
    {
    }
}
