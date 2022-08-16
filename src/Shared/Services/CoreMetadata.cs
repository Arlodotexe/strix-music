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
    public record CoreMetadata(string Id, string DisplayName)
    {
        /// <summary>
        /// The logo for this core, if known.
        /// </summary>
        [JsonIgnore]
        public ICoreImage? Logo { get; set; }
    }
}
