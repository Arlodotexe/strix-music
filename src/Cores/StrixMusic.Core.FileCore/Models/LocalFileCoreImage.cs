using StrixMusic.Sdk.Data.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace StrixMusic.Core.LocalFileCore.Models
{
    ///<inheritdoc/>
    public class LocalFileCoreImage : ICoreImage
    {
        /// <summary>
        /// Creates a new instance of <see cref="LocalFileCoreImage"/>.
        /// </summary>
        /// <param name="sourceCore"><inheritdoc cref="ICoreMember.SourceCore"/></param>
        public LocalFileCoreImage(ICore sourceCore)
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
