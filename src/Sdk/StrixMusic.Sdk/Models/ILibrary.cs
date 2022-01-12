using StrixMusic.Sdk.Models.Base;
using StrixMusic.Sdk.Models.Core;
using StrixMusic.Sdk.Models.Merged;

namespace StrixMusic.Sdk.Models
{
    /// <inheritdoc cref="ILibraryBase"/>
    /// <remarks>This interface should be implemented by the Sdk.</remarks>
    public interface ILibrary : ILibraryBase, IPlayableCollectionGroup, ISdkMember, IMerged<ICoreLibrary>
    {
    }
}