// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System.Xml.Serialization;

namespace StrixMusic.Sdk.FileMetadata.Models.Playlist.Smil
{
    /// <summary>
    /// Holds information regarding playlist sequences.
    /// </summary>
    public class Body
    {
        ///<inheritdoc cref="Seq"/>
        [XmlElement("seq")]
        public Seq? Seq { get; set; }
    }
}