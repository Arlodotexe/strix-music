using StrixMusic.Sdk.Models.Base;
using StrixMusic.Sdk.Models.Core;
using StrixMusic.Sdk.Models.Merged;

namespace StrixMusic.Sdk.Models
{
    /// <summary>
    /// A group of collections that represent a music library.
    /// </summary>
    /// <remarks>Instances of this class may contain data merged from one or more sources.</remarks>
    public interface ILibrary : ILibraryBase, IPlayableCollectionGroup, ISdkMember, IMerged<ICoreLibrary>
    {
    }
}