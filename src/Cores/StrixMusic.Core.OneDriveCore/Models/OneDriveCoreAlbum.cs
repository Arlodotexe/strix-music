using StrixMusic.Cores.LocalFiles.Models;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.Services.FileMetadataManager.Models;

namespace StrixMusic.Cores.OneDrive.Models
{
    /// <summary>
    /// Wraps around <see cref="AlbumMetadata"/> to provide album information extracted from a file to the Strix SDK.
    /// </summary>
    public class OneDriveCoreAlbum : LocalFilesCoreAlbum
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OneDriveCoreAlbum"/> class.
        /// </summary>
        /// <param name="sourceCore">The core that created this object.</param>
        /// <param name="albumMetadata">The source album metadata to wrap around.</param>
        public OneDriveCoreAlbum(ICore sourceCore, AlbumMetadata albumMetadata)
            : base(sourceCore, albumMetadata)
        {

        }
    }
}
