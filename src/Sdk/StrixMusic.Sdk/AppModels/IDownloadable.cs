// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System;
using System.Threading;
using System.Threading.Tasks;

namespace StrixMusic.Sdk.AppModels
{
    /// <summary>
    /// Indicates an item that can be downloaded for offline usage.
    /// </summary>
    public interface IDownloadable
    {
        /// <summary>
        /// Information about downloading this item.
        /// </summary>
        DownloadInfo DownloadInfo { get; }

        /// <summary>
        /// Begins a download operation for this playable item.
        /// </summary>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task StartDownloadOperationAsync(DownloadOperation operation, CancellationToken cancellationToken = default);

        /// <summary>
        /// Raised when <see cref="DownloadInfo"/> is updated.
        /// </summary>
        event EventHandler<DownloadInfo>? DownloadInfoChanged;
    }
}
