using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using StrixMusic.CoreInterfaces.Interfaces.CoreConfig;
using StrixMusic.Services.StorageService;
using Windows.Storage;
using Windows.Storage.AccessCache;

namespace Strix_Music.Services
{
    /// <inheritdoc/>
    public class FileSystemService : IFileSystemService
    {
        private Dictionary<string, StorageFile> _registeredFiles;

        /// <summary>
        /// Constructs a new <see cref="FileSystemService"/>.
        /// </summary>
        public FileSystemService()
        {
            _registeredFiles = new Dictionary<string, StorageFile>();
        }

        /// <inheritdoc/>
        public Task<Stream> GetFileStream(string id)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task<IMusicFileProperties> GetMusicFileProperties(string id)
        {
            throw new NotImplementedException();
        }

        public async Task RegisterFile(IStorageFile storageFile)
        {
            _registeredFiles.Add(storageFile.);
        }
    }
}
