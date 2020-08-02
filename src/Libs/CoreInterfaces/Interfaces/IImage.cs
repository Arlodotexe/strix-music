using System;

namespace StrixMusic.CoreInterfaces.Interfaces
{
    /// <summary>
    /// Contains details about an image.
    /// </summary>
    public interface IImage
    {
        /// <summary>
        ///  Local or remote resource pointing to the image.
        /// </summary>
        Uri Uri { get; set; }
        
        /// <summary>
        /// Height of the image.
        /// </summary>
        double Height { get; set; }
    
        /// <summary>
        /// Width of the image.
        /// </summary>
        double Width { get; set; }
    }
}
