using System.Xml.Serialization;

namespace StrixMusic.Sdk.FileMetadataManagement.Models.Playlist.Smil
{
    /// <summary>
    /// All info regarding the playlist track. 
    /// </summary>
    public class Seq
    {
        /// <summary>
        /// All info regarding the playlist track.
        /// </summary>
        [XmlElement("media")]
        public Media[]? Media { get; set; }
    }
}