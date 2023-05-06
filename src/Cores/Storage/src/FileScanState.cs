// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

namespace StrixMusic.Cores.Storage;

/// <summary>
/// Indicates the overall state of an ongoing file scan.
/// </summary>
public struct FileScanState
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FileScanState"/> struct.
    /// </summary>
    public FileScanState(FileScanStage stage, int filesProcessed, int filesFound)
        : this()
    {
        Stage = stage;
        FilesProcessed = filesProcessed;
        FilesFound = filesFound;
    }

    /// <summary>
    /// The total number of files discovered that need to be processed.
    /// </summary>
    public int FilesFound { get; internal set; }

    /// <summary>
    /// The number of files that have been processed.
    /// </summary>
    public int FilesProcessed { get; internal set; }

    /// <summary>
    /// The current stage of the file scanner.
    /// </summary>
    public FileScanStage Stage { get; }
}