using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using OwlCore.Extensions;
using OwlCore.Storage;
using StrixMusic.Sdk.CoreModels;

namespace StrixMusic.CoreModels
{
    /// <summary>
    /// A core image created from a file.
    /// </summary>
    public class CoreFileImage : ICoreImage
    {
        private readonly IFile _file;

        /// <summary>
        /// Initializes a new instance of the <see cref="CoreFileImage"/> class.
        /// </summary>
        /// <param name="file">The image file to open.</param>
        public CoreFileImage(IFile file)
        {
            _file = file;
        }

        /// <inheritdoc/>
        public string MimeType => Path.GetExtension(_file.Name).GetMimeType() ?? ThrowHelper.ThrowArgumentNullException<string>();

        /// <inheritdoc/>
        public double? Height { get; set; }

        /// <inheritdoc/>
        public double? Width { get; set; }

        /// <inheritdoc/>
        public ICore SourceCore { get; set; } = null!;

        /// <inheritdoc/>
        public Task<Stream> OpenStreamAsync() => _file.OpenReadAsync();
    }
}
