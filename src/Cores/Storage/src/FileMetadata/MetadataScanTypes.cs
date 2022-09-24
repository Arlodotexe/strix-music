// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System;

namespace StrixMusic.Cores.Storage.FileMetadata;

/// <summary>
/// The different ways to scan a file for metadata.
/// </summary>
[Flags]
internal enum MetadataScanTypes
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