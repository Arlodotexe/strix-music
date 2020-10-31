namespace StrixMusic.Sdk.Core.Data
{
    /// <inheritdoc cref="ILibraryBase"/>
    /// <remarks>This interface should be implemented by a core.</remarks>
    public interface ICoreLibrary : ILibraryBase, ICorePlayableCollectionGroup, ICoreMember
    {
    }
}