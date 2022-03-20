using System;
using System.Threading.Tasks;
using OwlCore.Provisos;

namespace StrixMusic.Sdk.Uno.Services
{
    /// <inheritdoc/>
    public sealed class Launcher : ILauncher
    {
        /// <inheritdoc/>
        public async Task<bool> LaunchUriAsync(Uri uri)
        {
            return await Windows.System.Launcher.LaunchUriAsync(uri);
        }
    }
}
