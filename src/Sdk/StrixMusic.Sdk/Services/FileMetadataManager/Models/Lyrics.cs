using System;
using System.Collections.Generic;
using StrixMusic.Sdk.Data.Base;

namespace StrixMusic.Sdk.Services.FileMetadataManager.Models
{
    /// <summary>
    /// Holds lyrics information from file metadata.
    /// </summary>
    public class Lyrics : ILyricsBase
    {
        ///<inheritdoc />
        public Dictionary<TimeSpan, string>? TimedLyrics { get; }

        ///<inheritdoc />
        public string? TextLyrics { get; }
    }
}
