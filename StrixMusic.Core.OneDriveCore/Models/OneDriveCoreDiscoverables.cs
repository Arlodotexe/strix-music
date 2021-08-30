using StrixMusic.Core.LocalFiles.Models;
using StrixMusic.Sdk.Data.Core;

namespace StrixMusic.Core.OneDriveCore.Models
{
    /// <summary>
    /// Discoverable music for the <see cref="OneDriveCore"/>.
    /// </summary>
    public class OneDriveCoreDiscoverables : LocalFilesCoreDiscoverables
    {
        /// <inheritdoc />
        public OneDriveCoreDiscoverables(ICore sourceCore)
            : base(sourceCore)
        {
        }
    }
}
