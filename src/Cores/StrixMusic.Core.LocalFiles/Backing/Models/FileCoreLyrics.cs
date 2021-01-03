using StrixMusic.Sdk.Data.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace StrixMusic.Core.LocalFiles.Backing.Models
{
    /// <summary>
    /// Holds lyrics information from file metadata.
    /// </summary>
    public class FileCoreLyrics : ILyricsBase
    {
        ///<inheritdoc />
        public Dictionary<TimeSpan, string>? TimedLyrics => throw new NotImplementedException();

        ///<inheritdoc />
        public string? TextLyrics => throw new NotImplementedException();
    }
}
