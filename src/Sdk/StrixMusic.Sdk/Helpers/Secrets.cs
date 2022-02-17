﻿// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

namespace StrixMusic.Sdk.Helpers
{
    /// <summary>
    /// A class that holds strings containing sensitive information.
    /// Additional members can be added with more partials at compile time.
    /// </summary>
    public static partial class Secrets
    {
#if DEBUG
        /// <summary>
        /// The Microsoft AppCenter Id used by Strix for analytics and crash reporting.
        /// </summary>
        /// <remarks>
        /// AppCenter will not be initialized if string is null or empty.
        /// </remarks>
        public static string AppCenterId => string.Empty;
#endif
    }
}
