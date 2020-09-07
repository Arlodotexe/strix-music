using System;

namespace StrixMusic.Sdk.Interfaces
{
    /// <summary>
    /// Contains details about an image.
    /// </summary>
    public interface IImage
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
