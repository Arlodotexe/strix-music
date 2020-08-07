using System.IO;
using System.Threading.Tasks;
using StrixMusic.CoreInterfaces.Interfaces.CoreConfig;

namespace StrixMusic.Services.StorageService
{
    /// <summary>
    /// Provides safe interactions with the file system.
    /// </summary>
    public interface IFileSystemService
    {
        /// <remarks>No implementation has been made yet.</remarks>
        /// <summary>
        /// Gets the file stream for a path.
        /// </summary>
        /// <param name="id">A unique file identifier.</param>
        /// <returns>A <see cref="FileStream"/> for the file. Null if not found. Throws on error.</returns>
        Task<Stream> GetFileStream(string id);

        /// <summary>
        /// Get the <see cref="IMusicFileProperties"/> for a given <paramref name="id"/>.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>A <see cref="Task"/> representing the asyncronous operation, with a result of <see cref="IMusicFileProperties"/></returns>
        Task<IMusicFileProperties> GetMusicFileProperties(string id);
    }
}
