using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using StrixMusic.Cores.Storage.FileMetadata.Models;
using StrixMusic.Sdk.CoreModels;

namespace StrixMusic.Cores.Storage.Models;

///<inheritdoc/>
public sealed class StorageCoreImage : ICoreImage
{
    private readonly ImageMetadata _imageMetadata;

    /// <summary>
    /// Creates a new instance of <see cref="StorageCoreImage"/>.
    /// </summary>
    /// <param name="sourceCore">The source core.</param>
    /// <param name="imageMetadata">The image metadata to wrap around.</param>
    public StorageCoreImage(ICore sourceCore, ImageMetadata imageMetadata)
    {
        _imageMetadata = imageMetadata;

        Height = imageMetadata.Height;
        Width = imageMetadata.Width;

        SourceCore = sourceCore;
    }

    /// <inheritdoc />
    public ICore SourceCore { get; }

    /// <inheritdoc />
    public async Task<Stream> OpenStreamAsync()
    {
        var fileMetadataManager = ((StorageCore)SourceCore).FileMetadataManager;
        Guard.IsNotNull(fileMetadataManager);
        Guard.IsNotNull(_imageMetadata.Id);

        var stream = await fileMetadataManager.GetImageStreamById(_imageMetadata.Id);
        Guard.IsNotNull(stream);

        return stream;
    }

    /// <inheritdoc />
    public string? MimeType => _imageMetadata.MimeType;

    /// <inheritdoc />
    public double? Height { get; }

    /// <inheritdoc />
    public double? Width { get; }
}
