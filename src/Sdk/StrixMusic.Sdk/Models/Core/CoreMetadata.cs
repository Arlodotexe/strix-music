// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System;

namespace StrixMusic.Sdk.Models.Core
{
    /// <summary>
    /// Contains metadata for a registered core. Used to identify a core before instantiation.
    /// </summary>
    public sealed class CoreMetadata
    {
        /// <summary>
        /// Creates a new instance of <see cref="CoreMetadata"/>.
        /// </summary>
        /// <param name="id">A unique identifier for this core, across all instances.</param>
        /// <param name="displayName">The user-friendly name of the core.</param>
        /// <param name="logoUri">A relative path pointing to a SVG file containing the logo for this core.</param>
        /// <param name="sdkVer">The version of the Strix Music SDK that this core was built against.</param>
        public CoreMetadata(string id, string displayName, Uri logoUri, Version sdkVer)
        {
            Id = id;
            LogoUri = logoUri;
            DisplayName = displayName;
            SdkVer = sdkVer;
        }

        /// <summary>
        /// A unique identifier for this registered core, across all instances.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// A relative path pointing to a SVG file containing the logo for this core.
        /// </summary>
        public Uri LogoUri { get; }

        /// <summary>
        /// The user-friendly name for the core.
        /// </summary>
        public string DisplayName { get; }

        /// <summary>
        /// The version of the Strix Music SDK that this core was built against.
        /// </summary>
        public Version SdkVer { get; }
    }
}