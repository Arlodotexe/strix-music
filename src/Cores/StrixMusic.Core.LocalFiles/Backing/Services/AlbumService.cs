﻿using MessagePack;
using Microsoft.Toolkit.Diagnostics;
using OwlCore.AbstractStorage;
using OwlCore.Provisos;
using StrixMusic.Core.LocalFiles.Backing.Models;
using StrixMusic.Core.LocalFiles.MetadataScanner;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrixMusic.Core.LocalFiles.Backing.Services
{
    /// <summary>
    /// Album service for creating or getting the album metadata.
    /// </summary>
    public class AlbumService : IAsyncInit
    {
        private readonly string _albumMetadataCacheFileName = "AlbumMeta.lfc"; //lfc represents LocalFileCore format.
        private readonly string _pathToMetadatafile;
        private readonly IFileSystemService _fileSystemService;
        private IFolderData? _folderData;

        /// <summary>
        /// Creates a new instance of <see cref="AlbumService"/>.
        /// </summary>
        ///  /// <param name="fileSystemService">The service to access the file system.</param>
        public AlbumService(IFileSystemService fileSystemService)
        {
            _fileSystemService = fileSystemService;
            _pathToMetadatafile = $"{_fileSystemService.RootFolder.Path}\\{_albumMetadataCacheFileName}";
        }

        /// <summary>
        /// Initializes the album service.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task InitAsync()
        {
            var folders = await _fileSystemService.GetPickedFolders();
            Guard.IsNotNull(folders, nameof(folders));
            _folderData = folders.ToList().FirstOrDefault();
            Guard.IsNotNull(_folderData, nameof(_folderData));
        }

        /// <summary>
        /// Gets all <see cref="AlbumMetadata"/>> over the file system.
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="limit"></param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task<IReadOnlyList<AlbumMetadata>> GetAlbumMetadata(int offset, int limit)
        {
            if (!File.Exists(_pathToMetadatafile))
                throw new FileNotFoundException(_pathToMetadatafile);

            var bytes = File.ReadAllBytes(_pathToMetadatafile);
            var albumMetadataLst = MessagePackSerializer.Deserialize<IReadOnlyList<AlbumMetadata>>(bytes, MessagePack.Resolvers.ContractlessStandardResolver.Options);

            return Task.FromResult<IReadOnlyList<AlbumMetadata>>(albumMetadataLst.Skip(offset).Take(limit).ToList());
        }

        /// <summary>
        /// Create or Update <see cref="AlbumMetadata"/> information in files.
        /// </summary>
        /// <returns>The <see cref="AlbumMetadata"/> collection.</returns>
        public async Task CreateOrUpdateAlbumMetadata()
        {
            if (_folderData is null)
                return;

            if (!await _fileSystemService.FileExistsAsync(_pathToMetadatafile))
                File.Create(_pathToMetadatafile).Close(); // creates the file and closes the file stream.

            throw new NotImplementedException();
        }
    }
}
