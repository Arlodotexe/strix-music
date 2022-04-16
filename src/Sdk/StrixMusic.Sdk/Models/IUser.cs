// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using StrixMusic.Sdk.BaseModels;
using StrixMusic.Sdk.CoreModels;

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
