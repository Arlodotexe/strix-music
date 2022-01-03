using StrixMusic.Sdk.Models.Base;

namespace StrixMusic.Sdk.Models.Core
{
    /// <inheritdoc cref="IUserProfileBase"/>
    /// <remarks>This interface should be implemented by a core.</remarks>
    public interface ICoreUserProfile : IUserProfileBase, ICoreUrlCollection, ICoreImageCollection, ICoreMember
    {
    }
}
