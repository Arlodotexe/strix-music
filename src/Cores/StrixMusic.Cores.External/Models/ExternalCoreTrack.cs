using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.Toolkit.Diagnostics;
using OwlCore.Collections;
using OwlCore.Events;
using StrixMusic.Core.External.Services;
using StrixMusic.Sdk.Data;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.Extensions;
using StrixMusic.Sdk.MediaPlayback;
using StrixMusic.Sdk.Services.FileMetadataManager;
using StrixMusic.Sdk.Services.FileMetadataManager.Models;

namespace StrixMusic.Core.External.Models
{
    /// <summary>
    /// Wraps around <see cref="TrackMetadata"/> to provide track information extracted from a file to the Strix SDK.
    /// </summary>
    public class ExternalCoreTrack : ICoreTrack
    {
        // re-copy from LocalFilesCore post genre refactor
    }
}
