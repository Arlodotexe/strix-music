using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

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
        /// <param name="path">The path to check</param>
        /// <returns>A <see cref="FileStream"/> for the file. Null if not found. Throws on error.</returns>
        Task<Stream> GetFileStream(string path);
    }
}
