using StrixMusic.Core.LocalFiles.Models;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.Services.FileMetadataManager.Models;

namespace StrixMusic.Core.OneDriveCore.Models
{
    /// <summary>
    /// Wraps around <see cref="ArtistMetadata"/> to provide artist information extracted from a file to the Strix SDK.
    /// </summary>
    public class OneDriveCoreArtist : LocalFilesCoreArtist
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OneDriveCoreArtist"/> class.
        /// </summary>
        /// <param name="sourceCore">The source core.</param>
        /// <param name="artistMetadata">The artist metadata to wrap around.</param>
        public OneDriveCoreArtist(ICore sourceCore, ArtistMetadata artistMetadata)
            : base(sourceCore, artistMetadata)
        {
        }
    }
}
