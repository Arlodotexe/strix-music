namespace StrixMusic.Sdk.Core.Data
{
    /// <inheritdoc cref="ILibraryBase"/>
    /// <remarks>This interface should be implemented by the Sdk.</remarks>
    public interface ILibrary : ILibraryBase, IPlayableCollectionGroup, ISdkMember
    {
    }
}