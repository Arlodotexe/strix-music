// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System;
using System.IO;
using System.Threading.Tasks;

namespace StrixMusic.Sdk.BaseModels
{
    /// <summary>
    /// Contains details about an image.
    /// </summary>
    public interface IImageBase : ICollectionItemBase
    {
        /// <summary>
        /// Opens a stream to the image resource.
        /// </summary>
        /// <returns>A Task containing a Stream of the raw image resource.</returns>
        Task<Stream> OpenStreamAsync();

        /// <summary>
        /// The mime type of the image, if known. A hint to help optimize image rendering.
        /// </summary>
        string? MimeType { get; }

        /// <summary>
        /// The height of the image, if known. A hint to help render the image at the correct size.
        /// </summary>
        double? Height { get; }

        /// <summary>
        /// The width of the image, if known. A hint to help render the image at the correct size.
        /// </summary>
        double? Width { get; }
    }
}
