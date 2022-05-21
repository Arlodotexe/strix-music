using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OwlCore.AbstractStorage;

namespace StrixMusic.Sdk.Tests.Mock.FileSystem
{
    internal class MockFileData : IFileData
    {
        private string _path;

        public MockFileData(string path)
        {
            _path = path;
        }

        public string? Id { get; }

        public string Path => _path;

        public string Name => System.IO.Path.GetFileName(_path);

        public string DisplayName => System.IO.Path.GetFileName(_path);

        public string FileExtension => System.IO.Path.GetExtension(_path);

        public IFileDataProperties Properties { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public Task Delete() => throw new NotImplementedException();
        public Task<IFolderData> GetParentAsync() => throw new NotImplementedException();
        public Task<Stream> GetStreamAsync(FileAccessMode accessMode = FileAccessMode.Read)
        {
            using var fs = File.Open(_path, FileMode.Open, FileAccess.ReadWrite);

            fs.Position = 0;
            var stream = new MemoryStream();
            fs.CopyTo(stream);

            return Task.FromResult((Stream)stream);
        }
        public Task<Stream> GetThumbnailAsync(ThumbnailMode thumbnailMode, uint requiredSize) => throw new NotImplementedException();
        public async Task WriteAllBytesAsync(byte[] bytes) => await File.WriteAllBytesAsync(_path, bytes);
    }
}
