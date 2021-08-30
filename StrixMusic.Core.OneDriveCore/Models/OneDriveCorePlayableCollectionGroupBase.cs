using StrixMusic.Core.LocalFiles.Models;
using StrixMusic.Sdk.Data.Core;

namespace StrixMusic.Core.OneDriveCore.Models
{
    /// <inheritdoc cref="ICorePlayableCollectionGroup"/>
    public abstract class OneDriveCorePlayableCollectionGroupBase : LocalFilesCorePlayableCollectionGroupBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OneDriveCorePlayableCollectionGroupBase"/> class.
        /// </summary>
        /// <param name="sourceCore">The instance of the core this object was created in.</param>
        protected OneDriveCorePlayableCollectionGroupBase(ICore sourceCore)
            : base(sourceCore)
        {
        }
    }
}
