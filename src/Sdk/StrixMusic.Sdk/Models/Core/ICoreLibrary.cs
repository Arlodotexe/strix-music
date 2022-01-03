using StrixMusic.Sdk.Models.Base;

namespace StrixMusic.Sdk.Models.Core
{
    /// <inheritdoc cref="ILibraryBase"/>
    /// <remarks>This interface should be implemented by a core.</remarks>
    public interface ICoreLibrary : ILibraryBase, ICorePlayableCollectionGroup, ICoreMember
    {
    }
}