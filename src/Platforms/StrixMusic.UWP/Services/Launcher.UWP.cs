using System;
using System.Threading.Tasks;

namespace OwlCore.AbstractLauncher
{
    /// <inheritdoc/>
    public class Launcher : ILauncher
    {
        /// <inheritdoc/>
        public async Task<bool> LaunchUriAsync(Uri uri)
        {
            return await Windows.System.Launcher.LaunchUriAsync(uri);
        }
    }
}
