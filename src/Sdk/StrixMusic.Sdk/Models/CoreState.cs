// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using StrixMusic.Sdk.Models.Core;

namespace StrixMusic.Sdk.Models
{
    /// <summary>
    /// The state of an <see cref="ICore"/>.
    /// </summary>
    public enum CoreState
    {
        /// <summary>
        /// The core is constructed but not initialized.
        /// </summary>
        Unloaded,

        /// <summary>
        /// The core need configuration data and has requested that the setup process be started.
        /// </summary>
        NeedsSetup,

        /// <summary>
        /// The setup process has finished and the core can be initialized.
        /// </summary>
        Configured,

        /// <summary>
        /// The core is currently loading.
        /// </summary>
        Loading,

        /// <summary>
        /// The core has finished initialization.
        /// </summary>
        Loaded,

        /// <summary>
        /// Something has gone wrong and the core may not function properly.
        /// </summary>
        Faulted,
    }
}
