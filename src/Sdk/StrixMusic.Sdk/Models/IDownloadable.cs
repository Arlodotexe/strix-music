using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StrixMusic.Sdk.Models
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
        Task StartDownloadOperationAsync(DownloadOperation operation);

        /// <summary>
        /// Raised when <see cref="DownloadInfo"/> is updated.
        /// </summary>
        event EventHandler<DownloadInfo>? DownloadInfoChanged;
    }
}
