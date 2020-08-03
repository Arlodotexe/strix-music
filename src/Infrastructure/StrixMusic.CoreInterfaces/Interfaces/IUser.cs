using System.Collections.Generic;

namespace StrixMusic.CoreInterfaces.Interfaces
{
    /// <summary>
    /// Represents a user that is authenticated with a core and interacts with the app.
    /// </summary>
    public interface IUser : IUserProfile, ICommonPlayableCollections
    {
        /// <summary>
        /// A list of devices that this user has access to.
        /// </summary>
        IList<IDevice> Devices { get; }
    }
}
