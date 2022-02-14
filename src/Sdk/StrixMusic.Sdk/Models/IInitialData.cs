// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System.Collections.Generic;
using StrixMusic.Sdk.Models.Core;

namespace StrixMusic.Sdk.Models
{
    /// <summary>
    /// A common interface for any data that is created by the user.
    /// </summary>
    public interface IInitialData
    {
        /// <summary>
        /// The created data will be added to any core in this list. If empty, this is decided by the Sdk.
        /// </summary>
        List<ICore>? TargetSourceCores { get; set; }
    }
}