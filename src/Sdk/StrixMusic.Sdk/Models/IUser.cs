using StrixMusic.Sdk.Models.Base;
using StrixMusic.Sdk.Models.Core;

namespace StrixMusic.Sdk.Models
{
    /// <summary>
    /// Contains information about a user.
    /// </summary>
    /// <remarks>Instances of this class may contain data merged from one or more sources.</remarks>
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
