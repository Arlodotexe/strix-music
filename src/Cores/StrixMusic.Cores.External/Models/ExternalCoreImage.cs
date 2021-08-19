using System;
using System.Threading.Tasks;
using StrixMusic.Sdk.Data.Core;

namespace StrixMusic.Core.External.Models
{
    /// <summary>
    /// An external, remotely synchronized implementation of <see cref="ICoreImage"/>
    /// </summary>
    public class ExternalCoreImage : ICoreImage, IAsyncInit
    {
        /// <summary>
        /// Creates a new instance of <see cref="ExternalCoreImage"/>.
        /// </summary>
        /// <param name="sourceCore"><inheritdoc cref="ICoreMember.SourceCore"/></param>
        /// <param name="uri">A <see cref="System.Uri"/> pointing to the image file.</param>
        /// <param name="width">The width of the image.</param>
        /// <param name="height">The height of the image.</param>
        public ExternalCoreImage(ICore sourceCore, Uri uri, double width, double height)
        {
            SourceCore = sourceCore;
            Uri = uri;
            Width = width;
            Height = height;
        }

        /// <inheritdoc />
        public ICore SourceCore { get; }

        /// <inheritdoc />
        public Uri Uri { get; }

        /// <inheritdoc />
        public double Height { get; }

        /// <inheritdoc />
        public double Width { get; }

        /// <inheritdoc />
        public ValueTask DisposeAsync()
        {
            return default;
        }
    }
}
