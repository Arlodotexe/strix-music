﻿using System.Xml.Serialization;

namespace StrixMusic.Sdk.Services.FileMetadataManager.Models.Playlist.Smil
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