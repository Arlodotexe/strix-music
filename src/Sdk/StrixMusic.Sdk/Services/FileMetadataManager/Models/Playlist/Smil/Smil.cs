using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace StrixMusic.Sdk.Services.FileMetadataManager.Models.Playlist.Smil
{
    /// <summary>
    /// Represents smil playlists model used for deserialize the xml.
    /// </summary>
    [XmlRoot("smil")]
    public class Smil
    {
        ///<inheritdoc cref="Head"/>
        [XmlElement("head")]
        public Head? Head { get; set; }

        ///<inheritdoc cref="PlaylistMetadata"/>
        [XmlElement("body")]
        public Body? Body { get; set; }
    }
}
