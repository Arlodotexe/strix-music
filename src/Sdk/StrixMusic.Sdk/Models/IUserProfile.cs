using StrixMusic.Sdk.Models.Base;

namespace StrixMusic.Sdk.Models
{
    /// <summary>
    /// Describes a generic user profile.
    /// </summary>
    /// <remarks>Instances of this class may contain data merged from one or more sources.</remarks>
    public interface IUserProfile : IUserProfileBase, IUrlCollection, IImageCollection, ISdkMember
    {
    }
}
