using StrixMusic.Sdk.Data.Base;

namespace StrixMusic.Sdk.Data.Core
{
    /// <inheritdoc cref="ILibraryBase"/>
    /// <remarks>This interface should be implemented by a core.</remarks>
    public interface ICoreLibrary : ILibraryBase, ICorePlayableCollectionGroup, ICoreMember
    {
    }
}