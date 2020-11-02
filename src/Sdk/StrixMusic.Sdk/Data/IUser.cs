using System.Diagnostics.CodeAnalysis;
using StrixMusic.Sdk.Data.Base;
using StrixMusic.Sdk.Data.Core;

namespace StrixMusic.Sdk.Data
{
    /// <inheritdoc cref="IUserBase"/>
    /// <remarks>This interface should be implemented by a core.</remarks>
    [SuppressMessage("ReSharper", "PossibleInterfaceMemberAmbiguity", Justification = "Ambiguity is handled")]
    public interface IUser : IUserBase, IUserProfile, ISdkMember<ICoreUser>
    {
        /// <summary>
        /// This user's library.
        /// </summary>
        ILibrary Library { get; }
    }
}
