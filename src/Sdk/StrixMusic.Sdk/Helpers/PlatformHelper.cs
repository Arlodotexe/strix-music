// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

namespace StrixMusic.Sdk.Helpers
{
    /// <summary>
    /// Static helpers related to the current platform.
    /// </summary>
    public static class PlatformHelper
    {
        /// <summary>
        /// Gets the current running platform.
        /// </summary>
        public static Platform Current { get; internal set; }
    }
}
