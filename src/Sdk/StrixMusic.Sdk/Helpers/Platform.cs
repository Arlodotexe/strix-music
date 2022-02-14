// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

namespace StrixMusic.Sdk.Helpers
{
    /// <summary>
    /// The platforms that Strix can run on.
    /// </summary>
    public enum Platform
    {
        /// <summary>
        /// Represents a platform that is not yet known.
        /// </summary>
        Unknown,

        /// <summary>
        /// Represents a Universal Windows Platform app.
        /// </summary>
        UWP,

        /// <summary>
        /// Represents a WebAssembly app.
        /// </summary>
        WASM,

        /// <summary>
        /// Indicates an Android app.
        /// </summary>
        Droid,
    }
}
