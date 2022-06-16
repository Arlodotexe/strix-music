using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using OwlCore.AbstractStorage;

namespace StrixMusic.Sdk.Tests.Mock.FileSystem
{
    public class InMemoryFolderData : IFolderData
    {
        private List<IFolderData> _folders = new List<IFolderData>();
        private List<IFileData> _files = new List<IFileData>();

        public string? Id { get; set; }

        public string Name { get; set; }

        public InMemoryFolderData(string name)
        {
            Name = name;
        }

        public string Path { get; set; } = string.Empty;

        public Task<IFileData> CreateFileAsync(string desiredName) => CreateFileAsync(desiredName, CreationCollisionOption.FailIfExists);

        public Task<IFileData> CreateFileAsync(string desiredName, CreationCollisionOption options)
        {
            lock (_files)
            {
                var existingFile = _files.FirstOrDefault(x => x.Id == desiredName);

                if (options == CreationCollisionOption.FailIfExists && existingFile is not null)
                    throw new Exception("The file already exists");

                if (options == CreationCollisionOption.OpenIfExists && existingFile is not null)
                    return Task.FromResult(existingFile);

                if (options == CreationCollisionOption.ReplaceExisting && existingFile is not null)
                    _files.Remove(existingFile);

                if (options == CreationCollisionOption.GenerateUniqueName && existingFile is not null)
                    desiredName += DateTime.Now;
            }

            var file = new InMemoryFileData(System.IO.Path.Combine(Path, desiredName), System.IO.Path.GetFileNameWithoutExtension(desiredName), System.IO.Path.GetExtension(desiredName))
            {
                Id = desiredName,
            };

            _files.Add(file);
            return Task.FromResult<IFileData>(file);
        }

        public Task<IFolderData> CreateFolderAsync(string desiredName) => CreateFolderAsync(desiredName, CreationCollisionOption.FailIfExists);

        public Task<IFolderData> CreateFolderAsync(string desiredName, CreationCollisionOption options)
        {
            lock (_folders)
            {
                var existingFolder = _folders.FirstOrDefault(x => x.Id == desiredName);

                if (options == CreationCollisionOption.FailIfExists && existingFolder is not null)
                    throw new Exception("The file already exists");

                if (options == CreationCollisionOption.OpenIfExists && existingFolder is not null)
                    return Task.FromResult(existingFolder);

                if (options == CreationCollisionOption.ReplaceExisting && existingFolder is not null)
                    _folders.Remove(existingFolder);

                if (options == CreationCollisionOption.GenerateUniqueName && existingFolder is not null)
                    desiredName += DateTime.Now;
            }

            var folder = new InMemoryFolderData(desiredName)
            {
                Id = desiredName,
            };

            _folders.Add(folder);
            return Task.FromResult<IFolderData>(folder);
        }

        public Task DeleteAsync() => throw new NotImplementedException();

        public Task EnsureExists() => throw new NotImplementedException();

        public Task<IFileData?> GetFileAsync(string name) => throw new NotImplementedException();

        public Task<IEnumerable<IFileData>> GetFilesAsync() => Task.FromResult<IEnumerable<IFileData>>(_files);

        public Task<IFolderData?> GetFolderAsync(string name) => Task.FromResult(_folders.FirstOrDefault(x => x.Name == name));

        public Task<IEnumerable<IFolderData>> GetFoldersAsync() => Task.FromResult<IEnumerable<IFolderData>>(_folders);

        public Task<IFolderData?> GetParentAsync() => throw new NotImplementedException();
    }
}
