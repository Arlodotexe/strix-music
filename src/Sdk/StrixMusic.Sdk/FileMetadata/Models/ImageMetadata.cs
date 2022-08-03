// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System;

namespace StrixMusic.Sdk.FileMetadata.Models
{
    /// <summary>
    /// Contains information that describes an image, scanned from a single file.
    /// </summary>
    public sealed class ImageMetadata : IFileMetadata
    {
        /// <summary>
        /// The unique identifier for this image.
        /// </summary>
        public string? Id { get; set; }

        /// <summary>
        /// The mime type of the image, if known. A hint to help optimize image rendering.
        /// </summary>
        public string? MimeType { get; set; }

        /// <summary>
        /// The width of this image.
        /// </summary>
        public int? Width { get; set; }

        /// <summary>
        /// The height of this image.
        /// </summary>
        public int? Height { get; set; }
    }
}
