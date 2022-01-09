using System.Diagnostics.CodeAnalysis;
using StrixMusic.Sdk.Models.Base;
using StrixMusic.Sdk.Models.Core;

namespace StrixMusic.Sdk.Models
{
    /// <inheritdoc cref="IUserBase"/>
    /// <remarks>This interface should be implemented by a core.</remarks>
    public interface IUser : IUserBase, IUserProfile, ISdkMember
    {
        /// <summary>
        /// This user's library.
        /// </summary>
        ILibrary Library { get; }

        /// <summary>
        /// The core that created the <see cref="Source"/>, if any.
        /// </summary>
        ICore? SourceCore { get; set; }

        /// <summary>
        /// The original <see cref="ICoreUser"/> implementation, if any.
        /// </summary>
        ICoreUser? Source { get; set; }
    }
}
