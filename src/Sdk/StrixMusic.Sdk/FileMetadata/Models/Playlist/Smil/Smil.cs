using StrixMusic.Sdk.FileMetadata.Models;
using System.Xml.Serialization;

namespace StrixMusic.Sdk.FileMetadata.Models.Playlist.Smil
{
    /// <summary>
    /// <see cref="Smil"/> playlist model used for deserialization.
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
