using System;
using System.Collections.Generic;
using StrixMusic.Sdk.BaseModels;

namespace StrixMusic.Cores.Storage.FileMetadata.Models;

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