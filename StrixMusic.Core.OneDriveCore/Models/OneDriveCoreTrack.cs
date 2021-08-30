using StrixMusic.Core.LocalFiles.Models;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.Services.FileMetadataManager.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace StrixMusic.Core.OneDriveCore.Models
{
    /// <summary>
    /// Wraps around <see cref="TrackMetadata"/> to provide track information extracted from a file to the Strix SDK.
    /// </summary>
    public class OneDriveCoreTrack : LocalFilesCoreTrack
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OneDriveCoreTrack"/> class.
        /// </summary>
        /// <param name="sourceCore">The source core.</param>
        /// <param name="trackMetadata">The track metadata to wrap around</param>
        public OneDriveCoreTrack(ICore sourceCore, TrackMetadata trackMetadata) : base(sourceCore, trackMetadata)
        {
        }
    }
}
