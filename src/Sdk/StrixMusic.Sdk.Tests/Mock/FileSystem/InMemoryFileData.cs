using System;
using System.IO;
using System.Threading.Tasks;
using OwlCore.AbstractStorage;

namespace StrixMusic.Sdk.Tests.Mock.FileSystem
{
    public class InMemoryFileData : IFileData
    {
        public Stream _stream = new MemoryStream();

        public InMemoryFileData(string path, string name, string fileExtension)
        {
            Path = path;
            Name = name;
            DisplayName = name;
            FileExtension = fileExtension;
        }

        public string? Id { get; set; }

        public string Path { get; set; }

        public string Name { get; set; }

        public string DisplayName { get; set; }

        public string FileExtension { get; set; }

        public IFileDataProperties Properties { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public Task Delete() => _stream.DisposeAsync().AsTask();

        public Task<IFolderData> GetParentAsync() => throw new NotSupportedException();

        public Task<Stream> GetStreamAsync(FileAccessMode accessMode = FileAccessMode.Read) => Task.FromResult(_stream);

        public Task<Stream> GetThumbnailAsync(ThumbnailMode thumbnailMode, uint requiredSize) => throw new NotImplementedException();

        public Task WriteAllBytesAsync(byte[] bytes) => _stream.WriteAsync(bytes).AsTask();
    }
}
