using OwlCore.Extensions;
using StrixMusic.Core.LocalFiles.Backing.Services;
using StrixMusic.Sdk.Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StrixMusic.Core.LocalFiles
{
    /// <summary>
    /// Manages multiple instances of LocalFileCore.
    /// </summary>
    public static class LocalFileCoreManager
    {
        /// <summary>
        /// Holds the instances of all constructed file cores.
        /// </summary>
        public static ConcurrentBag<LocalFileCore> Instances { get; set; } = new ConcurrentBag<LocalFileCore>();

        /// <summary>
        /// Loads data for all cores.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static async Task InitializeDataForAllCores()
        {
            await Instances.InParallel(x => x.GetService<TrackService>().InitAsync());
            await Instances.InParallel(x => x.GetService<AlbumService>().InitAsync());

            // This will currently throw an exception(file is being used by another process) because all cores instance are using the same folder. 
            await Instances.InParallel(x => x.GetService<AlbumService>().CreateOrUpdateAlbumMetadata());
            await Instances.InParallel(x => x.GetService<TrackService>().CreateOrUpdateTrackMetadata());
        }
    }
}
