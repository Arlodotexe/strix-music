using OwlCore.Extensions;
using StrixMusic.Core.LocalFiles.Models;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace StrixMusic.Core.LocalFiles
{
    /// <summary>
    /// Manages multiple instances of <see cref="LocalFilesCore"/>.
    /// </summary>
    public static class LocalFilesCoreManager
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
            await Instances.InParallel(x => x.Library.Cast<LocalFilesCoreLibrary>().InitAsync());

            await Task.CompletedTask;
        }
    }
}
