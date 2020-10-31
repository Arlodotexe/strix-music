using System;

namespace StrixMusic.Sdk.Data.Base
{
    /// <summary>
    /// Contains details about an image.
    /// </summary>
    public interface IImageBase
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