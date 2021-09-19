using StrixMusic.Sdk.Data.Base;

namespace StrixMusic.Sdk.Data.Core
{
    /// <inheritdoc cref="IUserProfileBase"/>
    /// <remarks>This interface should be implemented by a core.</remarks>
    public interface ICoreUserProfile : IUserProfileBase, ICoreUrlCollection, ICoreImageCollection, ICoreMember
    {
    }
}
