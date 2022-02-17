// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

namespace StrixMusic.Sdk.FileMetadata
{
    /// <summary>
    /// Indicates a type of file scan.
    /// </summary>
    public enum FileScanningType
    {
        /// <summary>
        /// No file scan.
        /// </summary>
        None,

        /// <summary>
        /// Indicating a scan of files containing raw audio.
        /// </summary>
        AudioFiles,

        /// <summary>
        /// Indicates a playlist metadata scan.
        /// </summary>
        Playlists,
    }
}