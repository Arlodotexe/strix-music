using StrixMusic.Sdk.Models.Base;

namespace StrixMusic.Sdk.Models.Core
{
    /// <inheritdoc cref="IUserBase"/>
    /// <remarks>This interface should be implemented by a core.</remarks>
    public interface ICoreUser : IUserBase, ICoreUserProfile, ICoreMember
    {
        /// <summary>
        /// This user's library.
        /// </summary>
        ICoreLibrary Library { get; }
    }
}
