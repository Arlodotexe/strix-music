using System.Threading.Tasks;
using OwlCore.AbstractStorage;

namespace StrixMusic.Cores.OneDrive.Storage
{
    /// <inheritdoc />
    public class OneDriveFileDataProperties : IFileDataProperties
    {
        /// <inheritdoc />
        public async Task<MusicFileProperties?> GetMusicPropertiesAsync()
        {
            // TODO. The Graph API supplies these.
            return null;
        }
    }
}