using StrixMusic.Sdk.Data.Base;

namespace StrixMusic.Sdk.Data
{
    /// <inheritdoc cref="IUserProfileBase"/>
    /// <remarks>This interface should be implemented by the Sdk.</remarks>
    public interface IUserProfile : IUserProfileBase, IImageCollection, ISdkMember
    {
    }
}
