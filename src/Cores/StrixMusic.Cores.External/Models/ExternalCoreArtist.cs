using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Toolkit.Diagnostics;
using OwlCore.Events;
using StrixMusic.Core.External.Services;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.Extensions;
using StrixMusic.Sdk.MediaPlayback;
using StrixMusic.Sdk.Services.FileMetadataManager;
using StrixMusic.Sdk.Services.FileMetadataManager.Models;

namespace StrixMusic.Core.External.Models
{
    /// <summary>
    /// Wraps around <see cref="ArtistMetadata"/> to provide artist information extracted from a file to the Strix SDK.
    /// </summary>
    public class ExternalCoreArtist : ICoreArtist
    {
        // re-copy from LocalFilesCore post genre refactor
    }
}
