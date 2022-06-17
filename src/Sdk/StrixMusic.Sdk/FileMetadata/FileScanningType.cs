// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

namespace StrixMusic.Sdk.FileMetadata
{
    /// <summary>
    /// Indicates a type of file scan.
    /// </summary>
    public enum FileScanStage
    {
        /// <summary>
        /// Not scanning.
        /// </summary>
        None,

        /// <summary>
        /// Scanning the provided folder for files.
        /// </summary>
        FileDiscovery,

        /// <summary>
        /// Scanning files containing raw audio.
        /// </summary>
        AudioFiles,

        /// <summary>
        /// Scanning playlist files
        /// </summary>
        Playlists,

        /// <summary>
        /// The scan has completed.
        /// </summary>
        Complete,
    }
}
