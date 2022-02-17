// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System;

namespace StrixMusic.Sdk.Models.Base
{
    /// <summary>
    /// Contains details about an image.
    /// </summary>
    public interface IImageBase : ICollectionItemBase
    {
        /// <summary>
        /// Local or remote resource pointing to the image.
        /// </summary>
        Uri Uri { get; }

        /// <summary>
        /// Height of the image.
        /// </summary>
        double Height { get; }

        /// <summary>
        /// Width of the image.
        /// </summary>
        double Width { get; }
    }
}