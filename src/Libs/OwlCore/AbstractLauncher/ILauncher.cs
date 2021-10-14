using System;
using System.Threading.Tasks;

namespace OwlCore.AbstractLauncher
{
    /// <summary>
    /// Provides methods to launch external applications.
    /// </summary>
    public interface ILauncher
    {
        /// <summary>
        /// Launches the specified <see cref="Uri"/> in the default application.
        /// </summary>
        /// <param name="uri">The <see cref="Uri"/> to launch.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the asynchronous operation.</returns>
        Task<bool> LaunchUriAsync(Uri uri);
    }
}
