using System;
using System.Collections.Generic;
using System.Text;

namespace StrixMusic.CoreInterfaces.Interfaces
{
    /// <summary>
    /// Contains all the data needed to run the app
    /// </summary>
    public interface PretendViewModel : ICommonPlayableCollections
    {
        /// <summary>
        /// A consolidated list of all users in the app
        /// </summary>
        IList<IUser> Users { get; }

        /// <summary>
        /// All available devices.
        /// </summary>
        IList<IDevice> Devices { get; }
    }
}
