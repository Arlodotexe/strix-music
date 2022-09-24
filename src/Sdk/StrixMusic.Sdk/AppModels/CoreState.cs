// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using StrixMusic.Sdk.CoreModels;

namespace StrixMusic.Sdk.AppModels
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
        /// The core is performing additional setup required before use.
        /// </summary>
        Loading,

        /// <summary>
        /// The core has finished loading and is ready to be used.
        /// </summary>
        Loaded,
    }
}
