using StrixMusic.Sdk.Data.Core;
using System;
using System.Threading.Tasks;

namespace StrixMusic.Sdk.Tests.Mock.Core.Items
{
    public class MockCoreImage : ICoreImage
    {
        public MockCoreImage(ICore sourceCore, Uri uri)
        {
            Uri = uri;
            SourceCore = sourceCore;
        }

        public Uri Uri { get; set; }

        public double Height { get; set; }

        public double Width { get; set; }

        public ICore SourceCore { get; set; }

        public ValueTask DisposeAsync()
        {
            return default;
        }
    }
}