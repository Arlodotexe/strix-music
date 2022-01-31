using System;

namespace StrixMusic.Sdk.FileMetadataManagement
{
    /// <summary>
    /// The different ways to scan a file for metadata.
    /// </summary>
    [Flags]
    public enum MetadataScanTypes
    {
        /// <summary>
        /// No specified scan type.
        /// </summary>
        None = 0,

        /// <summary>
        /// Manually scan file contents for metadata.
        /// </summary>
        TagLib = 1,

        /// <summary>
        /// Use audio metadata provided by the file system.
        /// </summary>
        FileProperties = 2,
    }
}