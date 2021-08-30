using StrixMusic.Core.LocalFiles.Models;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.Services.FileMetadataManager.Models;

namespace StrixMusic.Core.OneDriveCore.Models
{
    /// <summary>
    /// Wraps around <see cref="PlaylistMetadata"/> to provide playlist information extracted from a file to the Strix SDK.
    /// </summary>
    public class OneDriveCorePlaylist : LocalFileCorePlaylist
    {
        /// <summary>
        /// Creates a new instance of <see cref="OneDriveCorePlaylist"/>
        /// </summary>
        public OneDriveCorePlaylist(ICore sourceCore, PlaylistMetadata playlistMetadata)
            : base(sourceCore, playlistMetadata)
        {
        }
    }
}
