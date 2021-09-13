using StrixMusic.Cores.LocalFiles.Models;
using StrixMusic.Sdk.Data.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace StrixMusic.Cores.OneDrive.Models
{
    /// <inheritdoc />
    public class OneDriveCoreImage : LocalFilesCoreImage
    {
        /// <summary>
        /// Creates a new instance of <see cref="OneDriveCoreImage"/>.
        /// </summary>
        /// <param name="sourceCore"><inheritdoc cref="ICoreMember.SourceCore"/></param>
        /// <param name="uri">A <see cref="System.Uri"/> pointing to the image file on the disk.</param>
        /// <param name="width">The width of the image, or <see langword="null"/> if not available.</param>
        /// <param name="height">The height of the image, or <see langword="null"/> if not available.</param>
        public OneDriveCoreImage(ICore sourceCore, Uri uri, double? width = null, double? height = null) :
            base(sourceCore, uri, width, height)
        {
        }
    }
}
