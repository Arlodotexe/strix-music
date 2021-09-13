using StrixMusic.Cores.LocalFiles.Models;
using StrixMusic.Sdk.Data.Core;

namespace StrixMusic.Cores.OneDrive.Models
{
    /// <inheritdoc cref="ICoreLibrary"/>
    public class OneDriveCoreLibrary: LocalFilesCoreLibrary
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OneDriveCoreLibrary"/> class.
        /// </summary>
        /// <param name="sourceCore">The core that created this instance.</param>
        public OneDriveCoreLibrary(ICore sourceCore)
            : base(sourceCore)
        {
        }
    }
}
