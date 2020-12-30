using StrixMusic.Sdk.Data.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace StrixMusic.Core.LocalFiles.Models
{
    ///<inheritdoc/>
    public class LocalFilesCoreImage : ICoreImage
    {
        /// <summary>
        /// Creates a new instance of <see cref="LocalFilesCoreImage"/>.
        /// </summary>
        /// <param name="sourceCore"><inheritdoc cref="ICoreMember.SourceCore"/></param>
        public LocalFilesCoreImage(ICore sourceCore)
        {
            SourceCore = sourceCore;
        }

        /// <inheritdoc />
        public ICore SourceCore { get; }

        /// <inheritdoc />
        public Uri Uri { get; }

        /// <inheritdoc />
        public double Height { get; }

        /// <inheritdoc />
        public double Width { get; }
    }
}
