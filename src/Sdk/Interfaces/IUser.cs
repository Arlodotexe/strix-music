using System.Collections.Generic;

namespace StrixMusic.Sdk.Interfaces
{
    /// <summary>
    /// Represents a user that is authenticated with a core and interacts with the app.
    /// </summary>
    public interface IUser : IUserProfile
    {
        /// <summary>
        /// This user's library.
        /// </summary>
        ILibrary Library { get; }
    }
}
