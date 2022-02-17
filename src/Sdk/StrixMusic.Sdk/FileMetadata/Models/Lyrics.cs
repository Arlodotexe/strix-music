// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System;
using System.Collections.Generic;
using StrixMusic.Sdk.Models.Base;

namespace StrixMusic.Sdk.FileMetadata.Models
{
    /// <summary>
    /// Holds lyrics information from file metadata.
    /// </summary>
    public sealed class Lyrics : ILyricsBase
    {
        ///<inheritdoc />
        public Dictionary<TimeSpan, string>? TimedLyrics { get; }

        ///<inheritdoc />
        public string? TextLyrics { get; }
    }
}
