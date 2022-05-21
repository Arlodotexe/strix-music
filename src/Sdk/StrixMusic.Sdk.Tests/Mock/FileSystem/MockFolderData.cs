using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OwlCore.AbstractStorage;

namespace StrixMusic.Sdk.Tests.Mock.FileSystem
{
    public class MockFolderData : IFolderData
    {
        private readonly string _directoryPath;

        /// <summary>
        /// Creates new instance of <see cref="MockFolderData"/>.
        /// </summary>
        /// <param name="path"></param>
        public MockFolderData(string directory)
        {
            _directoryPath = directory;
        }

        public string? Id { get; }

        public string Name => System.IO.Path.GetDirectoryName(_directoryPath) ?? string.Empty;

        public string Path => _directoryPath;

        public Task<IFileData> CreateFileAsync(string desiredName)
        {
            var fileStream = File.Create($"{_directoryPath}\\{desiredName}");
            fileStream.Dispose();
            fileStream.Close();

            return Task.FromResult<IFileData>(new MockFileData($"{_directoryPath}\\{desiredName}"));
        }
        public Task<IFileData> CreateFileAsync(string desiredName, CreationCollisionOption options)
        {
            var fileStream = File.Create($"{_directoryPath}\\{desiredName}");
            fileStream.Dispose();
            fileStream.Close();

            return Task.FromResult<IFileData>(new MockFileData($"{_directoryPath}\\{desiredName}"));
        }
        public Task<IFolderData> CreateFolderAsync(string desiredName) => throw new NotImplementedException();
        public Task<IFolderData> CreateFolderAsync(string desiredName, CreationCollisionOption options)
        {
            Directory.CreateDirectory($"{_directoryPath}\\{desiredName}");

            return Task.FromResult<IFolderData>(new MockFolderData($"{_directoryPath}\\{desiredName}"));
        }
        public Task DeleteAsync() => throw new NotImplementedException();
        public Task EnsureExists() => throw new NotImplementedException();
        public Task<IFileData?> GetFileAsync(string name)
        {
            var files = Directory.GetFiles(_directoryPath);
            foreach (var item in files)
            {
                if (item == name)
                {
                    var fileData = new MockFileData($"{_directoryPath}\\{System.IO.Path.GetFileName(item)}");
                    return Task.FromResult<IFileData?>(fileData);
                }
            }

            return Task.FromResult<IFileData?>(null);
        }
        public Task<IEnumerable<IFileData>> GetFilesAsync()
        {
            var files = Directory.GetFiles(_directoryPath);
            var list = new List<IFileData>();

            foreach (var item in files)
            {
                var fileData = new MockFileData($"{_directoryPath}\\{System.IO.Path.GetFileName(item)}");

                list.Add(fileData);
            }

            return Task.FromResult<IEnumerable<IFileData>>(list);
        }
        public Task<IFolderData?> GetFolderAsync(string name)
        {
            var directories = Directory.GetDirectories(_directoryPath);

            foreach (var item in directories)
            {
                if (item == name)
                {
                    var folderData = new MockFolderData($"{_directoryPath}\\{name}");
                    return Task.FromResult<IFolderData?>(folderData);
                }
            }

            return Task.FromResult<IFolderData?>(null);
        }
        public Task<IEnumerable<IFolderData>> GetFoldersAsync()
        {
            var directories = Directory.GetDirectories(_directoryPath);

            var list = new List<IFolderData>();

            foreach (var item in directories)
            {
                var fileData = new MockFolderData($"{_directoryPath}\\{item}");

                list.Add(fileData);
            }

            return Task.FromResult<IEnumerable<IFolderData>>(list);
        }
        public Task<IFolderData?> GetParentAsync()
        {
            var parent = Directory.GetParent(_directoryPath)?.FullName;

            if (parent == null)
                throw new DirectoryNotFoundException();

            return Task.FromResult<IFolderData?>(new MockFolderData(parent));
        }
    }
}
