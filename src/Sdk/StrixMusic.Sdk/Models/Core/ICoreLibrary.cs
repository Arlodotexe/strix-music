using StrixMusic.Sdk.Models.Base;

namespace StrixMusic.Sdk.Models.Core
{
    /// <summary>
    /// A group of collections that represent a music library.
    /// </summary>
    /// <remarks>This interface should be implemented by a core.</remarks>
    public interface ICoreLibrary : ILibraryBase, ICorePlayableCollectionGroup, ICoreMember
    {
    }
}