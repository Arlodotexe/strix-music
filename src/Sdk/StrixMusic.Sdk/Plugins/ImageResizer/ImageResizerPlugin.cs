// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System;
using StrixMusic.Sdk.Plugins.Model;

namespace StrixMusic.Sdk.Plugins.ImageResizer;

/// <summary>
/// Allows resizing images on the fly.
/// </summary>
public class ImageResizerPlugin : SdkModelPlugin
{
    private static readonly ModelPluginMetadata _metadata = new(
        id: nameof(ImageResizerPlugin),
        displayName: "Image Resizer",
        description: "Resizes images on the fly",
        new Version(0, 0, 0));

    /// <summary>
    /// Initializes a new instance of the <see cref="ImageResizerPlugin"/> class.
    /// </summary>
    /// <param name="transformSize">A delegate that is called to determine the new sizes for an image. Return a different size than what you're given to force a resize.</param>
    public ImageResizerPlugin(Func<(double? Width, double? Height), (double? Width, double? Height)> transformSize)
        : base(_metadata)
    {
        Image.Add(x => new ResizeableImagePlugin(_metadata, x, transformSize));
    }
}
