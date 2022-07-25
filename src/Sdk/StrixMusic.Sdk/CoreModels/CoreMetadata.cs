// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System;
using System.Text.Json.Serialization;

namespace StrixMusic.Sdk.CoreModels
{
    /// <summary>
    /// Contains metadata for a registered core. Used to identify a core before instantiation.
    /// </summary>
    /// <param name="Id">A unique identifier for this core, across all instances.</param>
    /// <param name="DisplayName">The user-friendly name of the core.</param>
    /// <param name="LogoUri">A relative path pointing to a SVG file containing the logo for this core.</param>
    /// <param name="SdkVer">The version of the Strix Music SDK that this core was built against.</param>
    public record CoreMetadata(string Id, string DisplayName, Uri LogoUri, Version SdkVer);
}
