// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System.Xml.Serialization;

namespace StrixMusic.Sdk.FileMetadata.Models.Playlist.Smil
{
    /// <summary>
    /// The meta information regarding playlist.
    /// </summary>
    public class Meta
    {
        /// <summary>
        /// Name of the playlist.
        /// </summary>
        [XmlElement("name")]
        public string? Name { get; set; }

        /// <summary>
        /// The content of the metadata.
        /// </summary>
        [XmlElement("content")]
        public string? Content { get; set; }
    }
}