// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using StrixMusic.Sdk.Models.Base;

namespace StrixMusic.Sdk.Models.Core
{
    /// <summary>
    /// Contains information about a user.
    /// </summary>
    /// <remarks>This interface should be implemented by a core.</remarks>
    public interface ICoreUser : IUserBase, ICoreUserProfile, ICoreMember
    {
        /// <summary>
        /// This user's library.
        /// </summary>
        ICoreLibrary Library { get; }
    }
}
