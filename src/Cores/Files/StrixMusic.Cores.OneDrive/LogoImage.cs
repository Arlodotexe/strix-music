using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using StrixMusic.Sdk.CoreModels;

namespace StrixMusic.Cores.OneDrive
{
    public sealed class LogoImage : ICoreImage
    {
        public LogoImage(ICore sourceCore)
        {
            SourceCore = sourceCore;
        }

        public string? MimeType => "image/svg";

        public double? Height => 659.922;

        public double? Width => 1030.04;

        public ICore SourceCore { get; }

        public Task<Stream> OpenStreamAsync()
        {
            var assembly = Assembly.GetExecutingAssembly();

            return Task.FromResult(assembly.GetManifestResourceStream("StrixMusic.Cores.OneDrive.Logo.svg"));
        }
    }
}
