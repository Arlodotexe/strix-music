using StrixMusic.Sdk.Models.Base;

namespace StrixMusic.Sdk.Models.Core
{
    /// <summary>
    /// Describes a generic user profile.
    /// </summary>
    /// <remarks>This interface should be implemented by a core.</remarks>
    public interface ICoreUserProfile : IUserProfileBase, ICoreUrlCollection, ICoreImageCollection, ICoreMember
    {
    }
}
