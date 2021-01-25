using OwlCore.Extensions;
using StrixMusic.Core.LocalFiles.Backing.Services;
using StrixMusic.Core.LocalFiles.Models;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StrixMusic.Core.LocalFiles
{
    /// <summary>
    /// Manages multiple instances of <see cref="LocalFilesCore"/>.
    /// </summary>
    public static class LocalFileCoreManager
    {
        /// <summary>
        /// Holds the instances of all constructed file cores.
        /// </summary>
        public static ConcurrentBag<LocalFilesCore> Instances { get; set; } = new ConcurrentBag<LocalFilesCore>();

        /// <summary>
        /// Loads data for all cores.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static async Task InitializeDataForAllCores()
        {
            await Instances.InParallel(x => (x.Library as LocalFilesCoreLibrary).InitAsync());

            await Task.CompletedTask;
        }
    }
}
