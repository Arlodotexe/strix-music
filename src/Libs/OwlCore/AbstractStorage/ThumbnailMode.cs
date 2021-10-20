using System;
using System.Collections.Generic;
using System.Text;

namespace OwlCore.AbstractStorage
{
    /// <summary>
    /// Describes the purpose of the thumbnail to determine how to adjust the thumbnail image to retrieve.
    /// </summary>
    public enum ThumbnailMode
    {
        /// <summary>
        /// To display previews of picture files.
        /// </summary>
        PicturesView,

        /// <summary>
        /// To display previews of video files.
        /// </summary>
        VideosView,

        /// <summary>
        /// To display previews of music files.
        /// </summary>
        MusicView,
       
        /// <summary>
        /// 
        /// </summary>
        DocumentsView,

        /// <summary>
        /// To display previews of files (or other items) in a list.
        /// </summary>
        ListView,

        /// <summary>
        /// To display a preview of any single item (like a file, folder, or file group).
        /// </summary>
        SingleItem
    }
}
