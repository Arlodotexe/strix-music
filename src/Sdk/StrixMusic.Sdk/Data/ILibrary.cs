using StrixMusic.Sdk.Data.Base;

namespace StrixMusic.Sdk.Data
{
    /// <inheritdoc cref="ILibraryBase"/>
    /// <remarks>This interface should be implemented by the Sdk.</remarks>
    public interface ILibrary : ILibraryBase, IPlayableCollectionGroup, ISdkMember
    {
    }
}