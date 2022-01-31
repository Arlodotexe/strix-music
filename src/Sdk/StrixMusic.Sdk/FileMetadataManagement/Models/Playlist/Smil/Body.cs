using System.Xml.Serialization;

namespace StrixMusic.Sdk.FileMetadataManagement.Models.Playlist.Smil
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