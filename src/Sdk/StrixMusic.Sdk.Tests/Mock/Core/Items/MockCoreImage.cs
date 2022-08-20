using System;
using System.IO;
using System.Threading.Tasks;
using StrixMusic.Sdk.CoreModels;

namespace StrixMusic.Sdk.Tests.Mock.Core.Items
{
    public class MockCoreImage : ICoreImage
    {
        public MockCoreImage(ICore sourceCore)
        {
            SourceCore = sourceCore;
        }

        public Task<Stream> OpenStreamAsync() => throw new NotImplementedException();

        public string? MimeType { get; }
        public double? Height { get; set; }

        public double? Width { get; set; }

        public ICore SourceCore { get; set; }
    }
}
