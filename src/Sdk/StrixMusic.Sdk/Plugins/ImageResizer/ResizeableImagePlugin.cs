// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System;
using System.IO;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using StrixMusic.Sdk.Plugins.Model;
using IImage = StrixMusic.Sdk.AppModels.IImage;

namespace StrixMusic.Sdk.Plugins.ImageResizer;

/// <summary>
/// If the provided method returns a different size, the image is resized when the stream is opened.
/// </summary>
public class ResizeableImagePlugin : ImagePluginBase
{
    private readonly Func<(double? Width, double? Height), (double? Width, double? Height)> _transformSize;

    /// <summary>
    /// Initializes a new instance of <see cref="ResizeableImagePlugin"/>.
    /// </summary>
    /// <param name="metadata">Contains metadata for a plugin. Used to identify a plugin before instantiation.</param>
    /// <param name="inner">An implementation which member access is delegated to, unless the member is overridden in a derived class which changes the behavior.</param>
    /// <param name="transformSize">A delegate that is called to determine the new sizes for an image. Return a different size than what you're given to force a resize.</param>
    public ResizeableImagePlugin(ModelPluginMetadata metadata, IImage inner, Func<(double? Width, double? Height), (double? Width, double? Height)> transformSize)
        : base(metadata, inner)
    {
        _transformSize = transformSize;
    }

    /// <summary>
    /// The transformed height, or the original height.
    /// </summary>
    public override double? Height => _transformSize((base.Width, base.Height)).Height;

    /// <summary>
    /// The transformed height, or the original height.
    /// </summary>
    public override double? Width => _transformSize((base.Width, base.Height)).Width;

    /// <summary>
    /// Opens a stream to the image. If needed, the image is resized on the fly.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous result. The value is a stream to the image, resized if needed.</returns>
    public override async Task<Stream> OpenStreamAsync()
    {
        // If we don't need to resize, just return the data.
        var newSize = _transformSize((base.Width, base.Height));

        var isUnchanged = Equals(newSize.Height, base.Height) && Equals(newSize.Width, base.Width);

        if (isUnchanged || newSize.Height is null || newSize.Width is null)
            return await base.OpenStreamAsync();

        // Otherwise, open the underlying stream and resize the image to the desired size.
        using var actualStream = await base.OpenStreamAsync();
        actualStream.Position = 0;

        using var image = await Image.LoadAsync(actualStream);
        image.Mutate(x => x.Resize((int)newSize.Width, (int)newSize.Height));

        var memoryStream = new MemoryStream();
        await image.SaveAsJpegAsync(memoryStream);

        memoryStream.Position = 0;
        return memoryStream;
    }
}
