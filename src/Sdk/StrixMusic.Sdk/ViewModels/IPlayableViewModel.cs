// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System.Threading.Tasks;
using StrixMusic.Sdk.Models;

namespace StrixMusic.Sdk.ViewModels
{
    /// <summary>
    /// An interfaced ViewModel for anything that is playable. Multiple view models implement <see cref="IPlayable"/> and this interface allows us to 
    /// </summary>
    public interface IPlayableViewModel : ISdkViewModel, IPlayable
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
    }
}
